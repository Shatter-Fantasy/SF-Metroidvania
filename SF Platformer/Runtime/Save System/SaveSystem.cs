using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using SF.SpawnModule;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.DataManagement
{
    public static class SaveSystem
    {
        private static List<SaveFileData> SaveFiles = new() { new SaveFileData() };
        private static SaveFileData _currentSaveFileData;
        public static SaveFileData CurrentSaveFileData 
        { 
            get
            {
                if(_currentSaveFileData == null)
                    _currentSaveFileData = SaveFiles[0];

                return _currentSaveFileData;
            }
            set
            {
                if(value == null)
                    return;

                _currentSaveFileData = value;
            }
        }



        // This is the path when called from Unity editor.
        // C:\Users\jonat\AppData\LocalLow\Shatter Fantasy\Immortal Chronicles - The Realm of Imprisoned Sorrows\ICSaveData.txt
        
        private readonly static string SaveFileNameBase =
#if UNITY_EDITOR
            Application.persistentDataPath + "/Development ICSaveData.txt";
#else
            Application.persistentDataPath + "/ICSaveData.txt";
#endif
        // The data stream of the contents being written and read from the save file.
        private static FileStream DataStream;

        // Key for reading and writing encrypted data.
        // (This is a "hardcoded" secret key. )
        private readonly static byte[] SavedKey = { 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15 };

        public static void UpdateSaveFile()
        {
            // Save the scene with the last used save station so we know which one to load.
            CurrentSaveFileData.CurrentScene = SceneManager.GetActiveScene();
        }

        public static void SaveDataFile()
        {

            // First update the current information in the save file.
            UpdateSaveFile();


            // Create an AES instance
            // The i stands for input
            Aes iAes = Aes.Create();

            // Save the generated IV aka the initialization Vector = IV
            // This tells the AES where to start as it encrypts data.
            byte[] inputIV = iAes.IV;

            // Create a FileStream for writing data to.
            DataStream = new FileStream(SaveFileNameBase, FileMode.Create);

            // Just save the inputIV at the start of the fil before encrypting it.
            // It doesn't need to be private.
            DataStream.Write(inputIV,0,inputIV.Length);

            // Create a wrapper for the CryptoStream file to encrypt the FileStream
            CryptoStream cryptoStream = new CryptoStream(
                DataStream,
                iAes.CreateEncryptor(SavedKey, iAes.IV),
                CryptoStreamMode.Write
            );

            // Create StreamWriter, wrapping CryptoStream.
            StreamWriter streamWriter = new StreamWriter(cryptoStream);


            // Serialize the SaveFileData object into JSON and save string.
            string jsonString = JsonUtility.ToJson(SaveFiles[0]);

            //Write to the innermost stream which is the encryption one.
            streamWriter.WriteLine(jsonString);

            //Close the streams in reverse order as they were made.
            streamWriter.Close();
            cryptoStream.Close();
            DataStream.Close();
        }

        public static void LoadDataFile()
        {
            if(!File.Exists(SaveFileNameBase))
                return;

            // Create new AES instance.
            // The o stands for output
            Aes oAes = Aes.Create();

            // Crete an array of correct size
            byte[] outputIV = new byte[oAes.IV.Length];


            // Create a FileStream
            DataStream = new FileStream(SaveFileNameBase, FileMode.Open);

            // Read the AES IV from the file
            DataStream.Read(outputIV, 0, outputIV.Length);

            // Create a wrapper for the CryptoStream file to decrypt the FileStream
            CryptoStream cryptoStream = new CryptoStream(
                DataStream,
                oAes.CreateDecryptor(SavedKey, outputIV),
                CryptoStreamMode.Read
            );

            // Create a StreamReader to wrap the cryptoStream
            StreamReader streamReader = new StreamReader(cryptoStream);

            // Read the entire file
            string text = streamReader.ReadToEnd();
            // Close the stream after done using it
            streamReader.Close();

            //Deserialize the data from here and load it into Unity Object.
            SaveFiles[0] = JsonUtility.FromJson<SaveFileData>(text);
            CurrentSaveFileData = SaveFiles[0];

            // Finally initialize the Unity game data and load the scene and player.
            SetGameData();
        }

        /// <summary>
        /// Sets the data from the current save file into the game and starts the initialization process for loading the game scene.
        /// </summary>
        private static void SetGameData()
        {

            if(!string.IsNullOrEmpty(CurrentSaveFileData.CurrentScene.name))
            {
                // For testing purposes only. Will remove in actual game play.
                if(SceneManager.GetActiveScene() != CurrentSaveFileData.CurrentScene)
                    SceneManager.LoadSceneAsync(CurrentSaveFileData.CurrentScene.buildIndex, LoadSceneMode.Single);
            }
            // Set the spawning checkpoint which the SaveStation C# class ia a subclass of.
            // Checkpoint manager will have a execution order after the script that calls load game.
            CheckPointEvent.Trigger(CheckPointEventTypes.ChangeCheckPoint, CurrentSaveFileData.CurrentSaveStation);
        }
    }

    [System.Serializable]
    public class SaveFileData
    {
        public SaveStation CurrentSaveStation;
        public Scene CurrentScene;
        public int HoursPlayed = 0;
    }
}
