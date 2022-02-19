using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TwoMoveToGoal : Agent
{
    [SerializeField] private Rigidbody agentBody;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private int moveSpeed;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenreder;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-4, 0, 0);
        agentBody.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(targetTransform.localPosition);

        sensor.AddObservation(transform.InverseTransformDirection(agentBody.velocity));
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
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        agentBody.AddForce(dirToGo * 2f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //float moveX = actions.ContinuousActions[0];
        //float moveZ = actions.ContinuousActions[1];

        //transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        AddReward(-1f / MaxStep);
        MoveAgent(actions.DiscreteActions);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "goal")
        {
            SetReward(2f);
            floorMeshRenreder.material = winMaterial;
            EndEpisode();
        }
        if (other.tag == "wall")
        {
            SetReward(-1f);
            floorMeshRenreder.material = loseMaterial;
            EndEpisode();
        }

    }
}
