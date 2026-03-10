using Gedx8MusicDriver.Api;
using Gedx8MusicDriver.Core;
using Gedx8MusicDriver.Models;

namespace Gedx8MusicDriver.Interop
{
    internal delegate bool Gedx8Entry00_10001000Delegate(int arg0, int arg1);

    internal delegate void Gedx8Slot00_10001090Delegate();
    internal delegate Gedx8DriverInstance? Gedx8Slot08_100013F0Delegate();

    internal delegate bool Gedx8Slot10_10001920Delegate(Gedx8DriverInstance instance, Gedx8SynthInitConfig config);
    internal delegate bool Gedx8Slot14_10001940Delegate(Gedx8DriverInstance instance);
    internal delegate bool Gedx8Slot18_10001950Delegate(Gedx8DriverInstance instance, int value);
    internal delegate bool Gedx8Slot1C_10001970Delegate(Gedx8DriverInstance instance, int value);

    internal delegate bool Gedx8Slot20_10001990Delegate(Gedx8DriverInstance instance, Gedx8ObjectKind kind, string fileName, string? searchDirectory, out Gedx8LoadedObject? loadedObject);
    internal delegate bool Gedx8Slot24_10001AB0Delegate(Gedx8DriverInstance instance, int value0, int value1);
    internal delegate bool Gedx8Slot28_10001AD0Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject);
    internal delegate bool Gedx8Slot2C_10001B50Delegate(Gedx8DriverInstance instance);
    internal delegate bool Gedx8Slot30_10001B60Delegate(Gedx8DriverInstance instance);

    internal delegate bool Gedx8Slot34_10001D10Delegate(Gedx8DriverInstance instance, int selection, int mode);
    internal delegate bool Gedx8Slot38_10001D50Delegate(Gedx8DriverInstance instance, int selector, int value, out int storedValue);
    internal delegate bool Gedx8Slot3C_10001D70Delegate(Gedx8DriverInstance instance, int selector, int value, out int storedValue);

    internal delegate bool Gedx8Slot40_10001E30Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject, int arg0, int arg1, int arg2, int arg3, int arg4);
    internal delegate bool Gedx8Slot44_10001E70Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject, string? value);
    internal delegate bool Gedx8Slot48_10001E90Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject, out byte readyByte);
    internal delegate bool Gedx8Slot4C_10001EB0Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject, string name, out string? resolvedValue);
    internal delegate bool Gedx8Slot50_10001EE0Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject, string name, out string? resolvedValue);

    internal delegate bool Gedx8Slot54_100020D0Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject);
    internal delegate bool Gedx8Slot58_100020F0Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject, string name);
    internal delegate bool Gedx8Slot5C_10002110Delegate(Gedx8DriverInstance instance, Gedx8LoadedObject loadedObject, int mode, int value, out string? result);
    internal delegate bool Gedx8Slot6C_100022B0Delegate(Gedx8DriverInstance instance);

    internal sealed class Gedx8InterfaceMethodTable
    {
        internal Gedx8InterfaceMethodTable()
        {
            Slot00_10001090 = Gedx8GlobalRegistry.Shared.Sub10001090;
            Slot08_100013F0 = Gedx8GlobalRegistry.Shared.Sub100013F0;

            Slot10_10001920 = static (instance, config) => instance.InitializeSynthesizer(config);
            Slot14_10001940 = static instance => instance.Method10001940();
            Slot18_10001950 = static (instance, value) => instance.Method10001950(value);
            Slot1C_10001970 = static (instance, value) => instance.Method10001970(value);

            Slot20_10001990 = static (instance, kind, fileName, searchDirectory, out loadedObject) => instance.LoadObject(kind, fileName, searchDirectory, out loadedObject);
            Slot24_10001AB0 = static (instance, value0, value1) => instance.Method10001AB0(value0, value1);
            Slot28_10001AD0 = static (instance, loadedObject) => instance.Method10001AD0(loadedObject);
            Slot2C_10001B50 = static instance => instance.Method10001B50();
            Slot30_10001B60 = static instance => instance.Method10001B60();

            Slot34_10001D10 = static (instance, selection, mode) => instance.Method10001D10(selection, mode);
            Slot38_10001D50 = static (instance, selector, value, out storedValue) => instance.Method10001D50(selector, value, out storedValue);
            Slot3C_10001D70 = static (instance, selector, value, out storedValue) => instance.Method10001D70(selector, value, out storedValue);

            Slot40_10001E30 = static (instance, loadedObject, arg0, arg1, arg2, arg3, arg4) => instance.Method10001E30(loadedObject, arg0, arg1, arg2, arg3, arg4);
            Slot44_10001E70 = static (instance, loadedObject, value) => instance.Method10001E70(loadedObject, value);
            Slot48_10001E90 = static (instance, loadedObject, out readyByte) => instance.Method10001E90(loadedObject, out readyByte);
            Slot4C_10001EB0 = static (instance, loadedObject, name, out resolvedValue) => instance.Method10001EB0(loadedObject, name, out resolvedValue);
            Slot50_10001EE0 = static (instance, loadedObject, name, out resolvedValue) => instance.Method10001EE0(loadedObject, name, out resolvedValue);

            Slot54_100020D0 = static (instance, loadedObject) => instance.Method100020D0(loadedObject);
            Slot58_100020F0 = static (instance, loadedObject, name) => instance.Method100020F0(loadedObject, name);
            Slot5C_10002110 = static (instance, loadedObject, mode, value, out result) => instance.Method10002110(loadedObject, mode, value, out result);
            Slot6C_100022B0 = static instance => instance.Method100022B0();
        }

        internal Gedx8Slot00_10001090Delegate Slot00_10001090 { get; }
        internal Gedx8Slot08_100013F0Delegate Slot08_100013F0 { get; }

        internal Gedx8Slot10_10001920Delegate Slot10_10001920 { get; }
        internal Gedx8Slot14_10001940Delegate Slot14_10001940 { get; }
        internal Gedx8Slot18_10001950Delegate Slot18_10001950 { get; }
        internal Gedx8Slot1C_10001970Delegate Slot1C_10001970 { get; }

        internal Gedx8Slot20_10001990Delegate Slot20_10001990 { get; }
        internal Gedx8Slot24_10001AB0Delegate Slot24_10001AB0 { get; }
        internal Gedx8Slot28_10001AD0Delegate Slot28_10001AD0 { get; }
        internal Gedx8Slot2C_10001B50Delegate Slot2C_10001B50 { get; }
        internal Gedx8Slot30_10001B60Delegate Slot30_10001B60 { get; }

        internal Gedx8Slot34_10001D10Delegate Slot34_10001D10 { get; }
        internal Gedx8Slot38_10001D50Delegate Slot38_10001D50 { get; }
        internal Gedx8Slot3C_10001D70Delegate Slot3C_10001D70 { get; }

        internal Gedx8Slot40_10001E30Delegate Slot40_10001E30 { get; }
        internal Gedx8Slot44_10001E70Delegate Slot44_10001E70 { get; }
        internal Gedx8Slot48_10001E90Delegate Slot48_10001E90 { get; }
        internal Gedx8Slot4C_10001EB0Delegate Slot4C_10001EB0 { get; }
        internal Gedx8Slot50_10001EE0Delegate Slot50_10001EE0 { get; }

        internal Gedx8Slot54_100020D0Delegate Slot54_100020D0 { get; }
        internal Gedx8Slot58_100020F0Delegate Slot58_100020F0 { get; }
        internal Gedx8Slot5C_10002110Delegate Slot5C_10002110 { get; }
        internal Gedx8Slot6C_100022B0Delegate Slot6C_100022B0 { get; }
    }

    internal sealed class Gedx8RawInterfaceObject
    {
        internal Gedx8Entry00_10001000Delegate? Entry00_10001000 { get; private set; }

        internal Gedx8InterfaceMethodTable? MethodTable04 { get; private set; }

        internal void Sub10001030(Gedx8InterfaceMethodTable methodTable)
        {
            Entry00_10001000 = Gedx8Exports.Sub10001000;
            MethodTable04 = methodTable;
        }
    }

    internal static class Gedx8Exports
    {
        private static readonly object s_sync = new();
        private static readonly Gedx8RawInterfaceObject s_rawInterface = new();
        private static readonly Gedx8InterfaceMethodTable s_methodTable = new();
        private static bool s_staticConstructorRegistered;

        static Gedx8Exports()
        {
            Sub10001010();
        }

        internal static bool Sub10001000(int arg0, int arg1)
        {
            return true;
        }

        internal static Gedx8RawInterfaceObject Sub10001010()
        {
            lock (s_sync)
            {
                Sub10001020();
                Sub10001040();
                return s_rawInterface;
            }
        }

        internal static Gedx8RawInterfaceObject Sub10001020()
        {
            return Sub10001030(s_rawInterface);
        }

        internal static Gedx8RawInterfaceObject Sub10001030(Gedx8RawInterfaceObject target)
        {
            target.Sub10001030(s_methodTable);
            return target;
        }

        internal static void Sub10001040()
        {
            if (s_staticConstructorRegistered)
            {
                return;
            }

            s_staticConstructorRegistered = true;
        }

        internal static Gedx8RawInterfaceObject Sub10001050()
        {
            return Sub10001060(s_rawInterface);
        }

        internal static Gedx8RawInterfaceObject Sub10001060(Gedx8RawInterfaceObject target)
        {
            return target;
        }

        internal static bool Sub10001070(Gedx8RawInterfaceObject?[]? interfaceOut)
        {
            if (interfaceOut == null || interfaceOut.Length == 0)
            {
                return false;
            }

            interfaceOut[0] = s_rawInterface;
            return true;
        }

        internal static bool GetInterface2(out Gedx8RawInterfaceObject? rawInterface)
        {
            Gedx8RawInterfaceObject?[] interfaceOut = new Gedx8RawInterfaceObject?[1];
            bool result = Sub10001070(interfaceOut);
            rawInterface = interfaceOut[0];
            return result;
        }
    }
}