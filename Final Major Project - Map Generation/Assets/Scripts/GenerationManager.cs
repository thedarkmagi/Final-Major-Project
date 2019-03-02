using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;
using Delaunay.Geo;

public class GenerationManager : MonoBehaviour
{

    public int points = 300;

    private List<Vector2> m_points;
    private float m_mapWidth = 100;
    private float m_mapHeight = 50;
    private List<LineSegment> m_edges = null;
    private List<LineSegment> m_spanningTree;
    private List<LineSegment> m_delaunayTriangulation;

    // TEST DATA
    List<Vector3> vertices = new List<Vector3>();
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    GameObject meshObject;
    public Material material;
    Delaunay.Voronoi voronoi;
    // Start is called before the first frame update
    void Start()
    {
        generateVoronoiDiagram(generateRandomPoints(points));

        generateMeshTest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Vector2> generateRandomPoints(int nPoints)
    {
        List<Vector2> newListVec2 = new List<Vector2>();
        
        for (int i = 0; i < nPoints; i++)
        {
            newListVec2.Add(new Vector2(Random.Range(0, m_mapWidth), Random.Range(0, m_mapHeight)));
        }
        return newListVec2;
    }

    void generateVoronoiDiagram(List<Vector2> points)
    {
        List<uint> colors = new List<uint>();
        for (int i = 0; i < points.Count; i++)
        {
            colors.Add(0);
        }
        voronoi = new Delaunay.Voronoi(points, colors, new Rect(0, 0, m_mapWidth, m_mapHeight));
        m_edges = voronoi.VoronoiDiagram();
        m_spanningTree = voronoi.SpanningTree(KruskalType.MINIMUM);
        m_delaunayTriangulation = voronoi.DelaunayTriangulation();
        voronoi.SiteCoords();
        
        Delaunay.Geo.Polygon polygon = new Polygon(points);
    }


    void generateMeshTest()
    {
        Vector3[] localVerts = new Vector3[m_delaunayTriangulation.Count];
        Vector2[] UVs = new Vector2[m_delaunayTriangulation.Count];
        for (int i = 0; i < m_delaunayTriangulation.Count; i += 2)
        {
            Vector2 xy = (Vector2)m_delaunayTriangulation[i].p0;
            //vertices.Add(new Vector3(xy.x, 0, xy.y));
            localVerts[i] = new Vector3(xy.x, 0, xy.y);
            UVs[i] = xy;
            xy = (Vector2)m_delaunayTriangulation[i].p1;
            //vertices.Add(new Vector3(xy.x, 0,xy.y));
            if (i + 1 < localVerts.Length)
            {
                localVerts[i + 1] = new Vector3(xy.x, 0, xy.y);
                UVs[i+1] = xy;

            }
        }
        //Vector3[] localVerts = new Vector3[voronoi.SiteCoords().Count];
        //for (int i = 0; i < m_delaunayTriangulation.Count; i += 2)
        //{
        //    Vector2 xy = (Vector2)m_delaunayTriangulation[i].p0;
        //    //vertices.Add(new Vector3(xy.x, 0, xy.y));
        //    localVerts[i] = new Vector3(xy.x, 0, xy.y);
        //    xy = (Vector2)m_delaunayTriangulation[i].p1;
        //    //vertices.Add(new Vector3(xy.x, 0,xy.y));
        //    localVerts[i + 1] = new Vector3(xy.x, 0, xy.y);
        //}

        meshObject = new GameObject("TerrainChunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();

        meshRenderer.material = material;

        Mesh mesh = new Mesh();

        mesh.vertices = localVerts;
        mesh.triangles = convertTriTointArray(voronoi.GetSites());

        mesh.uv = UVs;// FlatShading(mesh.triangles);

        meshFilter.mesh = mesh;
    }


    int[] convertTriTointArray(SiteList triList)
    {
        //triList.Next()
        int[] triArray = new int[ triList.Count*3];
        int currentArrayIndex=0;
        for (int i = 0; i < triList.Count; i+=3)
        {
            
                // need a way to get a position in array for this site  
                triArray[currentArrayIndex] = (int)triList.SiteCoords()[i].x;
                triArray[currentArrayIndex+1] = (int)triList.SiteCoords()[i+1].x;
                triArray[currentArrayIndex+2] = (int)triList.SiteCoords()[i+2].x;
                currentArrayIndex += 3;
            
            
        }


        return triArray;
    }


    // lazy code copied from prior mesh gen shit. just needed something for uv's 
    Vector2[] FlatShading(int[] triangles)
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];
        Vector2[] uvs = new Vector2[triangles.Length * 3];

        for (int i = 0; i < triangles.Length; i++)
        {
            //flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            //triangles[i] = i;
        }

        //vertices = flatShadedVertices;
        uvs = flatShadedUvs;

        return uvs;
    }
}
