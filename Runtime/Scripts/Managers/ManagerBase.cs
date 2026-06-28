using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SF
{
    public abstract partial class ManagerBase<T> : MonoBehaviour where T : Object
    {
        [NoAutoStaticsCleanup]
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                
                return _instance;
            } 
            set => _instance = value;
        }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject); 
            }
            else if(Instance != this as T)
                Destroy(this);
        }
    }
    
    public abstract partial class ManagerBaseStaticCleanUp<T> : MonoBehaviour where T : Object
    {
        [AutoStaticsCleanup]
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                
                return _instance;
            } 
            set => _instance = value;
        }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject); 
            }
            else if (Instance != this as T)
            {
                Destroy(this);
            }
        }
    }
}
