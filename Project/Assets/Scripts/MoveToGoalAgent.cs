using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Rigidbody agentBody;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform smallTargetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-19f, -10f), 0, Random.Range(19f, -19f));
        targetTransform.localPosition = new Vector3(Random.Range(10f, 18f), 0, Random.Range(-8f, -18f));
        smallTargetTransform.localPosition = new Vector3(-14f, 0, -6.5f);

        agentBody.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(targetTransform.localPosition);
        //if(smallTargetTransform)
        //{
        //    sensor.AddObservation(smallTargetTransform.localPosition);
        //}
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
        agentBody.AddForce(dirToGo * 1f, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //float moveX = actions.ContinuousActions[0];
        //float moveZ = actions.ContinuousActions[1];

        //float moveSpeed = 5;
        //transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        AddReward(-1f / MaxStep);
        MoveAgent(actions.DiscreteActions);
    }

    //public override void Heuristic(in ActionBuffers actionsOut)
    //{
    //    ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    //    continuousActions[0] = Input.GetAxisRaw("Horizontal");
    //    continuousActions[1] = Input.GetAxisRaw("Vertical");
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(5f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if(smallTargetTransform != null)
        {
            if (other.TryGetComponent<SmallGoal>(out SmallGoal smallGoal))
            {
                AddReward(1f);
                floorMeshRenderer.material = baseMaterial;
                if (smallTargetTransform.localPosition == new Vector3(-14f, 0, -6.5f))
                {
                    smallTargetTransform.localPosition = new Vector3(0f, 0, 0f);
                }
                else if (smallTargetTransform.localPosition == new Vector3(0f, 0, 0f))
                {
                    smallTargetTransform.localPosition = new Vector3(6.5f, 0, 13.5f);
                }
                else if (smallTargetTransform.localPosition == new Vector3(6.5f, 0, 13.5f))
                {
                    smallTargetTransform.localPosition = new Vector3(14.5f, 0, 8.5f);
                }
                else
                {
                    smallTargetTransform.localPosition = new Vector3(14.5f, 0, -6.5f);
                }
            }
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-5f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
        
    }
}
