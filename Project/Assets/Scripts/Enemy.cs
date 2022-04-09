using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public int startingHealth = 100;
    public int speed = 5;
    private int currentHealt;

    void Start()
    {
        currentHealt = startingHealth;
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
        Respawn();
    }

    private void Die(PlayerController shooter)
    {
        Debug.Log("I died!");
        shooter.RegisterKill();
        Respawn();
    }

    private void Respawn()
    {
        currentHealt = startingHealth;
        transform.localPosition = new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f));
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
