using System;
using System.Collections.Generic;
namespace NeuronCrafter.NeuralNetwork
{
    [Serializable]
    public class Neuron
    {
        public float[] weights;
        public float bias;
        public float alpha;
        public MutatedNeuron[] mutatedNeurons;
        public List<float> weightsForNewConnections;

        public Neuron(float[] _weights, float _bias, float _alpha, MutatedNeuron[] _mutatedNeurons)
        {
            weights = _weights;
            bias = _bias;
            alpha = _alpha;
            mutatedNeurons = _mutatedNeurons;
        }

        public float GetValue(float[] _inputs, EActivationFunction _activationFunction)
        {
            float value = 0;

            for (int i = 0; i < _inputs.Length; i++)
            {
                if (mutatedNeurons[i].IsActive)
                {
                    value += (mutatedNeurons[i].GetValue(_inputs[i], _activationFunction) * weights[i]);
                }
                else
                {
                    value += (_inputs[i] * weights[i]);
                }
            }

            value += bias;
            return ActivationFunctionHandler.Calculate(_activationFunction, value, alpha);
        }
    }

    [Serializable]
    public class MutatedNeuron
    {
        public float weight;
        public float bias;
        public float alpha;
        public bool IsActive = false;

        public MutatedNeuron(float _weight, float _bias, float _alpha)
        {
            weight = _weight;
            bias = _bias;
            alpha = _alpha;
        }

        public float GetValue(float _input, EActivationFunction _activationFunction)
        {
            float value = _input * weight + bias;
            return ActivationFunctionHandler.Calculate(_activationFunction, value, alpha);
        }
    }
}
