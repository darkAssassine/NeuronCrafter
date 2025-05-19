using System;

namespace NeuronCrafter.NeuralNetwork
{
    [Serializable]
    public class NeuralLayer
    {
        public Neuron[] Neurons;

        private EActivationFunction activationFunction;

        public NeuralLayer(Neuron[] _neurons, EActivationFunction _activationFunction)
        {
            Neurons = _neurons;
            activationFunction = _activationFunction;
        }

        public float[] FeedForward(float[] _inputs)
        {
            float[] outputs = new float[Neurons.Length];

            for (int i = 0; i < Neurons.Length; i++)
            {
                outputs[i] = Neurons[i].GetValue(_inputs, activationFunction);
            }
            return outputs;
        }
    }
}
