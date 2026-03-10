using Gedx8MusicDriver.Models;

namespace Gedx8MusicDriver.Core
{
    internal static class Gedx8SynthProfiles
    {
        internal static Gedx8SynthInitConfig Resolve(int synthMode)
        {
            return synthMode switch
            {
                1 => new Gedx8SynthInitConfig(0, 22050, 0x10),
                2 => new Gedx8SynthInitConfig(0, 11025, 0x08),
                _ => new Gedx8SynthInitConfig(0, 44100, 0x40),
            };
        }
    }
}
