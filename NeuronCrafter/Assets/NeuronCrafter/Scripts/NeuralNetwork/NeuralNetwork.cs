using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NeuronCrafter.NeuralNetwork
{
    public class NeuralNetwork : MonoBehaviour
    {
        private const string typeTooltip = "All neural networks that have the same structure of layers and neurons should be the same type";
        private const string uniqueNameTooltip = "This will be uses to create the save file and in the managerclass thats holds each neural network is saved in a dictonary<sting Uniquename, NeuralNetwork this>";
        private const string initializeMethodTooltip = "If you want that the neural network starts with the best values from the previous run or if it uses it own values from the previous run";

        public float FitnessValue { get; private set; }

        [SerializeField, Header("Core"), Tooltip(typeTooltip)] private string type;
        [SerializeField, Tooltip(uniqueNameTooltip)] private string uniqueName;

        [SerializeField, Header("Structure")] private int inputCount;
        [SerializeField] private List<NeuralLayerConfig> layersConfig;

        [SerializeField, Header("Training"), Tooltip(initializeMethodTooltip)] private ETrainingMethod trainingMethod;
        [SerializeField, Range(0.0f, 100.0f)] private float DisableConnectionMuationChancePerTraining = 1f;
        [SerializeField, Range(0.0f, 100.0f)] private float AddNeuronMuationChancePerTraining = 1f;
        [SerializeField, Range(0.0f, 100.0f)] private float EnableDisabledConnnectionChangerPerTraining = 1f;
        [SerializeField] private AnimationCurve fitnessToStepSize;

        public string Type { get { return type; } }
        public string UniqueName { get { return uniqueName; } }

        private List<NeuralLayer> layers = new List<NeuralLayer>();
        private NeuralNetworkSaveData bestOwnSave;

        private void Start()
        {
            Load();
            if (Type == "")
            {
                Debug.LogError("Please Enter a type");
            }
            if (UniqueName == "")
            {
                Debug.LogError("Please Enter a Unique Name");
            }
        }

        private void OnDestroy()
        {
            Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Initialize(string _type, string _name)
        {
            type = _type;
            uniqueName = _name;
        }

        public void Load()
        {
            NeuralNetworkSaveData saveData = null;

            switch (trainingMethod)
            {
                case ETrainingMethod.UseBestSaveForLoadingAndTraing:
                case ETrainingMethod.UseBestSaveForInitialize:
                    string pathToBest = Application.persistentDataPath + "/" + type + "/" + type + "Best" + ".nn_save";
                    saveData = SaveSystem.SaveSystem.Load<NeuralNetworkSaveData>(pathToBest);
                    break;
                case ETrainingMethod.UseOwnSave:
                    string OwnPath = Application.persistentDataPath + "/" + type + "/" + uniqueName + ".nn_save";
                    saveData = SaveSystem.SaveSystem.Load<NeuralNetworkSaveData>(OwnPath);
                    break;
            }

            Load(saveData);
        }

        public void Load(NeuralNetworkSaveData _saveData)
        {
            if (_saveData != null)
            {
                if (_saveData.Type == type)
                {
                    if (inputCount == _saveData.InputCount && layersConfig.Count == _saveData.Layers.Count)
                    {
                        for (int i = 0; i < layersConfig.Count; i++)
                        {
                            if (_saveData.Layers[i].Neurons.Length != layersConfig[i].NeuronCount)
                            {
                                Debug.LogWarning("Loaded values can't be mapped to this neural network, because the structure is difference. Generating random values, it will 'forget' everything it has learned");
                                GenerateRandomValues();
                                return;
                            }
                        }
                        layers = _saveData.Layers;
                        FitnessValue = _saveData.FitnessValue;
                        return;
                    }
                }
            }
            Debug.LogWarning("Data to load is null, most likely because it has no save or a different name, another reason could be that saving failed.");
            GenerateRandomValues();
        }

        public void Save()
        {
            string directoryPath = Application.persistentDataPath + "/" + type + "/";
            string fullpath = directoryPath + uniqueName + ".nn_save";

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (bestOwnSave.FitnessValue > FitnessValue)
            {
                SaveSystem.SaveSystem.Save(fullpath, bestOwnSave);
            }
            else
            {
                SaveSystem.SaveSystem.Save(fullpath, GetSaveData());
            }
            
        }

        public NeuralNetworkSaveData GetSaveData()
        {
            NeuralNetworkSaveData save = new NeuralNetworkSaveData();
            save.InputCount = inputCount;
            save.Layers = layers;
            save.FitnessValue = FitnessValue;
            save.Name = UniqueName;
            save.Type = type;
            return save;
        }

        private void GenerateRandomValues()
        {
            FitnessValue = 0;

            int weightsAmount = inputCount;
            
            foreach (NeuralLayerConfig layer in layersConfig)
            {
                Neuron[] neurons = new Neuron[layer.NeuronCount];

                for (int i = 0; i < layer.NeuronCount; i++)
                {
                    float[] weights = new float[weightsAmount];

                    float bias = Random.Range(-GetStepSize(), GetStepSize());
                    float alpha = Random.Range(-GetStepSize(), GetStepSize());

                    MutatedNeuron[] mutatedNeurons = new MutatedNeuron[weightsAmount];

                    for (int j = 0; j < weightsAmount; j++)
                    {
                        weights[j] = Random.Range(-GetStepSize(), GetStepSize());

                        float mutatedWeight = Random.Range(-GetStepSize(), GetStepSize());
                        float mutatedAlpha = Random.Range(-GetStepSize(), GetStepSize());
                        float mutatedBias = Random.Range(-GetStepSize(), GetStepSize());

                        mutatedNeurons[j] = new MutatedNeuron(mutatedWeight, mutatedBias, mutatedAlpha);

                    }
                    neurons[i] = new Neuron(weights, bias, alpha, mutatedNeurons);
                }

                NeuralLayer newLayer = new NeuralLayer(neurons, layer.ActivationFunction);
                layers.Add(newLayer);
                weightsAmount = newLayer.Neurons.Length;
               
            }
        }

        public void AddFitnees(float value)
        {
            FitnessValue += value;
        }

        public float[] Run(float[] _inputs)
        {
            if (_inputs.Length != inputCount)
            {
                throw new Exception("The inputs count does not match the input amount from the neural network");
            }

            if (layers.Count <= 0)
            {
                Debug.LogError($"{gameObject.name} has no layer. That probaly means " +
                                $"it was not loaded/initialized correctly or something was wrong with the savedata. " +
                                $"It could also mean the inspector values are null.");
            }

            foreach (NeuralLayer layer in layers)
            {
                _inputs = layer.FeedForward(_inputs);
            }
            return _inputs;
        }

        private float GetStepSize()
        {
            return fitnessToStepSize.Evaluate(FitnessValue);
        }

        public void Train()
        {
            NeuralNetworkManager.UpdateResults(GetSaveData());
            
            if (trainingMethod == ETrainingMethod.UseBestSaveForLoadingAndTraing)
            {
               Load(NeuralNetworkManager.GetTheBestOfType(type));
                Debug.Log("LoadedBestTrained");
            }
            else if (bestOwnSave != null)
            {
                if (bestOwnSave.FitnessValue > FitnessValue)
                {
                    Load(bestOwnSave);
                }
                else
                {
                    bestOwnSave = GetSaveData();
                }
            }
            else
            {
                bestOwnSave = GetSaveData();
            }

            foreach (NeuralLayer layer in layers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {

                    for (int i = 0; i < neuron.weights.Length; i++)
                    {
                        if (neuron.weights[i] != 0)
                        {
                            neuron.weights[i] += Random.Range(-GetStepSize(), GetStepSize());
                        }
                        else
                        {
                            if (EnableDisabledConnnectionChangerPerTraining > Random.Range(0f, 100f))
                            {
                                neuron.weights[i] += Random.Range(-GetStepSize(), GetStepSize());
                            }
                        }

                        neuron.bias += Random.Range(-GetStepSize(), GetStepSize());
                        neuron.alpha += Random.Range(-GetStepSize(), GetStepSize());

                        if (neuron.mutatedNeuron[i].IsActive)
                        {
                            neuron.mutatedNeuron[i].weight += Random.Range(-GetStepSize(), GetStepSize());
                            neuron.mutatedNeuron[i].alpha += Random.Range(-GetStepSize(), GetStepSize());
                            neuron.mutatedNeuron[i].bias += Random.Range(-GetStepSize(), GetStepSize());
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
                        neuron.mutatedNeuron[randomConnection].IsActive = true;
                    }
                }
            }
            FitnessValue = 0;
        }
    }

    public enum ETrainingMethod
    {
        UseBestSaveForLoadingAndTraing,
        UseBestSaveForInitialize,
        UseOwnSave,
    }
}
