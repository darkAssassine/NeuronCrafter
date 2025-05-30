using System.Collections.Generic;
using UnityEngine;

namespace NeuronCrafter.NeuralNetwork
{
    public class NeuralNetworkTrainer : MonoBehaviour
    {
        private const string initializeMethodTooltip = "If you want that the neural network starts with the best values from the previous run or if it uses it own values from the previous run";

        [SerializeField, Header("Training"), Tooltip(initializeMethodTooltip)] private ETrainingMethod trainingMethod;
        [SerializeField] private float thresholdToUseBest;
        [SerializeField, Range(0.0f, 100.0f)] private float DisableConnectionMuationChancePerTraining = 1f;
        [SerializeField, Range(0.0f, 100.0f)] private float AddNeuronMuationChancePerTraining = 1f;
        [SerializeField, Range(0.0f, 100.0f)] private float EnableDisabledConnnectionChangerPerTraining = 1f;
        [SerializeField, Header("The min and max for Random.Range that gets added to train the neural network")] private AnimationCurve fitnessToWeightValue;
        [SerializeField] private AnimationCurve fitnessToBiasValue;
        [SerializeField] private AnimationCurve fitnessToAplhaValue;

        public void Train(NeuralNetwork _neuralNetwork, List<NeuralLayer> layers)
        {
            NeuralNetworkManager.UpdateResults(_neuralNetwork.GetSaveData());

            if (_neuralNetwork.GetBestSave() == null)
            {
                _neuralNetwork.SetBestSave(_neuralNetwork.GetSaveData());
            }

            if (trainingMethod == ETrainingMethod.UseBestSave)
            {
                _neuralNetwork.Load(NeuralNetworkManager.GetTheBestOfType(_neuralNetwork.Type));
            }
            else if (trainingMethod == ETrainingMethod.UseBestSaveIfFittnesOverThreshold && NeuralNetworkManager.GetTheBestOfType(_neuralNetwork.Type).FitnessValue > thresholdToUseBest)
            {
                _neuralNetwork.Load(NeuralNetworkManager.GetTheBestOfType(_neuralNetwork.Type));
            }
            else
            {
                if (_neuralNetwork.GetBestSave().FitnessValue > _neuralNetwork.FitnessValue)
                {
                    _neuralNetwork.Load(_neuralNetwork.GetBestSave());
                }

            }

            foreach (NeuralLayer layer in layers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {

                    for (int i = 0; i < neuron.weights.Length; i++)
                    {
                        if (neuron.weights[i] != 0)
                        {
                            neuron.weights[i] += Random.Range(-fitnessToWeightValue.Evaluate(_neuralNetwork.FitnessValue), fitnessToWeightValue.Evaluate(_neuralNetwork.FitnessValue));
                        }
                        else
                        {
                            if (EnableDisabledConnnectionChangerPerTraining > Random.Range(0f, 100f))
                            {
                                neuron.weights[i] += Random.Range(-fitnessToWeightValue.Evaluate(_neuralNetwork.FitnessValue), -fitnessToWeightValue.Evaluate(_neuralNetwork.FitnessValue));
                            }
                        }

                        neuron.bias += Random.Range(-fitnessToBiasValue.Evaluate(_neuralNetwork.FitnessValue), fitnessToBiasValue.Evaluate(_neuralNetwork.FitnessValue));
                        neuron.alpha += Random.Range(-fitnessToBiasValue.Evaluate(_neuralNetwork.FitnessValue), fitnessToBiasValue.Evaluate(_neuralNetwork.FitnessValue));

                        if (neuron.mutatedNeurons[i].IsActive)
                        {
                            neuron.mutatedNeurons[i].weight += Random.Range(-fitnessToWeightValue.Evaluate(_neuralNetwork.FitnessValue), -fitnessToWeightValue.Evaluate(_neuralNetwork.FitnessValue));
                            neuron.mutatedNeurons[i].alpha += Random.Range(-fitnessToAplhaValue.Evaluate(_neuralNetwork.FitnessValue), fitnessToAplhaValue.Evaluate(_neuralNetwork.FitnessValue));
                            neuron.mutatedNeurons[i].bias += Random.Range(-fitnessToBiasValue.Evaluate(_neuralNetwork.FitnessValue), fitnessToBiasValue.Evaluate(_neuralNetwork.FitnessValue));
                        }
                    }

                    if (Random.Range(0f, 100f) < DisableConnectionMuationChancePerTraining)
                    {
                        int randomConnection = Random.Range(0, neuron.weights.Length);
                        neuron.weights[randomConnection] = 0;
                    }

                    if (Random.Range(0f, 100) < AddNeuronMuationChancePerTraining)
                    {
                        int randomConnection = Random.Range(0, neuron.weights.Length);
                        neuron.mutatedNeurons[randomConnection].IsActive = true;
                    }
                }
            }
            _neuralNetwork.ResetFitness();
        }
    }

    public enum ETrainingMethod
    {
        UseBestSaveIfFittnesOverThreshold,
        UseBestSave,
        UseOwnSave,
    }
}
