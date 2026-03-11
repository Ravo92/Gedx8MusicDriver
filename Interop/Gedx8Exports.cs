using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Gedx8MusicDriver.Api;
using Gedx8MusicDriver.Core;
using Gedx8MusicDriver.Models;

namespace Gedx8MusicDriver.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Gedx8NativeSynthInitConfig
    {
        public int Reserved00;
        public int SampleRate;
        public int Config;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Gedx8NativeMethodTable
    {
        public IntPtr Slot00_10001090;
        public IntPtr Slot08_100013F0;

        public IntPtr Slot10_10001920;
        public IntPtr Slot14_10001940;
        public IntPtr Slot18_10001950;
        public IntPtr Slot1C_10001970;

        public IntPtr Slot20_10001990;
        public IntPtr Slot24_10001AB0;
        public IntPtr Slot28_10001AD0;
        public IntPtr Slot2C_10001B50;
        public IntPtr Slot30_10001B60;

        public IntPtr Slot34_10001D10;
        public IntPtr Slot38_10001D50;
        public IntPtr Slot3C_10001D70;

        public IntPtr Slot40_10001E30;
        public IntPtr Slot44_10001E70;
        public IntPtr Slot48_10001E90;
        public IntPtr Slot4C_10001EB0;
        public IntPtr Slot50_10001EE0;

        public IntPtr Slot54_10002010;
        public IntPtr Slot58_100020D0;
        public IntPtr Slot5C_100020F0;
        public IntPtr Slot60_10002110;
        public IntPtr Slot64_10002180;
        public IntPtr Slot68_100021F0;
        public IntPtr Slot6C_100022B0;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Gedx8NativeRawInterfaceObject
    {
        public IntPtr Entry00_10001000;
        public IntPtr MethodTable04;
    }

    internal static unsafe class Gedx8Exports
    {
        private static readonly object s_sync = new();
        private static readonly IntPtr s_methodTablePointer;
        private static readonly IntPtr s_rawInterfacePointer;

        private static readonly Dictionary<IntPtr, GCHandle> s_driverHandlesByPointer = new();
        private static readonly Dictionary<Gedx8DriverInstance, IntPtr> s_driverPointersByInstance = new(ReferenceEqualityComparer.Instance);

        private static readonly Dictionary<IntPtr, GCHandle> s_loadedObjectHandlesByPointer = new();
        private static readonly Dictionary<Gedx8LoadedObject, IntPtr> s_loadedObjectPointersByInstance = new(ReferenceEqualityComparer.Instance);

        private static bool s_initialized;

        static Gedx8Exports()
        {
            s_methodTablePointer = Marshal.AllocHGlobal(Marshal.SizeOf<Gedx8NativeMethodTable>());
            s_rawInterfacePointer = Marshal.AllocHGlobal(Marshal.SizeOf<Gedx8NativeRawInterfaceObject>());
            InitializeNativeSurface();
        }

        internal static IntPtr RawInterfacePointer => s_rawInterfacePointer;

        internal static void NotifyDriverDestroyed(Gedx8DriverInstance instance)
        {
            lock (s_sync)
            {
                if (!s_driverPointersByInstance.TryGetValue(instance, out IntPtr handlePointer))
                {
                    return;
                }

                ReleaseDriverHandleNoLock(handlePointer, instance);
            }
        }

        internal static void NotifyLoadedObjectDestroyed(Gedx8LoadedObject loadedObject)
        {
            lock (s_sync)
            {
                if (!s_loadedObjectPointersByInstance.TryGetValue(loadedObject, out IntPtr handlePointer))
                {
                    return;
                }

                ReleaseLoadedObjectHandleNoLock(handlePointer, loadedObject);
            }
        }

        private static void InitializeNativeSurface()
        {
            lock (s_sync)
            {
                if (s_initialized)
                {
                    return;
                }

                Gedx8NativeMethodTable methodTable = new Gedx8NativeMethodTable
                {
                    Slot00_10001090 = (IntPtr)(delegate* unmanaged[Stdcall]<void>)&Slot00_10001090,
                    Slot08_100013F0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr>)&Slot08_100013F0,

                    Slot10_10001920 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, Gedx8NativeSynthInitConfig, byte>)&Slot10_10001920,
                    Slot14_10001940 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, byte>)&Slot14_10001940,
                    Slot18_10001950 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, int, byte>)&Slot18_10001950,
                    Slot1C_10001970 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, int, byte>)&Slot1C_10001970,

                    Slot20_10001990 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, int, IntPtr, IntPtr, IntPtr, byte>)&Slot20_10001990,
                    Slot24_10001AB0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, int, int, byte>)&Slot24_10001AB0,
                    Slot28_10001AD0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, byte>)&Slot28_10001AD0,
                    Slot2C_10001B50 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, byte>)&Slot2C_10001B50,
                    Slot30_10001B60 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, byte>)&Slot30_10001B60,

                    Slot34_10001D10 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, int, int, byte>)&Slot34_10001D10,
                    Slot38_10001D50 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, int, int, IntPtr, byte>)&Slot38_10001D50,
                    Slot3C_10001D70 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, int, int, IntPtr, byte>)&Slot3C_10001D70,

                    Slot40_10001E30 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, int, int, int, int, byte>)&Slot40_10001E30,
                    Slot44_10001E70 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, byte>)&Slot44_10001E70,
                    Slot48_10001E90 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, byte>)&Slot48_10001E90,
                    Slot4C_10001EB0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr, byte>)&Slot4C_10001EB0,
                    Slot50_10001EE0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr, byte>)&Slot50_10001EE0,

                    Slot54_10002010 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, byte>)&Slot54_10002010,
                    Slot58_100020D0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, byte>)&Slot58_100020D0,
                    Slot5C_100020F0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, byte>)&Slot5C_100020F0,
                    Slot60_10002110 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, int, IntPtr, byte>)&Slot60_10002110,
                    Slot64_10002180 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, int, int, IntPtr, byte>)&Slot64_10002180,
                    Slot68_100021F0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, IntPtr, byte>)&Slot68_100021F0,
                    Slot6C_100022B0 = (IntPtr)(delegate* unmanaged[Stdcall]<IntPtr, byte>)&Slot6C_100022B0,
                };

                Gedx8NativeRawInterfaceObject rawInterface = new Gedx8NativeRawInterfaceObject
                {
                    Entry00_10001000 = (IntPtr)(delegate* unmanaged[Stdcall]<int, int, byte>)&Sub10001000,
                    MethodTable04 = s_methodTablePointer,
                };

                Marshal.StructureToPtr(methodTable, s_methodTablePointer, false);
                Marshal.StructureToPtr(rawInterface, s_rawInterfacePointer, false);

                Gedx8GlobalRegistry.Shared.Sub10001090();
                s_initialized = true;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "GetInterface2", CallConvs = new[] { typeof(CallConvStdcall) })]
        public static byte GetInterface2Export(IntPtr interfaceOut)
        {
            InitializeNativeSurface();

            if (interfaceOut == IntPtr.Zero)
            {
                return 0;
            }

            Marshal.WriteIntPtr(interfaceOut, s_rawInterfacePointer);
            return 1;
        }

        [UnmanagedCallersOnly(EntryPoint = "Gedx8Wrapper10001CF0", CallConvs = new[] { typeof(CallConvStdcall) })]
        public static byte Wrapper10001CF0Export(IntPtr instanceHandle, byte value)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method10001CF0(value));
        }

        [UnmanagedCallersOnly(EntryPoint = "Gedx8Wrapper10001D30", CallConvs = new[] { typeof(CallConvStdcall) })]
        public static byte Wrapper10001D30Export(IntPtr instanceHandle, IntPtr selectionOut)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            if (instance == null || selectionOut == IntPtr.Zero)
            {
                return 0;
            }

            bool result = instance.Method10001D30(out int selection);
            Marshal.WriteInt32(selectionOut, selection);
            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(EntryPoint = "Gedx8Wrapper10001F10", CallConvs = new[] { typeof(CallConvStdcall) })]
        public static byte Wrapper10001F10Export(IntPtr instanceHandle, IntPtr loadedObjectHandle, IntPtr value0Out, IntPtr value1Out)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            if (instance == null || loadedObject == null || value0Out == IntPtr.Zero || value1Out == IntPtr.Zero)
            {
                return 0;
            }

            bool result = instance.Method10001F10(loadedObject, out byte value0, out byte value1);
            Marshal.WriteByte(value0Out, value0);
            Marshal.WriteByte(value1Out, value1);
            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(EntryPoint = "Gedx8Wrapper10001FF0", CallConvs = new[] { typeof(CallConvStdcall) })]
        public static byte Wrapper10001FF0Export(IntPtr instanceHandle, IntPtr loadedObjectHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            return ToNativeBool(instance != null && loadedObject != null && instance.Method10001FF0(loadedObject));
        }

        internal static bool GetInterface2(out IntPtr rawInterface)
        {
            InitializeNativeSurface();
            rawInterface = s_rawInterfacePointer;
            return true;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Sub10001000(int arg0, int arg1)
        {
            return 1;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static void Slot00_10001090()
        {
            Gedx8GlobalRegistry.Shared.Sub10001090();
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static IntPtr Slot08_100013F0()
        {
            Gedx8DriverInstance? instance = Gedx8GlobalRegistry.Shared.Sub100013F0();
            if (instance == null)
            {
                return IntPtr.Zero;
            }

            return RegisterDriverHandle(instance);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot10_10001920(IntPtr instanceHandle, Gedx8NativeSynthInitConfig nativeConfig)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            if (instance == null)
            {
                return 0;
            }

            Gedx8SynthInitConfig config = new(nativeConfig.Reserved00, nativeConfig.SampleRate, nativeConfig.Config);
            return ToNativeBool(instance.InitializeSynthesizer(config));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot14_10001940(IntPtr instanceHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method10001940());
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot18_10001950(IntPtr instanceHandle, int value)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method10001950(value));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot1C_10001970(IntPtr instanceHandle, int value)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method10001970(value));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot20_10001990(IntPtr instanceHandle, int kind, IntPtr fileNamePointer, IntPtr searchDirectoryPointer, IntPtr loadedObjectOut)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            if (instance == null || loadedObjectOut == IntPtr.Zero)
            {
                return 0;
            }

            string? fileName = ReadAnsiString(fileNamePointer);
            string? searchDirectory = ReadAnsiString(searchDirectoryPointer);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Marshal.WriteIntPtr(loadedObjectOut, IntPtr.Zero);
                return 0;
            }

            if (!TryMapKind(kind, out Gedx8ObjectKind objectKind))
            {
                Marshal.WriteIntPtr(loadedObjectOut, IntPtr.Zero);
                return 0;
            }

            bool result = instance.LoadObject(objectKind, fileName, searchDirectory, out Gedx8LoadedObject? loadedObject);
            if (!result || loadedObject == null)
            {
                Marshal.WriteIntPtr(loadedObjectOut, IntPtr.Zero);
                return 0;
            }

            IntPtr loadedHandle = RegisterLoadedObjectHandle(loadedObject);
            Marshal.WriteIntPtr(loadedObjectOut, loadedHandle);
            return 1;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot24_10001AB0(IntPtr instanceHandle, int value0, int value1)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method10001AB0(value0, value1));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot28_10001AD0(IntPtr instanceHandle, IntPtr loadedObjectHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            if (instance == null || loadedObject == null)
            {
                return 0;
            }

            bool result = instance.Method10001AD0(loadedObject);
            if (result)
            {
                ReleaseLoadedObjectHandle(loadedObjectHandle);
            }

            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot2C_10001B50(IntPtr instanceHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method10001B50());
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot30_10001B60(IntPtr instanceHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method10001B60());
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot34_10001D10(IntPtr instanceHandle, int selection, int mode)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method10001D10(selection, mode));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot38_10001D50(IntPtr instanceHandle, int selector, int value, IntPtr storedValueOut)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            if (instance == null || storedValueOut == IntPtr.Zero)
            {
                return 0;
            }

            bool result = instance.Method10001D50(selector, value, out int storedValue);
            Marshal.WriteInt32(storedValueOut, storedValue);
            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot3C_10001D70(IntPtr instanceHandle, int selector, int value, IntPtr storedValueOut)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            if (instance == null || storedValueOut == IntPtr.Zero)
            {
                return 0;
            }

            bool result = instance.Method10001D70(selector, value, out int storedValue);
            Marshal.WriteInt32(storedValueOut, storedValue);
            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot40_10001E30(IntPtr instanceHandle, IntPtr loadedObjectHandle, int arg0, int arg1, int arg2, int arg3, int arg4)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            return ToNativeBool(instance != null && loadedObject != null && instance.Method10001E30(loadedObject, arg0, arg1, arg2, arg3, arg4));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot44_10001E70(IntPtr instanceHandle, IntPtr loadedObjectHandle, IntPtr valuePointer)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            string? value = ReadAnsiString(valuePointer);
            return ToNativeBool(instance != null && loadedObject != null && instance.Method10001E70(loadedObject, value));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot48_10001E90(IntPtr instanceHandle, IntPtr loadedObjectHandle, IntPtr readyByteOut)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            if (instance == null || loadedObject == null || readyByteOut == IntPtr.Zero)
            {
                return 0;
            }

            bool result = instance.Method10001E90(loadedObject, out byte readyByte);
            Marshal.WriteByte(readyByteOut, readyByte);
            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot4C_10001EB0(IntPtr instanceHandle, IntPtr loadedObjectHandle, IntPtr namePointer, IntPtr resolvedValueOut)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            if (instance == null || loadedObject == null || resolvedValueOut == IntPtr.Zero)
            {
                return 0;
            }

            string? name = ReadAnsiString(namePointer);
            if (string.IsNullOrWhiteSpace(name))
            {
                Marshal.WriteIntPtr(resolvedValueOut, IntPtr.Zero);
                return 0;
            }

            bool result = instance.Method10001EB0(loadedObject, name, out string? resolvedValue);
            Marshal.WriteIntPtr(resolvedValueOut, loadedObject.StoreCompositeResolveAForExport(resolvedValue));
            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot50_10001EE0(IntPtr instanceHandle, IntPtr loadedObjectHandle, IntPtr namePointer, IntPtr resolvedValueOut)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            if (instance == null || loadedObject == null || resolvedValueOut == IntPtr.Zero)
            {
                return 0;
            }

            string? name = ReadAnsiString(namePointer);
            if (string.IsNullOrWhiteSpace(name))
            {
                Marshal.WriteIntPtr(resolvedValueOut, IntPtr.Zero);
                return 0;
            }

            bool result = instance.Method10001EE0(loadedObject, name, out string? resolvedValue);
            Marshal.WriteIntPtr(resolvedValueOut, loadedObject.StoreCompositeResolveBForExport(resolvedValue));
            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot54_10002010(IntPtr instanceHandle, IntPtr loadedObjectHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            if (instance == null || loadedObject == null)
            {
                return 0;
            }

            bool result = instance.Method10002010(loadedObject);
            if (result)
            {
                ReleaseLoadedObjectHandle(loadedObjectHandle);
            }

            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot58_100020D0(IntPtr instanceHandle, IntPtr loadedObjectHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            return ToNativeBool(instance != null && loadedObject != null && instance.Method100020D0(loadedObject));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot5C_100020F0(IntPtr instanceHandle, IntPtr loadedObjectHandle, IntPtr namePointer)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            string? name = ReadAnsiString(namePointer);
            return ToNativeBool(instance != null && loadedObject != null && !string.IsNullOrWhiteSpace(name) && instance.Method100020F0(loadedObject, name));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot60_10002110(IntPtr instanceHandle, IntPtr loadedObjectHandle, int mode, int value, IntPtr resultOut)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            if (instance == null || loadedObject == null || resultOut == IntPtr.Zero)
            {
                return 0;
            }

            bool result = instance.Method10002110(loadedObject, mode, value, out string? textResult);
            Marshal.WriteIntPtr(resultOut, loadedObject.StoreThinType2QueryForExport(textResult));
            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot64_10002180(IntPtr instanceHandle, IntPtr loadedObjectHandle, int mode, int value, IntPtr textPointer)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            string? text = ReadAnsiString(textPointer);
            return ToNativeBool(instance != null && loadedObject != null && instance.Method10002180(loadedObject, mode, value, text));
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot68_100021F0(IntPtr instanceHandle, IntPtr loadedObjectHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            Gedx8LoadedObject? loadedObject = ResolveLoadedObject(loadedObjectHandle);
            if (instance == null || loadedObject == null)
            {
                return 0;
            }

            bool result = instance.Method100021F0(loadedObject);
            if (result)
            {
                ReleaseLoadedObjectHandle(loadedObjectHandle);
            }

            return ToNativeBool(result);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static byte Slot6C_100022B0(IntPtr instanceHandle)
        {
            Gedx8DriverInstance? instance = ResolveDriver(instanceHandle);
            return ToNativeBool(instance != null && instance.Method100022B0());
        }

        private static IntPtr RegisterDriverHandle(Gedx8DriverInstance instance)
        {
            lock (s_sync)
            {
                if (s_driverPointersByInstance.TryGetValue(instance, out IntPtr existingPointer))
                {
                    return existingPointer;
                }

                GCHandle handle = GCHandle.Alloc(instance, GCHandleType.Normal);
                IntPtr pointer = GCHandle.ToIntPtr(handle);

                s_driverHandlesByPointer[pointer] = handle;
                s_driverPointersByInstance[instance] = pointer;
                return pointer;
            }
        }

        private static IntPtr RegisterLoadedObjectHandle(Gedx8LoadedObject loadedObject)
        {
            lock (s_sync)
            {
                if (s_loadedObjectPointersByInstance.TryGetValue(loadedObject, out IntPtr existingPointer))
                {
                    return existingPointer;
                }

                GCHandle handle = GCHandle.Alloc(loadedObject, GCHandleType.Normal);
                IntPtr pointer = GCHandle.ToIntPtr(handle);

                s_loadedObjectHandlesByPointer[pointer] = handle;
                s_loadedObjectPointersByInstance[loadedObject] = pointer;
                return pointer;
            }
        }

        private static Gedx8DriverInstance? ResolveDriver(IntPtr handlePointer)
        {
            lock (s_sync)
            {
                if (!s_driverHandlesByPointer.TryGetValue(handlePointer, out GCHandle handle))
                {
                    return null;
                }

                return handle.Target as Gedx8DriverInstance;
            }
        }

        private static Gedx8LoadedObject? ResolveLoadedObject(IntPtr handlePointer)
        {
            lock (s_sync)
            {
                if (!s_loadedObjectHandlesByPointer.TryGetValue(handlePointer, out GCHandle handle))
                {
                    return null;
                }

                return handle.Target as Gedx8LoadedObject;
            }
        }

        private static void ReleaseLoadedObjectHandle(IntPtr handlePointer)
        {
            lock (s_sync)
            {
                if (!s_loadedObjectHandlesByPointer.TryGetValue(handlePointer, out GCHandle handle))
                {
                    return;
                }

                Gedx8LoadedObject? loadedObject = handle.Target as Gedx8LoadedObject;
                ReleaseLoadedObjectHandleNoLock(handlePointer, loadedObject);
            }
        }

        private static void ReleaseDriverHandleNoLock(IntPtr handlePointer, Gedx8DriverInstance? instance)
        {
            if (!s_driverHandlesByPointer.TryGetValue(handlePointer, out GCHandle handle))
            {
                return;
            }

            s_driverHandlesByPointer.Remove(handlePointer);

            if (instance != null)
            {
                s_driverPointersByInstance.Remove(instance);
            }
            else if (handle.Target is Gedx8DriverInstance targetInstance)
            {
                s_driverPointersByInstance.Remove(targetInstance);
            }

            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }

        private static void ReleaseLoadedObjectHandleNoLock(IntPtr handlePointer, Gedx8LoadedObject? loadedObject)
        {
            if (!s_loadedObjectHandlesByPointer.TryGetValue(handlePointer, out GCHandle handle))
            {
                return;
            }

            s_loadedObjectHandlesByPointer.Remove(handlePointer);

            if (loadedObject != null)
            {
                s_loadedObjectPointersByInstance.Remove(loadedObject);
            }
            else if (handle.Target is Gedx8LoadedObject targetLoadedObject)
            {
                s_loadedObjectPointersByInstance.Remove(targetLoadedObject);
            }

            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }

        private static string? ReadAnsiString(IntPtr pointer)
        {
            return pointer == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(pointer);
        }

        private static bool TryMapKind(int nativeValue, out Gedx8ObjectKind kind)
        {
            switch (nativeValue)
            {
                case 0:
                    kind = Gedx8ObjectKind.Composite;
                    return true;

                case 1:
                    kind = Gedx8ObjectKind.ThinType1;
                    return true;

                case 2:
                    kind = Gedx8ObjectKind.ThinType2;
                    return true;

                default:
                    kind = Gedx8ObjectKind.Composite;
                    return false;
            }
        }

        private static byte ToNativeBool(bool value)
        {
            return value ? (byte)1 : (byte)0;
        }
    }
}