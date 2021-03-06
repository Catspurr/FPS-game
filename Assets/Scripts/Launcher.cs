using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_Text errorText, roomNameText;
    [SerializeField] private Transform roomListContent, playerListContent;
    [SerializeField] private GameObject roomListItemPrefab, playerListItemPrefab, startGameButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        print("Connecting to Master");
        if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState == PeerStateValue.Disconnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    public override void OnJoinedLobby()
    {
        print("Joined Lobby!");
        MenuManager.Instance.OpenMenu("Title");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text)) return;

        PhotonNetwork.CreateRoom(roomNameInput.text);
        MenuManager.Instance.OpenMenu("Loading");
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = $"Room creation failed: {message}";
        MenuManager.Instance.OpenMenu("Error");
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

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }
        
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
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

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        
        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList) continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomInfo);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}