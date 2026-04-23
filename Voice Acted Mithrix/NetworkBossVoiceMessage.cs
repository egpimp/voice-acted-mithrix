using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace VoiceActedMithrix
{
    internal class NetworkBossVoiceMessage : INetMessage, ISerializableObject
    {
        internal string name;
        internal GameObject source;

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(name);
            writer.Write(source);
        }

        public void Deserialize(NetworkReader reader) 
        {
            name = reader.ReadString();
            source = reader.ReadGameObject();
        }

        public void OnReceived()
        {
            if (!source) return;
            if (NetworkServer.active) return;
            if (Configuration.PlayerSourcedVoice.Value) source = VoiceActedMithrix.GetLocalPlayer().gameObject;
            AkSoundEngine.PostEvent(name, source);
        }
    }
}
