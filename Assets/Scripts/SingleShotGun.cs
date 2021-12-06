using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] private Camera cam;
    
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
        }
    }
}