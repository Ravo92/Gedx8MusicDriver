
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

        private object? _recordOwner08;
        private object? _innerObject0C;
        private int _loaderMode08;

        internal Gedx8LoadedObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory, string? resolvedPath)
        {
            Kind = kind;
            FileName = fileName;
            SearchDirectory = searchDirectory;
            ResolvedPath = resolvedPath;
            ActiveByte00 = 0;

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

        internal int OuterType04 => (int)Kind;

        internal int OuterLoaderMode08 => _loaderMode08;

        internal object? OuterInner0C => _innerObject0C;

        internal object? CompositeOwner04 => GetCompositeRuntime()?.Source04;

        internal object? CompositeSource08 => GetCompositeRuntime()?.Driver08;

        internal object? CompositeContext0C => GetCompositeRuntime()?.Link0C;

        internal uint[]? CompositeTable10 => GetCompositeRuntime()?.Table10;

        internal int CompositeHandle14 => GetCompositeRuntime()?.DescriptorId14 ?? 0;

        internal ushort CompositeGroupCount18 => GetCompositeRuntime()?.GroupCount18 ?? 0;

        internal byte CompositeEntryCount1A => GetCompositeRuntime()?.EntryCount1A ?? 0;

        internal byte CompositeInitResult1B => GetCompositeRuntime()?.InitResult1B ?? (byte)0;

        public bool IsActive => ActiveByte00 != 0;

        public string? LastOpenName { get; private set; }

        public string? LastStringValue { get; private set; }

        internal void InitializeThinRecord(object? owner08, int loaderMode08, object? innerObject0C)
        {
            _recordOwner08 = owner08;
            _loaderMode08 = loaderMode08;
            _innerObject0C = innerObject0C;

            ResetCompositeRuntimeState();
            ReleaseExportBuffers();
        }

        internal void InitializeCompositeRecord(object? owner08, int loaderMode08, Gedx8CompositeRuntime compositeRuntime)
        {
            _recordOwner08 = owner08;
            _loaderMode08 = loaderMode08;
            _innerObject0C = compositeRuntime;

            ResetCompositeRuntimeState();
            ReleaseExportBuffers();

            SeedCompositeResolveDefaults(compositeRuntime);
        }

        internal void Sub10002D30(int outerType04, int outerLoaderMode08, object? outerInner0C)
        {
            _loaderMode08 = outerLoaderMode08;
            _innerObject0C = outerInner0C;
            ResetCompositeRuntimeState();
            ReleaseExportBuffers();
        }

        internal bool Sub10004120(int outerType04, int outerLoaderMode08, object? outerInner0C, object? compositeSource04, object? compositeDriver08, object? compositeLink0C, int compositeHandle14, byte compositeEntryCount1A, ushort compositeGroupCount18, uint[]? compositeTable)
        {
            Gedx8CompositeRuntime compositeRuntime = new Gedx8CompositeRuntime(compositeSource04, compositeDriver08, compositeLink0C, outerLoaderMode08, compositeHandle14, compositeEntryCount1A, compositeGroupCount18, compositeTable);
            InitializeCompositeRecord(compositeDriver08, outerLoaderMode08, compositeRuntime);
            return compositeRuntime.InitResult1B != 0;
        }

        internal void Sub10004170()
        {
            Gedx8CompositeRuntime? compositeRuntime = GetCompositeRuntime();
            if (compositeRuntime != null)
            {
                compositeRuntime.ReleaseTable();
            }

            ResetCompositeRuntimeState();
            ReleaseExportBuffers();
        }

        internal bool Sub10004670(uint[]? compositeTable)
        {
            Gedx8CompositeRuntime? compositeRuntime = GetCompositeRuntime();
            if (compositeRuntime == null)
            {
                return false;
            }

            bool result = compositeRuntime.LoadTable(compositeTable);
            if (result)
            {
                SeedCompositeResolveDefaults(compositeRuntime);
            }

            return result;
        }

        internal bool TryCompositeCall04190(int arg0, int arg1, int arg2, int arg3, int arg4)
        {
            Gedx8CompositeRuntime? compositeRuntime = GetCompositeRuntime();
            if (compositeRuntime == null)
            {
                return false;
            }

            _compositeArg0 = arg0;
            _compositeArg1 = arg1;
            _compositeArg2 = arg2;
            _compositeArg3 = arg3;
            _compositeArg4 = arg4;
            _compositeCall04190Latched = true;

            UpdateCompositeLatchedArguments();

            if (TryResolveCompositeWindowFromInteger(arg0, compositeRuntime, out Gedx8CompositeWindowMatch match))
            {
                ApplyCompositeWindowMatch(compositeRuntime, match, arg0);
                return true;
            }

            if (TryResolveCompositeWindowFromInteger(arg1, compositeRuntime, out match))
            {
                ApplyCompositeWindowMatch(compositeRuntime, match, arg1);
                return true;
            }

            compositeRuntime.LastProbeResult = compositeRuntime.InitResult1B != 0 ? 1 : -1;
            return compositeRuntime.LastProbeResult >= 0;
        }

        internal bool TryCompositeCall04250(string? value)
        {
            Gedx8CompositeRuntime? compositeRuntime = GetCompositeRuntime();
            if (compositeRuntime == null || compositeRuntime.Source04 == null || compositeRuntime.Driver08 == null)
            {
                return false;
            }

            _compositeTextValue = value;
            LastStringValue = value;
            _compositeResolveBValues["text"] = value;
            _compositeResolveBValues["value"] = value;

            if (TryParseFlexibleInteger(value, out int numericValue))
            {
                if (TryResolveCompositeWindowFromInteger(numericValue, compositeRuntime, out Gedx8CompositeWindowMatch match))
                {
                    ApplyCompositeWindowMatch(compositeRuntime, match, numericValue);
                    return true;
                }

                compositeRuntime.LastForwardValue = numericValue;
                _compositeResolveBValues["position"] = numericValue.ToString(CultureInfo.InvariantCulture);
            }

            compositeRuntime.LastProbeResult = compositeRuntime.InitResult1B != 0 ? 0 : -1;
            _compositeResolveBValues["lastprobe"] = compositeRuntime.LastProbeResult.ToString(CultureInfo.InvariantCulture);
            return compositeRuntime.LastProbeResult >= 0;
        }

        internal bool TryCompositeCall04280(out byte readyByte)
        {
            readyByte = 0;

            Gedx8CompositeRuntime? compositeRuntime = GetCompositeRuntime();
            if (compositeRuntime == null)
            {
                return false;
            }

            int probeResult = compositeRuntime.LastProbeResult;
            if (probeResult < 0)
            {
                return false;
            }

            if (probeResult == 0)
            {
                readyByte = 1;
            }

            return true;
        }

        internal bool TryCompositeCall042C0(string name, out string? value)
        {
            value = null;

            Gedx8CompositeRuntime? compositeRuntime = GetCompositeRuntime();
            if (compositeRuntime == null || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            LastOpenName = name;

            if (_compositeResolveAValues.TryGetValue(name, out string? cachedValue))
            {
                value = cachedValue;
                return !string.IsNullOrWhiteSpace(value);
            }

            if (TryResolveCompositeWindowByName(name, compositeRuntime, out value))
            {
                _compositeResolveAValues[name] = value;
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
                "handle" => compositeRuntime.DescriptorId14.ToString(CultureInfo.InvariantCulture),
                "groupcount" => compositeRuntime.GroupCount18.ToString(CultureInfo.InvariantCulture),
                "entrycount" => compositeRuntime.EntryCount1A.ToString(CultureInfo.InvariantCulture),
                "tablelength" => compositeRuntime.Table10?.Length.ToString(CultureInfo.InvariantCulture),
                _ => null,
            };

            if (value == null)
            {
                value = ResolvedPath ?? FileName;
            }

            _compositeResolveAValues[name] = value;
            return !string.IsNullOrWhiteSpace(value);
        }

        internal bool TryCompositeCall04490(string name, out string? value)
        {
            value = null;

            Gedx8CompositeRuntime? compositeRuntime = GetCompositeRuntime();
            if (compositeRuntime == null || string.IsNullOrWhiteSpace(name))
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
                "position" => compositeRuntime.LastForwardValue.ToString(CultureInfo.InvariantCulture),
                "reducedoffset" => compositeRuntime.LastReducedOffset.ToString(CultureInfo.InvariantCulture),
                "shiftedbase" => compositeRuntime.LastShiftedBase.ToString(CultureInfo.InvariantCulture),
                "stepcount" => compositeRuntime.LastStepCount.ToString(CultureInfo.InvariantCulture),
                "lastprobe" => compositeRuntime.LastProbeResult.ToString(CultureInfo.InvariantCulture),
                "group" => compositeRuntime.LastResolvedGroup.ToString(CultureInfo.InvariantCulture),
                "entry" => compositeRuntime.LastResolvedEntry.ToString(CultureInfo.InvariantCulture),
                "arg0" => _compositeArg0.ToString(CultureInfo.InvariantCulture),
                "arg1" => _compositeArg1.ToString(CultureInfo.InvariantCulture),
                "arg2" => _compositeArg2.ToString(CultureInfo.InvariantCulture),
                "arg3" => _compositeArg3.ToString(CultureInfo.InvariantCulture),
                "arg4" => _compositeArg4.ToString(CultureInfo.InvariantCulture),
                _ => null,
            };

            if (value == null)
            {
                value = _compositeTextValue ?? LastStringValue;
            }

            _compositeResolveBValues[name] = value;
            return !string.IsNullOrWhiteSpace(value);
        }

        internal bool TryCompositeCall04640(out byte value0, out byte value1)
        {
            value0 = 0;
            value1 = 0;

            Gedx8CompositeRuntime? compositeRuntime = GetCompositeRuntime();
            if (compositeRuntime == null || compositeRuntime.InitResult1B == 0)
            {
                return false;
            }

            value0 = unchecked((byte)compositeRuntime.GroupCount18);
            value1 = compositeRuntime.EntryCount1A;
            return true;
        }

        internal bool TryThinType1Call02D50()
        {
            return IsActive && Kind == Gedx8ObjectKind.ThinType1 && _innerObject0C != null;
        }

        internal bool TryThinType1Call02D70()
        {
            return IsActive && Kind == Gedx8ObjectKind.ThinType1 && _innerObject0C != null;
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

            if (_innerObject0C == null)
            {
                return false;
            }

            _innerObject0C = null;
            return true;
        }

        internal bool TryThinType2Release03E90()
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            return _innerObject0C != null;
        }

        internal bool TryThinType2Open03EB0(string name)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (_innerObject0C == null || string.IsNullOrWhiteSpace(name))
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

        internal bool TryThinType2Query03F00(string name, IntPtr contextPointer)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (_innerObject0C == null || contextPointer == IntPtr.Zero || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            int contextValue = ReadInt32Unsafe(contextPointer);
            string? value;
            if (!_thinType2Mode0Values.TryGetValue(contextValue, out value))
            {
                value = ResolvedPath ?? LastOpenName ?? name;
            }

            LastOpenName = name;
            LastStringValue = value;
            _thinType2Mode0Values[contextValue] = value;
            return true;
        }

        internal bool TryThinType2Query03F50(string name, IntPtr contextPointer)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (_innerObject0C == null || contextPointer == IntPtr.Zero || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            int contextValue = ReadInt32Unsafe(contextPointer);
            string? value;
            if (!_thinType2Mode1Values.TryGetValue(contextValue, out value))
            {
                value = LastOpenName ?? name;
            }

            LastOpenName = name;
            LastStringValue = value;
            _thinType2Mode1Values[contextValue] = value;
            return true;
        }

        internal bool TryThinType2Query03FA0(string name, IntPtr descriptorPointer)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (_innerObject0C == null || descriptorPointer == IntPtr.Zero || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            Gedx8ThinType2Descriptor03FA0 descriptor = ReadDescriptor03FA0(descriptorPointer);
            int descriptorKey = descriptor.GetStableKey();

            string? value;
            if (!_thinType2Mode2Values.TryGetValue(descriptorKey, out value))
            {
                value = LastStringValue ?? LastOpenName ?? name;
            }

            LastOpenName = name;
            LastStringValue = value;
            _thinType2Mode2Values[descriptorKey] = value;
            return true;
        }

        internal bool TryThinType2Configure04020(string name, IntPtr payloadPointer)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (_innerObject0C == null || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            int payloadKey = unchecked((int)payloadPointer.ToInt64());
            string effectiveText = name;
            _thinType2Mode0Values[payloadKey] = effectiveText;
            LastOpenName = name;
            LastStringValue = effectiveText;
            return true;
        }

        internal bool TryThinType2Configure04070(string name, IntPtr payloadPointer)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (_innerObject0C == null || payloadPointer == IntPtr.Zero || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            int value04 = ReadInt32Unsafe(payloadPointer + 4);
            string effectiveText = name;
            _thinType2Mode1Values[value04] = effectiveText;
            LastOpenName = name;
            LastStringValue = effectiveText;
            return true;
        }

        internal bool TryThinType2Configure040D0(string name, IntPtr payloadPointer)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            if (_innerObject0C == null || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            int payloadKey = unchecked((int)payloadPointer.ToInt64());
            string effectiveText = name;
            _thinType2Mode2Values[payloadKey] = effectiveText;
            LastOpenName = name;
            LastStringValue = effectiveText;
            return true;
        }

        internal void Activate()
        {
            ActiveByte00 = 1;
        }

        internal void Deactivate()
        {
            ActiveByte00 = 0;
            _recordOwner08 = null;
            _innerObject0C = null;
            _loaderMode08 = 0;
            LastOpenName = null;
            LastStringValue = null;

            _thinType2Mode0Values.Clear();
            _thinType2Mode1Values.Clear();
            _thinType2Mode2Values.Clear();

            ResetCompositeRuntimeState();
            ReleaseExportBuffers();
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

        private void SeedCompositeResolveDefaults(Gedx8CompositeRuntime compositeRuntime)
        {
            _compositeResolveAValues["path"] = ResolvedPath;
            _compositeResolveAValues["resolvedpath"] = ResolvedPath;
            _compositeResolveAValues["file"] = FileName;
            _compositeResolveAValues["filename"] = FileName;
            _compositeResolveAValues["searchdir"] = SearchDirectory;
            _compositeResolveAValues["searchdirectory"] = SearchDirectory;
            _compositeResolveAValues["handle"] = compositeRuntime.DescriptorId14.ToString(CultureInfo.InvariantCulture);
            _compositeResolveAValues["groupcount"] = compositeRuntime.GroupCount18.ToString(CultureInfo.InvariantCulture);
            _compositeResolveAValues["entrycount"] = compositeRuntime.EntryCount1A.ToString(CultureInfo.InvariantCulture);

            _compositeResolveBValues["text"] = _compositeTextValue;
            _compositeResolveBValues["value"] = _compositeTextValue;
            _compositeResolveBValues["lastprobe"] = compositeRuntime.LastProbeResult.ToString(CultureInfo.InvariantCulture);
        }

        private void UpdateCompositeLatchedArguments()
        {
            _compositeResolveBValues["arg0"] = _compositeArg0.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["arg1"] = _compositeArg1.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["arg2"] = _compositeArg2.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["arg3"] = _compositeArg3.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["arg4"] = _compositeArg4.ToString(CultureInfo.InvariantCulture);
        }

        private Gedx8CompositeRuntime? GetCompositeRuntime()
        {
            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return null;
            }

            if (_innerObject0C is not Gedx8CompositeRuntime compositeRuntime)
            {
                return null;
            }

            if (compositeRuntime.Source04 == null || compositeRuntime.Driver08 == null)
            {
                return null;
            }

            return compositeRuntime;
        }

        private bool TryResolveCompositeWindowByName(string name, Gedx8CompositeRuntime compositeRuntime, out string? value)
        {
            value = null;

            if (name.StartsWith("cell:", StringComparison.OrdinalIgnoreCase))
            {
                string coordinates = name.Substring(5);
                if (TryParseCompositeCellCoordinates(coordinates, out int groupIndex, out int entryIndex) && compositeRuntime.TryGetPackedValue(groupIndex, entryIndex, out uint packedValue))
                {
                    value = "0x" + packedValue.ToString("X8", CultureInfo.InvariantCulture);
                    return true;
                }

                return false;
            }

            if (name.StartsWith("window:", StringComparison.OrdinalIgnoreCase))
            {
                string numericText = name.Substring(7);
                if (TryParseFlexibleInteger(numericText, out int numericValue) && TryResolveCompositeWindowFromInteger(numericValue, compositeRuntime, out Gedx8CompositeWindowMatch match))
                {
                    value = match.GroupIndex.ToString(CultureInfo.InvariantCulture) + ":" +
                        match.EntryIndex.ToString(CultureInfo.InvariantCulture) + ":" +
                        match.ReducedOffset.ToString(CultureInfo.InvariantCulture) + ":" +
                        match.ShiftedBase.ToString(CultureInfo.InvariantCulture) + ":" +
                        match.StepCount.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                return false;
            }

            return false;
        }

        private bool TryResolveCompositeWindowFromInteger(int inputValue, Gedx8CompositeRuntime compositeRuntime, out Gedx8CompositeWindowMatch match)
        {
            for (int groupIndex = 0; groupIndex < compositeRuntime.GroupCount18; groupIndex++)
            {
                for (int entryIndex = 0; entryIndex < compositeRuntime.EntryCount1A; entryIndex++)
                {
                    if (!compositeRuntime.TryGetPackedValue(groupIndex, entryIndex, out uint packedValue))
                    {
                        continue;
                    }

                    if (Gedx8CompositeWindowMatch.TryCreate(groupIndex, entryIndex, packedValue, inputValue, out match))
                    {
                        return true;
                    }
                }
            }

            match = default;
            return false;
        }

        private void ApplyCompositeWindowMatch(Gedx8CompositeRuntime compositeRuntime, Gedx8CompositeWindowMatch match, int numericValue)
        {
            compositeRuntime.LastForwardValue = numericValue;
            compositeRuntime.LastResolvedGroup = match.GroupIndex;
            compositeRuntime.LastResolvedEntry = match.EntryIndex;
            compositeRuntime.LastReducedOffset = match.ReducedOffset;
            compositeRuntime.LastShiftedBase = match.ShiftedBase;
            compositeRuntime.LastStepCount = match.StepCount;
            compositeRuntime.LastProbeResult = 0;

            _compositeResolveBValues["position"] = numericValue.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["group"] = match.GroupIndex.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["entry"] = match.EntryIndex.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["reducedoffset"] = match.ReducedOffset.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["shiftedbase"] = match.ShiftedBase.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["stepcount"] = match.StepCount.ToString(CultureInfo.InvariantCulture);
            _compositeResolveBValues["lastprobe"] = compositeRuntime.LastProbeResult.ToString(CultureInfo.InvariantCulture);
        }

        private static bool TryParseCompositeCellCoordinates(string coordinates, out int groupIndex, out int entryIndex)
        {
            groupIndex = 0;
            entryIndex = 0;

            string[] parts = coordinates.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                return false;
            }

            return int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out groupIndex) &&
                int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out entryIndex);
        }

        private static bool TryParseFlexibleInteger(string? value, out int parsedValue)
        {
            parsedValue = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string trimmed = value.Trim();
            if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return int.TryParse(trimmed.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out parsedValue);
            }

            return int.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue);
        }

        private static int ReadInt32Unsafe(IntPtr pointer)
        {
            return Marshal.ReadInt32(pointer);
        }

        private static Gedx8ThinType2Descriptor03FA0 ReadDescriptor03FA0(IntPtr descriptorPointer)
        {
            Gedx8ThinType2Descriptor03FA0 descriptor = new Gedx8ThinType2Descriptor03FA0(
                Marshal.ReadInt32(descriptorPointer, 0),
                Marshal.ReadInt32(descriptorPointer, 4),
                Marshal.ReadInt32(descriptorPointer, 8),
                Marshal.ReadInt32(descriptorPointer, 12),
                Marshal.ReadByte(descriptorPointer, 16));
            return descriptor;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private readonly struct Gedx8ThinType2Descriptor03FA0
        {
            internal Gedx8ThinType2Descriptor03FA0(int value00, int value04, int value08, int value0C, byte value10)
            {
                Value00 = value00;
                Value04 = value04;
                Value08 = value08;
                Value0C = value0C;
                Value10 = value10;
            }

            internal int Value00 { get; }

            internal int Value04 { get; }

            internal int Value08 { get; }

            internal int Value0C { get; }

            internal byte Value10 { get; }

            internal int GetStableKey()
            {
                HashCode hashCode = new HashCode();
                hashCode.Add(Value00);
                hashCode.Add(Value04);
                hashCode.Add(Value08);
                hashCode.Add(Value0C);
                hashCode.Add(Value10);
                return hashCode.ToHashCode();
            }
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

        private sealed class Gedx8CompositeRuntime
        {
            internal Gedx8CompositeRuntime(object? source04, object? driver08, object? link0C, int loaderMode08, int descriptorId14, byte entryCount1A, ushort groupCount18, uint[]? table10)
            {
                Source04 = source04;
                Driver08 = driver08;
                Link0C = link0C;
                LoaderMode08 = loaderMode08;
                DescriptorId14 = descriptorId14;
                EntryCount1A = entryCount1A;
                GroupCount18 = groupCount18;
                InitResult1B = 0;
                LastProbeResult = -1;

                LoadTable(table10);
            }

            internal object? Source04 { get; }

            internal object? Driver08 { get; }

            internal object? Link0C { get; }

            internal int LoaderMode08 { get; }

            internal uint[]? Table10 { get; private set; }

            internal int DescriptorId14 { get; }

            internal ushort GroupCount18 { get; }

            internal byte EntryCount1A { get; }

            internal byte InitResult1B { get; private set; }

            internal int LastForwardValue { get; set; }

            internal int LastResolvedGroup { get; set; }

            internal int LastResolvedEntry { get; set; }

            internal int LastReducedOffset { get; set; }

            internal int LastShiftedBase { get; set; }

            internal int LastStepCount { get; set; }

            internal int LastProbeResult { get; set; }

            internal bool LoadTable(uint[]? table10)
            {
                Table10 = null;
                InitResult1B = 0;
                LastProbeResult = -1;
                LastResolvedGroup = 0;
                LastResolvedEntry = 0;
                LastReducedOffset = 0;
                LastShiftedBase = 0;
                LastStepCount = 0;

                if (EntryCount1A == 0 || GroupCount18 == 0)
                {
                    return false;
                }

                int expectedLength = EntryCount1A * GroupCount18;
                if (expectedLength <= 0)
                {
                    return false;
                }

                if (table10 == null || table10.Length != expectedLength)
                {
                    return false;
                }

                uint[] tableCopy = new uint[expectedLength];
                Array.Copy(table10, tableCopy, expectedLength);
                Table10 = tableCopy;
                InitResult1B = 1;
                LastProbeResult = 1;
                return true;
            }

            internal void ReleaseTable()
            {
                Table10 = null;
                InitResult1B = 0;
                LastProbeResult = -1;
            }

            internal bool TryGetPackedValue(int groupIndex, int entryIndex, out uint packedValue)
            {
                packedValue = 0;

                if (Table10 == null)
                {
                    return false;
                }

                if ((uint)groupIndex >= GroupCount18 || (uint)entryIndex >= EntryCount1A)
                {
                    return false;
                }

                int flatIndex = (groupIndex * EntryCount1A) + entryIndex;
                if ((uint)flatIndex >= Table10.Length)
                {
                    return false;
                }

                packedValue = Table10[flatIndex];
                return true;
            }
        }

        private readonly struct Gedx8CompositeWindowMatch
        {
            private Gedx8CompositeWindowMatch(int groupIndex, int entryIndex, int reducedOffset, int shiftedBase, int stepCount)
            {
                GroupIndex = groupIndex;
                EntryIndex = entryIndex;
                ReducedOffset = reducedOffset;
                ShiftedBase = shiftedBase;
                StepCount = stepCount;
            }

            internal int GroupIndex { get; }

            internal int EntryIndex { get; }

            internal int ReducedOffset { get; }

            internal int ShiftedBase { get; }

            internal int StepCount { get; }

            internal static bool TryCreate(int groupIndex, int entryIndex, uint packedValue, int inputValue, out Gedx8CompositeWindowMatch match)
            {
                int baseValue = (int)(packedValue >> 16);
                int windowLength = (int)((packedValue >> 8) & 0xFF) + 1;
                int stepValue = (int)(packedValue & 0xFF) + 1;

                if (inputValue < baseValue)
                {
                    match = default;
                    return false;
                }

                int delta = inputValue - baseValue;
                int stepCount = delta / stepValue;
                int reducedOffset = delta % stepValue;
                int shiftedBase = baseValue + (stepCount * stepValue);

                if (reducedOffset >= windowLength)
                {
                    match = default;
                    return false;
                }

                match = new Gedx8CompositeWindowMatch(groupIndex, entryIndex, reducedOffset, shiftedBase, stepCount);
                return true;
            }
        }
    }
}
