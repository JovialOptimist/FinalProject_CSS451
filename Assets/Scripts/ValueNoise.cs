using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ValueNoise : MonoBehaviour
{
    public float scale = 5f;
    private MeshGen meshGen;

    public TMP_Dropdown SceneSelector;

    void Start()
    {
        meshGen = GetComponent<MeshGen>();

        // Set up dropdown
        SceneSelector.options.Clear();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            SceneSelector.options.Add(new TMP_Dropdown.OptionData(sceneName));
        }
        SceneSelector.onValueChanged.AddListener(index =>
        {
            SceneManager.LoadScene(SceneSelector.options[index].text);
        });

        GenMap();
    }

    public void GenMap()
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.Value);
        
        float[][] noiseMap = new float[meshGen.width + 1][];
        for (int i = 0; i < meshGen.width + 1; i++)
        {
            noiseMap[i] = new float[meshGen.depth + 1];
        }
        for (int z = 0; z <= meshGen.depth; z++)
        {
            for (int x = 0; x <= meshGen.width; x++)
            {
                // find point given x, z, scale, width, depth, heightMultiplier
                noiseMap[x][z] = (noise.GetNoise(x * scale, z * scale) + 1)/2;
            }
        }
        meshGen.GenerateMesh(noiseMap);
    }

    
    
}