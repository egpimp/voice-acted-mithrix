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
        public const string MODVERS = "1.1.2";

        //Path for our lang folder
        internal static string BankPathBrother;
        internal static string BankPathFalseson;
        internal static string BankPathVulture;
        internal static string BankPathSolus;
        internal static string BankPathChef;
        internal static string BankPathSpex;

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
        static string[] solusFileNames = new string[]
        {
            "vasolus.bnk",
            "vabootlegbisonsolus.bnk"
        };

        static string vultureFileName = "vavulture.bnk";
        static string chefFileName = "vachef.bnk";
        static string spexFileName = "vaspex.bnk";

        internal static uint currentMithrixVoiceBank;
        internal static uint currentFalseSonVoiceBank;
        internal static uint currentSolusHeartVoiceBank;

        public static PluginInfo PInfo { get; private set; }

        private void Awake()
        {
            PInfo = base.Info;
            Configuration.LoadConfig();
            BankPathBrother = GetBrotherPath(Configuration.MithrixVoice.Value);
            BankPathFalseson = GetFalseSonPath(Configuration.FalseSonVoice.Value);
            BankPathSolus = GetSolusPath(Configuration.SolusHeartVoice.Value);
            BankPathVulture = GetVulturePath();
            BankPathChef = GetChefPath();
            BankPathSpex = GetSpexPath();

            SoundAPI.SoundBanks.Add(BankPathVulture);
            SoundAPI.SoundBanks.Add(BankPathChef);
            SoundAPI.SoundBanks.Add(BankPathSpex);

            currentMithrixVoiceBank = SoundAPI.SoundBanks.Add(BankPathBrother);
            currentFalseSonVoiceBank = SoundAPI.SoundBanks.Add(BankPathFalseson);
            currentSolusHeartVoiceBank = SoundAPI.SoundBanks.Add(BankPathSolus);

            NetworkingAPI.RegisterMessageType<NetworkBossVoiceMessage>();
            if (Configuration.MithrixVoice.Value == 0) HereticKurskanFix("Kur-skan");
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

        internal static string GetSolusPath(int index)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", solusFileNames[index]);
        }

        internal static string GetVulturePath()
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", vultureFileName);
        }

        internal static string GetChefPath()
        { 
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", chefFileName);
        }

        internal static string GetSpexPath()
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "soundbanks", spexFileName);
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

        internal static void LoadSolusHeartVoice()
        {
            SoundAPI.SoundBanks.Remove(currentSolusHeartVoiceBank);
            currentSolusHeartVoiceBank = SoundAPI.SoundBanks.Add(GetSolusPath(Configuration.SolusHeartVoice.Value));
        }

        private void OnEnable()
        {
            BrotherSpeechDriver.SendReponseFromPool += BrotherSpeechDriver_SendReponseFromPool;
            FalseSonBossSpeechDriver.SendReponseFromPool += FalseSonBossSpeechDriver_SendReponseFromPool;
            CharacterSpeechController.SpeakNow += OnSpeak;
            On.RoR2.OverrideFightBase.NpcMessage += VultureSpeak;
            On.RoR2.Chat.SendBroadcastChat_ChatMessageBase += Chat_SendBroadcastChat_ChatMessageBase;
            MealPrepSpeechDriver.Talk += MealPrepSpeechDriver_Talk;
            
            if(Configuration.DisplayUI.Value) On.RoR2.UI.MainMenu.MainMenuController.Start += UIManager.AddUI;
        }

        private void OnDisable()
        {
            BrotherSpeechDriver.SendReponseFromPool -= BrotherSpeechDriver_SendReponseFromPool;
            FalseSonBossSpeechDriver.SendReponseFromPool -= FalseSonBossSpeechDriver_SendReponseFromPool;
            CharacterSpeechController.SpeakNow -= OnSpeak;
            On.RoR2.OverrideFightBase.NpcMessage -= VultureSpeak;
            On.RoR2.Chat.SendBroadcastChat_ChatMessageBase -= Chat_SendBroadcastChat_ChatMessageBase;
            MealPrepSpeechDriver.Talk -= MealPrepSpeechDriver_Talk;

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
            else if (speechInfo.token.Contains("SOLUS_VENDOR")) SpexSpeak(speechInfo.token, sourceObject);
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
            string modified = GetModifiedToken(token, Configuration.SolusHeartVoice.Value);
            BossVoiceComponent component = sourceObject.GetComponent<BossVoiceComponent>();
            if (!component) component = sourceObject.AddComponent<BossVoiceComponent>();
            component.TryVoiceLine(new BossLineEvent(modified, GetLocalPlayer().bodyInstanceObject, sourceObject));
        }

        private static void SpexSpeak(string token, GameObject sourceObject)
        {
            string modified = "VAMR_" + token;
            BossVoiceComponent component = sourceObject.GetComponent<BossVoiceComponent>();
            if (!component) component = sourceObject.AddComponent<BossVoiceComponent>();
            component.TryVoiceLine(new BossLineEvent(modified, sourceObject, sourceObject));
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

        internal static void HereticKurskanFix(string name)
        {
            LanguageAPI.Add("BROTHER_SEE_HERETIC_1", $"Regret this journey, {name}.", "en");
            LanguageAPI.Add("BROTHER_KILL_HERETIC_2", $"Die again, {name}.", "en");
            LanguageAPI.Add("BROTHERHURT_KILL_HERETIC_2", $"DIE AGAIN, {name.ToUpper()}.", "en");
        }

        internal static CharacterMaster GetLocalPlayer()
        {
            return CharacterMaster.readOnlyInstancesList[0];
        }

        //How the fuck did they overlook this
        private static void FalseSonBossSpeechDriver_SendReponseFromPool(FalseSonBossSpeechDriver.orig_SendReponseFromPool orig, RoR2.CharacterSpeech.FalseSonBossSpeechDriver self, RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[] responsePool)
        {
            if (responsePool.Length == 0) return;
            self.characterSpeechController.EnqueueSpeech(responsePool[UnityEngine.Random.Range(0, responsePool.Length)]);
        }

        private static void BrotherSpeechDriver_SendReponseFromPool(BrotherSpeechDriver.orig_SendReponseFromPool orig, RoR2.CharacterSpeech.BrotherSpeechDriver self, RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[] responsePool)
        {
            if (responsePool.Length == 0) return;
            self.characterSpeechController.EnqueueSpeech(responsePool[UnityEngine.Random.Range(0, responsePool.Length)]);
        }

        //I'm going to fucking shit myself dude WHY DO THEY DO IT LIKE THIS?!?
        private void Chat_SendBroadcastChat_ChatMessageBase(On.RoR2.Chat.orig_SendBroadcastChat_ChatMessageBase orig, ChatMessageBase message)
        {
            if (message.GetType() == typeof(Chat.NpcChatMessage))
            {
                Chat.NpcChatMessage npcChatMessage = message as Chat.NpcChatMessage;
                GameObject sourceObject = GameObject.Find("SolusVendorBody(Clone)");
                if (sourceObject)
                {
                    if (npcChatMessage.baseToken.Contains("SOLUS_VENDOR_SHRINE_KEY_CRAFTED")) SpexSpeak(npcChatMessage.baseToken, sourceObject);
                    else Log.LogWarning("Where in the FUCK did SPEX go?!?!?");
                }
            }
            orig(message);
        }
        //Don't ask.  Or do, I hate this.
        private void MealPrepSpeechDriver_Talk(MealPrepSpeechDriver.orig_Talk orig, RoR2.CharacterSpeech.MealPrepSpeechDriver self, int itemID)
        {
            if (self.thankYouComeAgainDecryptedKeyCreated.Length == 1)
            {
                RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo info = self.thankYouComeAgainDecryptedKeyCreated[0];
                info.token = "SOLUS_VENDOR_SHRINE_KEY_CRAFTED_DISTANT";
                self.thankYouComeAgainDecryptedKeyCreated = new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[2]
                {
                    self.thankYouComeAgainDecryptedKeyCreated[0],
                    info
                };
            }
            orig(self, itemID);
        }
    }
}
