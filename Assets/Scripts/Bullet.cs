using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform targetCube;
    [SerializeField] Material bulletColor;

    // The character will call this to launch the bullet
    public void Initialize(Transform target, Material color)
    {
        targetCube = target;
        bulletColor = color;
        GetComponent<Renderer>().material = bulletColor;

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if(trail != null)
        {
            trail.startColor = bulletColor.color;
            trail.endColor = new Color(bulletColor.color.r, bulletColor.color.g, bulletColor.color.b, 0f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        // If the target was destroyed by another bullet, destroy this one to prevent errors
        if(targetCube == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetCube.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetCube.position) < 0.1f)
        {
            CubeBehaviour cubeScript = targetCube.GetComponent<CubeBehaviour>();

            if(cubeScript != null)
            {
                cubeScript.ExplodAndDie();
            }
            else
            {
                Destroy(targetCube.gameObject);
            }

            Destroy(gameObject);
        }

    }
}
