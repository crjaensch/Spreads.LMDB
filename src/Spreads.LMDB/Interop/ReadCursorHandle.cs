// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;

namespace Spreads.LMDB.Interop
{
    internal class ReadCursorHandle : SafeHandle
    {
        internal ReadCursorHandle() : base(IntPtr.Zero, ownsHandle: true)
        { }

        public override bool IsInvalid => handle == IntPtr.Zero;

        internal void SetNewHandle(IntPtr handle)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.mdb_cursor_close(handle);
            return true;
        }
    }
}