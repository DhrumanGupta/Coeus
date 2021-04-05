using System;
using UnityEngine;

namespace Game.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjectPrefab = null;

        private static bool _hasSpawned = false;

        private void Awake()
        {
            if (!_hasSpawned)
            {
                SpawnPersistentObjects();
            }
            _hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}