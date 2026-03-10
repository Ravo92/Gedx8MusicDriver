using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gedx8MusicDriver.Interop
{
    internal sealed class DirectMusicLoaderContext : IDisposable
    {
        private static readonly Guid ClsidDirectMusicLoader = new("D2AC2892-B39B-11D1-8704-00600893B1BD");
        private static readonly Guid GuidDirectMusicAllTypes = new("D2AC2893-B39B-11D1-8704-00600893B1BD");
        private IDirectMusicLoader8? _loader;
        private bool _comInitialized;
        private bool _disposed;

        internal DirectMusicLoaderContext(string searchDirectory)
        {
            SearchDirectory = searchDirectory;
        }

        internal string SearchDirectory { get; }

        [SupportedOSPlatform("windows")]
        internal bool TryPrepare()
        {
            if (_disposed)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(SearchDirectory) || !Directory.Exists(SearchDirectory))
            {
                return false;
            }

            int hr = NativeMethods.CoInitialize(IntPtr.Zero);
            bool ownsComInitialization = hr == 0 || hr == 1;
            bool alreadyInitialized = hr == unchecked((int)0x80010106);
            if (!ownsComInitialization && !alreadyInitialized)
            {
                return false;
            }

            _comInitialized = ownsComInitialization;

            Type? loaderType = Type.GetTypeFromCLSID(ClsidDirectMusicLoader, throwOnError: false);
            if (loaderType == null)
            {
                return false;
            }

            object? comObject = Activator.CreateInstance(loaderType);
            if (comObject == null)
            {
                return false;
            }

            _loader = (IDirectMusicLoader8?)comObject;
            if (_loader == null)
            {
                return false;
            }

            Guid allTypes = GuidDirectMusicAllTypes;
            int setSearchDirectoryHr = _loader.SetSearchDirectory(ref allTypes, SearchDirectory, 0);
            return setSearchDirectoryHr >= 0;
        }

        internal bool TryResolveFile(string fileName, out string? resolvedPath)
        {
            resolvedPath = null;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            string directCandidate = Path.Combine(SearchDirectory, fileName);
            if (File.Exists(directCandidate))
            {
                resolvedPath = directCandidate;
                return true;
            }

            string[] commonDirectories =
            [
                SearchDirectory,
                Path.Combine(SearchDirectory, "music"),
                Path.Combine(SearchDirectory, "sound"),
                Path.Combine(SearchDirectory, "audio"),
                Path.Combine(SearchDirectory, "data"),
                Path.Combine(SearchDirectory, "data", "music"),
                Path.Combine(SearchDirectory, "data", "sound"),
                Path.Combine(SearchDirectory, "data", "audio"),
            ];

            for (int i = 0; i < commonDirectories.Length; i++)
            {
                string directory = commonDirectories[i];
                if (!Directory.Exists(directory))
                {
                    continue;
                }

                string candidate = Path.Combine(directory, fileName);
                if (File.Exists(candidate))
                {
                    resolvedPath = candidate;
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_loader != null)
            {
                Marshal.ReleaseComObject(_loader);
                _loader = null;
            }

            if (_comInitialized)
            {
                NativeMethods.CoUninitialize();
                _comInitialized = false;
            }

            _disposed = true;
        }
    }
}
