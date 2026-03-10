using Gedx8MusicDriver.Core;
using Gedx8MusicDriver.Models;

namespace Gedx8MusicDriver.Api
{
    public sealed class Gedx8DriverInstance : IDisposable
    {
        private readonly Gedx8GlobalRegistry _globalRegistry;
        private readonly Gedx8SlotTable<Gedx8LoadedObject> _slotTable04;
        private readonly Gedx8SlotTable<Gedx8LoadedObject> _slotTable08;
        private readonly Gedx8EngineCore _engine10;
        private bool _disposed;

        internal Gedx8DriverInstance(Gedx8GlobalRegistry globalRegistry)
        {
            _globalRegistry = globalRegistry;
            _slotTable04 = new Gedx8SlotTable<Gedx8LoadedObject>(1);
            _slotTable08 = new Gedx8SlotTable<Gedx8LoadedObject>(1);
            _engine10 = new Gedx8EngineCore();
        }

        public Gedx8SynthInitConfig SynthesizerConfig => _engine10.SynthInitConfig;

        public Gedx8LoaderMode LoaderMode => _engine10.LoaderMode1B0;

        public string? SearchDirectory => _engine10.SearchDirectory;

        public bool InitializeSynthesizer(Gedx8SynthInitConfig config)
        {
            ThrowIfDisposed();
            return _engine10.InitializeSynthesizer(config);
        }

        public bool InitializeSynthesizerForMode(int synthMode)
        {
            ThrowIfDisposed();
            return _engine10.InitializeSynthesizer(Gedx8SynthProfiles.Resolve(synthMode));
        }

        public bool Method10001940()
        {
            ThrowIfDisposed();
            return _engine10.Call03E00();
        }

        public bool Method10001950(int value)
        {
            ThrowIfDisposed();
            return _engine10.Call03E20(value);
        }

        public bool Method10001970(int value)
        {
            ThrowIfDisposed();
            return _engine10.Call03E50(value);
        }

        public bool PrepareLoaderContext(string searchDirectory)
        {
            ThrowIfDisposed();
            return _engine10.PrepareLoaderContext(searchDirectory);
        }

        public bool LoadObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory, out Gedx8LoadedObject? loadedObject)
        {
            ThrowIfDisposed();
            loadedObject = _engine10.LoadObject(kind, fileName, searchDirectory);
            if (loadedObject == null)
            {
                return false;
            }

            _slotTable08.Add(loadedObject);
            return true;
        }

        public bool Method10001AB0(int value0, int value1)
        {
            ThrowIfDisposed();
            return _engine10.Call03BC0(value0, value1);
        }

        public bool Method10001AD0(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();
            if (!loadedObject.IsActive)
            {
                return false;
            }

            int index = _slotTable08.IndexOf(loadedObject);
            if (index < 0)
            {
                return false;
            }

            loadedObject.Deactivate();
            _slotTable08.RemoveAt(index);
            return true;
        }

        public bool Method10001B50()
        {
            ThrowIfDisposed();
            return _engine10.Call03C90();
        }

        public bool Method10001B60()
        {
            ThrowIfDisposed();
            return _engine10.Call03CA0();
        }

        public bool Method10001CF0(byte value)
        {
            ThrowIfDisposed();
            return _engine10.SetSelectionByte(value);
        }

        public bool Method10001D10(int selection, int mode)
        {
            ThrowIfDisposed();
            return _engine10.BindSelection(selection, mode);
        }

        public bool Method10001D30(out int selection)
        {
            ThrowIfDisposed();
            return _engine10.GetSelection(out selection);
        }

        public bool Method10001D50(int selector, int value, out int storedValue)
        {
            ThrowIfDisposed();
            return _engine10.DispatchProperty(selector, value, true, out storedValue);
        }

        public bool Method10001D70(int selector, int value, out int storedValue)
        {
            ThrowIfDisposed();
            return _engine10.DispatchProperty(selector, value, false, out storedValue);
        }

        public bool Method10001E30(Gedx8LoadedObject loadedObject, int arg0, int arg1, int arg2, int arg3, int arg4)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04190(arg0, arg1, arg2, arg3, arg4);
        }

        public bool Method10001E70(Gedx8LoadedObject loadedObject, string? value)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04250(value);
        }

        public bool Method10001E90(Gedx8LoadedObject loadedObject, string? value)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04280(value);
        }

        public bool Method10001EB0(Gedx8LoadedObject loadedObject, string name, out string? resolvedValue)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall042C0(name, out resolvedValue);
        }

        public bool Method10001EE0(Gedx8LoadedObject loadedObject, string name, out string? resolvedValue)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04490(name, out resolvedValue);
        }

        public bool Method10001F10(Gedx8LoadedObject loadedObject, string? value)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04640(value);
        }

        public bool Method10001FF0(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();
            return loadedObject.TryThinType1Call02D50();
        }

        public bool Method10002010(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();
            if (!loadedObject.TryThinType1Call02D70())
            {
                return false;
            }

            loadedObject.Deactivate();
            _slotTable08.Remove(loadedObject);
            return true;
        }

        public bool Method100020D0(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();
            return loadedObject.TryThinType2Release03E90();
        }

        public bool Method100020F0(Gedx8LoadedObject loadedObject, string name)
        {
            ThrowIfDisposed();
            return loadedObject.TryThinType2Open03EB0(name);
        }

        public bool Method10002110(Gedx8LoadedObject loadedObject, int mode, int value, out string? result)
        {
            ThrowIfDisposed();
            result = null;
            return mode switch
            {
                0 => loadedObject.TryThinType2Query03F00(mode, value, out result),
                1 => loadedObject.TryThinType2Query03F50(mode, value, out result),
                2 => loadedObject.TryThinType2Query03FA0(mode, value, out result),
                _ => false,
            };
        }

        public bool Method10002180(Gedx8LoadedObject loadedObject, int mode, int value, string? text)
        {
            ThrowIfDisposed();
            return mode switch
            {
                0 => loadedObject.TryThinType2Configure04020(mode, value, text),
                1 => loadedObject.TryThinType2Configure04070(mode, value, text),
                2 => loadedObject.TryThinType2Configure040D0(mode, value, text),
                _ => false,
            };
        }

        public bool Method100021F0(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();
            if (!loadedObject.TryThinType2Release03E90())
            {
                return false;
            }

            _slotTable08.Remove(loadedObject);
            return true;
        }

        public bool Method100022B0()
        {
            ThrowIfDisposed();
            return false;
        }

        public IReadOnlyList<Gedx8LoadedObject> GetTable04Objects()
        {
            ThrowIfDisposed();
            return _slotTable04.GetLiveObjects();
        }

        public IReadOnlyList<Gedx8LoadedObject> GetTable08Objects()
        {
            ThrowIfDisposed();
            return _slotTable08.GetLiveObjects();
        }

        public void Shutdown()
        {
            if (_disposed)
            {
                return;
            }

            _slotTable04.Clear();
            _slotTable08.Clear();
            _engine10.Dispose();
            _disposed = true;
            _globalRegistry.Unregister(this);
        }

        internal void ShutdownFromRegistry()
        {
            if (_disposed)
            {
                return;
            }

            _slotTable04.Clear();
            _slotTable08.Clear();
            _engine10.Dispose();
            _disposed = true;
        }

        public void Dispose()
        {
            Shutdown();
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, typeof(Gedx8DriverInstance));
        }
    }
}
