using System;
using System.Collections.Generic;

namespace NeuronCrafter.NeuralNetwork
{
    [Serializable]
    public class NeuralNetworkSaveData
    {
        public List<NeuralLayer> Layers = new List<NeuralLayer>();
        public float FitnessValue;
        public int InputCount;
        public string Type;
        public string Name;
    }
}
