using R2API;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using RoR2.UI;
using RoR2.UI.SkinControllers;
using TMPro;
using System;

namespace VoiceActedMithrix.UI
{
    internal class UIManager
    {
        static AssetBundle bundle;
        static GameObject prefab;
        static GameObject instance;
        static Sprite sprite;

        const string BUNDLE_NAME = "vamithrixbundle";
        const string FOLDER_NAME = "assets";

        static ColorBlock colors = new ColorBlock()
        {
            colorMultiplier = 1,
            disabledColor = new Color(0.254f, 0.2f, 0.2f, 0.713f),
            highlightedColor = new Color(0.988f, 1f, 0.693f, 0.733f),
            normalColor = new Color(0.327f, 0.402f, 0.471f, 1f),
            pressedColor = new Color(0.739f, 0.754f, 0.444f, 0.984f),
            selectedColor = new Color(0.988f, 1f, 0.693f, 0.733f)
        };

        internal static void Init()
        {
            RegisterTokens();
            RegisterPrefab();
        }

        static void RegisterPrefab()
        {
            string path = Path.Combine(Path.GetDirectoryName(VoiceActedMithrix.PInfo.Location), FOLDER_NAME, BUNDLE_NAME);
            bundle = AssetBundle.LoadFromFile(path);
            prefab = bundle.LoadAsset<GameObject>("VoiceSelectionPanel");
            GameObject btn1 = prefab.transform.Find("Canvas/B1").gameObject;
            GameObject btn2 = prefab.transform.Find("Canvas/B2").gameObject;
            btn1.GetComponent<Image>().sprite = btn2.GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texUICleanPanel.png").WaitForCompletion();
            ModifyButton(btn1).hoverToken = "VMR_TITLE_MITHRIXVOICE_DESC";
            ModifyButton(btn2).hoverToken = "VMR_TITLE_FALSESONVOICE_DESC";
        }

        static HGButton ModifyButton(GameObject button)
        {
            VoiceChanger vComponent = button.AddComponent<VoiceChanger>();
            HGButton bComponent = button.AddComponent<HGButton>();
            bComponent.targetGraphic = button.GetComponent<Image>();
            bComponent.selectOnPointerEnter = true;
            bComponent.submitOnPointerUp = true;
            bComponent.updateTextOnHover = true;
            bComponent.allowAllEventSystems = true;
            bComponent.colors = colors;
            LanguageTextMeshController lComponent = button.AddComponent<LanguageTextMeshController>();
            return bComponent;
        }

        static void RegisterTokens()
        {
            LanguageAPI.Add("VMR_TITLE_MITHRIXVOICE_DESC", "Cycle through which voice actor to use for the Mithrix voice.", "en");
            LanguageAPI.Add("VMR_TITLE_FALSESONVOICE_DESC", "Cycle through which voice actor to use for the False Son voice.", "en");
        }

        internal static void AddUI(On.RoR2.UI.MainMenu.MainMenuController.orig_Start orig, RoR2.UI.MainMenu.MainMenuController self)
        {
            orig(self);
            Transform child = self.transform.Find("MENU: Title/TitleMenu/SafeZone");
            if (child == null) return;
            instance = GameObject.Instantiate(prefab, child);
            GameObject b1 = instance.transform.Find("Canvas/B1").gameObject;
            GameObject b2 = instance.transform.Find("Canvas/B2").gameObject;
            HGButton b1Button = b1.GetComponent<HGButton>();
            VoiceChanger b1Component = b1.GetComponent<VoiceChanger>();
            HGButton b2Button = b2.GetComponent<HGButton>();
            VoiceChanger b2Component = b2.GetComponent<VoiceChanger>();

            b1Button.hoverLanguageTextMeshController = b2Button.hoverLanguageTextMeshController = child.Find("GenericMenuButtonPanel/JuicePanel/DescriptionPanel, Naked/ContentSizeFitter/DescriptionText").GetComponent<LanguageTextMeshController>();
            b1Button.onClick.AddListener(() => b1Component.UpdateText(true));
            b2Button.onClick.AddListener(() => b2Component.UpdateText(true));
        }
    }
}
