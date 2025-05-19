using System;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveSystem
    {
        private static readonly string encryptionCodeWord = "This is the best project you will see in your life!";

        /// <summary>
        /// Saves a object with the ISaveData implemented and can't have a constructor with params
        /// </summary>
        /// <param name="fullPath">The full path including filename and extension e.g. C:/User/Games/MySave.randomExtension, you should use a function that gives you a relative path like "Application.PersistenPath"</param>
        /// <param name="objectToSave">Must be marked as [Serializable]</param>
        public static void Save<T>(string fullPath, T objectToSave)
        {
            ValidateSaveDataType<T>();

            string dataToSave = JsonUtility.ToJson(objectToSave, true);

            //dataToSave = EncryptDecrypt(dataToSave);
            File.WriteAllText(fullPath, dataToSave);
        }

        public static T Load<T>(string fullPath) where T : new()
        {
            ValidateSaveDataType<T>();

            bool fileExist = File.Exists(fullPath);

            if (fileExist)
            {
                string savedContent = File.ReadAllText(fullPath);
                //savedContent = EncryptDecrypt(savedContent);
                return JsonUtility.FromJson<T>(savedContent);
            }
            return default(T);
        }

        // this function is from
        // https://youtu.be/aUi9aijvpgs?si=CamS1HS7SOrh1rtO
        // Shaped by Rain Studios
        private static string EncryptDecrypt(string data)
        {
            char[] encryptedChars = new char[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                encryptedChars[i] = (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
            }
            return new string(encryptedChars);
        }

        private static void ValidateSaveDataType<T>()
        {
            if (Attribute.IsDefined(typeof(T), typeof(SerializableAttribute)) == false)
            {
                throw new InvalidOperationException("The class your are are trying to save or load is not marked as a [Serializable]");
            }
        }
    }
}
