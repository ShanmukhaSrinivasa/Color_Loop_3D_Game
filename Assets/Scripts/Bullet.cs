using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform targetCube;
    [SerializeField] Material bulletColor;

    private Renderer meshRenderer;
    private TrailRenderer trailRenderer;
    private PooledObject pooledObject;

    private void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        pooledObject = GetComponent<PooledObject>();
    }

    // The character will call this to launch the bullet
    public void Initialize(Transform target, Material color)
    {
        targetCube = target;
        bulletColor = color;
        meshRenderer.material = bulletColor;

        if(trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.startColor = bulletColor.color;
            trailRenderer.endColor = new Color(bulletColor.color.r, bulletColor.color.g, bulletColor.color.b, 0f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        // If the target was destroyed by another bullet, destroy this one to prevent errors
        if(targetCube == null)
        {
            ReturnBullet();
            return;
        }

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetCube.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetCube.position) < 0.1f)
        {
            CubeBehaviour cubeScript = targetCube.GetComponent<CubeBehaviour>();

            if(cubeScript != null)
            {
                cubeScript.TakeDamage();
            }
            else
            {
                Destroy(targetCube.gameObject);
            }

            ReturnBullet();
        }
    }

    private void ReturnBullet()
    {
        targetCube = null;

        if(trailRenderer != null)
        {
            trailRenderer.Clear();
        }

        if(pooledObject == null)
        {
            pooledObject = GetComponent<PooledObject>();
        }

        pooledObject.ReturnToPool();
    }
}
