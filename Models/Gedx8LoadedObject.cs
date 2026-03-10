namespace Gedx8MusicDriver.Models
{
    internal sealed class Gedx8LoadedObject
    {
        internal Gedx8LoadedObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory, string? resolvedPath)
        {
            Kind = kind;
            FileName = fileName;
            SearchDirectory = searchDirectory;
            ResolvedPath = resolvedPath;
            ActiveByte00 = 1;
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

            Sub10004170();

            CompositeHandle14 = compositeHandle14;
            CompositeEntryCount1A = compositeEntryCount1A;
            CompositeGroupCount18 = compositeGroupCount18;

            CompositeInitResult1B = Sub10004670(compositeTable) ? (byte)1 : (byte)0;
            return CompositeInitResult1B != 0;
        }

        internal void Sub10004170()
        {
            CompositeTable10 = null;
            CompositeHandle14 = 0;
            CompositeGroupCount18 = 0;
            CompositeEntryCount1A = 0;
            CompositeInitResult1B = 0;
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

            if (CompositeOwner04 == null || CompositeSource08 == null)
            {
                return false;
            }

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

            LastStringValue = value;
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

            readyByte = CompositeInitResult1B != 0 ? (byte)1 : (byte)0;
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

            LastOpenName = name;
            value = ResolvedPath ?? name;
            return true;
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

            LastOpenName = name;
            value = ResolvedPath ?? name;
            return true;
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

            LastStringValue = text;
            return true;
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
            Sub10004170();
        }
    }
}