using Gedx8MusicDriver.Api;
using Gedx8MusicDriver.Interop;

namespace Gedx8MusicDriver.Core
{
    internal sealed class Gedx8GlobalRegistry
    {
        private static readonly Gedx8GlobalRegistry s_shared = new();
        private readonly Gedx8SlotTable<Gedx8DriverInstance> _instances = new(1);

        private Gedx8GlobalRegistry()
        {
        }

        internal static Gedx8GlobalRegistry Shared => s_shared;

        internal bool IsBootstrapped { get; private set; }

        internal int Sub100013D0()
        {
            nint moduleHandle = NativeMethods.LoadLibraryA("D3D8.DLL");
            if (moduleHandle == 0)
            {
                return 0;
            }

            NativeMethods.FreeLibrary(moduleHandle);
            return 0x800;
        }

        internal void Sub10001090()
        {
            Sub100013D0();
            _instances.EnsureCapacityExact10002330(1);
            _instances.Clear();
            IsBootstrapped = true;
        }

        internal bool Sub100010D0()
        {
            for (int index = _instances.HighWaterMark - 1; index >= 0; index--)
            {
                Gedx8DriverInstance? instance = _instances.GetAt(index);
                if (instance == null)
                {
                    continue;
                }

                if (instance.ActiveByte00 == 0)
                {
                    continue;
                }

                instance.Sub100010D0DestroyContents();
                _instances.RemoveAt(index);
            }

            IsBootstrapped = false;
            return true;
        }

        internal Gedx8DriverInstance? Sub100013F0()
        {
            if (!IsBootstrapped)
            {
                Sub10001090();
            }

            Gedx8DriverInstance? instance = Gedx8DriverInstance.Sub100013F0Create(this);
            if (instance == null)
            {
                return null;
            }

            _instances.Add(instance);
            return instance;
        }

        internal bool Sub10001630(Gedx8DriverInstance instance)
        {
            for (int index = _instances.HighWaterMark - 1; index >= 0; index--)
            {
                Gedx8DriverInstance? candidate = _instances.GetAt(index);
                if (!ReferenceEquals(candidate, instance))
                {
                    continue;
                }

                if (candidate == null || candidate.ActiveByte00 == 0)
                {
                    return false;
                }

                candidate.Sub10001630DestroyContents();
                _instances.RemoveAt(index);
                return true;
            }

            return false;
        }

        internal void Register(Gedx8DriverInstance instance)
        {
            _instances.Add(instance);
        }

        internal void Unregister(Gedx8DriverInstance instance)
        {
            Sub10001630(instance);
        }

        internal void Reset()
        {
            Sub100010D0();
        }
    }
}