using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager Instance;

    private SpawnPoint[] _spawnPoints;

    private void Awake()
    {
        Instance = this;
        _spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Length)].transform;
    }

    public void LeaveRoom()
    {
        Destroy(RoomManager.Instance.gameObject);
        Destroy(VoiceChatManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom(true);
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }
}