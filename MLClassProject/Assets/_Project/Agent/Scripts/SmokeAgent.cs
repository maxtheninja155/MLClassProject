using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace BossFight.RL
{
    /// <summary>
    /// The smallest possible agent. It exists only to prove the editor can talk to the Python trainer.
    /// Not part of the game. See README, "Checking your ML-Agents setup".
    /// </summary>
    public class SmokeAgent : Unity.MLAgents.Agent
    {
        const int StepsPerEpisode = 50;
        int _steps;

        public override void OnEpisodeBegin() => _steps = 0;

        public override void CollectObservations(VectorSensor sensor) => sensor.AddObservation(Random.value);

        public override void OnActionReceived(ActionBuffers actions)
        {
            _steps++;
            AddReward(actions.DiscreteActions[0] == 1 ? 0.1f : -0.1f);
            if (_steps >= StepsPerEpisode) EndEpisode();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discrete = actionsOut.DiscreteActions;
            discrete[0] = Random.value > 0.5f ? 1 : 0;
        }
    }
}
