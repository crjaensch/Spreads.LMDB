﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Spreads.Collections.Concurrent;
using Spreads.LMDB.Interop;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Spreads.LMDB
{
    public readonly struct Transaction : IDisposable
    {
        internal readonly TransactionImpl _impl;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Transaction(TransactionImpl txn)
        {
            _impl = txn;
        }

        /// <summary>
        /// Commit all the operations of a transaction into the database.
        /// All cursors opened within the transaction will be closed by this call.
        /// The cursors and transaction handle will be freed and must not be used again after this call.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Commit()
        {
#if DEBUG
            if (_impl._cursorCount > 0)
            {
                Environment.FailFast("Transaction has outstanding cursors that must be closed before Commit.");
            }
#endif
            _impl.Commit();
        }

        /// <summary>
        /// Abandon all the operations of the transaction instead of saving them.
        /// All cursors opened within the transaction will be closed by this call.
        /// The cursors and transaction handle will be freed and must not be used again after this call.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort()
        {
#if DEBUG
            if (_impl._cursorCount > 0)
            {
                Environment.FailFast("Transaction has outstanding cursors that must be closed before Abort.");
            }
#endif
            _impl.Abort();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
#if DEBUG
            if (_impl._cursorCount > 0)
            {
                Environment.FailFast("Transaction has outstanding cursors that must be closed before Dispose.");
            }
#endif
            _impl.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlyTransaction(Transaction value)
        {
            return new ReadOnlyTransaction(value._impl);
        }
    }

    public readonly struct ReadOnlyTransaction : IDisposable
    {
        internal readonly TransactionImpl _impl;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlyTransaction(TransactionImpl txn)
        {
            _impl = txn;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
#if DEBUG
            if (_impl._cursorCount > 0)
            {
                Environment.FailFast("Transaction has outstanding cursors that must be closed before Dispose.");
            }
#endif
            _impl.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
#if DEBUG
            if (_impl._cursorCount > 0)
            {
                Environment.FailFast("Transaction has outstanding cursors that must be closed before Reset.");
            }
#endif
            _impl.Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Renew()
        {
            _impl.Renew();
        }
    }

    internal class TransactionImpl : SafeHandle
    {
        private LMDBEnvironment _lmdbEnvironment;

        private bool _isReadOnly;

        private TransactionState _state;

        #region Lifecycle

#if DEBUG
        public string StackTrace;
        internal int _cursorCount;
#endif

        internal static readonly ObjectPool<TransactionImpl> TxPool =
            new ObjectPool<TransactionImpl>(() => new TransactionImpl(), Environment.ProcessorCount * 16);

        private TransactionImpl() : base(IntPtr.Zero, true)
        {
            _state = TransactionState.Disposed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TransactionImpl Create(LMDBEnvironment lmdbEnvironment, TransactionBeginFlags beginFlags)
        {
            lmdbEnvironment.EnsureOpened();

            TransactionImpl tx;

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            var isReadOnly = (beginFlags & TransactionBeginFlags.ReadOnly) == TransactionBeginFlags.ReadOnly;

            if (isReadOnly)
            {
                if ((tx = lmdbEnvironment.ReadTxnPool.Rent()) == null)
                {
                    tx = TxPool.Allocate();
                    tx._isReadOnly = true;
                    if (tx._state != TransactionState.Disposed)
                    {
                        ThrowShoudBeDisposed();
                    }
                }
                else
                {
                    Debug.Assert(tx._isReadOnly);
                    Debug.Assert(tx._state == TransactionState.Reset);
                }

                if (tx.IsInvalidFast)
                {
                    // create new handle
                    NativeMethods.AssertExecute(NativeMethods.mdb_txn_begin(
                            lmdbEnvironment._handle.Handle,
                            IntPtr.Zero, beginFlags, out var handle));
                    tx.SetNewHandle(handle);
                }
                else
                {
                    tx.Renew();
                }
            }
            else
            {
                tx = TxPool.Allocate();
                tx._isReadOnly = false;
                NativeMethods.AssertExecute(NativeMethods.mdb_txn_begin(
                    lmdbEnvironment._handle.Handle,
                    IntPtr.Zero, beginFlags, out IntPtr handle));
                tx.handle = handle;
                if (tx._state != TransactionState.Disposed)
                {
                    ThrowShoudBeDisposed();
                }
            }

            tx._lmdbEnvironment = lmdbEnvironment;
            tx._state = TransactionState.Active;

#if DEBUG
            tx.StackTrace = Environment.StackTrace;
#endif
            return tx;
        }

        protected override void Dispose(bool disposing)
        {
            var isReadOnly = IsReadOnly;
            if (_state == TransactionState.Disposed)
            {
                if (disposing)
                {
                    ThrowlAlreadyDisposed();
                }
                else
                {
                    // TxPool.Free could have dropped, currently we cannot detect this
                    // See comment in Dispose()
                    if (isReadOnly)
                    {
                        base.Dispose(true);
                    }
                    return;
                }
            }

            _state = TransactionState.Disposed;

            if (isReadOnly)
            {
                if (disposing)
                {
                    NativeMethods.mdb_txn_reset(handle);
                    _state = TransactionState.Reset;
                    var pooled = _lmdbEnvironment.ReadTxnPool.Return(this);
                    if (pooled)
                    {
                        Debug.Assert(_state == TransactionState.Reset); // set above
                        return;
                    }

                    _state = TransactionState.Disposed;
                    NativeMethods.mdb_txn_abort(handle);
                    handle = IntPtr.Zero;
                }
                else
                {
#if DEBUG
                    Trace.TraceWarning("Finalizing read transaction. Dispose it explicitly. " + StackTrace);
#else
                    Trace.TraceWarning("Finalizing read transaction. Dispose it explicitly.");
#endif
                    base.Dispose(false);
                }
            }
            else
            {
                if (disposing)
                {
                    if (_state == TransactionState.Active)
                    {
                        if (LmdbEnvironment.AutoCommit)
                        {
                            NativeMethods.mdb_txn_commit(handle);
                        }
                        else
                        {
                            NativeMethods.mdb_txn_abort(handle);
                            // This should not be catchable
                            Environment.FailFast("Transaction was not either commited or aborted. Aborting it. Set Environment.AutoCommit to true to commit automatically on transaction end.");
                        }
                    }
                }
                else
                {
                    if (_state == TransactionState.Active)
                    {
                        NativeMethods.mdb_txn_abort(handle);
                        Environment.FailFast("Finalizing active transaction. Will abort it. Set Environment.AutoCommit to true to commit automatically on transaction end.");
                    }
                    else
                    {
#if DEBUG
                        Trace.TraceWarning("Finalizing finished write transaction. Dispose it explicitly. " + StackTrace);
#else
                        Trace.TraceWarning("Finalizing finished write transaction. Dispose it explicitly.");
#endif
                    }
                }
                handle = IntPtr.Zero;
            }

            _lmdbEnvironment = null;

#if DEBUG
            StackTrace = null;
#endif

            if (disposing)
            {
                TxPool.Free(this);
            }
        }

        // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle.releasehandle?view=netframework-4.7.2#remarks
        // The ReleaseHandle method is guaranteed to be called only once and only if the handle
        // is valid as defined by the IsInvalid property. Implement this method in your SafeHandle
        // derived classes to execute any code that is required to free the handle.
        // Because one of the functions of SafeHandle is to guarantee prevention of resource leaks,
        // the code in your implementation of ReleaseHandle must never fail.
        protected override bool ReleaseHandle()
        {
            var h = handle;
            handle = IntPtr.Zero;
            NativeMethods.mdb_txn_abort(h);
            return true;
        }

        internal IntPtr Handle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => handle;
        }

        // inlined
        private bool IsInvalidFast
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get => handle == IntPtr.Zero;
        }

        // TODO check usages, internal should use FastVersion
        public override bool IsInvalid => IsInvalidFast;

        internal void SetNewHandle(IntPtr newHandle)
        {
            SetHandle(newHandle);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowShoudBeDisposed()
        {
            throw new InvalidOperationException("Pooled tx must be in disposed state");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowlAlreadyDisposed()
        {
            throw new InvalidOperationException("Pooled tx must be in disposed state");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowlTransactionIsReadOnly(string message = null)
        {
            message = message is null ? String.Empty : " " + message;
            throw new InvalidOperationException("Transaction is not readonly." + message);
        }

        #endregion Lifecycle

        /// <summary>
        /// Environment in which the transaction was opened.
        /// </summary>
        public LMDBEnvironment LmdbEnvironment => _lmdbEnvironment;

        /// <summary>
        /// Transaction is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _isReadOnly;
        }

        /// <summary>
        /// Current transaction state.
        /// </summary>
        internal TransactionState State
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Commit()
        {
            if (_state != TransactionState.Active)
            {
                ThrowTxNotActiveOnCommit();
            }

            if (IsReadOnly)
            {
                ThrowTxReadOnlyOnCommit();
            }

            NativeMethods.AssertExecute(NativeMethods.mdb_txn_commit(handle));
            _state = TransactionState.Commited;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort()
        {
            if (_state != TransactionState.Active)
            {
                ThrowTxNotActiveOnAbort();
            }
            NativeMethods.mdb_txn_abort(handle);
            handle = IntPtr.Zero;
            _state = TransactionState.Aborted;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            if (_state != TransactionState.Active)
            {
                ThrowTxNotActiveOnReset();
            }
            if (!_isReadOnly)
            {
                ThrowTxNotReadonlyOnReset();
            }
            NativeMethods.mdb_txn_reset(handle);
            _state = TransactionState.Reset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Renew()
        {
            if (_state != TransactionState.Reset)
            {
                ThrowTxNotResetOnRenew();
            }
            if (!_isReadOnly)
            {
                ThrowTxNotReadonlyOnRenew();
            }
            NativeMethods.AssertExecute(NativeMethods.mdb_txn_renew(handle));
            _state = TransactionState.Active;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTxNotActiveOnCommit()
        {
            throw new InvalidOperationException("Transaction state is not active on commit");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTxNotActiveOnAbort()
        {
            throw new InvalidOperationException("Transaction state is not active on abort");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTxNotActiveOnReset()
        {
            throw new InvalidOperationException("Transaction state is not active on reset");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTxNotReadonlyOnReset()
        {
            throw new InvalidOperationException("Transaction is not readonly on reset");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTxNotReadonlyOnRenew()
        {
            throw new InvalidOperationException("Transaction is not readonly on renew");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTxNotResetOnRenew()
        {
            throw new InvalidOperationException("Transaction state is not reset on renew");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTxReadOnlyOnCommit()
        {
            throw new InvalidOperationException("Cannot commit readonly transaction");
        }
    }
}
