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

        // Mirrors the mutable dword touched by sub_10003E20 / sub_10003E50.
        private int _value00;

        // Mirrors the two dwords written by sub_10003BC0 and read by sub_10003BE0.
        private int _value04;
        private int _value08;

        // Mirrors the runtime mode/type fields initialized in sub_10002380.
        private int _value1B0;
        private int _value1B4;

        // Mirrors the byte/int pair manipulated by sub_100024D0 / sub_10002520 / sub_10002560.
        private byte _value1B8;
        private int _value1BC;

        // Mirrors the cached dispatch object pointer and the secondary interface field.
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
            // sub_10003D00 uses a 0x28-byte init block and then applies four property writes.
            // Only the state that is observable from the current project is retained here.
            if (_disposed || config.SampleRate <= 0)
            {
                return false;
            }

            int initLength = 0x28;
            int channelGroups = 1;
            int voices = 7;
            int flags = 0x3F;

            _value00 = 0;
            SynthInitConfig = new Gedx8SynthInitConfig(initLength, config.SampleRate, config.Config);
            IsSynthInitialized = channelGroups == 1 && voices == 7 && flags == 0x3F;
            return IsSynthInitialized;
        }

        internal bool Call03BC0(int value0, int value1)
        {
            // sub_10003BC0
            if (_disposed || value0 == 0 || value1 == 0)
            {
                return false;
            }

            _value04 = value0;
            _value08 = value1;
            return true;
        }

        internal bool TryGetPair03BE0(out int value0, out int value1)
        {
            // sub_10003BE0
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
            // sub_10003C90 performs a vtable +0x30 call on the active runtime object.
            return !_disposed && IsSynthInitialized;
        }

        internal bool Call03CA0()
        {
            // sub_10003CA0 performs a vtable +0x24 call with token 1000C328.
            return !_disposed && IsSynthInitialized;
        }

        internal bool Call03E00()
        {
            // sub_10003E00 performs a vtable +0x98 call on the active runtime object.
            return !_disposed && IsSynthInitialized;
        }

        internal bool Call03E20(int value)
        {
            // sub_10003E20 updates this+0 when the value changes and then writes property 1000C2D8.
            if (_disposed || !IsSynthInitialized)
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
            // sub_10003E50 always forwards the supplied dword to property 1000C2D8.
            if (_disposed || !IsSynthInitialized)
            {
                return false;
            }

            _value00 = value;
            return true;
        }

        internal bool PrepareLoaderContext(string searchDirectory)
        {
            // This remains the managed equivalent of the observed DirectMusic loader path.
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
            _value1B0 = (int)Gedx8LoaderMode.DirectMusicCom;
            return true;
        }

        internal Gedx8LoadedObject? LoadObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory)
        {
            if (_disposed || !IsSynthInitialized || string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(searchDirectory))
            {
                PrepareLoaderContext(searchDirectory);
            }

            string? resolvedPath = ResolvePath(fileName, searchDirectory);
            return new Gedx8LoadedObject(kind, fileName, searchDirectory ?? SearchDirectory, resolvedPath);
        }

        internal bool ReleaseCurrentObject100024B0()
        {
            // sub_100024B0 releases the cached object at this+1C0 when it is non-null.
            if (_disposed)
            {
                return false;
            }

            _value1C0 = null;
            return true;
        }

        internal bool SetSelectionByte(byte value)
        {
            // sub_100024D0 updates this+1B8 when the active object accepts the new byte.
            if (_disposed)
            {
                return false;
            }

            _value1B8 = value;
            return true;
        }

        internal bool BindSelection(int selection, int mode)
        {
            // sub_10002520 stores the selection in this+1BC after a successful vtable +0x14 call.
            if (_disposed || selection < 0 || mode < 0)
            {
                return false;
            }

            _value1BC = selection;
            _value1B8 = unchecked((byte)mode);
            return true;
        }

        internal bool GetSelection(out int selection)
        {
            // sub_10002560
            selection = _value1BC;
            return !_disposed;
        }

        internal bool DispatchProperty(int selector, int value, bool setMode, out int storedValue)
        {
            // sub_10002580 is a large selector-based dispatcher.
            // The control structure below mirrors the observable preamble and selector cache behavior.
            storedValue = 0;
            if (_disposed || selector < 0 || selector >= _propertyValues17C.Length)
            {
                return false;
            }

            object? dispatchObject = _value1C0;
            if (dispatchObject == null)
            {
                return false;
            }

            object? cachedObject = _cachedObjects148[selector];
            if (cachedObject == null && setMode)
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
            // Mirrors the directly stored fields in sub_10002380.
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
            // Mirrors this+1C0.
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