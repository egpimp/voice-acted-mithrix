using R2API.Networking.Interfaces;
using UnityEngine;
using UnityEngine.Networking;

namespace VoiceActedMithrix
{
    internal class BossLineEvent
    {
        public string eventName;
        public GameObject source;
        public GameObject owner;
        public bool angyLine;

        public BossLineEvent(string name, GameObject gameObject, GameObject ownerObject, bool angy = false)
        {
            eventName = name;
            source = gameObject;
            angyLine = angy;
            owner = ownerObject;
        }

        public uint Play()
        {
            if (Configuration.PlayerSourcedVoice.Value) source = VoiceActedMithrix.GetLocalPlayer().gameObject;
            uint soundEventID = AkSoundEngine.PostEvent(eventName, source, (uint)AkCallbackType.AK_EndOfEvent, EndEventCallback, default);
            if (NetworkServer.active)
            {
                NetMessageExtensions.Send(new NetworkBossVoiceMessage()
                {
                    name = eventName,
                    source = source
                }, R2API.Networking.NetworkDestination.Clients);
            }
            return soundEventID;
        }

        void EndEventCallback(object in_cookie, AkCallbackType callbackType, object in_info)
        {
            if (callbackType == AkCallbackType.AK_EndOfEvent && NetworkServer.active)
            {
                if (!source || !owner) return;
                BossVoiceComponent component = owner.GetComponent<BossVoiceComponent>();
                if (!component) component = owner.AddComponent<BossVoiceComponent>();

                if (angyLine) component.angy = false;

                component.playingLine = false;
            }
        }
    }
}
