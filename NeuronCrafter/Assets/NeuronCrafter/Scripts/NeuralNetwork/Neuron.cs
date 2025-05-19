using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
namespace NeuronCrafter.NeuralNetwork
{
    [Serializable]
    public class Neuron
    {
        public float[] weights;
        public float bias;
        public float alpha;
        public MutatedNeuron[] mutatedNeuron;

        public Neuron(float[] _weights, float _bias, float _alpha, MutatedNeuron[] _mutatedNeurons)
        {
            weights = _weights;
            bias = _bias;
            alpha = _alpha;
            mutatedNeuron = _mutatedNeurons;
        }

        public float GetValue(float[] _inputs, EActivationFunction _activationFunction)
        {
            float value = 0;
           
            for (int i = 0; i < _inputs.Length; i++)
            {
                value += (_inputs[i] * weights[i]);
                if (mutatedNeuron[i].IsActive)
                {
                    value += (mutatedNeuron[i].GetValue(_inputs[i], _activationFunction) * weights[i]);
                }
                else
                {
                    value += (_inputs[i] * weights[i]);
                }

            }
            value += bias;

            return ActivationFunctionHandler.Calculate(_activationFunction, value, alpha);
        }

        public bool TryMutateRandomConnection(MutatedNeuron _mutatedNeuron)
        {
            List<int> freeConnections = new List<int>();

            for (int i = 0; i < mutatedNeuron.Length; i++)
            {
                if (mutatedNeuron[i] == null)
                {
                    freeConnections.Add(i);
                }
            }

            if (freeConnections.Count <= 0)
            {
                return false;
            }

            int x = freeConnections[Random.Range(0, freeConnections.Count)];
            mutatedNeuron[x] = _mutatedNeuron;

            return true;
        }

        public bool TryDisableRandomConnection()
        {
            List<int> notDisabledConnections = new List<int>();

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] != 0)
                {
                    notDisabledConnections.Add(i);
                }
            }

            if (notDisabledConnections.Count <= 0)
            {
                return false;
            }
            int x = notDisabledConnections[Random.Range(0, notDisabledConnections.Count)];
            weights[x] = 0;

            return true;
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
