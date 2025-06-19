using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 16;

    [SerializeField] private Block[,,] blocks = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>() ?? gameObject.AddComponent<MeshCollider>();

        GenerateBlocks();
        GenerateMesh();
    }

    void GenerateBlocks()
    {
        for (int x = 0; x < CHUNK_SIZE; x++)
            for (int y = 0; y < CHUNK_SIZE; y++)
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    blocks[x, y, z] = new Block(false);
                }
    }

    void GenerateMesh()
    {
        var vertices = new System.Collections.Generic.List<Vector3>();
        var triangles = new System.Collections.Generic.List<int>();

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_SIZE; y++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    if (!blocks[x, y, z].isEmpty)
                    {
                        // Add vertices and triangles for the block
                        AddBlockMesh(x, y, z, vertices, triangles);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void AddBlockMesh(int x, int y, int z, System.Collections.Generic.List<Vector3> vertices, System.Collections.Generic.List<int> triangles)
    {
        Vector3 blockPos = new Vector3(x, y, z);

        Vector3 v0 = blockPos + new Vector3(0, 0, 0);
        Vector3 v1 = blockPos + new Vector3(1, 0, 0);
        Vector3 v2 = blockPos + new Vector3(1, 1, 0);
        Vector3 v3 = blockPos + new Vector3(0, 1, 0);
        Vector3 v4 = blockPos + new Vector3(0, 0, 1);
        Vector3 v5 = blockPos + new Vector3(1, 0, 1);
        Vector3 v6 = blockPos + new Vector3(1, 1, 1);
        Vector3 v7 = blockPos + new Vector3(0, 1, 1);

        if (IsBlockEmpty(x, y + 1, z))
        {
            AddQuad(v7, v6, v2, v3, vertices, triangles); // Top
        }
        if (IsBlockEmpty(x, y - 1, z))
        {
            AddQuad(v1, v5, v4, v0, vertices, triangles); // Bottom
        }
        if (IsBlockEmpty(x, y, z + 1))
        {
            AddQuad(v5, v6, v7, v4, vertices, triangles); // Front
        }
        if (IsBlockEmpty(x, y, z - 1))
        {
            AddQuad(v3, v2, v1, v0, vertices, triangles); // Back
        }
        if (IsBlockEmpty(x + 1, y, z))
        {
            AddQuad(v2, v6, v5, v1, vertices, triangles); // Left
        }
        if (IsBlockEmpty(x - 1, y, z))
        {
            AddQuad(v4, v7, v3, v0, vertices, triangles); // Right
        }
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, System.Collections.Generic.List<Vector3> vertices, System.Collections.Generic.List<int> triangles)
    {
        int startIndex = vertices.Count;
        vertices.AddRange(new Vector3[] { v1, v2, v3, v4 });

        triangles.AddRange(new int[] {
            startIndex + 0, startIndex + 1, startIndex + 2,
            startIndex + 0, startIndex + 2, startIndex + 3
        });
    }

    private bool IsBlockEmpty(int x, int y, int z)
    {
        if (x < 0 || x >= CHUNK_SIZE || y < 0 || y >= CHUNK_SIZE || z < 0 || z >= CHUNK_SIZE)
        {
            return true;
        }

        return blocks[x, y, z].isEmpty;
    }
}