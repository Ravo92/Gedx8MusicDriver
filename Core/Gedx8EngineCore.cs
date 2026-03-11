using Gedx8MusicDriver.Interop;
using Gedx8MusicDriver.Models;

namespace Gedx8MusicDriver.Core
{
    internal sealed class Gedx8EngineCore : IDisposable
    {
        private readonly int[] _propertyValues17C = new int[13];
        private readonly bool[] _propertyResolved17C = new bool[13];
        private readonly object?[] _cachedObjects148 = new object?[13];
        private DirectMusicLoaderContext? _loaderContext;
        private bool _disposed;
        private int _value00;
        private int _value04;
        private int _value08;
        private int _value1B0;
        private int _value1B4;
        private byte _value1B8;
        private int _value1BC;
        private object? _value1C0;
        private int _value1C4;

        internal Gedx8SynthInitConfig SynthInitConfig { get; private set; } = Gedx8SynthInitConfig.Empty;

        internal Gedx8LoaderMode LoaderMode1B0
        {
            get => (Gedx8LoaderMode)_value1B0;
            set => _value1B0 = (int)value;
        }

        internal byte SelectionByte1B8 => _value1B8;

        internal int SelectionValue1BC => _value1BC;

        internal bool IsSynthInitialized { get; private set; }

        internal string? SearchDirectory => _loaderContext?.SearchDirectory;

        internal int Value00 => _value00;

        internal int Value04 => _value04;

        internal int Value08 => _value08;

        internal bool InitializeSynthesizer(Gedx8SynthInitConfig config)
        {
            if (_disposed || config.SampleRate <= 0)
            {
                return false;
            }

            SynthInitConfig = new Gedx8SynthInitConfig(0x28, config.SampleRate, config.Config);
            IsSynthInitialized = true;
            _value00 = 0;
            _value04 = 0;
            _value08 = 0;
            _value1B8 = 0;
            _value1BC = 0;

            SetRuntimeModeFields10002380(0, config.Config, 1);
            SetCurrentObject10002380(this);
            return true;
        }

        internal bool Call03BC0(int value0, int value1)
        {
            if (_disposed || !IsSynthInitialized)
            {
                return false;
            }

            if (value0 == 0 || value1 == 0)
            {
                return false;
            }

            _value04 = value0;
            _value08 = value1;

            if (_value1C0 == null)
            {
                _value1C0 = this;
            }

            return true;
        }

        internal bool TryGetPair03BE0(out int value0, out int value1)
        {
            if (_disposed || _value04 == 0 || _value08 == 0)
            {
                value0 = 0;
                value1 = 0;
                return false;
            }

            value0 = _value04;
            value1 = _value08;
            return true;
        }

        internal bool Call03C90()
        {
            return !_disposed && IsSynthInitialized && _value1C0 != null;
        }

        internal bool Call03CA0()
        {
            return !_disposed && IsSynthInitialized && _value1C0 != null;
        }

        internal bool Call03E00()
        {
            return !_disposed && IsSynthInitialized && _value1C0 != null;
        }

        internal bool Call03E20(int value)
        {
            if (_disposed || !IsSynthInitialized || _value1C0 == null)
            {
                return false;
            }

            if (_value00 != value)
            {
                _value00 = value;
            }

            return true;
        }

        internal bool Call03E50(int value)
        {
            if (_disposed || !IsSynthInitialized || _value1C0 == null)
            {
                return false;
            }

            _value00 = value;
            return true;
        }

        internal bool PrepareLoaderContext(string searchDirectory)
        {
            if (_disposed || string.IsNullOrWhiteSpace(searchDirectory))
            {
                return false;
            }

            DirectMusicLoaderContext? previous = _loaderContext;
            previous?.Dispose();

            DirectMusicLoaderContext context = new(searchDirectory);
            if (!context.TryPrepare())
            {
                context.Dispose();
                return false;
            }

            _loaderContext = context;
            _value1B0 = 2;
            _value1C4 = 1;
            if (_value1C0 == null)
            {
                _value1C0 = this;
            }

            return true;
        }

        internal Gedx8LoadedObject? LoadObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory)
        {
            if (_disposed || !IsSynthInitialized || string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            string? effectiveSearchDirectory = searchDirectory;
            int loaderMode;

            if (string.IsNullOrWhiteSpace(searchDirectory))
            {
                if (string.IsNullOrWhiteSpace(SearchDirectory))
                {
                    return null;
                }

                effectiveSearchDirectory = SearchDirectory;
                loaderMode = 1;
            }
            else
            {
                if (!PrepareLoaderContext(searchDirectory))
                {
                    return null;
                }

                effectiveSearchDirectory = searchDirectory;
                loaderMode = 2;
            }

            string? resolvedPath = ResolvePath(fileName, effectiveSearchDirectory);
            if (resolvedPath == null)
            {
                return null;
            }

            Gedx8LoadedObject loadedObject = new(kind, fileName, effectiveSearchDirectory, resolvedPath);

            switch (kind)
            {
                case Gedx8ObjectKind.Composite:
                    loadedObject.Sub10004120(0, loaderMode, resolvedPath, this, resolvedPath, effectiveSearchDirectory, 1, 1, 1, new uint[] { 0 });
                    break;

                case Gedx8ObjectKind.ThinType1:
                    loadedObject.Sub10002D30(1, loaderMode, resolvedPath);
                    break;

                case Gedx8ObjectKind.ThinType2:
                    loadedObject.Sub10002D30(2, loaderMode, resolvedPath);
                    break;

                default:
                    return null;
            }

            SetRuntimeModeFields10002380(loaderMode, SynthInitConfig.Config, 1);
            SetCurrentObject10002380(loadedObject);
            return loadedObject;
        }

        internal void ReleaseCurrentObject100024B0()
        {
            if (_disposed)
            {
                return;
            }

            _value1C0 = null;
        }

        internal bool SetSelectionByte(byte value)
        {
            if (_disposed || _value1C0 == null)
            {
                return false;
            }

            if (_value1B8 == value)
            {
                return true;
            }

            _value1B8 = value;
            return true;
        }

        internal bool BindSelection(int selection, int mode)
        {
            if (_disposed || _value1C0 == null)
            {
                return false;
            }

            if (selection < 0 || mode < 0)
            {
                return false;
            }

            _value1BC = selection;
            _value1B0 = mode;
            return true;
        }

        internal bool GetSelection(out int selection)
        {
            if (_disposed)
            {
                selection = 0;
                return false;
            }

            selection = _value1BC;
            return _value1C0 != null;
        }

        internal bool DispatchProperty(int selector, int value, bool setMode, out int storedValue)
        {
            storedValue = 0;

            if (_disposed || _value1C0 == null)
            {
                return false;
            }

            if (selector < 0 || selector >= _propertyValues17C.Length)
            {
                return false;
            }

            if (selector == 1 && _value1B0 != 2 && _value1B0 != 3)
            {
                return false;
            }

            object? cachedObject = _cachedObjects148[selector];
            if (cachedObject == null)
            {
                cachedObject = new object();
                _cachedObjects148[selector] = cachedObject;
            }

            _propertyResolved17C[selector] = true;

            if (setMode)
            {
                _propertyValues17C[selector] = value;
            }

            storedValue = _propertyValues17C[selector];
            return true;
        }

        internal void SetRuntimeModeFields10002380(int mode1B0, int value1B4, int interface1C4)
        {
            if (_disposed)
            {
                return;
            }

            _value1B0 = mode1B0;
            _value1B4 = value1B4;
            _value1C4 = interface1C4;
            _value1B8 = 0;
        }

        internal void SetCurrentObject10002380(object? currentObject)
        {
            if (_disposed)
            {
                return;
            }

            _value1C0 = currentObject;
        }

        internal string? ResolvePath(string fileName, string? searchDirectory)
        {
            if (Path.IsPathRooted(fileName) && File.Exists(fileName))
            {
                return fileName;
            }

            if (!string.IsNullOrWhiteSpace(searchDirectory))
            {
                string direct = Path.Combine(searchDirectory, fileName);
                if (File.Exists(direct))
                {
                    return direct;
                }
            }

            DirectMusicLoaderContext? context = _loaderContext;
            if (context != null && context.TryResolveFile(fileName, out string? resolvedPath))
            {
                return resolvedPath;
            }

            return null;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            DirectMusicLoaderContext? loaderContext = _loaderContext;
            if (loaderContext != null)
            {
                loaderContext.Dispose();
                _loaderContext = null;
            }

            Array.Clear(_propertyValues17C);
            Array.Clear(_propertyResolved17C);
            Array.Clear(_cachedObjects148);

            _value00 = 0;
            _value04 = 0;
            _value08 = 0;
            _value1B0 = 0;
            _value1B4 = 0;
            _value1B8 = 0;
            _value1BC = 0;
            _value1C0 = null;
            _value1C4 = 0;
            SynthInitConfig = Gedx8SynthInitConfig.Empty;
            IsSynthInitialized = false;
            _disposed = true;
        }
    }
}