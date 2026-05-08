using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 10;
    public int columns = 10;
    [SerializeField] private float spacing = 1.4f; // Gap between the 1x1 cubes
    [SerializeField] private GameObject targetCubePrefab;

    [Header("Dynamic Data (Assigned by Level Manager")]
    public string[] layout;
    public Material[] availableColors;

    // This dictionary tracks exactly how many cubes if each color exist
    // We will later use this data to assign the exact number of shots to our characters.
    public Dictionary<Material, int> colorCounts = new Dictionary<Material, int>();

    public void GenerateGrid()
    {
        colorCounts.Clear();

        // 1. Initialize the dictionary to 0 for all colors
        foreach (Material mat in availableColors)
        {
            colorCounts[mat] = 0;
        }

        int rows = layout.Length;
        int columns = layout[0].Length;

        // 2. Calculate the starting X and Y positions to center the grid in the world
        float startX = -(columns * spacing) / 2f + (spacing / 2f);
        float startY = 0f;

        // 3. Spawning Loop
        for(int x = 0; x < columns; x++)
        {
            for(int y = 0; y < rows; y++)
            {
                char tileChar = layout[y][x];

                if(tileChar == '0' || tileChar == ' ')
                {
                    continue;
                }

                CubeType spawnType = CubeType.Normal;

                int colorIndex = -1;

                if (tileChar >= '1' && tileChar <= '9')
                {
                    colorIndex = tileChar - '1';
                }
                else if (tileChar >= 'A' && tileChar <= 'E')
                {
                    spawnType = CubeType.Armored;
                    colorIndex = tileChar - 'A';
                }
                else if (tileChar == 'X')
                {
                    spawnType = CubeType.bomb;
                }

                // Determing the spawn position
                Vector3 spawnPosition = new Vector3(startX + (x * spacing), startY + ((rows - y -1) * spacing), 0);

                // Instantiate and organize
                GameObject newCube = Instantiate(targetCubePrefab, spawnPosition, Quaternion.identity);
                newCube.transform.parent = this.transform;
                newCube.name = $"Cube_{x}_{y}";
                newCube.tag = "TargetCube";

                CubeBehaviour cb = newCube.GetComponent<CubeBehaviour>();
                Material chosenMat = null;

                if (spawnType == CubeType.bomb)
                {
                    cb.Initialize(spawnType, null);
                }
                else if(colorIndex >= 0 && colorIndex < availableColors.Length)
                {

                    // Assign a random color from the available colors
                    chosenMat = availableColors[colorIndex];
                    newCube.GetComponent<Renderer>().material = chosenMat;
                    cb.Initialize(spawnType, chosenMat);

                    if (spawnType == CubeType.Armored)
                    {
                        colorCounts[chosenMat] += 2;
                    }
                    else
                    {
                        colorCounts[chosenMat]+=1;
                    }
                }
            }
        }

        // 4. Debug Log the color counts
        foreach(var kvp in colorCounts)
        {
            Debug.Log($"Logic Check - Color: {kvp.Key.name}, Count: {kvp.Value}");
        }
    }
}
