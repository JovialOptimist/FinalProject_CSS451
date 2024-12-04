using UnityEngine;

public class GenerateMountains_GradientPerlin_Script : MonoBehaviour
{
    public int width = 100; // Width of the ground
    public int depth = 100; // Depth of the ground
    public float scale = 5f; // Perlin Scale (kinda like "sharpness") (small = some details, large = lots of details)
    public float heightMultiplier = 5f; // Multiplier for height
    public int octaves = 4; // Number of layers of noise
    public float persistence = 0.5f; // How alike close points should be
    public float lacunarity = 2f; // higher => more or bigger gaps, lower => less or smaller gaps

    void Start()
    {
        // Basic stuff: positioning and color
        gameObject.transform.position = Vector3.zero;
        gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        // Declare vertices and texture coordinate
        Vector3[] vertices = new Vector3[(width + 1) * (depth + 1)];
        Vector2[] uv = new Vector2[vertices.Length]; // uv are the texture coordinates

        // Find the vertices and uv using Mathf.PerlinNoise
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                int index = z * (width + 1) + x;
                // find point given x, z, scale, width, depth, heightMultiplier
                float y = GenerateFractalNoise(x, z) * heightMultiplier;
                vertices[index] = new Vector3(x, y, z);
                uv[index] = new Vector2((float)x / width, (float)z / depth);
            }
        }

        // Generate the mesh
        Mesh mesh = new()
        {
            vertices = vertices,
            uv = uv,
            triangles = FindTriangles()
        };
        mesh.RecalculateNormals();
        mesh.Optimize();

        // Add the mesh to the gameObject
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
    }

    private float GenerateFractalNoise(float x, float z)
    {
        float y = 0f; // start at zero, add procedurally
        float frequency = scale;
        float amplitude = 1f;
        float maxAmplitude = 0f;

        float weightedX = x / width;
        float weightedZ = z / depth;

        for (int i = 0; i < octaves; i++)
        {
            y += Mathf.PerlinNoise(weightedX * frequency, weightedZ * frequency) * amplitude;
            maxAmplitude += amplitude;

            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return y / maxAmplitude; // Normalize the result
    }

    private int[] FindTriangles()
    {
        int[] triangles = new int[width * depth * 6];

        int triangleIndex = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int topLeft = z * (width + 1) + x;
                int topRight = topLeft + 1;
                int bottomLeft = (z + 1) * (width + 1) + x;
                int bottomRight = bottomLeft + 1;

                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topRight;

                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = bottomRight;
            }
        }

        return triangles;
    }
}
