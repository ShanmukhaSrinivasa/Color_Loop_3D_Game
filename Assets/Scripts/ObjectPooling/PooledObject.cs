using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private ObjectPool owningPool;

    public void SetPool(ObjectPool pool)
    {
        owningPool = pool;
    }

    public void ReturnToPool()
    {
        if(owningPool != null)
        {
            owningPool.ReturnObject(gameObject);
        }
    }
}
