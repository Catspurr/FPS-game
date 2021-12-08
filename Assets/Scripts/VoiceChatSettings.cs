using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(2)]
public class VoiceChatSettings : MonoBehaviour
{
    [SerializeField] private Slider voiceChatVolumeSlider, microphoneVolumeSlider;
    [SerializeField] private Button autoJoinVoiceChatButton;

    private const string PlayerPrefsVoiceChatVolumeKey = "VcVolume",
        PlayerPrefsMicrophoneVolumeKey = "MicrophoneVolume",
        PlayerPrefsAutoJoinVcKey = "AutoJoinVcKey";

    public bool autoJoinVc;

    private void Start()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsVoiceChatVolumeKey) && voiceChatVolumeSlider != null)
        {
            voiceChatVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsVoiceChatVolumeKey);
            VoiceChatManager.Instance.GetRtcEngine().AdjustPlaybackSignalVolume(
                Mathf.RoundToInt(PlayerPrefs.GetFloat(PlayerPrefsVoiceChatVolumeKey)));
        }

        if (PlayerPrefs.HasKey(PlayerPrefsMicrophoneVolumeKey) && microphoneVolumeSlider != null)
        {
            microphoneVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsMicrophoneVolumeKey);
            VoiceChatManager.Instance.GetRtcEngine().AdjustRecordingSignalVolume(
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

    public void AutoJoinVcToggle()
    {
        autoJoinVc = !autoJoinVc;
        PlayerPrefs.SetInt(PlayerPrefsAutoJoinVcKey, autoJoinVc? 1 : 0);
        autoJoinVoiceChatButton.GetComponent<Image>().color = autoJoinVc ? Color.green : Color.red;
        var mode = autoJoinVc ? "ON" : "OFF";
        autoJoinVoiceChatButton.GetComponentInChildren<TMP_Text>().text = $"Automatically join voice chat: {mode}";
    }

    public void OnVoiceChatVolumeSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(PlayerPrefsVoiceChatVolumeKey, value);
        VoiceChatManager.Instance.GetRtcEngine().AdjustPlaybackSignalVolume(Mathf.RoundToInt(value));
    }

    public void OnMicrophoneVolumeSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(PlayerPrefsMicrophoneVolumeKey, value);
        VoiceChatManager.Instance.GetRtcEngine().AdjustRecordingSignalVolume(Mathf.RoundToInt(value));
    }
}