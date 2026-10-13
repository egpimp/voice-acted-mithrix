using UnityEngine;
using UnityEngine.UI;

namespace VoiceActedMithrix.UI
{
    public class VoiceChanger : MonoBehaviour
    {
        static string MithrixPreface = "Mithrix Voice Actor: ";
        static string FalseSonPreface = "False Son Voice Actor: ";
        static string SolusHeartPreface = "Solus Heart Voice Actor: ";

        string voiceTarget;

        Text textAsset;

        static string[] voiceMapM = new string[]
        {
            "MotherEgg",
            "KKing"
        };

        static string[] voiceMapFS = new string[]
        {
            "SkellyVA",
            "KKing"
        };

        static string[] voiceMapSH = new string[]
        {
            "KaioVA",
            "BootlegBison"
        };

        void Start()
        {
            voiceTarget = gameObject.name;
            textAsset = transform.Find("Text").GetComponent<Text>();
            UpdateText(false);
        }

        public void UpdateText(bool changeConfig)
        {
            switch (voiceTarget)
            {
                case "B1":
                    if (changeConfig)
                    {
                        Configuration.MithrixVoice.Value = (Configuration.MithrixVoice.Value + 1) % voiceMapM.Length;
                        VoiceActedMithrix.LoadBrotherVoice();
                    }
                    textAsset.text = MithrixPreface + voiceMapM[Configuration.MithrixVoice.Value];
                    if (Configuration.MithrixVoice.Value == 0) VoiceActedMithrix.HereticKurskanFix("Kur-skan");
                    else VoiceActedMithrix.HereticKurskanFix("Heretic");
                    break;
                case "B2":
                    if (changeConfig)
                    {
                        Configuration.FalseSonVoice.Value = (Configuration.FalseSonVoice.Value + 1) % voiceMapFS.Length;
                        VoiceActedMithrix.LoadFalseSonVoice();
                    }
                    textAsset.text = FalseSonPreface + voiceMapFS[Configuration.FalseSonVoice.Value];
                    break;
                case "B3":
                    if (changeConfig)
                    {
                        Configuration.SolusHeartVoice.Value = (Configuration.SolusHeartVoice.Value + 1) % voiceMapSH.Length;
                        VoiceActedMithrix.LoadSolusHeartVoice();
                    }
                    textAsset.text = SolusHeartPreface + voiceMapSH[Configuration.SolusHeartVoice.Value];
                    break;
                default:
                    break;
            }
        }
    }
}
