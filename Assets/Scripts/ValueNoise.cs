using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ValueNoise : MonoBehaviour
{
    public float scale = 5f;
    public int seed = 1000;

    public UnityEngine.UI.Slider SeedSlider;
    public UnityEngine.UI.Slider ScaleSlider;

    public TextMeshProUGUI SeedLabel;
    public TextMeshProUGUI ScaleLabel;

    private MeshGen meshGen;
    public TMP_Dropdown SceneSelector;

    void Start()
    {
        // Set up dropdown
        SetUpDropdown.SetUp(SceneSelector);

        ScaleSlider.value = scale;
        SeedSlider.value = seed;

        UpdateLabels();

        ScaleSlider.onValueChanged.AddListener(OnScaleChanged);
        SeedSlider.onValueChanged.AddListener(OnSeedChanged);

        meshGen = GetComponent<MeshGen>();
        GenMap();
    }

    public void GenMap()
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetSeed(seed);
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

    void UpdateLabels()
    {
        ScaleLabel.text = scale.ToString("#.##");
        SeedLabel.text = seed.ToString("#");
    }

    void OnScaleChanged(float value)
    {
        scale = value;
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