using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Topology;

public class VoronoiGeneration : MonoBehaviour
{
    TriangleNet.Mesh mesh;
    TriangleNet.Mesh shrunkMesh;
    Polygon polygon;
    public int randomPoints;
    public int xsize, ysize;
    public int trianglesInChunk;
    public GameObject chunkPrefab;
    private List<float> elevations = new List<float>();
    private float maxMeshHeight = 10;


    //
    private Bounds meshBounds;
    public int chunksPerEdge;
    public bool displayTrianglesGizmos;
    public bool displayVoronoiGizmos;
    private TriangleNet.Voronoi.BoundedVoronoi boundedVoronoi;
    

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < chunksPerEdge; i++)
        {
            for (int j = 0; j < chunksPerEdge; j++)
            {
                generateMesh(xsize * i, ysize * j);
            }
        }
    }


    void generateMesh(int xOffSet, int yOffSet)
    {
        polygon = new Polygon();
        mesh = null;
        for (int i = 0; i < randomPoints; i++)
        {
            //polygon.Add(new Vertex(seededRandom.Next(0, xsize), seededRandom.Next(0, ysize)));
            polygon.Add(new Vertex(Random.Range(0.0f, xsize), Random.Range(0.0f, ysize))); 
        }
        TriangleNet.Meshing.ConstraintOptions options =
            new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true };
        mesh = (TriangleNet.Mesh)polygon.Triangulate(options);
        boundedVoronoi = new TriangleNet.Voronoi.BoundedVoronoi(mesh);

        meshBounds = new Bounds(new Vector3((float)mesh.Bounds.Left- (float)mesh.Bounds.Right , (float)mesh.Bounds.Top - (float)mesh.Bounds.Bottom), new Vector3((float)mesh.Bounds.Width, (float)mesh.Bounds.Height));


        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            float sample = Mathf.PerlinNoise((float)mesh.vertices[i].x + xOffSet, (float)mesh.vertices[i].y + yOffSet);
            sample = defineIsland(sample, mesh.vertices[i]);

            elevations.Add(sample);
        }

        //foreach (Vertex vert in mesh.Vertices)
        //{
        //    float sample = Mathf.PerlinNoise((float)vert.x+xOffSet, (float)vert.y+yOffSet);
        //    sample = defineIsland(sample);
        //    //sample = defineFlat(sample);
        //    //sample = forceIslands(vert,sample);

        //    elevations.Add(sample);
        //}

        Renderer textureRenderer = chunkPrefab.GetComponent<Renderer>();
        textureRenderer.sharedMaterial.SetFloat("MinHeight", 0);
        textureRenderer.sharedMaterial.SetFloat("MaxHeight", maxMeshHeight);

        MakeMesh(xOffSet, yOffSet);

    }

    float defineIsland(float height)
    {
        if (height > 0.5f)
        {
            height = maxMeshHeight;
        }
        else
        {
            height = 0;
        }
        return height;
    }
    float defineIsland(float height, Vertex vertex)
    {
        if (height > 0.5f)
        {
            height = maxMeshHeight;
            vertex.biomeType = BiomeType.land;
        }
        else
        {
            height = 0;
            vertex.biomeType = BiomeType.water;
        }
        return height;
    }
    float defineFlat(float height)
    {
        return 0;
    }
    //float forceIslands(Vertex vert, float height)
    //{
    //    Vector3 point = new Vector3((float)vert.x, (float)vert.y,0);
    //    //contains doesn't appear to actually be a useful function. either bounds are setup improperly 
    //    //if(!meshBounds.Contains(point))
        
    //    if(!shrunkMesh.Bounds.Contains(vert.x ,vert.y))
    //    {
    //        return 0;
    //    }
    //    else
    //    {
    //       return defineIsland(height);
    //    }

    //}

    public void MakeMesh(int xOffSet, int yOffSet)
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

            //vertex colours
            List<Color> vertColors = new List<Color>();
            //Bounds bounds = new Bounds(new Vector3((float)mesh.Bounds.Left - (float)mesh.Bounds.Right, (float)mesh.Bounds.Top - (float)mesh.Bounds.Bottom), new Vector3((float)mesh.Bounds.Width, (float)mesh.Bounds.Height));
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

                vertColors.Add(getVertColour(v0));
                vertColors.Add(getVertColour(v1));
                vertColors.Add(getVertColour(v2));

                //this will need more work, need to get mesh vert form triangle.id to be passed in
                //vertColors.Add(getBiomeType(triangle.vertices[2])); 
                //vertColors.Add(getVertColour(v1));
                //vertColors.Add(getVertColour(v2));

                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);

                ////correctly calulates the UV's based on the position of the triangle
                uvs.Add(new Vector2((float)triangle.vertices[2].x / meshBounds.size.x, (float)triangle.vertices[2].y / meshBounds.size.y));
                uvs.Add(new Vector2((float)triangle.vertices[1].x / meshBounds.size.x, (float)triangle.vertices[1].y / meshBounds.size.y));
                uvs.Add(new Vector2((float)triangle.vertices[0].x / meshBounds.size.x, (float)triangle.vertices[0].y / meshBounds.size.y));
            }

            UnityEngine.Mesh chunkMesh = new UnityEngine.Mesh();
            chunkMesh.vertices = vertices.ToArray();
            chunkMesh.triangles = triangles.ToArray();

            chunkMesh.uv = uvs.ToArray();
            chunkMesh.normals = normals.ToArray();
            chunkMesh.colors = vertColors.ToArray();
            //GameObject chunk = Instantiate<GameObject>(chunkPrefab, transform.position, transform.rotation);
            GameObject chunk = Instantiate(chunkPrefab, new Vector3(transform.position.x + xOffSet, transform.position.y, transform.position.z + yOffSet), transform.rotation);
            chunk.GetComponent<MeshFilter>().mesh = chunkMesh;
            chunk.GetComponent<MeshCollider>().sharedMesh = chunkMesh;
            chunk.transform.parent = transform;
        }
    }

    /* Returns a point's local coordinates. */
    public Vector3 GetPoint3D(int index)
    {
        Vertex vertex = mesh.vertices[index];
        float elevation = elevations[index];
        return new Vector3((float)vertex.x, elevation, (float)vertex.y);
    }

    public Color getVertColour(Vector3 point)
    {
        if(point.y<=0)
        {
            return Color.blue;
        }
        else
        {
            return Color.green;
        }
    }
    public Color getBiomeType(Vertex vert)
    {
        Color result = new Color();
        switch (vert.biomeType)
        {
            case BiomeType.ocean:
                result= Color.blue;
                break;
            case BiomeType.water:
                result = Color.blue;
                break;
            case BiomeType.land:
                result = Color.green;
                break;
            case BiomeType.mountain:
                result = Color.gray;
                break;
            default:
                break;
        }
        return result;
    }
    // draw lines
    public void OnDrawGizmos()
    {
        if (mesh == null)
        {
            // We're probably in the editor
            return;
        }
        if (displayTrianglesGizmos)
        {
            Gizmos.color = Color.red;
            foreach (Edge edge in mesh.Edges)
            {
                Vertex v0 = mesh.vertices[edge.P0];
                Vertex v1 = mesh.vertices[edge.P1];
                Vector3 p0 = new Vector3((float)v0.x, 0.0f, (float)v0.y);
                Vector3 p1 = new Vector3((float)v1.x, 0.0f, (float)v1.y);
                Gizmos.DrawLine(p0, p1);
            }
        }
        if (displayVoronoiGizmos)
        {
            Gizmos.color = Color.black;
            foreach (Edge edge in boundedVoronoi.Edges)
            {
                var v0 = boundedVoronoi.Vertices[edge.P0];
                var v1 = boundedVoronoi.Vertices[edge.P1];
                Vector3 p0 = new Vector3((float)v0.x, 0.0f, (float)v0.y);
                Vector3 p1 = new Vector3((float)v1.x, 0.0f, (float)v1.y);
                Gizmos.DrawLine(p0, p1);
            }
        }
    }
}
