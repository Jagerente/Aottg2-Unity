using UnityEngine;
using ApplicationManagers;
using Characters;
using Settings;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using Photon.Voice;
using Utility;
using Unity.VisualScripting;
using GameManagers;

namespace ApplicationManagers
{
    class VoiceChatManager : MonoBehaviour
    {
        private static VoiceChatManager _instance;
        public static PunVoiceClient Client;
        public static float ProximitySpatialBlend = 1.0f;

        public static string[] MicrophoneDevices
        {
            get
            {
                return Microphone.devices;
            }
        }

        public static string DefaultDevice
        {   get
            {
                return Microphone.devices.Length > 0 ? Microphone.devices[0] : string.Empty;
            }
        }

        public static void Init()
        {
            _instance = SingletonFactory.CreateSingleton(_instance);
            Client = _instance.AddComponent<PunVoiceClient>();
            Client.AutoConnectAndJoin = false;
        }
        
        public static void ApplySoundSettings() 
        {
            if (SceneLoader.CurrentGameManager == null || !(SceneLoader.CurrentGameManager is InGameManager))
            {
                return;
            }
            foreach (var sync in ((InGameManager)SceneLoader.CurrentGameManager).PhotonVoiceSyncs)
            {
                if (sync != null)
                    sync.Apply();
            }
        }

        public static float GetInputVolume()
        {
            if (SettingsManager.SoundSettings.VoiceChatInput.Value == (int)VoiceChatInputMode.Off)
            {
                return 0f;
            }
            return Mathf.Clamp(SettingsManager.SoundSettings.VoiceChatMicVolume.Value, 0f, 1f) * 4f;
        }

        public static float GetOuputVolume(PhotonView view)
        {
            var pActorID = view.OwnerActorNr;
            if (SettingsManager.SoundSettings.VoiceChatInput.Value == (int)VoiceChatInputMode.Off || InGameManager.MuteVoiceChat.Contains(pActorID))
            {
                return 0f;
            }
            float multiplier = 1f;
            if (InGameManager.VoiceChatVolumeMultiplier.ContainsKey(pActorID))
            {
                multiplier = InGameManager.VoiceChatVolumeMultiplier[pActorID];
            }
            return Mathf.Clamp(SettingsManager.SoundSettings.VoiceChatAudioVolume.Value * multiplier, 0f, 1f);
        }
    }
}