using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
#endif

namespace NeuronCrafter.NeuralNetwork
{
    public class NeuralNetwork : MonoBehaviour
    {
        private const string typeTooltip = "All neural networks that have the same structure of layers and neurons should be the same type";
        private const string uniqueNameTooltip = "This will be uses to create the save file and in the managerclass thats holds each neural network is saved in a dictonary<sting Uniquename, NeuralNetwork this>";
        public float FitnessValue { get; private set; }

        [SerializeField, Header("Core"), Tooltip(typeTooltip)] private string type;
        [SerializeField, Tooltip(uniqueNameTooltip)] private string uniqueName;
        [SerializeField] private NeuralNetworkTrainer trainer;

        [SerializeField, Header("Structure")] private int inputCount;
        [SerializeField] private List<NeuralLayerConfig> layersConfig;

        [SerializeField, Header("Initalize")] private EInitializeMethod initializeMethod;

        [SerializeField, Header("values for generating new values")] private float weightsRange;
        [SerializeField] private float biasRange;
        [SerializeField] private float alphaRange;

        public string Type { get { return type; } }
        public string UniqueName { get { return uniqueName; } }
        public int InputCount { get { return inputCount; } }
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

            switch (initializeMethod)
            {
                case EInitializeMethod.UseBestSave:
                    string pathToBest = Application.streamingAssetsPath + "/" + "NeuralNetworkSaveFiles" + "/" + type + "/" + type + "Best" + ".nn_save";
                    saveData = SaveSystem.SaveSystem.Load<NeuralNetworkSaveData>(pathToBest);
                    break;
                case EInitializeMethod.UseOwnSave:
                    string OwnPath = Application.streamingAssetsPath + "/" + "NeuralNetworkSaveFiles" + "/" + type + "/" + uniqueName + ".nn_save";
                    saveData = SaveSystem.SaveSystem.Load<NeuralNetworkSaveData>(OwnPath);
                    break;
            }

            Load(saveData, false);
        }

        public void SetBestSave(NeuralNetworkSaveData _bestOwnSave)
        {
            bestOwnSave = _bestOwnSave;
        }

        public NeuralNetworkSaveData GetBestSave()
        {
            return bestOwnSave;
        }

        public void Load(NeuralNetworkSaveData _saveData, bool _loadFitnees = true)
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
                        if (_loadFitnees)
                        {
                            FitnessValue = _saveData.FitnessValue;
                        }
                        return;
                    }
                }
            }
            Debug.LogWarning("Data to load is null, most likely because it has no save or a different name, another reason could be that saving failed.");
            GenerateRandomValues();
        }

        public void Save()
        {
#if UNITY_EDITOR
            string directoryPath = Application.streamingAssetsPath + "/" + "NeuralNetworkSaveFiles" + "/" + type + "/";
            string fullpath = directoryPath + uniqueName + ".nn_save";

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (bestOwnSave != null)
            {
                if (bestOwnSave.FitnessValue > FitnessValue)
                {
                    SaveSystem.SaveSystem.Save(fullpath, bestOwnSave);
                }
            }
            else
            {
                SaveSystem.SaveSystem.Save(fullpath, GetSaveData());
            }
            Debug.Log(fullpath);
#endif
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

                    float bias = Random.Range(-biasRange, biasRange);
                    float alpha = Random.Range(-alphaRange, alphaRange);

                    MutatedNeuron[] mutatedNeurons = new MutatedNeuron[weightsAmount];

                    for (int j = 0; j < weightsAmount; j++)
                    {
                        weights[j] = Random.Range(-weightsRange, weightsRange);

                        float mutatedWeight = Random.Range(-weightsRange, weightsRange);
                        float mutatedAlpha = Random.Range(-alphaRange, alphaRange);
                        float mutatedBias = Random.Range(-biasRange, biasRange);

                        mutatedNeurons[j] = new MutatedNeuron(mutatedWeight, mutatedBias, mutatedAlpha);

                    }
                    neurons[i] = new Neuron(weights, bias, alpha, mutatedNeurons);
                }

                NeuralLayer newLayer = new NeuralLayer(neurons, layer.ActivationFunction);
                layers.Add(newLayer);
                weightsAmount = newLayer.Neurons.Length;
            }
        }

        public void ResetFitness()
        {
            FitnessValue = 0;
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

        public void Train()
        {
#if UNITY_EDITOR
            trainer.Train(this, layers);
#endif
        }
    }

    public enum EInitializeMethod
    {
        UseBestSave,
        UseOwnSave,
    }
}
