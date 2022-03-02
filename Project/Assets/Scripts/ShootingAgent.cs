using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ShootingAgent : Agent
{
    public int score = 0;

    public Transform shootingPoint;
    public int minStepsBetweenShots = 50;
    public int damage = 100;

    private bool shotAvailable = true;
    private int stepsUntilShotIsAvailable = 0;

    private Vector3 startingPostion;
    private Rigidbody rb;

    private void Shoot()
    {
        if (!shotAvailable)
            return;

        var layerMask = 1 << LayerMask.NameToLayer("Enemy");
        var direction = transform.forward;

        Debug.Log("Shot");
        Debug.DrawRay(shootingPoint.position, direction * 200f, Color.green, 2f);

        if(Physics.Raycast(shootingPoint.position, direction, out var hit, 200f, layerMask))
        {
            hit.transform.GetComponent<EnemyScript>().GetShot(damage, this);
        }

        shotAvailable = false;
        stepsUntilShotIsAvailable = minStepsBetweenShots;
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

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-1f / MaxStep);
        MoveAgent(actions.DiscreteActions);
    }

    //public override void CollectObservations(VectorSensor sensor)
    //{
    //    base.CollectObservations(sensor);
    //}

    public override void Initialize()
    {
        shotAvailable = true;
        startingPostion = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //continuousActions[0] = Input.GetKey(KeyCode.P) ? 1f : 0f;

        Debug.Log("Heuristic");

        var dirToGo = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();

        if (Input.GetKey(KeyCode.W))
            dirToGo = transform.forward * 1f;

        var rotateDir = Input.GetAxis("Horizontal");
        transform.Rotate(transform.up * rotateDir, Time.deltaTime * 200f);
        rb.AddForce(dirToGo * 1f, ForceMode.VelocityChange);
    }

    public void RegisterKill()
    {
        AddReward(1.0f);
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode Begin");
        transform.position = startingPostion;
        rb.velocity = Vector3.zero;
        shotAvailable = true;
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
            case 5:
                Shoot();
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        rb.AddForce(dirToGo * 1f, ForceMode.VelocityChange);
    }
}
