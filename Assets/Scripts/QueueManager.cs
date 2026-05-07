using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("Queue Settings")]
    [SerializeField] GameObject characterPrefab;
    [SerializeField] Transform[] queueStartPoints;
    [SerializeField] float queueSpacing = 2.5f;

    [Header("Loop & Resting Rules")]
    public int maxLoopLimit = 5;
    public Transform restingAreaStart;
    [SerializeField] float restingSpacing = 2f;
    public List<CharacterController> activeInLoop = new List<CharacterController>();
    public List<CharacterController> restingLine = new List<CharacterController>();

    [Header("System Reference")]
    [SerializeField] List<Transform> mainLoopNodes;
    public GridManager gridManager;
    public UIManager uiManager;

    private Queue<CharacterController>[] queues;

    [Header("Auto-Finish System")]
    public bool isAutoFinishing = false;

    void Awake()
    {
        // initialize the 3 queues
        queues = new Queue<CharacterController>[3];

        for(int i=0; i<3; i++)
        {
            queues[i] = new Queue<CharacterController>();
        }
    }

    public void ResetAndGenerateQueue()
    {
        WipeAllCharacters();

        isAutoFinishing = false;
        Time.timeScale = 1f;

        GenerateSmartQueues();

        uiManager.UpdateConveyorLimit(0, maxLoopLimit);
    }

    public void WipeAllCharacters()
    {
        foreach (var cc in activeInLoop)
        {
            if (cc != null)
            {
                cc.isRunningLoop = false;
                Destroy(cc.gameObject);
            }
        }

        foreach (var cc in restingLine)
        {
            if (cc != null)
            {
                cc.isRunningLoop = false;
                Destroy(cc.gameObject);
            }
        }

        activeInLoop.Clear();
        restingLine.Clear();

        for (int i = 0; i < 3; i++)
        {
            while (queues[i].Count > 0)
            {
                CharacterController cc = queues[i].Dequeue();
                if (cc != null)
                {
                    cc.isRunningLoop = false;
                    Destroy(cc.gameObject);
                }
            }
            queues[i].Clear();
        }
    }

    void GenerateSmartQueues()
    {
        // 1. Create a temporary list to hold our exact required characters
        List<CharacterData> charactersToSpawn = new List<CharacterData>();

        // 2. Read the grid data and creat chunks of ammo
        foreach(var kvp in gridManager.colorCounts)
        {
            Material mat = kvp.Key;
            int totalCubes = kvp.Value;

            // Split the Total into characters wuth atleast 10 shots
            while(totalCubes >= 20)
            {
                charactersToSpawn.Add(new CharacterData(mat, 10));
                totalCubes -= 10;
            }

            // Give the remaining cubes to one last character
            if(totalCubes > 0)
            {
                // Ensures minimum 10 shots, just in case a color spanwed very few cubes
                int finalShots = totalCubes;
                charactersToSpawn.Add(new CharacterData(mat, finalShots));
            }
        }

        // 3. Shuffle the list so the colors are mixed up like ral puzzle 
        for(int i=0; i<charactersToSpawn.Count; i++)
        {
            CharacterData temp = charactersToSpawn[i];
            int randomindex = Random.Range(i, charactersToSpawn.Count);
            charactersToSpawn[i] = charactersToSpawn[randomindex];
            charactersToSpawn[randomindex] = temp;
        }

        // 4. Distribute time evenly into 3 queues
        int currentQueueIndex = 0;
        foreach(var data in charactersToSpawn)
        {
            SpawnCharacter(currentQueueIndex, data.color, data.ammo);
            currentQueueIndex++;
            if(currentQueueIndex > 2)
            {
                currentQueueIndex = 0;
            }
        }

    }

    void SpawnCharacter( int lineIndex, Material color, int ammo)
    {
        GameObject newChar = Instantiate(characterPrefab);
        CharacterController cc = newChar.GetComponent<CharacterController>();

        cc.InitializeCharacter(color, ammo);

        queues[lineIndex].Enqueue(cc);
        UpdateLiveVisuals(lineIndex);
    }

    // Moves the characters into their proper physical positions in the line 
    void UpdateLiveVisuals(int lineIndex)
    {
        int positionInLine = 0;

        foreach(CharacterController cc in queues[lineIndex])
        {
            // Calculate position
            Vector3 targetPos = queueStartPoints[lineIndex].position + new Vector3(0, -positionInLine * queueSpacing, 0);

            cc.transform.position = targetPos;
            positionInLine++;
        }
    }

    private void Update()
    {
        if(!UIManager.isGameActive)
        {
            return;
        }

        // Detect mouse click
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                // Did we click a character
                CharacterController clickedChar = hit.collider.GetComponent<CharacterController>();
                if (clickedChar != null && !clickedChar.isRunningLoop)
                {
                    if(activeInLoop.Count < maxLoopLimit)
                    {
                        TrySendToLoop(clickedChar);
                    }
                    else
                    {
                        Debug.Log("Loop is full! Wait for someone to finish");
                    }
                }
            }
        }
    }

    private void TrySendToLoop(CharacterController cc)
    {
        // 1. Check is it coming from a main queue?
        for (int i = 0; i < queues.Length; i++)
        {
            if(queues[i].Count > 0 && queues[i].Peek() == cc)
            {
                // 2. Remove them from the waiting queue
                queues[i].Dequeue();

                // 3. move everyone else forward!
                UpdateLiveVisuals(i);

                // 4. Send the clicked character into the main loop
                SendCharacter(cc);
                break;
            }
        }

        // 2. Check is it coming from a resting queue?
        if(restingLine.Contains(cc))
        {
            restingLine.Remove(cc);
            UpdateRestingVisuals();
            SendCharacter(cc);
        }

        CheckForAutoFinish();
    }

    private void SendCharacter(CharacterController cc)
    {
        activeInLoop.Add(cc);
        cc.StartLoop(mainLoopNodes, this);

        uiManager.UpdateConveyorLimit(activeInLoop.Count, maxLoopLimit);
    }

    public void CharacterFinishedlap(CharacterController cc)
    {
        activeInLoop.Remove(cc);

        uiManager.UpdateConveyorLimit(activeInLoop.Count, maxLoopLimit);

        if(cc.currentShots > 0)
        {
            restingLine.Add(cc);
            UpdateRestingVisuals();
        }
        else
        {
            Destroy(cc.gameObject);
        }

        CheckWinLoseConditions();

        CheckForAutoFinish();
    }

    private void UpdateRestingVisuals()
    {
        restingLine.RemoveAll(item => item == null);
        for(int i=0; i<restingLine.Count; i++)
        {
            Vector3 targetPos = restingAreaStart.position + new Vector3(i * restingSpacing, 0, 0);
            restingLine[i].transform.position = targetPos;
        }
    }

    private void CheckWinLoseConditions()
    {
        GameObject[] remainingCubes = GameObject.FindGameObjectsWithTag("TargetCube");

        if(remainingCubes.Length == 0)
        {
            Debug.Log("YOU WIN! LEVEL COMPLETE");
            uiManager.ShowVictoryPanel();
        }
        else if(restingLine.Count >= maxLoopLimit)
        {
            Debug.Log("GAME OVER!!");
            uiManager.ShowGameOver();
        }
    }

    public void CheckForAutoFinish()
    {
        // If we are already finishing, or if the level is over do nothing.
        if (isAutoFinishing || !UIManager.isGameActive)
        {
            return;
        }

        // 1. Count everyone left waiting in the 3 lines
        int toalWaiting = queues[0].Count + queues[1].Count + queues[2].Count;

        // 2. Count everyone in the game
        int totalCharactersLeft = toalWaiting + activeInLoop.Count + restingLine.Count;

        // 3. If there are characters waiting, And the total characters can fit in the 5 resting spots
        if (totalCharactersLeft > 0 && totalCharactersLeft <= maxLoopLimit)
        {
            Debug.Log("<color=magenta>AUTO-FINISH TRIGGERED!</color>");
            isAutoFinishing = true;

            // FAST FORWARD TIME
            Time.timeScale = 2.5f;

            // Start Launching Characters Automatically
            StartCoroutine(AutoDeployRoutine());
        }
    }

    private IEnumerator AutoDeployRoutine()
    {
        while (isAutoFinishing && UIManager.isGameActive)
        {
            bool deployedSomeone = false;

            // 1. Go through all 3 queues
            for (int i = 0; i < 3; i++)
            {
                while (queues[i].Count > 0)
                {
                    // Grab the front character
                    CharacterController cc = queues[i].Dequeue();
                    UpdateLiveVisuals(i);

                    // Send them to the loop
                    SendCharacter(cc);

                    deployedSomeone = true;

                    // Wait a tiny fraction of second so they dont clip inside each other
                    yield return new WaitForSeconds(1f);
                }
            }

            // 2. Check if anyone is in the resting line
            if (restingLine.Count > 0)
            {
                CharacterController cc = restingLine[0]; // grab the first one
                restingLine.Remove(cc); // remove them from resting
                SendCharacter(cc);
                deployedSomeone = true;
                yield return new WaitForSeconds(1f);
            }

            // 3. If nobody was in a line, it means everyone is currently running on the track!
            // we just wait a tiny bit and check again.
            if (!deployedSomeone)
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private struct CharacterData
    {
        public Material color;
        public int ammo;
        public CharacterData(Material c, int a)
        {
            color = c;
            ammo = a;
        }
    }
}
