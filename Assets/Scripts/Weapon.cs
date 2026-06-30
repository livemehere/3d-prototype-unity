using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform muzzle;
    public GameObject bulletPrefab;
    public float bulletSpeed = 30f;

    public void Fire(Vector3 targetPoint)
    {
        Vector3 dir = (targetPoint - muzzle.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(dir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * bulletSpeed;
    }
}