using System.Runtime.InteropServices;
using Gedx8MusicDriver.Core;
using Gedx8MusicDriver.Models;

namespace Gedx8MusicDriver.Interop
{
    internal sealed class Gedx8DriverInstance : IDisposable
    {
        private readonly Gedx8GlobalRegistry _globalRegistry;
        private readonly Gedx8SlotTable<Gedx8LoadedObject> _slotTable04;
        private readonly Gedx8SlotTable<Gedx8LoadedObject> _slotTable08;
        private readonly Gedx8EngineCore _engine10;
        private Gedx8Block08_10003CC0? _block0C;
        private Gedx8Block14_10003810? _block10;
        private Gedx8Block0C_10002D90? _block14;
        private bool _disposed;
        private readonly Dictionary<Gedx8Audiopath, int> _audiopathTokens = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<Gedx8LoadedObject, byte> _segmentPlaybackStates = new(ReferenceEqualityComparer.Instance);
        private int _nextAudiopathToken;

        private Gedx8DriverInstance(Gedx8GlobalRegistry globalRegistry)
        {
            _globalRegistry = globalRegistry;
            _slotTable04 = new Gedx8SlotTable<Gedx8LoadedObject>(1);
            _slotTable08 = new Gedx8SlotTable<Gedx8LoadedObject>(1);
            _engine10 = new Gedx8EngineCore();

            _block0C = new Gedx8Block08_10003CC0();
            _block14 = new Gedx8Block0C_10002D90(_block0C);
            _block10 = new Gedx8Block14_10003810(_block0C, _block14);

            ActiveByte00 = 0;
        }

        internal byte ActiveByte00 { get; private set; }

        internal Gedx8SynthInitConfig SynthesizerConfig => _engine10.SynthInitConfig;

        internal Gedx8LoaderMode LoaderMode => _engine10.LoaderMode1B0;

        internal string? SearchDirectory => _engine10.SearchDirectory;

        internal static Gedx8DriverInstance? Sub100013F0Create(Gedx8GlobalRegistry globalRegistry)
        {
            Gedx8DriverInstance instance = new(globalRegistry)
            {
                ActiveByte00 = 1
            };
            return instance;
        }

        internal bool InitializeSynthesizer(Gedx8SynthInitConfig config)
        {
            ThrowIfDisposed();
            return _engine10.InitializeSynthesizer(config);
        }

        internal bool InitializeSynthesizerForMode(int synthMode)
        {
            ThrowIfDisposed();
            return _engine10.InitializeSynthesizer(Gedx8SynthProfiles.Resolve(synthMode));
        }

        internal bool Method10001940()
        {
            ThrowIfDisposed();
            return _engine10.Call03E00();
        }

        internal bool Method10001950(int value)
        {
            ThrowIfDisposed();
            return _engine10.Call03E20(value);
        }

        internal bool Method10001970(int value)
        {
            ThrowIfDisposed();
            return _engine10.Call03E50(value);
        }

        internal bool PrepareLoaderContext(string searchDirectory)
        {
            ThrowIfDisposed();
            return _engine10.PrepareLoaderContext(searchDirectory);
        }

        internal bool LoadObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory, out Gedx8LoadedObject? loadedObject)
        {
            ThrowIfDisposed();

            loadedObject = _engine10.LoadObject(kind, fileName, searchDirectory, this, _block10);
            if (loadedObject == null)
            {
                return false;
            }

            RegisterLoadedObject10001990(loadedObject);
            return true;
        }

        internal bool Method10001AB0(int value0, int value1)
        {
            ThrowIfDisposed();
            return _engine10.Call03BC0(value0, value1);
        }

        internal bool Method10001AD0(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();

            if (!DestroyLoadedObject10003C30(loadedObject, true))
            {
                return false;
            }

            RemoveLoadedObjectFromTable0810001AD0(loadedObject);
            return true;
        }

        internal bool Method10001B50()
        {
            ThrowIfDisposed();
            return _engine10.Call03C90();
        }

        internal bool Method10001B60()
        {
            ThrowIfDisposed();
            return _engine10.Call03CA0();
        }

        internal bool Method10001B70(IntPtr configPointer, Gedx8LoadedObject? seedObject, out Gedx8Audiopath? audiopath)
        {
            ThrowIfDisposed();

            if (!_engine10.IsSynthInitialized)
            {
                audiopath = null;
                return false;
            }

            int token = unchecked(++_nextAudiopathToken);
            int profile = SynthesizerConfig.Config;
            if (configPointer != IntPtr.Zero)
            {
                profile = Marshal.ReadInt32(configPointer, 4);
            }

            audiopath = new Gedx8Audiopath(token, profile, seedObject);
            _audiopathTokens[audiopath] = token;
            return true;
        }

        internal bool Method10001CF0ActivateAudiopath(Gedx8Audiopath audiopath, int activeState)
        {
            ThrowIfDisposed();

            if (!_audiopathTokens.ContainsKey(audiopath))
            {
                return false;
            }

            audiopath.SetActive(activeState != 0);
            return true;
        }

        internal bool Method10001D10SetVolumeOfAudiopath(Gedx8Audiopath audiopath, int volume, int fadeMilliseconds)
        {
            ThrowIfDisposed();

            if (!_audiopathTokens.ContainsKey(audiopath))
            {
                return false;
            }

            audiopath.SetVolume(volume, fadeMilliseconds);
            return true;
        }

        internal bool Method10001CF0(byte value)
        {
            ThrowIfDisposed();
            return _engine10.SetSelectionByte(value);
        }

        internal bool Method10001D10(int selection, int mode)
        {
            ThrowIfDisposed();
            return _engine10.BindSelection(selection, mode);
        }

        internal bool Method10001D30(out int selection)
        {
            ThrowIfDisposed();
            return _engine10.GetSelection(out selection);
        }

        internal bool Method10001D50(int selector, int value, out int storedValue)
        {
            ThrowIfDisposed();
            return _engine10.DispatchProperty(selector, value, true, out storedValue);
        }

        internal bool Method10001D70(int selector, int value, out int storedValue)
        {
            ThrowIfDisposed();
            return _engine10.DispatchProperty(selector, value, false, out storedValue);
        }

        internal bool Method10001DA0DestroyAudiopath(Gedx8Audiopath audiopath)
        {
            ThrowIfDisposed();

            if (!_audiopathTokens.Remove(audiopath))
            {
                return false;
            }

            audiopath.SetActive(false);
            return true;
        }

        internal bool Method10001E30StartSegmentPlayback(Gedx8Audiopath audiopath, Gedx8LoadedObject loadedObject, int flags, int startTime, int repeatCount, int reserved, int immediateFlag)
        {
            ThrowIfDisposed();

            if (!_audiopathTokens.ContainsKey(audiopath) || !loadedObject.IsActive)
            {
                return false;
            }

            if (!audiopath.IsActive)
            {
                return false;
            }

            _segmentPlaybackStates[loadedObject] = 1;
            audiopath.AttachSegment(loadedObject, flags, startTime, repeatCount, reserved, immediateFlag);
            return true;
        }

        internal bool Method10001E70ResetSegmentPlayback(Gedx8LoadedObject loadedObject, int stopMode)
        {
            ThrowIfDisposed();

            if (!loadedObject.IsActive)
            {
                return false;
            }

            _segmentPlaybackStates[loadedObject] = 0;
            return true;
        }

        internal bool Method10001E90GetPlaybackStateOfSegment(Gedx8LoadedObject loadedObject, out byte state)
        {
            ThrowIfDisposed();

            state = 0;
            if (!loadedObject.IsActive)
            {
                return false;
            }

            if (_segmentPlaybackStates.TryGetValue(loadedObject, out byte storedState))
            {
                state = storedState;
            }

            return true;
        }

        internal bool Method10001E30(Gedx8LoadedObject loadedObject, int arg0, int arg1, int arg2, int arg3, int arg4)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04190(arg0, arg1, arg2, arg3, arg4);
        }

        internal bool Method10001E70(Gedx8LoadedObject loadedObject, IntPtr rawValue)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04250(rawValue);
        }

        internal bool Method10001E90(Gedx8LoadedObject loadedObject, out byte readyByte)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04280(out readyByte);
        }

        internal bool Method10001EB0(Gedx8LoadedObject loadedObject, int mode, IntPtr structurePointer)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall042C0(mode, structurePointer);
        }

        internal bool Method10001EE0(Gedx8LoadedObject loadedObject, int mode, IntPtr structurePointer)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04490(mode, structurePointer);
        }

        internal bool Method10001F10(Gedx8LoadedObject loadedObject, out byte value0, out byte value1)
        {
            ThrowIfDisposed();
            return loadedObject.TryCompositeCall04640(out value0, out value1);
        }

        internal bool Method10001F30DestroySegment(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();

            _segmentPlaybackStates.Remove(loadedObject);

            if (!DestroyLoadedObject10003C30(loadedObject, true))
            {
                return false;
            }

            RemoveLoadedObjectFromTable0810001AD0(loadedObject);
            return true;
        }

        internal bool Method10001FF0(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();
            return loadedObject.TryThinType1Call02D50();
        }

        internal bool Method10002010(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();

            if (!loadedObject.TryThinType1Call02D70())
            {
                return false;
            }

            if (!loadedObject.ReleaseThinObject10003C30(true))
            {
                return false;
            }

            RemoveLoadedObjectFromTable0810001AD0(loadedObject);
            return true;
        }

        internal bool Method100020D0(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();
            return loadedObject.TryThinType2Release03E90();
        }

        internal bool Method100020F0(Gedx8LoadedObject loadedObject, string name)
        {
            ThrowIfDisposed();
            return loadedObject.TryThinType2Open03EB0(name);
        }

        internal bool Method10002110(Gedx8LoadedObject loadedObject, string name, int mode, IntPtr payloadPointer)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return mode switch
            {
                0 => loadedObject.TryThinType2Query03F00(name, payloadPointer),
                1 => loadedObject.TryThinType2Query03F50(name, payloadPointer),
                2 => loadedObject.TryThinType2Query03FA0(name, payloadPointer),
                _ => false,
            };
        }

        internal bool Method10002180(Gedx8LoadedObject loadedObject, string name, int mode, IntPtr payloadPointer)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return mode switch
            {
                0 => loadedObject.TryThinType2Configure04020(name, payloadPointer),
                1 => loadedObject.TryThinType2Configure04070(name, payloadPointer),
                2 => loadedObject.TryThinType2Configure040D0(name, payloadPointer),
                _ => false,
            };
        }

        internal bool Method100021F0(Gedx8LoadedObject loadedObject)
        {
            ThrowIfDisposed();

            _segmentPlaybackStates.Remove(loadedObject);

            if (!loadedObject.TryThinType2Release03E90())
            {
                return false;
            }

            if (!loadedObject.ReleaseThinObject10003C30(false))
            {
                return false;
            }

            RemoveLoadedObjectFromTable0810001AD0(loadedObject);
            return true;
        }

        internal bool Method100022B0()
        {
            ThrowIfDisposed();
            return false;
        }

        internal IReadOnlyList<Gedx8LoadedObject> GetTable04Objects()
        {
            ThrowIfDisposed();
            return _slotTable04.GetLiveObjects();
        }

        internal IReadOnlyList<Gedx8LoadedObject> GetTable08Objects()
        {
            ThrowIfDisposed();
            return _slotTable08.GetLiveObjects();
        }

        internal void Shutdown()
        {
            if (_disposed)
            {
                return;
            }

            _globalRegistry.Sub10001630(this);
        }

        internal void ShutdownFromRegistry()
        {
            Sub10001630DestroyContents();
        }

        internal void Sub100010D0DestroyContents()
        {
            if (_disposed)
            {
                ActiveByte00 = 0;
                return;
            }

            DestroyBlock0C100010D0();
            DestroyBlock10100010D0();
            DestroyBlock14100010D0();
            DestroyTable04100010D0();
            DestroyTable08100010D0();

            _engine10.Dispose();
            ActiveByte00 = 0;
            _disposed = true;
        }

        internal void Sub10001630DestroyContents()
        {
            if (_disposed)
            {
                ActiveByte00 = 0;
                return;
            }

            DestroyBlock0C100010D0();
            DestroyBlock10100010D0();
            DestroyBlock14100010D0();
            DestroyTable04100010D0();
            DestroyTable08100010D0();

            _engine10.Dispose();
            ActiveByte00 = 0;
            _disposed = true;
        }

        private void RegisterLoadedObject10001990(Gedx8LoadedObject loadedObject)
        {
            loadedObject.Activate();
            _slotTable08.Add(loadedObject);
        }

        private void RemoveLoadedObjectFromTable0810001AD0(Gedx8LoadedObject loadedObject)
        {
            _segmentPlaybackStates.Remove(loadedObject);

            int index = _slotTable08.IndexOf(loadedObject);
            if (index >= 0)
            {
                loadedObject.Deactivate();
                _slotTable08.RemoveAt(index);
            }

            Gedx8Exports.NotifyLoadedObjectDestroyed(loadedObject);
        }

        private void DestroyBlock0C100010D0()
        {
            Gedx8Block08_10003CC0? block0C = _block0C;
            if (block0C == null)
            {
                return;
            }

            block0C.Sub10003CF0();
            _block0C = null;
        }

        private void DestroyBlock10100010D0()
        {
            Gedx8Block14_10003810? block10 = _block10;
            if (block10 == null)
            {
                return;
            }

            block10.Sub10003860();
            _block10 = null;
        }

        private void DestroyBlock14100010D0()
        {
            Gedx8Block0C_10002D90? block14 = _block14;
            if (block14 == null)
            {
                return;
            }

            block14.Sub10002DC0();
            _block14 = null;
        }

        private void DestroyTable04100010D0()
        {
            for (int index = _slotTable04.HighWaterMark - 1; index >= 0; index--)
            {
                Gedx8LoadedObject? loadedObject = _slotTable04.GetAt(index);
                if (loadedObject == null)
                {
                    continue;
                }

                DestroyTable04Entry100010D0(loadedObject);
                loadedObject.Deactivate();
                _slotTable04.RemoveAt(index);
                Gedx8Exports.NotifyLoadedObjectDestroyed(loadedObject);
            }
        }

        private void DestroyTable08100010D0()
        {
            for (int index = _slotTable08.HighWaterMark - 1; index >= 0; index--)
            {
                Gedx8LoadedObject? loadedObject = _slotTable08.GetAt(index);
                if (loadedObject == null)
                {
                    continue;
                }

                DestroyTable08Entry100010D0(loadedObject);
                loadedObject.Deactivate();
                _slotTable08.RemoveAt(index);
                Gedx8Exports.NotifyLoadedObjectDestroyed(loadedObject);
            }
        }

        private static void DestroyTable04Entry100010D0(Gedx8LoadedObject loadedObject)
        {
            loadedObject.Deactivate();
        }

        private static void DestroyTable08Entry100010D0(Gedx8LoadedObject loadedObject)
        {
            switch (loadedObject.Kind)
            {
                case Gedx8ObjectKind.Composite:
                    loadedObject.Sub10004170();
                    break;

                case Gedx8ObjectKind.ThinType1:
                    loadedObject.ReleaseThinObject10003C30(false);
                    break;

                case Gedx8ObjectKind.ThinType2:
                    loadedObject.TryThinType2Release03E90();
                    loadedObject.ReleaseThinObject10003C30(false);
                    break;
            }
        }

        private static bool DestroyLoadedObject10003C30(Gedx8LoadedObject loadedObject, bool notifyRuntime)
        {
            if (!loadedObject.IsActive)
            {
                return false;
            }

            return loadedObject.Kind switch
            {
                Gedx8ObjectKind.Composite => DestroyCompositeObject10003C30(loadedObject),
                Gedx8ObjectKind.ThinType1 => loadedObject.ReleaseThinObject10003C30(notifyRuntime),
                Gedx8ObjectKind.ThinType2 => loadedObject.ReleaseThinObject10003C30(notifyRuntime),
                _ => false,
            };
        }

        private static bool DestroyCompositeObject10003C30(Gedx8LoadedObject loadedObject)
        {
            if (!loadedObject.IsActive)
            {
                return false;
            }

            loadedObject.Sub10004170();
            return true;
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

    internal sealed class Gedx8Block08_10003CC0
    {
        internal Gedx8Block08_10003CC0()
        {
            Active00 = 1;
        }

        internal byte Active00 { get; private set; }

        internal void Sub10003CF0()
        {
            Active00 = 0;
        }
    }

    internal sealed class Gedx8Block0C_10002D90
    {
        internal Gedx8Block0C_10002D90(Gedx8Block08_10003CC0 owner08)
        {
            Owner08 = owner08;
            Active00 = 1;
        }

        internal byte Active00 { get; private set; }

        internal Gedx8Block08_10003CC0? Owner08 { get; private set; }

        internal void Sub10002DC0()
        {
            Owner08 = null;
            Active00 = 0;
        }
    }

    internal sealed class Gedx8Block14_10003810
    {
        internal Gedx8Block14_10003810(Gedx8Block08_10003CC0 owner0C, Gedx8Block0C_10002D90 link10)
        {
            Owner0C = owner0C;
            Link10 = link10;
            Active00 = 1;
        }

        internal byte Active00 { get; private set; }

        internal Gedx8Block08_10003CC0? Owner0C { get; private set; }

        internal Gedx8Block0C_10002D90? Link10 { get; private set; }

        internal void Sub10003860()
        {
            Owner0C = null;
            Link10 = null;
            Active00 = 0;
        }
    }
}