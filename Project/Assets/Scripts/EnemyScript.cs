using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int startingHealth = 100;
    private int currentHealt;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        currentHealt = startingHealth;
    }

    public void GetShot(int damage, ShootingAgent shooter)
    {
        ApplyDamage(damage, shooter);
    }

    private void ApplyDamage(int damage, ShootingAgent shooter)
    {
        currentHealt -= damage;

        if(currentHealt <= 0)
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

    private void Respawn()
    {
        currentHealt = startingHealth;
        transform.position = startPosition;
    }
}
