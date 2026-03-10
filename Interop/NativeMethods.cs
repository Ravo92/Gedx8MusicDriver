using System.Runtime.InteropServices;

namespace Gedx8MusicDriver.Interop
{
    internal static class NativeMethods
    {
        [DllImport("kernel32", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern nint LoadLibraryA(string fileName);

        [DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(nint moduleHandle);

        [DllImport("ole32", EntryPoint = "CoInitialize", SetLastError = false)]
        internal static extern int CoInitialize(IntPtr reserved);

        [DllImport("ole32", EntryPoint = "CoUninitialize", SetLastError = false)]
        internal static extern void CoUninitialize();
    }
}