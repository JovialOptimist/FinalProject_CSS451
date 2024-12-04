using System;
using UnityEngine;

public class FractalNoiseMap : MonoBehaviour
{
    public int octaves = 4; // Number of layers of noise
    public float persistence = .5f; // How alike close points should be
    public float lacunarity = 3f; // higher => more or bigger gaps, lower => less or smaller gaps
    public float scale = 5f; // Perlin Scale (kinda like "sharpness")
    private MeshGen meshGen;

    void Start()
    {
        meshGen = GetComponent<MeshGen>();
        GenMap();
    }

    public void GenMap()
    {
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
                noiseMap[x][z] = GenerateFractalNoise(x, z);
            }
        }
        meshGen.GenerateMesh(noiseMap);
    }

    private float GenerateFractalNoise(float x, float z)
    {
        float y = 0f; // start at zero, add procedurally
        float frequency = scale;
        float amplitude = 1f;
        float maxAmplitude = 0f;

        float weightedX = x / meshGen.width;
        float weightedZ = z / meshGen.depth;

        for (int i = 0; i < octaves; i++)
        {
            y += Mathf.PerlinNoise(weightedX * frequency, weightedZ * frequency) * amplitude;
            maxAmplitude += amplitude;

            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return y / maxAmplitude; // Normalize the result
    }
    
}
