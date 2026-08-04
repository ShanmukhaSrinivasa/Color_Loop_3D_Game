using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance {get; private set;}

    private Dictionary<GameObject, ObjectPool> pools = new();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CreatePool(GameObject prefab, int initialSize)
    {
        // Don't create the same pool twice
        if(pools.ContainsKey(prefab))
        {
            return;
        }

        GameObject poolParent = new GameObject(prefab.name + " Pool");
        poolParent.transform.SetParent(transform);

        ObjectPool pool = new ObjectPool(prefab, initialSize, poolParent.transform);

        pools.Add(prefab, pool);
    }

    public GameObject GetObject(GameObject prefab)
    {
        if(!pools.TryGetValue(prefab, out ObjectPool pool))
        {
            Debug.LogError($"No pool exists for {prefab.name}");
            return null;
        }

        return pool.GetObject();
    }
}
