using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
//
//
//
// ROBOT NEEDS RESPAWN AFTER ENDEPISODE!!!!!!!
//
//

public class RobotController : Agent
{
    
    public float speed = 5f;

    public Transform TargetTransform;
    
    //public Transform CrosswalkTransform;

    private enum ACTIONS
    {
        LEFT = 0,
        FORWARD = 1,
        RIGHT = 2,
        BACKWARD = 3
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-20.4f, 0.3f, 20);

        // Generate a random position for the target prefab 
        float target_xPosition = UnityEngine.Random.Range(-27.8f, -27.8f);
        float target_zPosition = UnityEngine.Random.Range(47, 20);

        // Assign the randomly generated position to the target prefab
        TargetTransform.localPosition = new Vector3(target_xPosition, 1.0f, target_zPosition);

        // Generate a random position for the crosswalk prefab 
        //float crosswalk_xPosition = UnityEngine.Random.Range(24, -2);
        //float crosswalk_zPosition = UnityEngine.Random.Range(-1.3f, -1.3f);

        // Assign the randomly generated position to the crosswalk prefab
        //CrosswalkTransform.localPosition = new Vector3(crosswalk_xPosition, -0.1f, crosswalk_zPosition);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // The position of the agent
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);

        // The position of the target prefab
        sensor.AddObservation(TargetTransform.localPosition.x);
        sensor.AddObservation(TargetTransform.localPosition.y);

        // The distance between the agent and the target
        sensor.AddObservation(Vector3.Distance(TargetTransform.localPosition, transform.localPosition));

        //Crosswalk position needed
       // sensor.AddObservation(CrosswalkTransform.localPosition.x);
       // sensor.AddObservation(CrosswalkTransform.localPosition.y);

        // The distance between the agent and the crosswalk
        //sensor.AddObservation(Vector3.Distance(CrosswalkTransform.localPosition, transform.localPosition));
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == -1)
        {
            actions[0] = (int)ACTIONS.LEFT;
        }
        else if (horizontal == +1)
        {
            actions[0] = (int)ACTIONS.RIGHT;
        }
        else if (vertical == +1)
        {
            actions[0] = (int)ACTIONS.FORWARD;
        }
        else if (vertical == -1)
        {
            actions[0] = (int)ACTIONS.BACKWARD;
        }
        else
        {
            actions[0] = 3;
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.DiscreteActions[0];

        switch (actionTaken)
        {
            case (int)ACTIONS.FORWARD:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case (int)ACTIONS.LEFT:
                transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
            case (int)ACTIONS.RIGHT:
                transform.rotation = Quaternion.Euler(0, +90, 0);
                break;
            /*case (int)ACTIONS.BACKWARD:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;*/
        }

        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);

        AddReward(-0.01f);
        //
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            AddReward(-1);
            EndEpisode();
        }
        else if (collision.collider.tag == "Target")
        {
            AddReward(1);
            Debug.Log("Found" + TargetTransform);
            EndEpisode();
        }
        //Crosswalk etc.  Crosswalk trigger
    }
}