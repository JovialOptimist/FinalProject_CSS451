using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FractalNoiseMap : MonoBehaviour
{
    public int octaves = 4; // Number of layers of noise
    public float persistence = .5f; // How alike close points should be
    public float lacunarity = 3f; // higher => more or bigger gaps, lower => less or smaller gaps
    public float scale = 5f; // Perlin Scale (kinda like "sharpness")
    public int seed = 1000;

    public UnityEngine.UI.Slider OctaveSlider;
    public UnityEngine.UI.Slider PersistenceSlider;
    public UnityEngine.UI.Slider LacunaritySlider;
    public UnityEngine.UI.Slider ScaleSlider;
    public UnityEngine.UI.Slider SeedSlider;

    public TextMeshProUGUI OctaveLabel;
    public TextMeshProUGUI PersistenceLabel;
    public TextMeshProUGUI LacunarityLabel;
    public TextMeshProUGUI ScaleLabel;
    public TextMeshProUGUI SeedLabel;

    private MeshGen meshGen;
    public TMP_Dropdown SceneSelector;

    void Start()
    {
        // Set up dropdown
        SetUpDropdown.SetUp(SceneSelector);

        // Set initial slider values
        OctaveSlider.value = octaves;
        PersistenceSlider.value = persistence;
        LacunaritySlider.value = lacunarity;
        ScaleSlider.value = scale;
        SeedSlider.value = seed;

        UpdateLabels();

        OctaveSlider.onValueChanged.AddListener(OnOctaveChanged);
        PersistenceSlider.onValueChanged.AddListener(OnPersistenceChanged);
        LacunaritySlider.onValueChanged.AddListener(OnLacunarityChanged);
        ScaleSlider.onValueChanged.AddListener(OnScaleChanged);
        SeedSlider.onValueChanged.AddListener(OnSeedChanged);

        meshGen = GetComponent<MeshGen>();
        GenMap();
    }

    public void GenMap()
    {
        // Initialize the noise generator
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFrequency(scale);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetFractalGain(persistence);
        noise.SetFractalOctaves(octaves);
        noise.SetSeed(seed);

        float[][] noiseMap = new float[meshGen.width + 1][];
        for (int i = 0; i < meshGen.width + 1; i++)
        {
            noiseMap[i] = new float[meshGen.depth + 1];
        }
        for (int z = 0; z <= meshGen.depth; z++)
        {
            for (int x = 0; x <= meshGen.width; x++)
            {
                float weightedX = (float)x / meshGen.width;
                float weightedZ = (float)z / meshGen.depth;
                // find point given x, z, scale, width, depth, heightMultiplier
                noiseMap[x][z] = noise.GetNoise(weightedX, weightedZ);
            }
        }
        meshGen.GenerateMesh(noiseMap);
    }

    void UpdateLabels()
    {
        OctaveLabel.text = octaves.ToString("#.##");
        PersistenceLabel.text = persistence.ToString("#.##");
        LacunarityLabel.text = lacunarity.ToString("#.##");
        ScaleLabel.text = scale.ToString("#.##");
        SeedLabel.text = seed.ToString("#");
    }

    // Slider value change handlers
    void OnOctaveChanged(float value)
    {
        octaves = Mathf.RoundToInt(value);
        UpdateLabels();
        GenMap();
    }

    void OnPersistenceChanged(float value)
    {
        persistence = value;
        UpdateLabels();
        GenMap();
    }

    void OnLacunarityChanged(float value)
    {
        lacunarity = value;
        UpdateLabels();
        GenMap();
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
