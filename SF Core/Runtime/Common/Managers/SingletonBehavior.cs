using System;
using UnityEditor;
using UnityEngine;

namespace SF.Utilities
{
    public abstract class SingletonBehavior<TSingleton> : MonoBehaviour where TSingleton : MonoBehaviour
    {
        protected static TSingleton _instance;
        public static TSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Try to find an object with a component already attached.
                    _instance = FindAnyObjectByType<TSingleton>();
                    
                    // If none are found create a game object called SF Singleton and add the component to it. 
                    // Set the Singleton value as the newly created component on the Object.
                    if (_instance == null)
                    {
                        Debug.Log($"There was no {typeof(TSingleton)} inside the scene.");
                    }
                }
                
                return _instance;
            }
            set
            {
                if(_instance == null)
                    _instance = value;  
            }
        }

        protected virtual void Awake()
        {
            if( _instance != null && _instance != this)
                Destroy(this);
        }
    }
}
