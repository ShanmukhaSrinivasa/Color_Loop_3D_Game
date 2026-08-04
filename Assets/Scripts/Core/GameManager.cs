using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object Pools")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletPoolSzie = 20;

    void Awake()
    {
        ObjectPoolManager.Instance.CreatePool(bulletPrefab, bulletPoolSzie);
    }
}
