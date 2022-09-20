using System;
using System.Collections.Generic;
using System.Linq;
using GameplayScripts;
using JetBrains.Annotations;
using UnityEngine;

namespace RSNManagers
{
    public interface _IPersistData
    {
        public void Save();
    }

    [LoadAssetFrom(AssetSource.PersistFolder, fileNameSuffix: "SaveData")]
    public abstract class _PersistData<T> : ScriptableSingleton<T>, _IPersistData where T : ScriptableRsnObject
    {
        [Serializable]
        private struct _PersistDynamicType<T>
        {
            public string key;
            public T value;

            public _PersistDynamicType(string key, T value)
            {
                this.key = key;
                this.value = value;
            }
        }
        
        [Serializable]
        private struct _PersistDynamicTypeList<T>
        {
            public string key;
            public List<T> values;

            public _PersistDynamicTypeList(string key, List<T> values)
            {
                this.key = key;
                this.values = values;
            }
        }
        
        [SerializeField] private List<_PersistDynamicType<int>> __dynamic_ints;
        [SerializeField] private List<_PersistDynamicType<float>> __dynamic_floats;
        [SerializeField] private List<_PersistDynamicType<bool>> __dynamic_booleans;
        [SerializeField] private List<_PersistDynamicType<string>> __dynamic_strings;
        [SerializeField] private List<_PersistDynamicType<Vector2>> __dynamic_vector2s;
        [SerializeField] private List<_PersistDynamicType<Vector3>> __dynamic_vector3s;
        [SerializeField] private List<_PersistDynamicTypeList<Machine>> __rooms;
        [field: SerializeField] public string firstInstalledVersion { private set; get; } = null;


        #region Life Cycle

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            var isDirty = false;
            
            if (string.IsNullOrEmpty(firstInstalledVersion))
            {
                firstInstalledVersion = Application.version;
                isDirty = true;
            }

            isDirty = true;
            
            if (isDirty)
            {
                Save();
            }
        }

        public override void Clear()
        {
            base.Clear();

            firstInstalledVersion = null;
            
            Save();
        }

    #endregion



    #region Dynamic Keys
        
        private void SetDynamicKeyValue<T>(string key, T value, List<_PersistDynamicType<T>> list)
        {
            list.RemoveAll(v => v.key == key);
            list.Add(new _PersistDynamicType<T>(key, value));
        }

        private T GetDynamicValue<T>(string key, T defaultValue, List<_PersistDynamicType<T>> list)
        {
            var v = list.Cast<_PersistDynamicType<T>?>().FirstOrDefault(v => v.Value.key == key);
            return v.HasValue ? v.Value.value : defaultValue;
        }
        
        private void SetDynamicKeyValueList<T>(string key, List<T> value, List<_PersistDynamicTypeList<T>> list)
        {
            list.RemoveAll(v => v.key == key);
            list.Add(new _PersistDynamicTypeList<T>(key, value));
        }

        private List<T> GetDynamicValueList<T>(string key, List<T> defaultValue, List<_PersistDynamicTypeList<T>> list)
        {
            var v = list.Cast<_PersistDynamicTypeList<T>?>().FirstOrDefault(v => v.Value.key == key);
            return v.HasValue ? v.Value.values : defaultValue;
        }
        

        // Integer
        public void SetInt(string key, int value) => SetDynamicKeyValue(key, value, __dynamic_ints);
        public int GetInt(string key, int defaultValue = 0) => GetDynamicValue(key, defaultValue, __dynamic_ints);
        
        // Float
        public void SetFloat(string key, float value) => SetDynamicKeyValue(key, value, __dynamic_floats);
        public float GetFloat(string key, float defaultValue = 0) => GetDynamicValue(key, defaultValue, __dynamic_floats);
        
        // Boolean
        public void SetBool(string key, bool value) => SetDynamicKeyValue(key, value, __dynamic_booleans);
        public bool GetBool(string key, bool defaultValue = false) => GetDynamicValue(key, defaultValue, __dynamic_booleans);
        
        // String
        public void SetString(string key, string value) => SetDynamicKeyValue(key, value, __dynamic_strings);
        public string GetString(string key, string defaultValue = "") => GetDynamicValue(key, defaultValue, __dynamic_strings);
        
        // Vector2
        public void SetVector2(string key, Vector2 value) => SetDynamicKeyValue(key, value, __dynamic_vector2s);
        public Vector2 GetVector2(string key, Vector2 defaultValue) => GetDynamicValue(key, defaultValue, __dynamic_vector2s);
        
        // Vector3
        public void SetVector3(string key, Vector3 value) => SetDynamicKeyValue(key, value, __dynamic_vector3s);
        public Vector3 GetVector3(string key, Vector3 defaultValue) => GetDynamicValue(key, defaultValue, __dynamic_vector3s);
        
        public void SetRoom(string key, List<Machine> value) => SetDynamicKeyValueList(key, value, __rooms);
        public List<Machine> GetRoom(string key, List<Machine> defaultValue) => GetDynamicValueList(key, defaultValue, __rooms);

        #endregion



    #region Encode / Decode

        protected override void EncodeFields()
        {
            base.EncodeFields();
        }

        protected override void DecodeFields()
        {
            base.DecodeFields();
        }

    #endregion

    }
    
}
