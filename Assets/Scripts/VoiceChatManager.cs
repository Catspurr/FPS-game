using System;
using UnityEngine;
using agora_gaming_rtc;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class VoiceChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider voiceChatVolumeSlider, microphoneVolumeSlider;
    [SerializeField] private Button autoJoinVoiceChatButton, roomJoinVcButton, roomLeaveVcButton;
    [SerializeField] private GameObject eventSystem;
    private const string AppID = "a78e0b5b837441389243739d16aa8979",
        PlayerPrefsVoiceChatVolumeKey = "VcVolume",
        PlayerPrefsMicrophoneVolumeKey = "MicrophoneVolume",
        PlayerPrefsAutoJoinVcKey = "AutoJoinVcKey";

    public static VoiceChatManager Instance;

    private IRtcEngine _rtcEngine;
    private bool _autoJoinVc;

    [HideInInspector] public bool inVc;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        _rtcEngine = IRtcEngine.GetEngine(AppID);
        _rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
        _rtcEngine.OnLeaveChannel += OnLeaveChannel;
        _rtcEngine.OnError += OnError;

        _rtcEngine.EnableSoundPositionIndication(true);
        
        if (PlayerPrefs.HasKey(PlayerPrefsVoiceChatVolumeKey) && voiceChatVolumeSlider != null)
        {
            voiceChatVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsVoiceChatVolumeKey);
            _rtcEngine.AdjustPlaybackSignalVolume(
                Mathf.RoundToInt(PlayerPrefs.GetFloat(PlayerPrefsVoiceChatVolumeKey)));
        }

        if (PlayerPrefs.HasKey(PlayerPrefsMicrophoneVolumeKey) && microphoneVolumeSlider != null)
        {
            microphoneVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsMicrophoneVolumeKey);
            _rtcEngine.AdjustRecordingSignalVolume(
                Mathf.RoundToInt(PlayerPrefs.GetFloat(PlayerPrefsMicrophoneVolumeKey)));
        }

        if (PlayerPrefs.HasKey(PlayerPrefsAutoJoinVcKey) && autoJoinVoiceChatButton != null)
        {
            if (PlayerPrefs.GetInt(PlayerPrefsAutoJoinVcKey) == 1)
            {
                AutoJoinVcToggle();
            } //1 is true 0 is false
        }
    }

    private void OnError(int error, string msg)
    {
        Debug.Log($"Error with Agora: {msg}");
    }

    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log($"Joined channel {channelName}");
        var hash = new Hashtable();
        hash.Add("agoraID", uid.ToString());
        PhotonNetwork.SetPlayerCustomProperties(hash);
    }

    public IRtcEngine GetRtcEngine()
    {
        return _rtcEngine;
    }
    
    private void OnLeaveChannel(RtcStats stats)
    {
        Debug.Log($"Left channel with duration {stats.duration}");
    }

    public override void OnJoinedRoom()
    {
        if (!_autoJoinVc) return;
        JoinVcChannel();
    }

    public override void OnLeftRoom()
    {
        LeaveVcChannel();
    }

    public void JoinVcChannel()
    {
        _rtcEngine.JoinChannel(PhotonNetwork.CurrentRoom.Name);
        inVc = true;
        if (roomJoinVcButton != null && roomLeaveVcButton != null)
        {
            roomJoinVcButton.interactable = false;
            roomLeaveVcButton.interactable = true;
        }
    }

    public void LeaveVcChannel()
    {
        _rtcEngine.LeaveChannel();
        inVc = false;
        if (roomJoinVcButton != null && roomLeaveVcButton != null)
        {
            roomJoinVcButton.interactable = true;
            roomLeaveVcButton.interactable = false;
        }
    }

    private void OnDestroy()
    {
        IRtcEngine.Destroy();
    }
    
    public void AutoJoinVcToggle()
    {
        if (eventSystem != null)
        {
            //eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
        _autoJoinVc = !_autoJoinVc;
        PlayerPrefs.SetInt(PlayerPrefsAutoJoinVcKey, _autoJoinVc? 1 : 0);
        autoJoinVoiceChatButton.GetComponent<Image>().color = _autoJoinVc ? Color.green : Color.red;
        var mode = _autoJoinVc ? "ON" : "OFF";
        autoJoinVoiceChatButton.GetComponentInChildren<TMP_Text>().text = $"Automatically join voice chat: {mode}";
    }
    
    public void OnVoiceChatVolumeSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(PlayerPrefsVoiceChatVolumeKey, value);
        _rtcEngine.AdjustPlaybackSignalVolume(Mathf.RoundToInt(value));
    }

    public void OnMicrophoneVolumeSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(PlayerPrefsMicrophoneVolumeKey, value);
        _rtcEngine.AdjustRecordingSignalVolume(Mathf.RoundToInt(value));
    }
}