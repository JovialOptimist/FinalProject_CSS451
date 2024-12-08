using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FractalSimplexNoiseMap : MonoBehaviour
{
    public int octaves = 4; 
    public float persistence = .5f;
    public float lacunarity = 3f; 
    public float scale = 5f;

    public UnityEngine.UI.Slider OctaveSlider;
    public UnityEngine.UI.Slider PersistenceSlider;
    public UnityEngine.UI.Slider LacunaritySlider;
    public UnityEngine.UI.Slider ScaleSlider;

    public TextMeshProUGUI OctaveLabel;
    public TextMeshProUGUI PersistenceLabel;
    public TextMeshProUGUI LacunarityLabel;
    public TextMeshProUGUI ScaleLabel;

    public TMP_Dropdown SceneSelector;

    private MeshGen meshGen;
    

    void Start()
    {
        // Set up dropdown
        SetUpDropdown.SetUp(SceneSelector);

        meshGen = GetComponent<MeshGen>();

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