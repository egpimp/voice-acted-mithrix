using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoiceActedMithrix
{
    class BossVoiceComponent : MonoBehaviour
    {
        //internal GameObject vb;
        internal bool playingLine = false;
        internal bool delaying = false;
        List<BossLineEvent> queuedLines = new List<BossLineEvent>();

        internal bool gone = false;
        internal bool angy = false;
        bool hurt = false;
        internal bool canBeAngy = false;
        int angyCounter = 2;

        float timer;

        float baseDelay = 1f;
        float deltaDelay = 0.3f;

        void Start()
        {
            timer = baseDelay + Random.Range(-deltaDelay, deltaDelay);
        }

        internal void FixedUpdate()
        {
            if (timer > 0f) timer -= Time.fixedDeltaTime;
            else delaying = false;

            if (queuedLines.Count > 0 && !playingLine && !delaying)
            {
                PlaySound(queuedLines[0]);
                queuedLines.RemoveAt(0);
            }
        }

        internal void TryVoiceLine(BossLineEvent sound)
        {
            if (angy) return;
            if (!hurt && sound.eventName.Contains("BROTHERHURT")) hurt = true;
            if (queuedLines.Count > 2) BecomePissed(sound.source);
            else if (playingLine || delaying || queuedLines.Count > 0) queuedLines.Add(sound);
            else PlaySound(sound);
        }

        internal void BecomePissed(GameObject source)
        {
            if (!canBeAngy) return;
            queuedLines.Clear();
            if(Random.Range(0, angyCounter * 4 + 1) == 0)
            {
                angy = true;
                BossLineEvent angySound;
                if (base.gameObject.name.Contains("FalseSon")) angySound = new BossLineEvent("VAMR_" + Configuration.FalseSonVoice.Value + "_FALSESON_QUEUEFULL", source, source, true);
                else angySound = hurt ? new BossLineEvent("VAMR_" + Configuration.MithrixVoice.Value + "_BROTHERHURT_QUEUEFULL", source, source, true) : new BossLineEvent("VAMR_" + Configuration.MithrixVoice.Value + "_BROTHER_QUEUEFULL", source, source, true);
                queuedLines.Add(angySound);
                angyCounter = 2;
                return;
            }            
            angyCounter--;
        }

        internal void PlaySound(BossLineEvent soundEvent)
        {
            timer = baseDelay + Random.Range(-deltaDelay, deltaDelay);
            playingLine = true;
            delaying = true;
            uint soundEventID = soundEvent.Play();
            uint[] playingSoundIDs = new uint[10];
            uint count = (uint)playingSoundIDs.Length;
            AKRESULT result = AkSoundEngine.GetPlayingIDsFromGameObject(soundEvent.source, ref count, playingSoundIDs);
            if (result != AKRESULT.AK_Success || !playingSoundIDs.Contains(soundEventID)) playingLine = angy = false;
        }
    }
}
