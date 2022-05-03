using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public int startingHealth = 100;
    public int speed = 5;
    public bool canRespawn;
    private int currentHealt;
    private Vector3 respawnPosition;

    void Start()
    {
        currentHealt = startingHealth;
        respawnPosition = transform.position;
    }

    public void GetShot(int damage, ShootingAgent shooter)
    {
        ApplyDamage(damage, shooter);
    }

    public void GetShot(int damage, PlayerController shooter)
    {
        ApplyDamage(damage, shooter);
    }

    private void ApplyDamage(int damage, ShootingAgent shooter)
    {
        currentHealt -= damage;

        if (currentHealt <= 0)
        {
            Die(shooter);
        }
    }

    private void ApplyDamage(int damage, PlayerController shooter)
    {
        currentHealt -= damage;

        if (currentHealt <= 0)
        {
            Die(shooter);
        }
    }

    private void Die(ShootingAgent shooter)
    {
        Debug.Log("I died!");
        shooter.RegisterKill();
        if (canRespawn)
            Respawn();
        else
            Destroy(gameObject);
    }

    private void Die(PlayerController shooter)
    {
        Debug.Log("I died!");
        shooter.RegisterKill();
        if(canRespawn)
            Respawn();
        else
            Destroy(gameObject);
    }

    private void Respawn()
    {
        currentHealt = startingHealth;
        transform.localPosition = respawnPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        Respawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        Respawn();
    }
}
