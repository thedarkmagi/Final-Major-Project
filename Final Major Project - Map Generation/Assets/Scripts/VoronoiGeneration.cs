using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Topology;

public class VoronoiGeneration : MonoBehaviour
{
    private float islandHeight = 0.5f;
    TriangleNet.Mesh mesh;
    TriangleNet.Mesh shrunkMesh;
    Polygon polygon;
    public int randomPoints;
    public int xsize, ysize;
    public int trianglesInChunk;
    public GameObject chunkPrefab;
    private List<float> elevations = new List<float>();
    private float maxMeshHeight = 10;
    public float waterBoarderPercentage;
    public float minimumHeightForLand;
    //
    private Bounds meshBounds;
    public int chunksPerEdge;
    public bool displayTrianglesGizmos;
    public bool displayVoronoiGizmos;
    private TriangleNet.Voronoi.BoundedVoronoi boundedVoronoi;

    public Texture2D circleGradient;
    public List<Texture2D> genMaps = new List<Texture2D>();
    public List<Texture2D> blobsOnlyMaps = new List<Texture2D>();
    public List<Texture2D> allOfTheImages = new List<Texture2D>();
    private List<List<Texture2D>> typesOfImages = new List<List<Texture2D>>();
    private int selectedImagePoolIndex;
    public bool useImagePool;
    public bool enabledElevation;

    public bool usePerlinNoise;
    IEnumerator delayStart()
    {
        print("Starting");
        yield return new WaitForSeconds(0);
        
        print("finishing");
        //Debug.Break();
    }

    public void SetGenerationSettings(MenuGenerationInterface.GenerationSettings settings)
    {
        randomPoints = settings.chunkSize.nPointsInputted;
        xsize = settings.chunkSize.Xsize;
        ysize = settings.chunkSize.Ysize;
        islandHeight = settings.islandThreshHold;
        useImagePool = settings.useImagePool;
        enabledElevation = settings.useElevationSystem;
        chunksPerEdge = settings.nChunks;
        selectedImagePoolIndex = settings.selectedImagePoolIndex;
        usePerlinNoise = settings.usePerlin;
    }

    public void StartGeneration()
    {
        for (int i = 0; i < chunksPerEdge; i++)
        {
            for (int j = 0; j < chunksPerEdge; j++)
            {
                generateMesh(xsize * i, ysize * j);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        typesOfImages.Add(blobsOnlyMaps);
        typesOfImages.Add(genMaps);
        typesOfImages.Add(allOfTheImages);
        //StartCoroutine(delayStart());
    }


    float normalise(float number, float min, float max, float scaledMin, float scaledMax)
    {
        float result = (scaledMax - scaledMin) * (number - min) / (max - min) + scaledMin;
        return result;
    }

    void generateMesh(int xOffSet, int yOffSet)
    {
        elevations.Clear();
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

        meshBounds = new Bounds(new Vector3((float)mesh.Bounds.Left - (float)mesh.Bounds.Right, (float)mesh.Bounds.Top - (float)mesh.Bounds.Bottom), new Vector3((float)mesh.Bounds.Width, (float)mesh.Bounds.Height));
        //print(meshBounds.extents);

        int randomMap = Random.Range(0, typesOfImages[selectedImagePoolIndex].Count);

        int min = 0;
        int maxW; 
        int maxH; 

        int maxX = 0;
        int maxY = 0;
        if (useImagePool)
        {
            circleGradient = typesOfImages[selectedImagePoolIndex][randomMap];
            maxW = typesOfImages[selectedImagePoolIndex][randomMap].width;
            maxH = typesOfImages[selectedImagePoolIndex][randomMap].height;
        }
        else
        {
            maxW = circleGradient.width;
            maxH = circleGradient.height;
        }
        

        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            float sample;
            if (usePerlinNoise)
            {
                sample = Mathf.PerlinNoise((float)mesh.vertices[i].x + xOffSet, (float)mesh.vertices[i].y + yOffSet);
            }
            else
            {

                int x = Mathf.FloorToInt(normalise((float)mesh.vertices[i].x, min, xsize, min, maxW));
                int y = Mathf.FloorToInt(normalise((float)mesh.vertices[i].y, min, ysize, min, maxH));
                maxX = Mathf.Max(x, maxX);
                maxY = Mathf.Max(y, maxY);

                sample = circleGradient.GetPixel(x, y).grayscale;
            }
            if (sample > 0)
            {
                //print("vertex: " + i + " X:" + x + " Y:" + y + " Sampled value: " + sample);
            }
            sample = defineIsland(sample, mesh.vertices[i]);

            elevations.Add(sample);
        }
        //print("Max X: " + maxX + " Max Y: " + maxY);
        //findMountainPeak(mesh.vertices);

        Renderer textureRenderer = chunkPrefab.GetComponent<Renderer>();
        textureRenderer.sharedMaterial.SetFloat("MinHeight", 0);
        textureRenderer.sharedMaterial.SetFloat("MaxHeight", maxMeshHeight);

        MakeMesh(xOffSet, yOffSet);

    }

    
    float defineIsland(float height, Vertex vertex)
    {
        if (height > islandHeight && !enforceWaterEdge(vertex))
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

    #region unused stuff that I should delete
    float defineIsland(float height)
    {
        if (height > minimumHeightForLand)
        {
            height = maxMeshHeight;
        }
        else
        {
            height = 0;
        }
        return height;
    }
    void findMountainPeak(Dictionary<int, Vertex> vertices)
    {
        //use this to work out if the vert isn't connected to current island?? 
        //if(vertices[ vertices[0].tri.Triangle.neighbors[0].tri.vertices[0].id].biomeType == BiomeType.water)
        //{

        //}
        int vertIndex = 0;
        float furthestFromCoast = 0;
        for (int land = 0; land < vertices.Count; land++)
        {
            float distanceFromCoast = float.MinValue;
            if (vertices[land].biomeType != BiomeType.water)
            {
                //isPointEnclosedInWater(vertices, land);
                for (int water = 0; water < vertices.Count; water++)
                {
                    if (vertices[water].biomeType == BiomeType.water)
                    {
                        if (distanceFromWater(vertices[land], vertices[water]) > distanceFromCoast)
                        {
                            distanceFromCoast = distanceFromWater(vertices[land], vertices[water]);
                        }
                    }
                }
                if (distanceFromCoast > furthestFromCoast)
                {
                    furthestFromCoast = distanceFromCoast;
                    vertIndex = land;
                }
            }
        }
        elevations[vertIndex] = 50;
    }
    float distanceFromWater(Vertex land, Vertex water)
    {
        Vector2 landVert = new Vector2((float)land.x, (float)land.y);
        Vector2 waterVert = new Vector2((float)water.x, (float)water.y);
        return Vector2.Distance(landVert, waterVert);
    }
    float defineFlat(float height)
    {
        return 0;
    }
    #endregion

    bool enforceWaterEdge(Vertex vert)
    {
        float leftLimits, rightLimits, topLimits, bottomLimits;
        leftLimits = meshBounds.size.x - meshBounds.size.x * waterBoarderPercentage;
        rightLimits = meshBounds.size.x * waterBoarderPercentage;
        topLimits = meshBounds.size.y - meshBounds.size.y * waterBoarderPercentage;
        bottomLimits = meshBounds.size.y * waterBoarderPercentage;
        if (vert.x < leftLimits || vert.x > rightLimits || vert.y < topLimits || vert.y > bottomLimits)
        {
            return true;
        }

        return false;
    }


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
            // triangles custom stored in a way I can search to find which verts are part 
            List<List<int>> trisList = new List<List<int>>();
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
                List<int> thisTri = new List<int>();
                thisTri.Add(vertices.Count);
                thisTri.Add(vertices.Count + 1);
                thisTri.Add(vertices.Count + 2);
                trisList.Add(thisTri);
                //this will need more work,vertices.Count + 1 need to get mesh vert form triangle.id to be passed in
                //vertColors.Add(getBiomeTyvertices.Count + 2pe(triangle.vertices[2])); 
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

            if (enabledElevation)
            {
                Dictionary<int, BiomeType> vertBiomes = new Dictionary<int, BiomeType>();
                //printBiomeDictionary(vertBiomes);
                VertexConnection[] vertCons = FindAllOverLappingVert(chunkMesh);
                //print(vertCons.Length);
                vertBiomes = setVertBiomes(chunkMesh);
                //printBiomeDictionary(vertBiomes);
                List<int> borderVerts = findBorderVerts(trisList, vertBiomes, chunkMesh, BiomeType.land, BiomeType.water);
                //printList(borderVerts);

                findCentreOfIsland(chunkMesh, vertBiomes, borderVerts, vertCons);
            }
            //GameObject chunk = Instantiate<GameObject>(chunkPrefab, transform.position, transform.rotation);
            GameObject rotationParent = new GameObject();
            GameObject chunk = Instantiate(chunkPrefab, new Vector3(transform.position.x + xOffSet, transform.position.y, transform.position.z + yOffSet), transform.rotation);
            chunk.GetComponent<MeshFilter>().mesh = chunkMesh;
            chunk.GetComponent<MeshCollider>().sharedMesh = chunkMesh;
            chunk.transform.parent = rotationParent.transform;
            int rotationMultiplier = Random.Range(0, 3);
            //rotationParent.transform.RotateAround(rotationParent,)
            //rotationParent.transform.localRotation = Quaternion.Euler(0, 90*rotationMultiplier, 0);
            Camera.main.gameObject.transform.position = new Vector3(chunk.transform.position.x + xsize/2 , Camera.main.transform.position.y, chunk.transform.position.z + ysize/2);
            rotationParent.transform.parent = transform;
            
        }
        
    }
    #region Custom Print Functions
    public void printList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            print(list[i] + "BorderVerts");
        }
    }
    void printBiomeDictionary(Dictionary<int,BiomeType> dictionary)
    {
        foreach (var item in dictionary)
        {
            print(item.Value);
        }
    }
    #endregion

    #region Mesh searching 
    bool findVertIndexOfAdjenctVerts(Mesh mesh,int vertIndex, List<List<int>> triList, Dictionary<int, BiomeType> vertBiomes, BiomeType targetBiome, BiomeType secondTargetBiome)
    {
        List<int> adjecentVertIndexs = new List<int>();
        for (int i = 0; i < triList.Count; i++)
        {
            for (int j = 0; j < triList[i].Count; j++)
            {
                if (triList[i].Contains(vertIndex))
                {
                    if (triList[i][j] != vertIndex)
                    {
                        adjecentVertIndexs.Add(triList[i][j]);
                    }
                }
            }
        }
        return findTransitionVerts(adjecentVertIndexs, vertBiomes, vertIndex, targetBiome, secondTargetBiome);
    }
    List<int> findVertIndexOfAdjenctVerts(Mesh mesh, int vertIndex)
    {
        List<int> adjecentVertIndexs = new List<int>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (mesh.vertices[i] == mesh.vertices[vertIndex])
            {
                adjecentVertIndexs.Add(i);
            }
        }
        return adjecentVertIndexs;
    }
    Dictionary<int, BiomeType> setVertBiomes(Mesh mesh)
    {
        Dictionary<int, BiomeType> result = new Dictionary<int, BiomeType>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            result[i] = setBiomeType(mesh.vertices[i].y);

        }
        return result;
    }
    BiomeType setBiomeType(float height)
    {
        if (height > islandHeight)
            return BiomeType.land;
        else
            return BiomeType.water;
    }


    bool findTransitionVerts(List<int> adjecentVertIndexs, Dictionary<int, BiomeType> vertBiomes, int vertIndex, BiomeType targetBiome, BiomeType secondTargetBiome)
    {
        bool result = false;
        bool biomeOne = false;
        bool biomeTwo = false;


        for (int i = 0; i < adjecentVertIndexs.Count; i++)
        {
            if (vertBiomes[vertIndex] == targetBiome)
            {
                biomeOne = true;
                BiomeType temp = vertBiomes[adjecentVertIndexs[i]];
                //print("tempCheck " + temp + " secondBiome target " + secondTargetBiome);
                if (temp == secondTargetBiome)
                {
                    biomeTwo = true;
                }
            }
            if (vertBiomes[vertIndex] == secondTargetBiome)
            {
                biomeTwo = true;
                BiomeType temp = vertBiomes[adjecentVertIndexs[i]];
                //print("tempCheck " + temp + " FirstBiome target " + targetBiome);
                if (temp == targetBiome)
                {
                    biomeOne = true;
                }
            }
        
            if (biomeOne && biomeTwo)
            {
                //print("TransitionFound <3");
                result = true;
                break;
            }
        }

        return result;
    }

    void findCentreOfIsland(Mesh mesh, Dictionary<int, BiomeType> vertBiomes, List<int> borderVerts, VertexConnection[] vertCons)
    {
        Dictionary<int, float> islandVertDistancesFromBorder = new Dictionary<int, float>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (vertBiomes[i] == BiomeType.land)
            {
                islandVertDistancesFromBorder[i] = distanceBetweenBorderVertandAnyOther(mesh, i, borderVerts);
            }
        }
        int indexOfCentreVertex=0;
        float distanceOfCurrentCentre=0;
        foreach (var item in islandVertDistancesFromBorder)
        {
            if (item.Value > distanceOfCurrentCentre)
            {
                distanceOfCurrentCentre = item.Value;
                indexOfCentreVertex = item.Key;
            }
        }
        mesh.vertices = updateVertPositionsFromList(findVertsOfTheSamePosition(vertCons, indexOfCentreVertex), mesh, 0,100,0);
        Instantiate(new GameObject(), mesh.vertices[indexOfCentreVertex], Quaternion.identity);
    }

    Vector3[] updateVertPositionsFromList(List<int> vertIndexs, Mesh mesh, int xMod=0, int yMod=0, int zMod =0)
    {
        Vector3[] templist = mesh.vertices;
        for (int i = 0; i < vertIndexs.Count; i++)
        {
            templist[vertIndexs[i]] = new Vector3(mesh.vertices[vertIndexs[i]].x + xMod, mesh.vertices[vertIndexs[i]].y + yMod, mesh.vertices[vertIndexs[i]].z + zMod);
        }

        return templist;
    }

    float distanceBetweenBorderVertandAnyOther(Mesh mesh, int vertIndex, List<int> borderVerts)
    {
        float result = float.MaxValue;
        
        for (int i = 0; i < borderVerts.Count; i++)
        {
            if(sqrDistance( mesh.vertices[vertIndex], mesh.vertices[borderVerts[i]])<result)
            {
                result = sqrDistance(mesh.vertices[vertIndex], mesh.vertices[borderVerts[i]]);
            }
        }
        return result;
    }
    #region helper Class from https://answers.unity.com/questions/371115/is-there-an-easy-way-to-find-connected-vertices.html 
    public class VertexConnection
    {
        public List<int> connections = new List<int>();
    }

    VertexConnection[] FindAllOverLappingVert(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        VertexConnection[] connections = new VertexConnection[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            var P1 = vertices[i];
            var VC1 = connections[i];
            for (int n = i + 1; n < vertices.Length; n++)
            {
                if (P1 == vertices[n])
                {
                    var VC2 = connections[n];
                    if (VC2 == null)
                        VC2 = connections[n] = new VertexConnection();
                    if (VC1 == null)
                        VC1 = connections[i] = new VertexConnection();
                    VC1.connections.Add(n);
                    VC2.connections.Add(i);
                }
            }
        }
        return connections;
    }
    #endregion
    List<int> findVertsOfTheSamePosition(VertexConnection[] vertCons, int vertIndex)
    {
        List<int> verts = new List<int>();
        verts.Add(vertIndex);
        if (vertCons[vertIndex].connections != null)
        {
            for (int i = 0; i < vertCons[vertIndex].connections.Count; i++)
            {
                verts.Add(vertCons[vertIndex].connections[i]);
            }
        }
        return verts;
    }

#endregion

    #region helper Functions
    float sqrDistance(Vector3 one, Vector3 two)
    {
        //float result = 0;
        float result= ((two.x-one.x) * (two.x - one.x))+ ((two.y - one.y) * (two.y - one.y))+ ((two.z - one.z) * (two.z - one.z));
        return result;
    }
    List<int> findBorderVerts(List<List<int>> triList, Dictionary<int, BiomeType> vertBiomes, Mesh mesh, BiomeType targetBiome, BiomeType secondTargetBiome)
    {
        List<int> borderVerts = new List<int>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (findVertIndexOfAdjenctVerts(mesh,i,triList, vertBiomes, targetBiome, secondTargetBiome))
            {
                //print("inside the loop " + i);
                borderVerts.Add(i);
            }
        }

        return borderVerts;
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
        if (point.y <= 0)
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
                result = Color.blue;
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

    #endregion
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
