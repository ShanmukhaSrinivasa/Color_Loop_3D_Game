using UnityEngine;

public class CharacterShooter : MonoBehaviour
{
    private CharacterController owner;

    [Header("Shooting settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float firerate = 0.5f;

    private float nextFireTime;

    public void Initialize(CharacterController character)
    {
        owner = character;
    }

    public void UpdateShooter()
    {
        if(owner.currentShots <= 0)
        {
            return;
        }

        if(Time.time < nextFireTime)
        {
            return;
        }

        TryShoot();
    }

    void TryShoot()
    {
        // Find all active cubes in the Scene
        CubeBehaviour closestCube = null;

        float closestDistance = float.MaxValue;
        float alignmentThreshold = 0.6f;

        foreach(CubeBehaviour cube in CubeRegistry.Instance.ActiveCubes)
        {
            // ignore cubes that are already dead or shrinking
            if (cube == null || cube.health <= 0)
            {
                continue;
            }

            // Calculate how far off-center the cube is on both axes
            float diffX = Mathf.Abs(cube.transform.position.x - transform.position.x);
            float diffY = Mathf.Abs(cube.transform.position.y - transform.position.y);

            bool isAligned = false;
            float distanceToCube = 0f;

            // 2. is it in the same column
            if(diffX < alignmentThreshold)
            {
                isAligned = true;
                distanceToCube = diffY;
            }
            // 3. Or is it in the same Row?
            else if(diffY < alignmentThreshold)
            {
                isAligned = true;
                distanceToCube = diffX;
            }

            // 4. If aligned, is it the closest one we've found so far?
            if(isAligned && distanceToCube < closestDistance)
            {
                closestDistance = distanceToCube;
                closestCube = cube;
            }
        }

        if (closestCube != null)
        {
            if(closestCube.CanBeTarget() && closestCube.myColor == owner.myColor)
            {
                Shoot(closestCube);
            }
        }
    }

    void Shoot(CubeBehaviour target)
    {
        target.incomingDamage++;
        nextFireTime = Time.time + firerate;
        
        owner.ConsumeAmmo();

        // Spawn bullet
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        newBullet.GetComponent<Bullet>().Initialize(target.transform, owner.myColor);

        if(owner.currentShots <= 0)
        {
            owner.FinishLap();
        }
    }
}
