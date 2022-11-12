using System;
using System.Collections.Generic;
using System.IO;
namespace MidiPlayerTK
{
    using UnityEngine;
    public class MidiEditorLib
    {
        public MidiFileEditorPlayer MidiPlayer;
        private GameObject goSequencer;
        private string nameComponent;
        private bool logSoundFontLoaded;
        private bool logDebug;
        public MidiEditorLib(string _nameComponent = null, bool _logSoundFontLoaded = false, bool _logDebug = false)
        {
            nameComponent = _nameComponent != null ? _nameComponent : "MidiSequencerEditor";
            logSoundFontLoaded = _logSoundFontLoaded;
            logDebug = _logDebug;
            //Debug.Log(">>> Awake MidiSequencerWindow ..." + nameSequencer + " Application.isPlaying:" + Application.isPlaying);
            LoadPlayer();
        }

        private void LoadPlayer()
        {
            if (logDebug) Debug.Log(">>> Load Editor Player ..." + nameComponent + " Application.isPlaying:" + Application.isPlaying);

            GameObject oldGo = GameObject.Find(nameComponent);
            if (oldGo != null)
            {
                if (logDebug) Debug.Log("Delete previous " + nameComponent);
                if (Application.isPlaying)
                    Object.Destroy(oldGo);
                else
                    Object.DestroyImmediate(oldGo, true);
            }

            goSequencer = new GameObject();
            goSequencer.name = nameComponent;
            goSequencer.hideFlags = HideFlags.DontSave;
            MidiPlayer = goSequencer.AddComponent<MidiFileEditorPlayer>();

            MidiPlayerGlobal midiPlayerGlobal;
            GameObject goMidiGlobal = GameObject.Find("MidiPlayerGlobal");
            if (goMidiGlobal == null)
            {
                if (logDebug) Debug.Log("Not found MidiPlayerGlobal");
                if (logDebug) Debug.Log("     ... create a Midi Global");
                GameObject objectMidiGlobal = new GameObject();
                objectMidiGlobal.hideFlags = HideFlags.DontSave;
                objectMidiGlobal.name = "MidiPlayerGlobal";
                midiPlayerGlobal = objectMidiGlobal.gameObject.AddComponent<MidiPlayerGlobal>();
            }
            else
            {
                if (logDebug)
                {
                    Debug.Log("Found MidiPlayerGlobal");
                    Transform parent = goMidiGlobal.transform.parent;
                    if (parent == null)
                        Debug.Log("     ... parent is null");
                    else
                        Debug.Log("     ... parent is " + parent.name);
                }
                midiPlayerGlobal = goMidiGlobal.GetComponent<MidiPlayerGlobal>();
                if (logDebug && midiPlayerGlobal == null)
                    Debug.LogWarning("     ... midiPlayerGlobal is null");
            }

            MidiPlayerGlobal.InitPath();
            ToolsEditor.LoadMidiSet();
            midiPlayerGlobal.InitInstance();
            if (logSoundFontLoaded)
                MidiPlayerGlobal.OnEventPresetLoaded.AddListener(SoundFontIsReadyEvent);
            //MidiPlayerGlobal.MPTK_LoadLiveSF("file://" + MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.SF2Path, -1, -1, false);
            MidiPlayerGlobal.LoadCurrentSF();

            MidiPlayer.MPTK_CorePlayer = true;
            MidiPlayer.MPTK_StartPlayAtFirstNote = true;
            MidiPlayer.MPTK_DirectSendToPlayer = true;
            MidiPlayer.CoreAudioSource.clip = Create();
            MidiPlayer.CoreAudioSource.Play();

            if (logDebug) Debug.Log("<<< Load Editor Player ..." + nameComponent);
        }

        public void SoundFontIsReadyEvent()
        {
            Debug.LogFormat("Loaded SF '{0}', MPTK is ready to play", MidiPlayerGlobal.ImSFCurrent.SoundFontName);
            Debug.Log("   Time To Load SoundFont: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadSoundFont.TotalSeconds, 3).ToString() + " second");
            Debug.Log("   Time To Load Samples: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadWave.TotalSeconds, 3).ToString() + " second");
            Debug.Log("   Presets Loaded: " + MidiPlayerGlobal.MPTK_CountPresetLoaded);
            Debug.Log("   Samples Loaded: " + MidiPlayerGlobal.MPTK_CountWaveLoaded);
        }

        // Read all the samples from the clip and half the gain
        private AudioClip Create()
        {
            int samplerate = 44100;
            int sampleCount = 10;
            int sampleChannel = 1;
            //float frequency = 440;
            AudioClip myClip = AudioClip.Create("MySinusoid", sampleCount, sampleChannel, samplerate, false);
            float[] samples = new float[sampleCount * sampleChannel];
            for (int i = 0; i < samples.Length; ++i)
            {
                samples[i] = 0f; // Mathf.Sin(2 * Mathf.PI * frequency * i / samplerate);
            }
            myClip.SetData(samples, 0);
            return myClip;
        }

        public void DestroyMidiObject()
        {
            if (logDebug) Debug.Log(">>> DestroyMidiObject ... Application.isPlaying:" + Application.isPlaying);

            if (goSequencer != null)
                if (Application.isPlaying)
                    Object.Destroy(goSequencer);
                else
                    Object.DestroyImmediate(goSequencer, true);
            MidiPlayer = null;
            goSequencer = null;
            if (logDebug) Debug.Log("<<< DestroyMidiObject ... " + Application.isPlaying);
        }
    }
}