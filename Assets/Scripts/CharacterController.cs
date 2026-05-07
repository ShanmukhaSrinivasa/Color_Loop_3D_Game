using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    private QueueManager myManager;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float entrySpeed = 20f;
    [SerializeField] List<Transform> loopNodes = new List<Transform>();
    [SerializeField] private int currentIndexNode = 0;
    public bool isRunningLoop = false;

    [Header("Shooting Systems")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float firerate = 0.5f;
    [SerializeField] float nextFireTime = 0f;

    [Header("Character Data")]
    [SerializeField] Material myColor;
    public int currentShots = 10;

    [Header("UI References")]
    [SerializeField] public TextMeshProUGUI ammoText;

    void Update()
    {
        if(isRunningLoop && loopNodes.Count > 0)
        {
            MoveAlongPath();

            if(isRunningLoop && currentIndexNode > 0 && Time.time >= nextFireTime && currentShots > 0)
            {
                TryShoot();
            }
        }
    }

    void MoveAlongPath()
    {
        // Find the target Node
        Transform targetNode = loopNodes[currentIndexNode];

        float currentSpeed = (currentIndexNode == 0) ? entrySpeed : moveSpeed;

        // Move towards it smoothly
        transform.position = Vector3.MoveTowards(transform.position, targetNode.position, currentSpeed * Time.deltaTime);

        // Check if we reached the node
        if(Vector3.Distance(transform.position, targetNode.position) < 0.01f)
        {
            currentIndexNode++;

            // If we reach the end of the list, loop back to the start
            if(currentIndexNode >= loopNodes.Count)
            {
                isRunningLoop = false;
                myManager.CharacterFinishedlap(this);
            }
        }
    }

    void TryShoot()
    {
        // Find all active cubes in the Scene
        GameObject[] allCubes = GameObject.FindGameObjectsWithTag("TargetCube");
        GameObject closestCube = null;

        float closestDistance = float.MaxValue;
        float alignmentThreshold = 0.6f;

        foreach(GameObject cube in allCubes)
        {
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

        if (closestCube != null && closestCube.GetComponent<Renderer>().sharedMaterial == myColor)
        {
            Shoot(closestCube.transform);
        }
    }

    void Shoot(Transform target)
    {
        target.gameObject.tag = "Untagged";
        nextFireTime = Time.time + firerate;
        currentShots--; // reduce ammo
        UpdateAmmoText();

        // Spawn bullet
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        newBullet.GetComponent<Bullet>().Initialize(target, myColor);

        if(currentShots <= 0)
        {
            isRunningLoop = false;
            myManager.CharacterFinishedlap(this);
        }
    }

    public void InitializeCharacter(Material assignedColor, int shots)
    {
        myColor = assignedColor;
        currentShots = shots;

        // Update Material to match the assigned color
        GetComponent<Renderer>().material = myColor;

        UpdateAmmoText();

        // Start tiny, then animate up to full size!
        transform.localScale = Vector3.zero;
        StartCoroutine(SpawnScaleRoutine());
    }

    private IEnumerator SpawnScaleRoutine()
    {
        float duration = 0.25f; // how fast they pop in
        float elapsed = 0f;
        Vector3 targetScale = new Vector3(1.3f,1.3f,1.3f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // this creates a smooth ease ot effect 
            float progress = elapsed / duration;
            float easeOut = Mathf.Sin(progress * Mathf.PI*0.5f);

            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, easeOut);
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it ends perfectly at original scale
    }

    private void UpdateAmmoText()
    {
        if(ammoText != null)
        {
            ammoText.text = currentShots.ToString();
        }
    }

    public void StartLoop(List<Transform> nodes, QueueManager manager)
    {
        loopNodes = nodes;
        myManager = manager;
        currentIndexNode = 0;
        
        isRunningLoop = true;
    }
}
