using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.DataManagement
{
    public class SaveSystem
    {
        protected static SaveSystem _instance;
        public static SaveSystem Instance
        {
            get => _instance;
            set
            {
                if(_instance != null)
                    return;

                _instance = value;
            }
        }
        
        /// <summary>
        /// Creates a static instance of the class on compilation.
        /// </summary>
        /// <remarks>
        /// Only works with non-Unity <see cref="UnityEngine.Object"/> classes.
        /// </remarks>
        static SaveSystem()
        {
            Instance = new SaveSystem();
        }
        
        protected static List<SaveFileData> SaveFile = new() { new SaveFileData() };
        
        protected static SaveFileData _currentSaveFileData;
        public static SaveFileData CurrentSaveFileData 
        { 
            get
            {
                if(_currentSaveFileData == null)
                    _currentSaveFileData = SaveFile[0];

                return _currentSaveFileData;
            }
            set
            {
                if(value == null)
                    return;

                _currentSaveFileData = value;
            }
        }

        /// <summary>
        /// Updates the data that will be saved next time <see cref="SaveDataFile"/> is called.
        /// </summary>
        public static event Action UpdateSaveHandler;
        /// <summary>
        /// Saves the game data to a real save file. 
        /// </summary>
        public static Action SaveDataHandler;
        /// <summary>
        /// Used to do stuff before event handler do loading events.
        /// Think like scene loading data clean up or settings and so forth.
        /// </summary>
        public static event Action BeforeLoadSaveDataHandler;
        /// <summary>
        /// Runs when a save data has been loaded.
        /// </summary>
        /// <remarks>
        /// To do stuff before the save data has been loaded use <see cref="BeforeLoadSaveDataHandler"/>
        /// </remarks>
        public static event Action LoadSaveDataHandler;
        
        // This is the path when called from Unity editor.
        // C:\Users\jonat\AppData\LocalLow\Shatter Fantasy\Immortal Chronicles - The Realm of Imprisoned Sorrows\ICSaveData.txt
        
        protected static readonly string SaveFileNameBase =
#if UNITY_EDITOR
            Application.persistentDataPath + "/Development ICSaveData.txt";
#else
            Application.persistentDataPath + "/ICSaveData.txt";
#endif
        // The data stream of the contents being written and read from the save file.
        protected static FileStream DataStream;

        // Key for reading and writing encrypted data.
        // (This is a "hardcoded" secret key. )
        protected static readonly  byte[] SavedKey = { 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15 };

        public static void UpdateSaveFile()
        {
            CurrentSaveFileData.SceneDataBlock = new SceneSaveData
            {
                CurrentScene = SceneManager.GetActiveScene(),
                CurrentSaveStation = CurrentSaveFileData.CurrentSaveStation
            };
            
            UpdateSaveHandler?.Invoke();
            // Save the scene with the last used save station so we know which one to load.
            CurrentSaveFileData.CurrentScene = SceneManager.GetActiveScene();
        }

        public static void SaveDataFile()
        {

            // First update the current information in the save file.
            UpdateSaveFile();

            // Create a FileStream for writing data to.
            DataStream = new FileStream(SaveFileNameBase, FileMode.Create);
#if UNITY_EDITOR
            StreamWriter streamWriter = new StreamWriter(DataStream);
#else
            // Create an AES instance
            // The i stands for input
            Aes iAes = Aes.Create();

            // Save the generated IV aka the initialization Vector = IV
            // This tells the AES where to start as it encrypts data.
            byte[] inputIV = iAes.IV;
            
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
#endif
            
            
            // Serialize the SaveFileData object into JSON and save string.
            string jsonString = JsonUtility.ToJson(CurrentSaveFileData);
            
            //Write to the innermost stream which is the encryption one.
            streamWriter.WriteLine(jsonString);

            //Close the streams in reverse order as they were made.
            streamWriter.Close();
            
#if UNITY_EDITOR
#else
            cryptoStream.Close();
#endif
            DataStream.Close();
        }

        public static void LoadDataFile()
        {
            // If there is no save file, and we are loading in the game this means a new game save needs made possibly. 
            if (!File.Exists(SaveFileNameBase))
            {
                return;
            }


#if UNITY_EDITOR
            DataStream = new FileStream(SaveFileNameBase, FileMode.Open);
            StreamReader streamReader = new StreamReader(DataStream);
#else
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
#endif

            // Read the entire file
            string text = streamReader.ReadToEnd();
            // Close the stream after done using it
            streamReader.Close();

            SaveFile[0] = JsonUtility.FromJson<SaveFileData>(text);
            //Deserialize the data from here and load it into Unity Object.
            CurrentSaveFileData = SaveFile[0];
            // Finally initialize the Unity game data and load the scene and player.
            SetGameData();
        }

        /// <summary>
        /// Sets the data from the current save file into the game and starts the initialization process for loading the game scene.
        /// </summary>
        protected static void SetGameData()
        {
            if(!string.IsNullOrEmpty(CurrentSaveFileData.CurrentScene.name))
            {
                // For testing purposes only. Will remove in actual game play.
                if(SceneManager.GetActiveScene() != CurrentSaveFileData.CurrentScene)
                    SceneManager.LoadSceneAsync(CurrentSaveFileData.CurrentScene.buildIndex, LoadSceneMode.Single);
            }
            // Set the spawning checkpoint which the SaveStation C# class ia a subclass of.
            // Checkpoint manager will have an execution order after the script that calls load game.

            
       
            BeforeLoadSaveDataHandler?.Invoke();
            LoadSaveDataHandler?.Invoke();
        }

        public static List<SaveDataBlock> CurrentSaveDataBlocks()
        {
            return CurrentSaveFileData.SaveDatas;
        }

        public static bool HasSaveFiles()
        {
            // If there is no save file, and we are loading in the game this means a new game save needs made possibly. 
            return File.Exists(SaveFileNameBase);
        }
    }
    
    [System.Serializable]
    public class SaveFileData
    {
        public SceneSaveData SceneDataBlock;
        
        [SerializeReference]
        public List<SaveDataBlock> SaveDatas = new List<SaveDataBlock>();
        
        public SavePoint CurrentSaveStation;
        public Scene CurrentScene;
        
        public int HoursPlayed = 0;

        public TSaveData GetSaveDataBlock<TSaveData>() where TSaveData : SaveDataBlock
        {
            var dataQuery = SaveDatas.Where(saveData => saveData is TSaveData).ToArray();
            
            if (!dataQuery.Any())
            {
                return null;
            }
            
            return dataQuery.First() as TSaveData;
        }
        
        public bool TryAddOrSetDataBlock<TSaveData>(TSaveData newData) where TSaveData : SaveDataBlock
        {
            TSaveData indexedData = GetSaveDataBlock<TSaveData>();
            
            if (indexedData != null)
            {
                // There is already a data block of the attempted added type.
                // Don't add a new one instead just override the already existing one.
                indexedData = newData;
                return false;
            }
            
            // No data block of that type exists than it is safe to add a new one.
            SaveDatas.Add(newData);
            return true;
        }
        
        // WE MIGHT NOT NEED THIS.
        // SaveDataBlock is a class so it is passed by reference. We could just edit that data. 
        public void SetSaveDataBlock<TSaveData>(TSaveData newData) where TSaveData : SaveDataBlock
        { 
            // Find the SaveDataBlock of the type we are trying to set.  
           var data = SaveDatas.First(saveData => saveData is TSaveData);
           // Find the index for that datablock type.
           int dataIndex = SaveDatas.IndexOf(data);

           // Now set the new data 
           SaveDatas[dataIndex] = newData;
        }
    }

    [System.Serializable]
    public class SceneSaveData : SaveDataBlock
    {
        public SavePoint CurrentSaveStation;
        public Scene CurrentScene;
        
        public int HoursPlayed = 0;
    }
    
    [System.Serializable]
    public class SaveDataBlock
    {
    }
}
