using UnityEngine;

namespace NeuronCrafter.Example
{
    public class Arena : MonoBehaviour
    {
        [SerializeField] private NeuralNetwork.NeuralNetwork agent;

        public void Initialize(string agentName)
        {
            agent.Initialize(agent.Type, agentName);
        }
    }
}
