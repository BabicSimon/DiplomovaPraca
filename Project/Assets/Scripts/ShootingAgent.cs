using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ShootingAgent : Agent
{
    [SerializeField] private Rigidbody agentBody;

    public int score = 0;
    public Transform shootingPoint;
    public int minStepsBetweenShots = 500;
    public int damage = 100;
    public int range = 30;

    private bool shotAvailable = false;
    private int stepsUntilShotIsAvailable = 0;
    private Vector3 startingPostion;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startingPostion;
        agentBody.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    public override void Initialize()
    {
        startingPostion = transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (!shotAvailable)
        {
            stepsUntilShotIsAvailable--;

            if (stepsUntilShotIsAvailable <= 0)
                shotAvailable = true;
        }
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 0:
                Shoot();
                return;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        agentBody.AddForce(dirToGo * 1f, ForceMode.VelocityChange);
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-1f / MaxStep);
        MoveAgent(actions.DiscreteActions);
    }

    private void Shoot()
    {
        if (!shotAvailable)
            return;

        var layerMask = 1 << LayerMask.NameToLayer("Player");
        var direction = transform.forward;

        //Physics.Raycast(shootingPoint.position, direction, out var hit, 50f, layerMask)

        

        if (Physics.SphereCast(shootingPoint.position, 1f, direction, out var hit, 50f, layerMask))
        {
            Debug.DrawRay(shootingPoint.position, direction * range, Color.green, 2f);

            if(hit.transform.GetComponent<DummyEnemy>() != null)
                hit.transform.GetComponent<DummyEnemy>().GetShot(damage, this);
            if (hit.transform.GetComponent<PlayerController>() != null)
                hit.transform.GetComponent<PlayerController>().Respawn();
        }
        else
        {
            Debug.DrawRay(shootingPoint.position, direction * range, Color.red, 2f);
            AddReward(-.1f);
        }

        shotAvailable = false;
        stepsUntilShotIsAvailable = minStepsBetweenShots;
    }

    public void RegisterKill()
    {
        AddReward(1.0f);
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic");

        var dirToGo = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();

        if (Input.GetKey(KeyCode.W))
            dirToGo = transform.forward * 1f;

        var rotateDir = Input.GetAxis("Horizontal");
        transform.Rotate(transform.up * rotateDir, Time.deltaTime * 200f);
        agentBody.AddForce(dirToGo * 1f, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wall") || other.CompareTag("player") || other.CompareTag("enemy"))
        {
            AddReward(-5f);
            EndEpisode();
        }
    }

}
