using System;
using UnityEngine;

namespace NeuronCrafter.NeuralNetwork
{
    public static class ActivationFunctionHandler
    {
        public static float Calculate(EActivationFunction _activationMethod, float _value, float _alpha)
        {
            switch (_activationMethod)
            {
                case EActivationFunction.Sigmoid:
                    return Sigmoid(_value);

                case EActivationFunction.Tanh:
                    return Tanh(_value);

                case EActivationFunction.ReLU:
                    return ReLU(_value);

                case EActivationFunction.LeakyReLU:
                    return LeakyReLU(_value, _alpha);

                case EActivationFunction.ELU:
                    return ELU(_value, _alpha);

                case EActivationFunction.Swish:
                    return Swish(_value);

                case EActivationFunction.GELU:
                    return GELU(_value);

                case EActivationFunction.None:
                default:
                    throw new NotImplementedException();
            }
        }

        private static float Sigmoid(float x)
        {
            return 1f / (1f + Mathf.Exp(-x));
        }

        private static float Tanh(float x)
        {
            return (float)Math.Tanh(x);
        }

        private static float ReLU(float x)
        {
            return Mathf.Max(0f, x);
        }

        private static float LeakyReLU(float x, float alpha)
        {
            return x >= 0f ? x : alpha * x;
        }

        private static float ELU(float x, float alpha)
        {
            return x >= 0f ? x : alpha * (Mathf.Exp(x) - 1f);
        }

        private static float Swish(float x)
        {
            return x * (1f / (1f + Mathf.Exp(-x)));
        }

        private static float GELU(float x)
        {
            return 0.5f * x * (1f + (float)Math.Tanh(Mathf.Sqrt(2f / Mathf.PI) * (x + 0.044715f * Mathf.Pow(x, 3))));
        }
    }

    public enum EActivationFunction
    {
        None = -1,

        /// <summary>
        /// Outputs values in the range (0, 1). Smooth, S-shaped curve.
        /// </summary>
        Sigmoid,

        /// <summary>
        /// Outputs values in the range (-1, 1). Smooth, symmetric curve.
        /// </summary>
        Tanh,

        /// <summary>
        /// Outputs 0 for negative inputs, linear for positive. Range [0, ∞).
        /// </summary>
        ReLU,

        /// <summary>
        /// Like ReLU but with a small negative slope for x < 0.
        /// </summary>
        LeakyReLU,

        /// <summary>
        /// Exponential Linear Unit. Like ReLU, but smooth for x < 0.
        /// </summary>
        ELU,

        /// <summary>
        /// Smooth, non-monotonic function. x * sigmoid(x).
        /// </summary>
        Swish,

        /// <summary>
        /// Gaussian Error Linear Unit. Smooth and probabilistic.
        /// </summary>
        GELU,
    }
}
