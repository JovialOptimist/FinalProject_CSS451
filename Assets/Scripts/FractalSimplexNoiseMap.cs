using System;
using UnityEngine;

public class FractalSimplexNoiseMap : MonoBehaviour
{
    public int octaves = 4; 
    public float persistence = .5f;
    public float lacunarity = 3f; 
    public float scale = 5f;
    private MeshGen meshGen;

    void Start()
    {
        

        meshGen = GetComponent<MeshGen>();
        GenMap();
    }

    public void GenMap()
    {


        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFractalOctaves(octaves);
        noise.SetFractalGain(persistence);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        
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