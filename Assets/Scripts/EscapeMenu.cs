using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject escapeMenu;
    [SerializeField] private Button joinVoiceChatButton, leaveVoiceChatButton;

    private void Start()
    {
        switch (VoiceChatManager.Instance.inVc)
        {
            case true:
                joinVoiceChatButton.interactable = false;
                leaveVoiceChatButton.interactable = true;
                break;
            case false:
                joinVoiceChatButton.interactable = true;
                leaveVoiceChatButton.interactable = false;
                break;
        }
    }

    public void ResumeButton()
    {
        escapeMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void JoinVoiceChatButton()
    {
        VoiceChatManager.Instance.JoinVcChannel();
        joinVoiceChatButton.interactable = false;
        leaveVoiceChatButton.interactable = true;
    }

    public void LeaveVoiceChatButton()
    {
        VoiceChatManager.Instance.LeaveVcChannel();
        joinVoiceChatButton.interactable = true;
        leaveVoiceChatButton.interactable = false;
    }

    public void LeaveGameButton()
    {
        SpawnManager.Instance.LeaveRoom();
        //PhotonNetwork.LeaveRoom(true);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Photon Room.");
        //SceneManager.LoadScene(0);
        PhotonNetwork.LoadLevel(0);
    }
}