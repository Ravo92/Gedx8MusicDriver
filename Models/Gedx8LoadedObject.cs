namespace Gedx8MusicDriver.Models
{
    public sealed class Gedx8LoadedObject
    {
        internal Gedx8LoadedObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory, string? resolvedPath)
        {
            Kind = kind;
            FileName = fileName;
            SearchDirectory = searchDirectory;
            ResolvedPath = resolvedPath;
            IsActive = true;
        }

        public Gedx8ObjectKind Kind { get; }

        public string FileName { get; }

        public string? SearchDirectory { get; }

        public string? ResolvedPath { get; }

        public bool IsActive { get; private set; }

        public string? LastOpenName { get; private set; }

        public string? LastStringValue { get; private set; }

        internal bool TryCompositeCall04190(int arg0, int arg1, int arg2, int arg3, int arg4)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            LastStringValue = $"04190:{arg0}:{arg1}:{arg2}:{arg3}:{arg4}";
            return true;
        }

        internal bool TryCompositeCall04250(string? value)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            LastStringValue = value;
            return true;
        }

        internal bool TryCompositeCall04280(string? value)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            LastStringValue = value;
            return true;
        }

        internal bool TryCompositeCall042C0(string name, out string? value)
        {
            value = null;
            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
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

            LastOpenName = name;
            value = ResolvedPath ?? name;
            return true;
        }

        internal bool TryCompositeCall04640(string? value)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.Composite)
            {
                return false;
            }

            LastStringValue = value;
            return true;
        }

        internal bool TryThinType1Call02D50()
        {
            return IsActive && Kind == Gedx8ObjectKind.ThinType1;
        }

        internal bool TryThinType1Call02D70()
        {
            return IsActive && Kind == Gedx8ObjectKind.ThinType1 && !string.IsNullOrWhiteSpace(ResolvedPath);
        }

        internal bool TryThinType2Release03E90()
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            IsActive = false;
            return true;
        }

        internal bool TryThinType2Open03EB0(string name)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
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

            result = $"03F00:{selector}:{value}:{ResolvedPath}";
            return true;
        }

        internal bool TryThinType2Query03F50(int selector, int value, out string? result)
        {
            result = null;
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            result = $"03F50:{selector}:{value}:{LastOpenName}";
            return true;
        }

        internal bool TryThinType2Query03FA0(int selector, int value, out string? result)
        {
            result = null;
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            result = $"03FA0:{selector}:{value}:{LastStringValue}";
            return true;
        }

        internal bool TryThinType2Configure04020(int selector, int value, string? text)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            LastStringValue = $"04020:{selector}:{value}:{text}";
            return true;
        }

        internal bool TryThinType2Configure04070(int selector, int value, string? text)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            LastStringValue = $"04070:{selector}:{value}:{text}";
            return true;
        }

        internal bool TryThinType2Configure040D0(int selector, int value, string? text)
        {
            if (!IsActive || Kind != Gedx8ObjectKind.ThinType2)
            {
                return false;
            }

            LastStringValue = $"040D0:{selector}:{value}:{text}";
            return true;
        }

        internal void Deactivate()
        {
            IsActive = false;
        }
    }
}
