using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // Simple class to define pool settings per prefab in the Inspector
    [System.Serializable]
    public class Pool
    {
        public Transform prefab;
        public int size;
    }

    public static ObjectPooler Instance;

    [Header("Pool Configurations")]
    [SerializeField] private List<Pool> pools; // You can now set unique sizes here

    // Dictionary to store the queues of inactive objects
    private Dictionary<Transform, Queue<Transform>> _poolDictionary = new Dictionary<Transform, Queue<Transform>>();

    // Mapping instances back to their original prefab for easy despawning
    private Dictionary<Transform, Transform> _instanceToPrefabMap = new Dictionary<Transform, Transform>();

    private void Awake()
    {
        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (Pool pool in pools)
        {
            if (pool.prefab == null) continue;

            Queue<Transform> objectPool = new Queue<Transform>();

            for (int i = 0; i < pool.size; i++)
            {
                Transform obj = Instantiate(pool.prefab);
                obj.gameObject.SetActive(false);
                obj.SetParent(this.transform);

                objectPool.Enqueue(obj);

                // Track which prefab template this specific instance belongs to
                _instanceToPrefabMap[obj] = pool.prefab;
            }

            _poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    /// <summary>
    /// Spawns the specified prefab at the given position and rotation.
    /// </summary>
    public Transform Spawn(Transform prefab, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"Pool for {prefab.name} hasn't been initialized!");
            return null;
        }

        Transform objectToSpawn;

        if (_poolDictionary[prefab].Count > 0)
        {
            objectToSpawn = _poolDictionary[prefab].Dequeue();
        }
        else
        {
            // If the pool is empty, create a new one to prevent errors (Dynamic Expansion)
            objectToSpawn = Instantiate(prefab);
            _instanceToPrefabMap[objectToSpawn] = prefab;
        }

        objectToSpawn.position = position;
        objectToSpawn.rotation = rotation;
        objectToSpawn.gameObject.SetActive(true);

        return objectToSpawn;
    }

    /// <summary>
    /// Returns the instance to its correct pool.
    /// </summary>
    /// <param name="instance">The object currently in the scene to be deactivated.</param>
    public void Despawn(Transform instance)
    {
        if (!_instanceToPrefabMap.ContainsKey(instance))
        {
            Debug.LogError($"{instance.name} was not spawned via the ObjectPooler!");
            Destroy(instance.gameObject); // Fallback to avoid memory leaks
            return;
        }

        Transform prefabKey = _instanceToPrefabMap[instance];

        instance.gameObject.SetActive(false);
        instance.SetParent(this.transform);
        _poolDictionary[prefabKey].Enqueue(instance);
    }
}