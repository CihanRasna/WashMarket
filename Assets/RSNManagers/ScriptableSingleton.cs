using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace RSNManagers
{
    public abstract class ScriptableSingleton : ScriptableRsnObject
    {
    }

    public abstract class ScriptableSingleton<T> : ScriptableSingleton where T : ScriptableRsnObject
    {
        static T instance = null;
        public static T Instance => GetInstance();

        private static LoadAssetFrom loadAssetFrom;
        private static bool shouldLoadFromPersistFolder;
        private static string persistFileLocation;

        private static string dir = $"{Application.persistentDataPath}{SaveDirectory}";
        private const string SaveDirectory = "/SaveData/";


        #region Life Cycle

        private static T GetInstance()
        {
            if (!instance)
            {
                var type = typeof(T);
                loadAssetFrom = type.GetCustomAttribute<LoadAssetFrom>();
                shouldLoadFromPersistFolder = loadAssetFrom?.source == AssetSource.PersistFolder;

                if (shouldLoadFromPersistFolder)
                {
                    persistFileLocation = $"{dir}{loadAssetFrom?.fileNameSuffix}.rsn";
                }
                
                var so = loadAssetFrom == null || shouldLoadFromPersistFolder
                    ? CreateInstance<T>()
                    : Resources.Load<T>(type.Name);
                if (so as ScriptableSingleton<T> == null)
                {
                    throw new Exception(
                        "An unknown error occured while creating the instance for ScriptableSingle of type " +
                        type.FullName + "!");
                }

                instance = so;
                
                (so as ScriptableSingleton<T>).OnInitialize();
            }

            return instance;
        }

        protected virtual void OnInitialize()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            if (shouldLoadFromPersistFolder && File.Exists(persistFileLocation))
            {
                var file = File.Open(persistFileLocation, FileMode.Open);
                var formatter = new BinaryFormatter();
                var json = (string)formatter.Deserialize(file);
                JsonUtility.FromJsonOverwrite(json, this);
                file.Close();
                DecodeFields();
            }
        }

        public static void Destroy()
        {
            if (instance == null)
            {
                return;
            }

            if (shouldLoadFromPersistFolder)
            {
                (instance as ScriptableSingleton<T>).Save();
            }

            ScriptableObject.Destroy(instance);
            instance = null;
        }

        #endregion


        #region Encode / Decode

        protected virtual void EncodeFields()
        {
        }

        protected virtual void DecodeFields()
        {
        }

        #endregion


        #region Persistence

        public void Save()
        {
            if (!shouldLoadFromPersistFolder)
            {
                throw new Exception("Only ScriptableSingletons with LoadAssetFrom(TempFolder) attribute can be saved!");
            }
            
            EncodeFields();

            var json = JsonUtility.ToJson(this);
            FileStream file = File.Open(persistFileLocation,FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(file, json);
            file.Close();
        }

        public virtual void Clear()
        {
            if (!shouldLoadFromPersistFolder)
            {
                throw new Exception(
                    "Only ScriptableSingletons with LoadAssetFrom(TempFolder) attribute can be cleared!");
            }

            File.Delete(persistFileLocation);
        }

        #endregion
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class LoadAssetFrom : Attribute
    {
        public readonly AssetSource source;
        public readonly string fileNameSuffix;

        public LoadAssetFrom(AssetSource source, string fileNameSuffix = null)
        {
            this.source = source;
            this.fileNameSuffix = fileNameSuffix;
        }
    }

    public enum AssetSource
    {
        Resources,
        PersistFolder
    }
}