using System;
using System.IO;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView _photonView;
    private GameObject _controller;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        var spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        _controller = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "PlayerController"), 
            spawnPoint.position, 
            spawnPoint.rotation,
            0,
            new object[] {_photonView.ViewID});
    }

    public void Die()
    {
        PhotonNetwork.Destroy(_controller);
        CreateController();
    }
}