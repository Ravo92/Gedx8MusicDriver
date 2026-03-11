using System.Globalization;
using System.Runtime.InteropServices;

namespace Gedx8MusicDriver.Models
{
    internal sealed class Gedx8LoadedObject
    {
        private readonly Dictionary<int, string?> _thinType2Mode0Values;
        private readonly Dictionary<int, string?> _thinType2Mode1Values;
        private readonly Dictionary<int, string?> _thinType2Mode2Values;

        private readonly Dictionary<string, string?> _compositeResolveAValues;
        private readonly Dictionary<string, string?> _compositeResolveBValues;

        private IntPtr _exportAnsiCompositeResolveA;
        private IntPtr _exportAnsiCompositeResolveB;
        private IntPtr _exportAnsiThinType2Query;

        private bool _compositeCall04190Latched;
        private int _compositeArg0;
        private int _compositeArg1;
        private int _compositeArg2;
        private int _compositeArg3;
        private int _compositeArg4;
        private string? _compositeTextValue;

        internal Gedx8LoadedObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory, string? resolvedPath)
        {
            Kind = kind;
            FileName = fileName;
            SearchDirectory = searchDirectory;
            ResolvedPath = resolvedPath;
            ActiveByte00 = 1;

            _thinType2Mode0Values = new Dictionary<int, string?>();
            _thinType2Mode1Values = new Dictionary<int, string?>();
            _thinType2Mode2Values = new Dictionary<int, string?>();

            _compositeResolveAValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            _compositeResolveBValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }

        public Gedx8ObjectKind Kind { get; }

        public string FileName { get; }

        public string? SearchDirectory { get; }

        public string? ResolvedPath { get; }

        internal byte ActiveByte00 { get; private set; }

        internal int OuterType04 { get; private set; }

        internal int OuterLoaderMode08 { get; private set; }

        internal object? OuterInner0C { get; private set; }

        internal object? CompositeOwner04 { get; private set; }

        internal object? CompositeSource08 { get; private set; }

        internal object? CompositeContext0C { get; private set; }

        internal uint[]? CompositeTable10 { get; private set; }

        internal int CompositeHandle14 { get; private set; }

        internal ushort CompositeGroupCount18 { get; private set; }

        internal byte CompositeEntryCount1A { get; private set; }

        internal byte CompositeInitResult1B { get; private set; }

        public bool IsActive => ActiveByte00 != 0;

        public string? LastOpenName { get; private set; }

        public string? LastStringValue { get; private set; }

        internal void Sub10002D30(int outerType04, int outerLoaderMode08, object? outerInner0C)
        {
            OuterType04 = outerType04;
            OuterLoaderMode08 = outerLoaderMode08;
            OuterInner0C = outerInner0C;

            CompositeOwner04 = null;
            CompositeSource08 = null;
            CompositeContext0C = null;
            ResetCompositeRuntimeState();
            Sub10004170();
        }

        internal bool Sub10004120(int outerType04, int outerLoaderMode08, object? outerInner0C, object? compositeOwner04, object? compositeSource08, object? compositeContext0C, int compositeHandle14, byte compositeEntryCount1A, ushort compositeGroupCount18, uint[]? compositeTable)
        {
            OuterType04 = outerType04;
            OuterLoaderMode08 = outerLoaderMode08;
            OuterInner0C = outerInner0C;

            CompositeOwner04 = compositeOwner04;
            CompositeSource08 = compositeSource08;
            CompositeContext0C = compositeContext0C;

            ResetCompositeRuntimeState();
            Sub10004170();

            CompositeHandle14 = compositeHandle14;
            CompositeEntryCount1A = compositeEntryCount1A;
            CompositeGroupCount18 = compositeGroupCount18;

            if (compositeEntryCount1A == 0 || compositeGroupCount18 == 0)
            {
                CompositeHandle14 = compositeHandle14 != 0 ? compositeHandle14 : 1;
                CompositeEntryCount1A = 1;
                CompositeGroupCount18 = 1;
                CompositeTable10 = new uint[] { 0 };
                CompositeInitResult1B = 1;
                SeedCompositeResolveDefaults();
                return true;
            }

            CompositeInitResult1B = Sub10004670(compositeTable) ? (byte)1 : (byte)0;
            if (CompositeInitResult1B != 0)
            {
                SeedCompositeResolveDefaults();
            }

            return CompositeInitResult1B != 0;
        }

        internal void Sub10004170()
        {
            CompositeTable10 = null;
            CompositeHandle14 = 0;
            CompositeGroupCount18 = 0;
            CompositeEntryCount1A = 0;
            CompositeInitResult1B = 0;
            ResetCompositeRuntimeState();
        }

        internal bool Sub10004670(uint[]? compositeTable)
        {
            CompositeTable10 = null;

            if (CompositeEntryCount1A == 0 || CompositeGroupCount18 == 0)
            {
                return false;
            }

            int expectedEntryCount = CompositeEntryCount1A * CompositeGroupCount18;
            if (expectedEntryCount <= 0)
            {
                return false;
            }

            if (compositeTable == null || compositeTable.Length != expectedEntryCount)
            {
                return false;
            }

            uint[] tableCopy = new uint[expectedEntryCount];
            Array.Copy(compositeTable, tableCopy, expectedEntryCount);
            CompositeTable10 = tableCopy;
            return true;
        }

        internal bool TryCompositeCall04190(int arg0, int arg1, int arg2, int arg3, int arg4)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            if (CompositeOwner04 == null || CompositeSource08 == null || CompositeContext0C == null)
            {
                return false;
            }

            if (CompositeInitResult1B == 0 || CompositeTable10 == null)
            {
                return false;
            }

            _compositeArg0 = arg0;
            _compositeArg1 = arg1;
            _compositeArg2 = arg2;
            _compositeArg3 = arg3;
            _compositeArg4 = arg4;
            _compositeCall04190Latched = true;

            _compositeResolveBValues["arg0"] = arg0.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["arg1"] = arg1.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["arg2"] = arg2.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["arg3"] = arg3.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["arg4"] = arg4.ToString(CultureInfo.InvariantCulture);

            LastStringValue = null;
            return true;
        }

        internal bool TryCompositeCall04250(string? value)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            if (CompositeOwner04 == null || CompositeSource08 == null)
            {
                return false;
            }

            if (CompositeInitResult1B == 0)
            {
                return false;
            }

            _compositeTextValue = value;
            LastStringValue = value;
            _compositeResolveBValues["text"] = value;
            _compositeResolveBValues["value"] = value;
            return true;
        }

        internal bool TryCompositeCall04280(out byte readyByte)
        {
            readyByte = 0;

            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            if (CompositeOwner04 == null || CompositeSource08 == null)
            {
                return false;
            }

            readyByte = CompositeInitResult1B != 0 && _compositeCall04190Latched ? (byte)1 : (byte)0;
            return true;
        }

        internal bool TryCompositeCall042C0(string name, out string? value)
        {
            value = null;

            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (CompositeInitResult1B == 0)
            {
                return false;
            }

            LastOpenName = name;

            if (_compositeResolveAValues.TryGetValue(name, out string? cachedValue))
            {
                value = cachedValue;
                return !string.IsNullOrWhiteSpace(value);
            }

            value = name switch
            {
                "path" => ResolvedPath,
                "resolvedpath" => ResolvedPath,
                "file" => FileName,
                "filename" => FileName,
                "searchdir" => SearchDirectory,
                "searchdirectory" => SearchDirectory,
                _ => ResolvedPath ?? FileName ?? name,
            };

            _compositeResolveAValues[name] = value;
            return !string.IsNullOrWhiteSpace(value);
        }

        internal bool TryCompositeCall04490(string name, out string? value)
        {
            value = null;

            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (CompositeInitResult1B == 0)
            {
                return false;
            }

            LastOpenName = name;

            if (_compositeResolveBValues.TryGetValue(name, out string? cachedValue))
            {
                value = cachedValue;
                return !string.IsNullOrWhiteSpace(value);
            }

            value = name switch
            {
                "text" => _compositeTextValue,
                "value" => _compositeTextValue,
                "arg0" => _compositeArg0.ToString(CultureInfo.InvariantCulture),
                "arg1" => _compositeArg1.ToString(CultureInfo.InvariantCulture),
                "arg2" => _compositeArg2.ToString(CultureInfo.InvariantCulture),
                "arg3" => _compositeArg3.ToString(CultureInfo.InvariantCulture),
                "arg4" => _compositeArg4.ToString(CultureInfo.InvariantCulture),
                _ => _compositeTextValue ?? LastStringValue,
            };

            _compositeResolveBValues[name] = value;
            return !string.IsNullOrWhiteSpace(value);
        }

        internal bool TryCompositeCall04640(out byte value0, out byte value1)
        {
            value0 = 0;
            value1 = 0;

            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            if (CompositeInitResult1B == 0)
            {
                return false;
            }

            value0 = unchecked((byte)CompositeGroupCount18);
            value1 = CompositeEntryCount1A;
            return true;
        }

        internal bool TryThinType1Call02D50()
        {
            return IsActive && Kind == Gedx8ObjectKind.ThinType1 && OuterInner0C != null;
        }

        internal bool TryThinType1Call02D70()
        {
            return IsActive && Kind == Gedx8ObjectKind.ThinType1 && OuterInner0C != null;
        }

        internal bool ReleaseThinObject10003C30(bool notifyRuntime)
        {
            if (!IsActive)
            {
                return false;
            }

            if (Kind != Gedx8ObjectKind.ThinType1 && Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (OuterInner0C == null)
            {
                return false;
            }

            OuterInner0C = null;
            return true;
        }

        internal bool TryThinType2Release03E90()
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            return OuterInner0C != null;
        }

        internal bool TryThinType2Open03EB0(string name)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (OuterInner0C == null || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            LastOpenName = name;

            if (!_thinType2Mode1Values.ContainsKey(0))
            {
                _thinType2Mode1Values[0] = name;
            }

            if (!_thinType2Mode0Values.ContainsKey(0))
            {
                _thinType2Mode0Values[0] = ResolvedPath ?? name;
            }

            return true;
        }

        internal bool TryThinType2Query03F00(int selector, int value, out string? result)
        {
            result = null;

            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (OuterInner0C == null)
            {
                return false;
            }

            if (_thinType2Mode0Values.TryGetValue(value, out string? storedValue))
            {
                result = storedValue;
                return !string.IsNullOrWhiteSpace(result);
            }

            result = ResolvedPath ?? LastOpenName;
            return !string.IsNullOrWhiteSpace(result);
        }

        internal bool TryThinType2Query03F50(int selector, int value, out string? result)
        {
            result = null;

            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (OuterInner0C == null)
            {
                return false;
            }

            if (_thinType2Mode1Values.TryGetValue(value, out string? storedValue))
            {
                result = storedValue;
                return !string.IsNullOrWhiteSpace(result);
            }

            result = LastOpenName;
            return !string.IsNullOrWhiteSpace(result);
        }

        internal bool TryThinType2Query03FA0(int selector, int value, out string? result)
        {
            result = null;

            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (OuterInner0C == null)
            {
                return false;
            }

            if (_thinType2Mode2Values.TryGetValue(value, out string? storedValue))
            {
                result = storedValue;
                return !string.IsNullOrWhiteSpace(result);
            }

            result = LastStringValue;
            return !string.IsNullOrWhiteSpace(result);
        }

        internal bool TryThinType2Configure04020(int selector, int value, string? text)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (OuterInner0C == null)
            {
                return false;
            }

            _thinType2Mode0Values[value] = text;
            LastStringValue = text;
            return true;
        }

        internal bool TryThinType2Configure04070(int selector, int value, string? text)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (OuterInner0C == null)
            {
                return false;
            }

            _thinType2Mode1Values[value] = text;
            LastStringValue = text;
            return true;
        }

        internal bool TryThinType2Configure040D0(int selector, int value, string? text)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (OuterInner0C == null)
            {
                return false;
            }

            _thinType2Mode2Values[value] = text;
            LastStringValue = text;
            return true;
        }

        internal IntPtr StoreCompositeResolveAForExport(string? value)
        {
            _exportAnsiCompositeResolveA = ReplaceAnsiBuffer(_exportAnsiCompositeResolveA, value);
            return _exportAnsiCompositeResolveA;
        }

        internal IntPtr StoreCompositeResolveBForExport(string? value)
        {
            _exportAnsiCompositeResolveB = ReplaceAnsiBuffer(_exportAnsiCompositeResolveB, value);
            return _exportAnsiCompositeResolveB;
        }

        internal IntPtr StoreThinType2QueryForExport(string? value)
        {
            _exportAnsiThinType2Query = ReplaceAnsiBuffer(_exportAnsiThinType2Query, value);
            return _exportAnsiThinType2Query;
        }

        internal void Deactivate()
        {
            ActiveByte00 = 0;
            OuterInner0C = null;
            CompositeOwner04 = null;
            CompositeSource08 = null;
            CompositeContext0C = null;
            LastOpenName = null;
            LastStringValue = null;

            _thinType2Mode0Values.Clear();
            _thinType2Mode1Values.Clear();
            _thinType2Mode2Values.Clear();

            ResetCompositeRuntimeState();
            ReleaseExportBuffers();
            Sub10004170();
        }

        private void ResetCompositeRuntimeState()
        {
            _compositeCall04190Latched = false;
            _compositeArg0 = 0;
            _compositeArg1 = 0;
            _compositeArg2 = 0;
            _compositeArg3 = 0;
            _compositeArg4 = 0;
            _compositeTextValue = null;

            _compositeResolveAValues.Clear();
            _compositeResolveBValues.Clear();
        }

        private void SeedCompositeResolveDefaults()
        {
            _compositeResolveAValues["path"] = ResolvedPath;
            _compositeResolveAValues["resolvedpath"] = ResolvedPath;
            _compositeResolveAValues["file"] = FileName;
            _compositeResolveAValues["filename"] = FileName;
            _compositeResolveAValues["searchdir"] = SearchDirectory;
            _compositeResolveAValues["searchdirectory"] = SearchDirectory;

            _compositeResolveBValues["text"] = _compositeTextValue;
            _compositeResolveBValues["value"] = _compositeTextValue;
        }

        private void ReleaseExportBuffers()
        {
            FreeAnsiBuffer(ref _exportAnsiCompositeResolveA);
            FreeAnsiBuffer(ref _exportAnsiCompositeResolveB);
            FreeAnsiBuffer(ref _exportAnsiThinType2Query);
        }

        private static IntPtr ReplaceAnsiBuffer(IntPtr currentBuffer, string? value)
        {
            if (currentBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(currentBuffer);
                currentBuffer = IntPtr.Zero;
            }

            if (string.IsNullOrEmpty(value))
            {
                return IntPtr.Zero;
            }

            return Marshal.StringToHGlobalAnsi(value);
        }

        private static void FreeAnsiBuffer(ref IntPtr buffer)
        {
            if (buffer == IntPtr.Zero)
            {
                return;
            }

            Marshal.FreeHGlobal(buffer);
            buffer = IntPtr.Zero;
        }
    }
}