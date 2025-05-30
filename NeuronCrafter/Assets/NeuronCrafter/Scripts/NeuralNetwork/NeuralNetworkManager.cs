using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NeuronCrafter.NeuralNetwork
{
    public class NeuralNetworkManager : MonoBehaviour
    {
        private static Dictionary<string, NeuralNetworkSaveData> BestTrainingResult = new Dictionary<string, NeuralNetworkSaveData>();

        private void Awake()
        {
            BestTrainingResult = new Dictionary<string, NeuralNetworkSaveData>();
        }

        public static void UpdateResults(NeuralNetworkSaveData _result)
        {
            if (_result == null)
            {
                Debug.LogError($"The neural networks of the was null");
            }
            if (_result.Type == "")
            {
                Debug.LogError($"The neural networks of the had no Type");
            }
            if (_result.Name == "")
            {
                Debug.LogError($"The neural networks of the had no Name");
            }

            if (BestTrainingResult.ContainsKey(_result.Type))
            {
                if (BestTrainingResult[_result.Type].FitnessValue < _result.FitnessValue)
                {
                    BestTrainingResult[_result.Type] = _result;
                    Debug.Log($"Best of {_result.Type} was {BestTrainingResult[_result.Type].Name}: {BestTrainingResult[_result.Type].FitnessValue}");
                }
            }
            else
            {
                BestTrainingResult[_result.Type] = _result;
                Debug.Log($"Best of {_result.Type} was {BestTrainingResult[_result.Type].Name}: {BestTrainingResult[_result.Type].FitnessValue}");
            }

        }

        public static NeuralNetworkSaveData GetTheBestOfType(string _type)
        {
            return BestTrainingResult[_type];
        }

        public static Dictionary<string, NeuralNetworkSaveData> GetTheBestForAllType()
        {
            return BestTrainingResult;
        }

        private void OnDestroy()
        {
            foreach (NeuralNetworkSaveData result in BestTrainingResult.Values)
            {
                Save(result);
            }
        }

        private void OnApplicationQuit()
        {
            foreach (NeuralNetworkSaveData result in BestTrainingResult.Values)
            {
                Save(result);
            }
        }

        private void Save(NeuralNetworkSaveData _neuralNetworkSaveData)
        {
#if UNITY_EDITOR
            string directoryPath = Application.streamingAssetsPath + "/" + "NeuralNetworkSaveFiles" + "/" + _neuralNetworkSaveData.Type + "/";
            string fullpath = directoryPath + _neuralNetworkSaveData.Type + "Best" + ".nn_save";

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
            SaveSystem.SaveSystem.Save(fullpath, _neuralNetworkSaveData);
        }
#endif
    }
}
