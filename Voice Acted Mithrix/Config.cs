using BepInEx;
using BepInEx.Configuration;

namespace VoiceActedMithrix
{
    internal class Configuration
    {
        private static ConfigFile configFile;

        internal static ConfigEntry<int> MithrixVoice { get; private set; }
        internal static ConfigEntry<int> FalseSonVoice { get; private set; }
        internal static ConfigEntry<bool> PlayerSourcedVoice { get; private set; }
        internal static ConfigEntry<bool> DisplayUI { get; private set; }

        internal const int DefaultMithrixVoice = 0;
        internal const int DefaultFalseSonVoice = 0;
        internal const bool DefaultPlayerSourcedVoice = false;
        internal const bool DefaultDisplayUI = true;

        internal static void LoadConfig()
        {
            configFile = new ConfigFile(Paths.ConfigPath + "/VAMithrix.cfg", true);
            MithrixVoice = configFile.Bind("Voices", "Mithrix VA", DefaultMithrixVoice, "Voice actor to use for Mithrix boss.  0: MotherEgg | 1: KKing");
            FalseSonVoice = configFile.Bind("Voices", "False Son VA", DefaultFalseSonVoice, "Voice actor to use for False Son boss.  0: SkellyVA | 1: KKing");
            PlayerSourcedVoice = configFile.Bind("Settings", "Use Player As Voice Source", DefaultPlayerSourcedVoice, "If true, the voice will source directly from the player object rather than the boss, giving more consistent volume without scaling over distance");
            DisplayUI = configFile.Bind("Settings", "Display Voice Changing UI", DefaultDisplayUI, "If false, will no longer display the UI on the title screen and will not allow changing voice without opening config file");
        }
    }
}