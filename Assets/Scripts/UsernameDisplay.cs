using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView playerPhotonView;
    [SerializeField] private TMP_Text text;

    private void Start()
    {
        gameObject.SetActive(!playerPhotonView.IsMine);
        
        text.text = playerPhotonView.Owner.NickName;
    }
}