using System;

namespace NeuronCrafter.NeuralNetwork
{
    [Serializable]
    public class NeuralLayerConfig
    {
        public EActivationFunction ActivationFunction;
        public int NeuronCount;
    }
}
