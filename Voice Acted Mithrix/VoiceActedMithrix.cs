using BepInEx;
using On.RoR2.CharacterSpeech;
using R2API;
using R2API.Networking;
using RoR2;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;
using VoiceActedMithrix.UI;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace VoiceActedMithrix
{
    [BepInDependency(SoundAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(LanguageAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(NetworkingAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(MODNAME, MODTITLE, MODVERS)]

    public class VoiceActedMithrix : BaseUnityPlugin
    {
        //Mod focused stuff / strings
        public const string MODNAME = "com.Egg.VoiceActedMithrix";
        public const string MODTITLE = "Voice Acted Mithrix Redux";
        public const string MODVERS = "1.1.1";

        //Path for our lang folder
        internal static string BankPathBrother;
        internal static string BankPathFalseson;
        internal static string BankPathVulture;
        internal static string BankPathSolus;
        internal static string BankPathChef;

        static string[] brotherFileNames = new string[]
        {
            "vamithrixredux.bnk",
            "vakkingmithrix.bnk"
        };
        static string[] falseSonFileNames = new string[]
        {
            "vafalseson.bnk",
            "vakkingfalseson.bnk"
        };
        static string vultureFileName = "vavulture.bnk";
        static string solusFileName = "vasolus.bnk";
        static string chefFileName = "vachef.bnk";

        internal static uint currentMithrixVoiceBank;
        internal static uint currentFalseSonVoiceBank;

        public static PluginInfo PInfo { get; private set; }

        private void Awake()
        {
            PInfo = base.Info;
            Configuration.LoadConfig();
            BankPathBrother = GetBrotherPath(Configuration.MithrixVoice.Value);
            BankPathFalseson = GetFalseSonPath(Configuration.FalseSonVoice.Value);
            BankPathVulture = GetVulturePath();
            BankPathChef = GetChefPath();
            BankPathSolus = GetSolusPath();

            SoundAPI.SoundBanks.Add(BankPathVulture);
            SoundAPI.SoundBanks.Add(BankPathChef);
            SoundAPI.SoundBanks.Add(BankPathSolus);

            currentMithrixVoiceBank = SoundAPI.SoundBanks.Add(BankPathBrother);
            currentFalseSonVoiceBank = SoundAPI.SoundBanks.Add(BankPathFalseson);

            NetworkingAPI.RegisterMessageType<NetworkBossVoiceMessage>();
            if (Configuration.MithrixVoice.Value == 0) HereticKurskanFix();
            if(Configuration.DisplayUI.Value) UIManager.Init();
        }

        internal static string GetBrotherPath(int index)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", brotherFileNames[index]);
        }

        internal static string GetFalseSonPath(int index)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", falseSonFileNames[index]);

        }

        internal static string GetVulturePath()
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", vultureFileName);
        }

        internal static string GetChefPath()
        { 
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", chefFileName);
        }

        internal static string GetSolusPath()
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", solusFileName);
        }

        internal static void LoadBrotherVoice()
        {
            SoundAPI.SoundBanks.Remove(currentMithrixVoiceBank);
            currentMithrixVoiceBank = SoundAPI.SoundBanks.Add(GetBrotherPath(Configuration.MithrixVoice.Value));
        }

        internal static void LoadFalseSonVoice()
        {
            SoundAPI.SoundBanks.Remove(currentFalseSonVoiceBank);
            currentFalseSonVoiceBank = SoundAPI.SoundBanks.Add(GetFalseSonPath(Configuration.FalseSonVoice.Value));
        }


        private void OnEnable()
        {
            BrotherSpeechDriver.SendReponseFromPool += BrotherSpeechDriver_SendReponseFromPool;
            FalseSonBossSpeechDriver.SendReponseFromPool += FalseSonBossSpeechDriver_SendReponseFromPool;
            CharacterSpeechController.SpeakNow += OnSpeak;
            On.RoR2.OverrideFightBase.NpcMessage += VultureSpeak;
            
            if(Configuration.DisplayUI.Value) On.RoR2.UI.MainMenu.MainMenuController.Start += UIManager.AddUI;
        }

        private void OnDisable()
        {
            BrotherSpeechDriver.SendReponseFromPool -= BrotherSpeechDriver_SendReponseFromPool;
            FalseSonBossSpeechDriver.SendReponseFromPool -= FalseSonBossSpeechDriver_SendReponseFromPool;
            CharacterSpeechController.SpeakNow -= OnSpeak;
            On.RoR2.OverrideFightBase.NpcMessage -= VultureSpeak;

            if (Configuration.DisplayUI.Value) On.RoR2.UI.MainMenu.MainMenuController.Start -= UIManager.AddUI;
        }

        private static void OnSpeak(CharacterSpeechController.orig_SpeakNow orig, RoR2.CharacterSpeech.CharacterSpeechController self, ref RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo speechInfo)
        {
            orig(self, ref speechInfo);

            CharacterBody body = self.currentCharacterBody;

            GameObject sourceObject;

            if (body) sourceObject = self.currentCharacterBody.gameObject;
            else sourceObject = self.gameObject;

            if (speechInfo.token.Contains("BROTHER")) BrotherSpeak(speechInfo.token, sourceObject);
            else if (speechInfo.token.Contains("FALSESONBOSS")) FalseSonSpeak(speechInfo.token, sourceObject);
            else if (speechInfo.token.Contains("SOLUSHEART")) HeartSpeak(speechInfo.token, sourceObject);
            else if (speechInfo.token.Contains("MEALPREP")) ChefSpeak(speechInfo.token, sourceObject);    
            
        }

        private static void BrotherSpeak(string token, GameObject sourceObject)
        {
            string modified = GetModifiedToken(token, Configuration.MithrixVoice.Value);
            BossVoiceComponent component = sourceObject.GetComponent<BossVoiceComponent>();
            if (!component)
            {
                component = sourceObject.AddComponent<BossVoiceComponent>();
                component.canBeAngy = true;
            }
            component.TryVoiceLine(new BossLineEvent(modified, sourceObject, sourceObject));
        }

        private static void FalseSonSpeak(string token, GameObject sourceObject)
        {
            string modified = GetModifiedToken(token, Configuration.FalseSonVoice.Value);
            if (modified.Contains("_DEATH_") || modified.Contains("PURIFICATION")) sourceObject = GameObject.Find("MeridianEventTriggerCore");
            BossVoiceComponent component = sourceObject.GetComponent<BossVoiceComponent>();
            if (!component)
            {
                component = sourceObject.AddComponent<BossVoiceComponent>();
                component.canBeAngy = true;
            }
            component.TryVoiceLine(new BossLineEvent(modified, sourceObject, sourceObject));
        }

        private static void HeartSpeak(string token, GameObject sourceObject)
        {
            string modified = "VAMR_" + token;
            BossVoiceComponent component = sourceObject.GetComponent<BossVoiceComponent>();
            if (!component) component = sourceObject.AddComponent<BossVoiceComponent>();
            component.TryVoiceLine(new BossLineEvent(modified, GetLocalPlayer().bodyInstanceObject, sourceObject));
        }

        private static void ChefSpeak(string token, GameObject sourceObject)
        {
            string modified = "VAMR_" + token;
            BossVoiceComponent component = sourceObject.GetComponent<BossVoiceComponent>();
            if (!component) component = sourceObject.AddComponent<BossVoiceComponent>();
            component.TryVoiceLine(new BossLineEvent(modified, sourceObject, sourceObject));
        }

        private static void VultureSpeak(On.RoR2.OverrideFightBase.orig_NpcMessage orig, OverrideFightBase self, string message, string formatString, string sound)
        {
            orig(self, message, formatString, sound);
            string modified = "VAMR_" + message;
            BossVoiceComponent component = self.gameObject.GetComponent<BossVoiceComponent>();
            if (!component) component = self.gameObject.AddComponent<BossVoiceComponent>();
            component.TryVoiceLine(new BossLineEvent(modified, GetLocalPlayer().bodyInstanceObject, self.gameObject));
        }

        private static string GetModifiedToken(string baseToken, int voiceIndex)
        {
            return "VAMR_" + voiceIndex + "_" + baseToken;
        }

        private void HereticKurskanFix()
        {
            LanguageAPI.Add("BROTHER_SEE_HERETIC_1", "Regret this journey, Kur-skan.", "en");
            LanguageAPI.Add("BROTHER_KILL_HERETIC_2", "Die again, Kur-skan.", "en");
            LanguageAPI.Add("BROTHERHURT_KILL_HERETIC_2", "DIE AGAIN, KUR-SKAN.", "en");
        }

        internal static CharacterMaster GetLocalPlayer()
        {
            return CharacterMaster.readOnlyInstancesList[0];
        }

        //How the fuck did they overlook this
        private static void FalseSonBossSpeechDriver_SendReponseFromPool(FalseSonBossSpeechDriver.orig_SendReponseFromPool orig, RoR2.CharacterSpeech.FalseSonBossSpeechDriver self, RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[] responsePool)
        {
            if (responsePool.Length == 0) return;
            self.characterSpeechController.EnqueueSpeech(responsePool[Random.Range(0, responsePool.Length)]);
        }

        private static void BrotherSpeechDriver_SendReponseFromPool(BrotherSpeechDriver.orig_SendReponseFromPool orig, RoR2.CharacterSpeech.BrotherSpeechDriver self, RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[] responsePool)
        {
            if (responsePool.Length == 0) return;
            self.characterSpeechController.EnqueueSpeech(responsePool[Random.Range(0, responsePool.Length)]);
        }
    }
}
