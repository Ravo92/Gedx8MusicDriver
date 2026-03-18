namespace Gedx8MusicDriver.Models
{
    internal sealed class Gedx8Audiopath
    {
        internal Gedx8Audiopath(int token, int profile, Gedx8LoadedObject? seedObject)
        {
            Token = token;
            Profile = profile;
            SeedObject = seedObject;
        }

        internal int Token { get; }

        internal int Profile { get; }

        internal Gedx8LoadedObject? SeedObject { get; }

        internal bool IsActive { get; private set; }

        internal int Volume { get; private set; }

        internal int FadeMilliseconds { get; private set; }

        internal Gedx8LoadedObject? LastSegment { get; private set; }

        internal int LastFlags { get; private set; }

        internal int LastStartTime { get; private set; }

        internal int LastRepeatCount { get; private set; }

        internal int LastReservedValue { get; private set; }

        internal int LastImmediateFlag { get; private set; }

        internal void SetActive(bool value)
        {
            IsActive = value;
        }

        internal void SetVolume(int volume, int fadeMilliseconds)
        {
            Volume = volume;
            FadeMilliseconds = fadeMilliseconds;
        }

        internal void AttachSegment(Gedx8LoadedObject loadedObject, int flags, int startTime, int repeatCount, int reserved, int immediateFlag)
        {
            LastSegment = loadedObject;
            LastFlags = flags;
            LastStartTime = startTime;
            LastRepeatCount = repeatCount;
            LastReservedValue = reserved;
            LastImmediateFlag = immediateFlag;
        }
    }
}
