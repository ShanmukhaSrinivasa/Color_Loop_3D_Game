using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    private QueueManager myManager;
    private CharacterShooter shooter;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float entrySpeed = 20f;
    [SerializeField] List<Transform> loopNodes = new List<Transform>();
    [SerializeField] private int currentIndexNode = 0;
    public bool isRunningLoop = false;

    [Header("Character Data")]
    public Material myColor;
    public int currentShots = 10;

    [Header("UI References")]
    [SerializeField] public TextMeshProUGUI ammoText;

    private Renderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<Renderer>();

        shooter = GetComponent<CharacterShooter>();

        shooter.Initialize(this);
    }
    void Update()
    {
        if (!UIManager.isGameActive)
        {
            return;
        }

        if(isRunningLoop && loopNodes.Count > 0)
        {
            MoveAlongPath();

            if(isRunningLoop && currentIndexNode > 0)
            {
                shooter.UpdateShooter();
            }
        }
    }

    public void ConsumeAmmo()
    {
        currentShots--;
        UpdateAmmoText();
    }

    public void FinishLap()
    {
        isRunningLoop = false;
        myManager.CharacterFinishedlap(this);
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
                FinishLap();
            }
        }
    }

    public void InitializeCharacter(Material assignedColor, int shots)
    {
        myColor = assignedColor;
        currentShots = shots;

        // Update Material to match the assigned color
        meshRenderer.material = myColor;

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

    public void UpdateAmmoText()
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

    public void SetLockState(bool isLocked)
    {
        if (ammoText != null)
        {
            Color textColor = ammoText.color;

            if (isLocked)
            {
                textColor.a = 0.3f;
            }
            else
            {

                textColor.a = 1.0f;
            }

            ammoText.color = textColor;
        }
    }
}
