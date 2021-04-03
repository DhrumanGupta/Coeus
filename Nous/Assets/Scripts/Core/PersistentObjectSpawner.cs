using UnityEngine;

namespace Game.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjectPrefab = null;
        private static bool hasSpawned = false;

        private void Awake()
        {
            if (!hasSpawned)
            {
                SpawnPersistentObjects();
            }
            hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}