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
    Vector3[] localVerts;

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
        //voronoi.Regions()[0][0].
    }

    int findVertPosInList(Vector2 vert)
    {
        for (int i = 0; i < localVerts.Length; i++)
        {
            if(vert.x == localVerts[i].x && vert.y == localVerts[i].z)
            {
                return i;
            }
        }
            return 0;
    }

    Vector3[] generateVertList(List<LineSegment> list)
    {
        localVerts = new Vector3[list.Count];
        Vector2[] UVs = new Vector2[list.Count];
        for (int i = 0; i < list.Count; i+=2)
        {
            Vector2 xy = (Vector2)list[i].p0;
            //vertices.Add(new Vector3(xy.x, 0, xy.y));
            localVerts[i] = new Vector3(xy.x, 0, xy.y);
            UVs[i] = xy;
            xy = (Vector2)list[i].p1;
            //vertices.Add(new Vector3(xy.x, 0,xy.y));
            if (i + 1 < localVerts.Length)
            {
                localVerts[i + 1] = new Vector3(xy.x, 0, xy.y);
                UVs[i + 1] = xy;

            }
        }

        return localVerts;
    }


    Vector2[] generateVertList()
    {
        int indexSize=0;
        for (int i = 0; i < voronoi.Regions().Count; i++)
        {
            foreach (var item in voronoi.Regions()[i])
            {
                indexSize++;
                //Debug.Log(indexSize);
            }
        } //voronoi.Regions()

        localVerts = new Vector3[indexSize];
        Vector2[] UVs = new Vector2[indexSize];
        indexSize = 0;
        for (int i = 0; i < voronoi.Regions().Count; i++)
        {
            foreach (var item in voronoi.Regions()[i])
            {
                Vector2 xy = item;
                Debug.Log(xy);
                localVerts[indexSize] = new Vector3(xy.x, 0, xy.y);
                UVs[indexSize] = xy;
            }
        }

        

        return UVs;
    }

    void generateMeshTest()
    {
        //Vector3[] localVerts = new Vector3[m_delaunayTriangulation.Count];
        //Vector2[] UVs = new Vector2[m_delaunayTriangulation.Count];
        //for (int i = 0; i < m_delaunayTriangulation.Count; i += 2)
        //{
        //    Vector2 xy = (Vector2)m_delaunayTriangulation[i].p0;
        //    //vertices.Add(new Vector3(xy.x, 0, xy.y));
        //    localVerts[i] = new Vector3(xy.x, 0, xy.y);
        //    UVs[i] = xy;
        //    xy = (Vector2)m_delaunayTriangulation[i].p1;
        //    //vertices.Add(new Vector3(xy.x, 0,xy.y));
        //    if (i + 1 < localVerts.Length)
        //    {
        //        localVerts[i + 1] = new Vector3(xy.x, 0, xy.y);
        //        UVs[i+1] = xy;

        //    }
        //}
        //generateVertList(voronoi.VoronoiDiagram());

        //Vector3[] localVerts = new Vector3[voronoi.VoronoiDiagram().Count];
        //Vector2[] UVs = new Vector2[voronoi.VoronoiDiagram().Count];
        //int[] tris = new int[voronoi.VoronoiDiagram().Count];
        //int triIndex = 0;
        //for (int i = 0; i < voronoi.VoronoiDiagram().Count; i += 2)
        //{
        //    Vector2 xy = (Vector2)voronoi.VoronoiDiagram()[i].p0;
        //    //vertices.Add(new Vector3(xy.x, 0, xy.y));
        //    tris[i] = triIndex;
        //    //voronoi.Region(xy);
        //    List<Vector2> nextPoint = voronoi.NeighborSitesForSite(xy);
        //    tris[i+1] = findVertPosInList(nextPoint[0]);
        //    localVerts[i] = new Vector3(xy.x, 0, xy.y);
        //    UVs[i] = xy;


        //    xy = (Vector2)voronoi.VoronoiDiagram()[i].p1;
        //    //vertices.Add(new Vector3(xy.x, 0,xy.y));
        //    if (i + 1 < localVerts.Length)
        //    {
        //        nextPoint.Clear();
        //        //voronoi.Region(xy);
        //        nextPoint = voronoi.NeighborSitesForSite(xy);
        //        tris[i + 2] = findVertPosInList(nextPoint[0]);
        //        localVerts[i + 1] = new Vector3(xy.x, 0, xy.y);
        //        UVs[i + 1] = xy;

        //    }
        //}

        //Vector3[] localVerts = new Vector3[voronoi.SiteCoords().Count];
        //Vector2[] UVs = new Vector2[voronoi.SiteCoords().Count];
        //for (int i = 0; i < voronoi.SiteCoords().Count; i ++)
        //{
        //    Vector2 xy = voronoi.SiteCoords()[i]; 
        //    //vertices.Add(new Vector3(xy.x, 0, xy.y));
        //    localVerts[i] = new Vector3(xy.x, 0, xy.y);
        //    UVs[i] = xy;
        //    //UVs[i] = new Vector2(xy.x / 100.0f, xy.y / 100.0f);
        //    //xy = voronoi.SiteCoords()[i+1];
        //    ////vertices.Add(new Vector3(xy.x, 0,xy.y));
        //    //localVerts[i + 1] = new Vector3(xy.x, 0, xy.y);
        //}



        Vector2[] UVs = generateVertList();

        meshObject = new GameObject("TerrainChunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();

        meshRenderer.material = material;

        Mesh mesh = new Mesh();

        mesh.vertices = localVerts;
        //mesh.triangles = convertTriTointArray(voronoi.GetSites());
        mesh.triangles = triArray();
        mesh.uv = UVs;// FlatShading(mesh.triangles);

        meshFilter.mesh = mesh;
    }
    int[] triArray()
    {
        int[] triArray = new int[localVerts.Length*3];
        int triangleIndex = 0;
        foreach (var item in localVerts)
        {
            triArray[triangleIndex] = triangleIndex;
            triangleIndex++;
        }

        return triArray;

    }

    int[] convertTriTointArray(SiteList triList)
    {
        //triList.Next()
        int[] triArray = new int[ triList.Count*3];
        int currentArrayIndex=0;
        int triangleIndex=0;
        for (int i = 0; i < triList.Count; i+=3)
        {

            // need a way to get a position in array for this site  as X is just fundimenally incorrect 
            triArray[currentArrayIndex] = triangleIndex;//(int)triList.SiteCoords()[i].x;
            triangleIndex++;
            if (triList.SiteCoords().Count > i + 1)
            {
                triArray[currentArrayIndex + 1] = triangleIndex;//(int)triList.SiteCoords()[i + 1].x;
                triangleIndex++;
                if (triList.SiteCoords().Count > i + 2)
                {
                    triArray[currentArrayIndex + 2] = triangleIndex;//(int)triList.SiteCoords()[i + 2].x;
                    triangleIndex++;
                }
            }
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
