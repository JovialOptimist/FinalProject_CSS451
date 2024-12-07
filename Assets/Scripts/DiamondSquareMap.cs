using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class DiamondSquareMap : MonoBehaviour
{
    public int terrainSize = 128;
    public float seed = 1000;
    public float roughness = 500;
    private MeshGen meshGen;

    public UnityEngine.UI.Slider TerrainSizeSlider;
    public UnityEngine.UI.Slider RoughnessSlider;
    public UnityEngine.UI.Slider SeedSlider;

    public TextMeshProUGUI TerrainSizeLabel;
    public TextMeshProUGUI RoughnessLabel;
    public TextMeshProUGUI SeedLabel;

    void Start()
    {
        meshGen = GetComponent<MeshGen>();

        // Set initial slider values
        TerrainSizeSlider.value = Mathf.Log(terrainSize, 2) + 1;
        RoughnessSlider.value = roughness;
        SeedSlider.value = seed;

        TerrainSizeSlider.onValueChanged.AddListener(OnTerrainSizeChanged);
        RoughnessSlider.onValueChanged.AddListener(OnRoughnessChanged);
        SeedSlider.onValueChanged.AddListener(OnSeedChanged);

        GenMap();
    }

    public void GenMap()
    {
        float[][] noiseMap = new float[meshGen.width + 1][];
        var diamondSquare = DiamondSquare();
        
        for (int i = 0; i < meshGen.width + 1; i++)
        {
            noiseMap[i] = new float[meshGen.depth + 1];
        }
        for (int z = 0; z <= meshGen.depth; z++)
        {
            for (int x = 0; x <= meshGen.width; x++)
            {
                // find point given x, z, scale, width, depth, heightMultiplier
                noiseMap[x][z] = diamondSquare[x, z];
            }
        }
        meshGen.GenerateMesh(noiseMap);
    }

    // https://www.youtube.com/watch?v=4GuAV1PnurU
    // great explanation provided by https://stackoverflow.com/questions/2755750/diamond-square-algorithm?newreg=ee2a40d2fe9f49b9b938151e933860d2
    private float[,] DiamondSquare()
    {
        float[,] map = new float[terrainSize + 1, terrainSize + 1];
        Random r = new Random();

        var h = roughness;
        // map out four corners
        var size = terrainSize + 1;
        map[0, 0] = seed;
        map[0, terrainSize] = seed;
        map[terrainSize, 0] =seed;
        map[terrainSize, terrainSize] = seed;
        
        for (int sideLength = size - 1; sideLength >= 2; sideLength /= 2, h /= 2)
        {
            SquareStep(map, size, sideLength, h, r);
            DiamondStep(map, size, sideLength, h, r);
        }
        
        // normalize the final map
        float max = map.Cast<float>().Max();
        float min = map.Cast<float>().Min();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                map[x, y] = (map[x, y] - min) / (max - min);
            }
        }

        return map;
    }

    private void SquareStep(float[,] map, int size, int sideLength, float h, Random r)
    {
        int halfSide = sideLength / 2;

        for (int x = 0; x < size-1; x+= sideLength)
        {
            for (int y = 0; y < size-1; y+= sideLength)
            {
                float avg = map[x, y] + 
                            map[x + sideLength, y] + 
                            map[x, y + sideLength] +
                            map[x + sideLength, y + sideLength]; // add up all four corners for average
                avg /= 4;

                // middle is average + random offset, which is ranged using roughness
                map[x + halfSide, y + halfSide] = (float)(avg + (r.NextDouble() * 2 * h) - h);

            }
        }
    }

    private void DiamondStep(float[,] map, int size, int sideLength, float h, Random r)
    {
        int halfSide = sideLength / 2;

        for (int x = 0; x < size - 1; x += halfSide)
        {
            for (int y = (x+halfSide) % sideLength; y < size - 1; y += sideLength)
            {
                
                float avg = map[(x - halfSide + (size - 1)) % (size - 1), y] + // left of center
                            map[(x + halfSide) % (size - 1), y] + // right of center
                            map[x, (y + halfSide) % (size - 1)] + // below center
                            map[x, (y - halfSide + (size - 1)) % (size - 1)]; // above center
                avg /= 4;
                
                avg = avg + (float)((r.NextDouble() * 2 * h) - h);
                map[x, y] = avg;
                
                if (x == 0) map[size - 1, y] = avg;
                if (y == 0) map[x, size - 1] = avg;
            }
        }
    }

    void UpdateLabels()
    {
        TerrainSizeLabel.text = terrainSize.ToString("#");
        RoughnessLabel.text = roughness.ToString("#.##");
        SeedLabel.text = seed.ToString("#");
    }

    void OnTerrainSizeChanged(float value)
    {
        terrainSize = (int)Mathf.Pow(2, value); // Convert slider value back to power of 2
        UpdateLabels();
        GenMap();
    }

    void OnRoughnessChanged(float value)
    {
        roughness = value;
        UpdateLabels();
        GenMap();
    }

    void OnSeedChanged(float value)
    {
        seed = Mathf.RoundToInt(value);
        UpdateLabels();
        GenMap();
    }
}