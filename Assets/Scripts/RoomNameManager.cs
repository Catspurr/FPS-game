using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomNameManager : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField roomNameInputField;
    
    private const string PlayerPrefsRoomNameKey = "RoomName";

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsRoomNameKey))
        {
            roomNameInputField.text = PlayerPrefs.GetString(PlayerPrefsRoomNameKey);
        }
        createButton.interactable = roomNameInputField.text != string.Empty;
    }

    public void OnRoomNameInputFieldValueChanged(string roomName)
    {
        PlayerPrefs.SetString(PlayerPrefsRoomNameKey, roomName);
        createButton.interactable = roomNameInputField.text != string.Empty;
    }
}