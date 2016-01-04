using Microsoft.Win32.SafeHandles;
using System;
//
//The MIT License (MIT)
//Copyright(c) 2013 Matt Johnson<mj1856@hotmail.com>
//
//And this is further modified by Lin Zhishun (jaksonlin@gmail.com) to fix an issue for LogonType with new credential.
//
namespace GammaLib.Impersonation
{
    sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeTokenHandle(IntPtr handle)
            : base(true)
        {
            this.handle = handle;
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }
    }
}
