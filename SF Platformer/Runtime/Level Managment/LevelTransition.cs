using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.LevelModule
{
    public class LevelTransition : MonoBehaviour
    {
        [SerializeField] private string _nextSceneName;
        private Scene _nextScene;

        private void Start()
        {
            if(string.IsNullOrEmpty(_nextSceneName))
            {
                _nextScene = SceneManager.GetSceneByName(_nextSceneName);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(_nextScene == null)
                return;

            SceneManager.LoadScene(_nextSceneName);
        }
    }
}
