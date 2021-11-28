using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_Text errorText, roomNameText;
    [SerializeField] private Transform roomListContent, playerListContent;
    [SerializeField] private GameObject roomListItemPrefab, playerListItemPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        print("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnJoinedLobby()
    {
        print("Joined Lobby!");
        MenuManager.Instance.OpenMenu("Title");
        PhotonNetwork.NickName = $"Player {Random.Range(0, 1001)}";
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text)) return;

        PhotonNetwork.CreateRoom(roomNameInput.text);
        MenuManager.Instance.OpenMenu("Loading");
    }
    
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = $"Room creation failed: {message}";
        MenuManager.Instance.OpenMenu("Error");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = $"Failed to join room: {message}";
        MenuManager.Instance.OpenMenu("Error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        
        foreach (var roomInfo in roomList)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomInfo);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}