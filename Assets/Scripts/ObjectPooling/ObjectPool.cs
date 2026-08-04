using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject prefab;
    private Transform parent;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public ObjectPool(GameObject prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        for(int i=0; i<initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab, parent);

            SetUpPooledObject(obj);

            obj.SetActive(false);

            availableObjects.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        // If we don't have any available objects,
        // create a new one and add it to the Pool
        if(availableObjects.Count == 0)
        {
            GameObject obj = Object.Instantiate(prefab, parent);

            SetUpPooledObject(obj);
            
            obj.SetActive(false);

            availableObjects.Enqueue(obj);
        }

        // Take the first available object
        GameObject pooledObject = availableObjects.Dequeue();

        // Activate it before returning
        pooledObject.SetActive(true);

        return pooledObject;
    }

    public void ReturnObject(GameObject obj)
    {
        if(obj == null)
        {
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(parent);
        availableObjects.Enqueue(obj);
    }

    private void SetUpPooledObject(GameObject obj)
    {
        PooledObject pooledObject = obj.GetComponent<PooledObject>();

        if(pooledObject == null)
        {
            pooledObject = obj.AddComponent<PooledObject>();
        }

        pooledObject.SetPool(this);
    }
}
