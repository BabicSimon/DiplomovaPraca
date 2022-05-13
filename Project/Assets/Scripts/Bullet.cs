using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 shootDirection;
    private float muzzleVelocity = 200f;
    public void Setup(Vector3 shootDirection)
    {
        this.shootDirection = shootDirection;
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        transform.position += shootDirection * muzzleVelocity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
