using System.Collections;
using Game.Saving;
using UnityEngine;

namespace Game.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save_file";
        [SerializeField] float fadeInTime = 1f;
        private SavingSystem _savingSystem;

        private void Awake()
        {
            _savingSystem = GetComponent<SavingSystem>();
        }

        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImediate();
            yield return _savingSystem.LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }
        
        #if UNITY_EDITOR

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }
        
        #endif

        public void Save()
        {
            // Call saving system to save
            _savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            // Call saving system to load
            _savingSystem.Load(defaultSaveFile);
        }
    }
}