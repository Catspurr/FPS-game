using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ScoreboardItem : MonoBehaviour
{
    public TMP_Text usernameText, killsText, deathsText;

    public void Initialize(Player player)
    {
        usernameText.text = player.NickName;
    }
}