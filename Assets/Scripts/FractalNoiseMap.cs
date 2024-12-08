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

    public UnityEngine.UI.Slider OctaveSlider;
    public UnityEngine.UI.Slider PersistenceSlider;
    public UnityEngine.UI.Slider LacunaritySlider;
    public UnityEngine.UI.Slider ScaleSlider;

    public TextMeshProUGUI OctaveLabel;
    public TextMeshProUGUI PersistenceLabel;
    public TextMeshProUGUI LacunarityLabel;
    public TextMeshProUGUI ScaleLabel;

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

        UpdateLabels();

        OctaveSlider.onValueChanged.AddListener(OnOctaveChanged);
        PersistenceSlider.onValueChanged.AddListener(OnPersistenceChanged);
        LacunaritySlider.onValueChanged.AddListener(OnLacunarityChanged);
        ScaleSlider.onValueChanged.AddListener(OnScaleChanged);

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

    void UpdateLabels()
    {
        OctaveLabel.text = octaves.ToString("#.##");
        PersistenceLabel.text = persistence.ToString("#.##");
        LacunarityLabel.text = lacunarity.ToString("#.##");
        ScaleLabel.text = scale.ToString("#.##");
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
}
