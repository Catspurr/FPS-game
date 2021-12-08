using System;
using System.Collections.Generic;
using agora_gaming_rtc;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;

public class SpatialAudio : MonoBehaviour
{
    [SerializeField] private float radius;

    private PhotonView _photonView;
    private IAudioEffectManager _agoraAudioEffects;

    private static Dictionary<Player, SpatialAudio> _spatialAudioFromPlayers = new Dictionary<Player, SpatialAudio>();

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _agoraAudioEffects = VoiceChatManager.Instance.GetRtcEngine().GetAudioEffectManager();
        _spatialAudioFromPlayers[_photonView.Owner] = this;
    }

    //Not gonna lie, I have no clue what is going on here. It removes players from the dictionary somehow when destroyed
    private void OnDestroy()
    {
        foreach (var item in _spatialAudioFromPlayers.Where(x=> x.Value == this).ToList())
        {
            _spatialAudioFromPlayers.Remove(item.Key);
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.IsLocal) continue;

            if (player.CustomProperties.TryGetValue("agoraID", out var agoraID))
            {
                if (_spatialAudioFromPlayers.ContainsKey(player))
                {
                    var other = _spatialAudioFromPlayers[player];
                    var pos = other.transform.position;

                    _agoraAudioEffects.SetRemoteVoicePosition(uint.Parse((string)agoraID), GetPan(pos), GetGain(pos));
                }
                else
                {
                    _agoraAudioEffects.SetRemoteVoicePosition(uint.Parse((string)agoraID), 0, 0);
                }
            }
        }
    }

    private float GetGain(Vector3 otherPosition)
    {
        //var distance = Vector3.Distance(transform.position, otherPosition);
        //var gain = Mathf.Max(1 - (distance / radius), 0) * 100f;

        return Mathf.Max(1 - (Vector3.Distance(transform.position, otherPosition) / radius), 0) * 100f;
    }

    private float GetPan(Vector3 otherPosition)
    {
        var direction = otherPosition - transform.position;
        return Vector3.Dot(transform.right, direction.normalized);
    }
}