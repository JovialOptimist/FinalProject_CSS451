using UnityEngine;

public class PerlinNoiseMap : MonoBehaviour
{
    public float scale = 5f; // Perlin Scale (kinda like "sharpness")
    public MeshGen meshGen;

    void Start()
    {
        GenMap();
    }

    public void GenMap()
    {
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
                noiseMap[x][z] = Mathf.PerlinNoise(x * scale / meshGen.width, z * scale / meshGen.depth);
            }
        }
        meshGen.GenerateMesh(noiseMap);
    }

}