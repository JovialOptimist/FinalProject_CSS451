using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DLAMap : MonoBehaviour
{
    public MeshGen meshGen;
    public TMP_Dropdown SceneSelector;

    void Start()
    {
        // Set up dropdown
        SceneSelector.options.Clear();
        string currentSceneName = SceneManager.GetActiveScene().name;
        int myIndex = -1;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == currentSceneName) myIndex = i;
            SceneSelector.options.Add(new TMP_Dropdown.OptionData(sceneName));
        }
        SceneSelector.onValueChanged.AddListener(index =>
        {
            SceneManager.LoadScene(SceneSelector.options[index].text);
        });
        SceneSelector.SetValueWithoutNotify(myIndex);

        GenMap();
    }

    public void GenMap()
    {
        var dlaMap = GenerateMap(new Vector2Int(meshGen.width+1, meshGen.depth+1), 10000);
        
        float[][] noiseMap = new float[meshGen.width+1][];
        for (int i = 0; i < meshGen.width+1; i++)
        {
            noiseMap[i] = new float[meshGen.depth+1];
        }
        for (int z = 0; z <= meshGen.depth; z++)
        {
            for (int x = 0; x <= meshGen.width; x++)
            {
                // find point given x, z, scale, width, depth, heightMultiplier
                noiseMap[x][z] = dlaMap[x, z];
            }
        }
        meshGen.GenerateMesh(noiseMap);
    }


    public float[,] GenerateMap(Vector2Int size_map, int nb_steps)
    {
        float[,] grid = new float[size_map.x, size_map.y];

        // create a tracker for allocated points for the diffusion process 
        List<Vector2Int> recorded_points = new();

        // Diffusion process moves from a given point to another point in a one cell movement so we're setting the directions here as a reusable element
        Vector2Int[] directions = new Vector2Int[4] { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };

        // set a initial seed, a cluster of tiles at the center of the room 
        Vector2Int center_map = new(
            size_map.x / 2,
            size_map.y / 2
        );

        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                Vector2Int temp = center_map + new Vector2Int(i, j);
                grid[
                    temp.x,
                    temp.y
                ] = 1;
            }
        }

        // The diffusion process consists in firing from a random point in a random direction until we hit a previously allocated point
        System.Random rng = new();
        float weight = 1f;

        for (int i = 0; i < nb_steps; i++)
        {
            weight -= 1f / nb_steps;
            // choose a random point 
            Vector2Int origin_point = new(
                    rng.Next(size_map.x),
                    rng.Next(size_map.y)
            );

            // move in a random direction until we hit something, or get out of bounds
            while (true)
            {
                // choose a direction
                Vector2Int direction = directions[rng.Next(4)];

                Vector2Int new_point = origin_point + direction;

                // check if the new point is in the grid. If it isn't, go to next step
                if (new_point.x < 0 || new_point.y < 0 || new_point.x >= size_map.x || new_point.y >= size_map.y)
                {
                    break;
                }

                // check if this is not a previously visited point
                // if it is a visited point, we can add the "origin_point" to our grid 
                if (grid[new_point.x, new_point.y] > 0)
                {
                    grid[origin_point.x, origin_point.y] = weight;
                    break;
                }

                // else we update origin point and continue moving our particle  
                origin_point = new_point;
            }
        }

        return grid;
    }
    
}
