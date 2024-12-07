using UnityEngine;

public class MeshGen : MonoBehaviour
{
    public int width = 100; // Width of the ground
    public int depth = 100; // Depth of the ground
    public float heightMultiplier = 5f;
    public float colorCutoff = .5f;
    public bool UseCutoff = true;
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
                if (UseCutoff)
                    colors[index] = vertices[index].y / heightMultiplier <= colorCutoff ? Color.Lerp(Color.black, Color.gray, vertices[index].y / heightMultiplier) : Color.white;
                else
                {
                    colors[index] = Color.Lerp(Color.black, Color.white, vertices[index].y / heightMultiplier);
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
