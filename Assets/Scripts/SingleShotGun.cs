using System;
using Photon.Pun;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] private Camera cam;

    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
    }

    private void Shoot()
    {
        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out var hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            _photonView.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    private void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        var colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            var bulletImpactObject = Instantiate(
                bulletImpactPrefab, 
                hitPosition + hitNormal * 0.001f, 
                Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObject, 5f);
            bulletImpactObject.transform.SetParent(colliders[0].transform);
        }
    }
}