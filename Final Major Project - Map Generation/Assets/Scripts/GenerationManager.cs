using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Topology;

public class GenerationManager : MonoBehaviour
{
    TriangleNet.Mesh mesh;
    Polygon polygon;
    public int randomPoints;
    public int xsize, ysize;
    public int trianglesInChunk;
    public Transform chunkPrefab;
    private List<float> elevations = new List<float>();
    public float persistance;
    public int octaves;
    public float frequencyBase = 2;
    private float sampleSize = 1.0f;
    private float elevationScale = 100.0f;


    // Start is called before the first frame update
    void Start()
    {
        generateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generateMesh()
    {
        polygon = new Polygon();
        for (int i = 0; i < randomPoints; i++)
        {
            polygon.Add(new Vertex(Random.Range(0.0f, xsize), Random.Range(0.0f, ysize)));
        }
        TriangleNet.Meshing.ConstraintOptions options =
            new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true };
        mesh = (TriangleNet.Mesh)polygon.Triangulate(options);


        float[] seed = new float[octaves];

        for (int i = 0; i < octaves; i++)
        {
            seed[i] = Random.Range(0.0f, 100.0f);
        }

        


        foreach (Vertex vert in mesh.Vertices)
        {
            float elevation = 0.0f;
            float amplitude = Mathf.Pow(persistance, octaves);
            float frequency = 1.0f;
            float maxVal = 0.0f;

            for (int o = 0; o < octaves; o++)
            {
                float sample = (Mathf.PerlinNoise(seed[o] + (float)vert.x * sampleSize / (float)xsize * frequency,
                    seed[o] + (float)vert.y * sampleSize / (float)ysize * frequency) - 0.5f) * amplitude;
                elevation += sample;
                maxVal += amplitude;
                amplitude /= persistance;
                frequency *= frequencyBase;
            }
            elevation = elevation / maxVal;
            elevations.Add(elevation * elevationScale);
        }


        MakeMesh();
    }

    public void MakeMesh()
    {
        //enumerator to conver triangles to array interface for indexing
        IEnumerator<TriangleNet.Topology.Triangle> triangleEnumerator = mesh.Triangles.GetEnumerator();

        // create more than one chunk if necessary
        for (int chunkStart = 0; chunkStart < mesh.Triangles.Count; chunkStart += trianglesInChunk)
        {
            // vertices in unity mesh
            List<Vector3> vertices = new List<Vector3>();

            //per-vertex normals
            List<Vector3> normals = new List<Vector3>();

            //per-vertex uvs
            List<Vector2> uvs = new List<Vector2>();

            // triangles
            List<int> triangles = new List<int>();

            //iterate over all triangles until chunk size 
            int chunkEnd = chunkStart + trianglesInChunk;
            for (int i = chunkStart; i < chunkEnd; i++)
            {
                if (!triangleEnumerator.MoveNext())
                {
                    // last triangle 
                    break;
                }

                // get current triangle 
                Triangle triangle = triangleEnumerator.Current;

                // triangles need to be wound backwards to be rightways up 
                Vector3 v0 = GetPoint3D(triangle.vertices[2].id);
                Vector3 v1 = GetPoint3D(triangle.vertices[1].id);
                Vector3 v2 = GetPoint3D(triangle.vertices[0].id);

                triangles.Add(vertices.Count);
                triangles.Add(vertices.Count + 1);
                triangles.Add(vertices.Count + 2);

                vertices.Add(v0);
                vertices.Add(v1);
                vertices.Add(v2);

                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);

                //correctly calulates the UV's based on the position of the triangle
                uvs.Add(new Vector2((float)triangle.vertices[0].x/xsize, (float)triangle.vertices[0].y / ysize));
                uvs.Add(new Vector2((float)triangle.vertices[1].x / xsize, (float)triangle.vertices[1].y / ysize));
                uvs.Add(new Vector2((float)triangle.vertices[2].x / xsize, (float)triangle.vertices[2].y / ysize));
            }

            UnityEngine.Mesh chunkMesh = new UnityEngine.Mesh();
            chunkMesh.vertices = vertices.ToArray();
            chunkMesh.uv = uvs.ToArray();
            chunkMesh.triangles = triangles.ToArray();
            chunkMesh.normals = normals.ToArray();

            Transform chunk = Instantiate<Transform>(chunkPrefab, transform.position, transform.rotation);
            chunk.GetComponent<MeshFilter>().mesh = chunkMesh;
            chunk.GetComponent<MeshCollider>().sharedMesh = chunkMesh;
            chunk.transform.parent = transform;

        }
    }

    

    //unsure If I will use this approach, seems lame
    /* Returns a point's local coordinates. */
    public Vector3 GetPoint3D(int index)
    {
        Vertex vertex = mesh.vertices[index];
        float elevation = elevations[index];
        return new Vector3((float)vertex.x, elevation, (float)vertex.y);
    }
}


public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;

    int triangleIndex;

    public MeshData(int nVerts)
    {
        vertices = new Vector3[nVerts];
        triangles = new int[nVerts * 3];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = c;
        triangleIndex += 3;
    }
}