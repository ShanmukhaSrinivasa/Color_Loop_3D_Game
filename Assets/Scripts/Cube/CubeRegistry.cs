using System.Collections.Generic;
using UnityEngine;

public class CubeRegistry : MonoBehaviour
{
    public static CubeRegistry Instance {get; private set;}

    private readonly List<CubeBehaviour> activeCubes = new();

    public IReadOnlyList<CubeBehaviour> ActiveCubes => activeCubes;

    private void Awake()
    {
        if(Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Register(CubeBehaviour cube)
    {
        if(!activeCubes.Contains(cube))
        {
            activeCubes.Add(cube);
            Debug.Log($"Registered: {cube.name} | Total Cubes: {activeCubes.Count}");
        }
    }

    public void Unregister(CubeBehaviour cube)
    {
        activeCubes.Remove(cube);
        Debug.Log($"UnRegistered: {cube.name} | Total Cubes: {activeCubes.Count}");
    }

    public void ClearCubes()
    {
        activeCubes.Clear();
    }
}
