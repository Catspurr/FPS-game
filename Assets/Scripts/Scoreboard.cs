using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject scoreboardItemPrefab;

    private Dictionary<Player, ScoreboardItem> _scoreboardItems = new Dictionary<Player, ScoreboardItem>();

    private void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
    
    private void AddScoreboardItem(Player player)
    {
        var item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
        _scoreboardItems[player] = item;
    }

    private void RemoveScoreboardItem(Player player)
    {
        Destroy(_scoreboardItems[player].gameObject);
        _scoreboardItems.Remove(player);
    }
}