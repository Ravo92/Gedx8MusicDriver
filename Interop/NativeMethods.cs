using System.Runtime.InteropServices;

namespace Gedx8MusicDriver.Interop
{
    internal static class NativeMethods
    {
        [DllImport("ole32", EntryPoint = "CoInitialize", SetLastError = false)]
        internal static extern int CoInitialize(IntPtr reserved);

        [DllImport("ole32", EntryPoint = "CoUninitialize", SetLastError = false)]
        internal static extern void CoUninitialize();
    }
}
