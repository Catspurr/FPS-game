using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button findButton, createButton;
    private const string PlayerPrefsUsernameKey = "Username";

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsUsernameKey))
        {
            usernameInputField.text = PlayerPrefs.GetString(PlayerPrefsUsernameKey);
            PhotonNetwork.NickName = PlayerPrefs.GetString(PlayerPrefsUsernameKey);
        }
        findButton.interactable = createButton.interactable = usernameInputField.text != string.Empty;
    }

    public void OnUsernameInputFieldValueChanged(string username)
    {
        PhotonNetwork.NickName = username;
        PlayerPrefs.SetString(PlayerPrefsUsernameKey, username);
        findButton.interactable = createButton.interactable = usernameInputField.text != string.Empty;
    }
}