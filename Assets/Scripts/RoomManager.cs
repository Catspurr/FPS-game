using System;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

/*
 * This singleton handles spawning PlayerManagers for each player that loads in to the game
 */

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) //Game scene
        {
            PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "PlayerManager"), 
                Vector3.zero,
                Quaternion.identity);
        }
    }
}