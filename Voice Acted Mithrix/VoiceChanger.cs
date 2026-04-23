using UnityEngine;
using UnityEngine.UI;

namespace VoiceActedMithrix.UI
{
    public class VoiceChanger : MonoBehaviour
    {
        static string MithrixPreface = "Mithrix Voice Actor: ";
        static string FalseSonPreface = "False Son Voice Actor: ";

        bool voiceM;

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

        void Start()
        {
            voiceM = gameObject.name == "B1";
            textAsset = transform.Find("Text").GetComponent<Text>();
            UpdateText(false);
        }

        public void UpdateText(bool changeConfig)
        {
            if (changeConfig)
            {
                if (voiceM)
                {
                    Configuration.MithrixVoice.Value = (Configuration.MithrixVoice.Value + 1) % voiceMapM.Length;
                    VoiceActedMithrix.LoadBrotherVoice();
                }
                else
                {
                    Configuration.FalseSonVoice.Value = (Configuration.FalseSonVoice.Value + 1) % voiceMapFS.Length;
                    VoiceActedMithrix.LoadFalseSonVoice();
                }
            }

            if (voiceM) textAsset.text = MithrixPreface + voiceMapM[Configuration.MithrixVoice.Value];
            else textAsset.text = FalseSonPreface + voiceMapFS[Configuration.FalseSonVoice.Value];
        }
    }
}
