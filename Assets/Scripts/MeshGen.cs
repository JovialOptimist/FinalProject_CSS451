using UnityEngine;

public class MeshGen : MonoBehaviour
{
    public int width = 100; // Width of the ground
    public int depth = 100; // Depth of the ground
    public float heightMultiplier = 5f;
    public float colorCutoff = .5f;
    public Material colorMat;
    private bool isFirstTime = true;

    public void GenerateMesh(float[][] noiseMap)
    {
        if (isFirstTime)
        {
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshCollider>();
            isFirstTime = false;
        }

        // Basic stuff: positioning and color
        gameObject.transform.position = Vector3.zero;
        gameObject.GetComponent<MeshRenderer>().material = colorMat;

        // Declare vertices and texture coordinate
        Vector3[] vertices = new Vector3[(width + 1) * (depth + 1)];
        Vector2[] uv = new Vector2[vertices.Length]; // uv are the texture coordinates
        Color[] colors = new Color[vertices.Length];

        // Find the vertices and uv using Mathf.PerlinNoise
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                int index = z * (width + 1) + x;
                // find point given x, z, scale, width, depth, heightMultiplier
                float y = noiseMap[x][z] * heightMultiplier;
                vertices[index] = new Vector3(x, y, z);
                uv[index] = new Vector2((float)x / width, (float)z / depth);
                // Generate color gradient for mountain
                float heightPercentage = vertices[index].y / heightMultiplier;

                // Corrected colors with normalized RGB values
                Color darkBrown = ColorHelper(64, 36, 21);
                Color lightBrown = ColorHelper(116, 66, 38);
                Color green = ColorHelper(16, 116, 26);

                float bottom = 0.0f;
                float cutoffOne = 0.2f;
                float cutoffTwo = 0.7f;
                float topCutoff = 0.9f;

                if (heightPercentage < cutoffOne)
                {
                    colors[index] = Color.Lerp(darkBrown, lightBrown, heightPercentage / (cutoffOne - bottom));
                }
                else if (heightPercentage < cutoffTwo)
                {
                    colors[index] = Color.Lerp(lightBrown, green, (heightPercentage - cutoffOne) / (cutoffTwo - cutoffOne));
                }
                else if (heightPercentage < topCutoff)
                {
                    colors[index] = Color.Lerp(green, Color.white, (heightPercentage - cutoffTwo) / (topCutoff - cutoffTwo));
                }
                else
                {
                    colors[index] = Color.white;
                }
            }
        }

        // Generate the mesh
        Mesh mesh = new()
        {
            vertices = vertices,
            uv = uv,
            triangles = FindTriangles(),
            colors = colors
        };
        mesh.RecalculateNormals();
        mesh.Optimize();

        // Add the mesh to the gameObject
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private UnityEngine.Color ColorHelper(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
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
