using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
//
//The MIT License (MIT)
//Copyright(c) 2013 Matt Johnson<mj1856@hotmail.com>
//
//And this is further modified by Jason Lam to fix an issue for LogonType with new credential.
//
namespace GammaLib.Impersonation
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class Impersonation : IDisposable
    {
        private readonly SafeTokenHandle _handle;
        private readonly WindowsImpersonationContext _context;

        public static Impersonation LogonUser(string domain, string username, string password, LogonType logonType)
        {
            return new Impersonation(domain, username, password, logonType);
        }

        private Impersonation(string domain, string username, string password, LogonType logonType)
        {
            IntPtr handle;
            bool ok = false;
            if (logonType != LogonType.NewCredentials)
            {
                ok = NativeMethods.LogonUser(username, domain, password, (int)logonType, (int)LogonProvider.LOGON32_PROVIDER_DEFAULT, out handle);
            }
            else
            {
                //this is the fix for LogonType.NewCredentials
                ok = NativeMethods.LogonUser(username, domain, password, (int)logonType, (int)LogonProvider.LOGON32_PROVIDER_WINNT50, out handle);
            }
            if (!ok)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));
            }

            _handle = new SafeTokenHandle(handle);
            _context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        public void Dispose()
        {
            _context.Dispose();
            _handle.Dispose();
        }
    }
}
