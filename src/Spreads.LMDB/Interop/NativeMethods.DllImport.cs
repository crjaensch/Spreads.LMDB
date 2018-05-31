// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Spreads.Utils.Bootstrap;
using System;
using System.Runtime.InteropServices;

// ReSharper disable UnusedMember.Global

namespace Spreads.LMDB.Interop
{
#pragma warning disable IDE1006 // Naming Styles

    [System.Security.SuppressUnmanagedCodeSecurity]
    internal static partial class NativeMethods
    {
        static NativeMethods()
        {
            // Ensure Bootstrapper is initialized and native libraries are loaded
            Bootstrapper.Instance.Bootstrap<Environment>(
                DbLibraryName,
                null,
                () => { },
                (lib) => { },
                () =>
                {
                });
        }

        public const string DbLibraryName = "spreads_lmdb";

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_create(out EnvironmentHandle env);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mdb_env_close(IntPtr env);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mdb_env_close(EnvironmentHandle env);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_open(IntPtr env, string path, DbEnvironmentFlags flags, UnixAccessMode mode);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_open(EnvironmentHandle env, string path, DbEnvironmentFlags flags, UnixAccessMode mode);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_open(IntPtr env, string path, DbEnvironmentFlags flags, int mode);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_open(EnvironmentHandle env, string path, DbEnvironmentFlags flags, int mode);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_set_mapsize(IntPtr env, IntPtr size);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_set_mapsize(EnvironmentHandle env, IntPtr size);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_get_maxreaders(IntPtr env, out uint readers);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_get_maxreaders(EnvironmentHandle env, out uint readers);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_get_maxkeysize(IntPtr env);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_get_maxkeysize(EnvironmentHandle env);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_set_maxreaders(IntPtr env, uint readers);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_set_maxreaders(EnvironmentHandle env, uint readers);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_set_maxdbs(IntPtr env, uint dbs);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_set_maxdbs(EnvironmentHandle env, uint dbs);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_dbi_open(IntPtr txn, string name, DbFlags flags, out uint db);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mdb_dbi_close(IntPtr env, uint dbi);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mdb_dbi_close(EnvironmentHandle env, uint dbi);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_drop(IntPtr txn, uint dbi, bool del);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_txn_begin(EnvironmentHandle env, IntPtr parent, TransactionBeginFlags flags, out IntPtr txn);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_txn_begin(EnvironmentHandle env, IntPtr parent, TransactionBeginFlags flags, out ReadTransactionHandle txn);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_txn_commit(IntPtr txn);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mdb_txn_abort(IntPtr txn);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mdb_txn_abort(ReadTransactionHandle txn);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mdb_txn_reset(ReadTransactionHandle txn);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_txn_renew(ReadTransactionHandle txn);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mdb_version(out IntPtr major, out IntPtr minor, out IntPtr patch);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mdb_strerror(int err);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_stat(IntPtr txn, uint dbi, out MDB_stat stat);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_stat(ReadTransactionHandle txn, uint dbi, out MDB_stat stat);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_copy(IntPtr env, string path);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_copy2(IntPtr env, string path, EnvironmentCopyFlags copyFlags);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_copy2(EnvironmentHandle env, string path, EnvironmentCopyFlags copyFlags);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_info(IntPtr env, out MDB_envinfo stat);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_info(EnvironmentHandle env, out MDB_envinfo stat);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_stat(IntPtr env, out MDB_stat stat);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_stat(EnvironmentHandle env, out MDB_stat stat);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_sync(IntPtr env, bool force);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_env_sync(EnvironmentHandle env, bool force);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_get(IntPtr txn, uint dbi, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_put(IntPtr txn, uint dbi, ref MDB_val key, ref MDB_val data, TransactionPutOptions flags);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_del(IntPtr txn, uint dbi, ref MDB_val key, ref MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_del(IntPtr txn, uint dbi, ref MDB_val key, IntPtr data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_open(IntPtr txn, uint dbi, out IntPtr cursor);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_open(ReadTransactionHandle txn, uint dbi, out IntPtr cursor);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mdb_cursor_close(IntPtr cursor);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_renew(ReadTransactionHandle txn, ReadCursorHandle cursor);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_get(IntPtr cursor, ref MDB_val key, ref MDB_val data, CursorGetOption op);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_get(ReadCursorHandle cursor, ref MDB_val key, ref MDB_val data, CursorGetOption op);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_put(IntPtr cursor, ref MDB_val key, ref MDB_val data, CursorPutOptions flags);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_put(IntPtr cursor, ref MDB_val key, MDB_val2[] data, CursorPutOptions flags);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_del(IntPtr cursor, CursorDeleteOption flags);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_count(IntPtr cursor, out UIntPtr countp);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_cursor_count(ReadCursorHandle cursor, out UIntPtr countp);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_set_compare(IntPtr txn, uint dbi, [MarshalAs(UnmanagedType.FunctionPtr)]CompareFunction cmp);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mdb_set_dupsort(IntPtr txn, uint dbi, [MarshalAs(UnmanagedType.FunctionPtr)]CompareFunction cmp);

        // Spreads extensoins to LMDB

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_lt(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_le(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_eq(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_ge(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_gt(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_lt_dup(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_le_dup(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_eq_dup(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_ge_dup(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_gt_dup(IntPtr cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_set_dupsort_as_sint64(IntPtr txn, uint dbi);

        // Same as above with Ctrl+H IntPtr -> ReadCursorHandle
        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_lt(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_le(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_eq(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_ge(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_gt(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_lt_dup(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_le_dup(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_eq_dup(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_ge_dup(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);

        [DllImport(DbLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sdb_cursor_get_gt_dup(ReadCursorHandle cursor, ref MDB_val key, out MDB_val data);
    }

#pragma warning restore IDE1006 // Naming Styles
}