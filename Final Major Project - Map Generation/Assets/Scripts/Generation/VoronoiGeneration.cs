using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using System.Linq;

public class VoronoiGeneration : MonoBehaviour
{
    private const float secondaryIslandSampleHeight = 0.5f;
    private const int maximumRiverIterations = 40;
    private const int riverIterationsDecrimentAmount = 5;
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
    public bool enableSlowElevationGeneration;
    public bool enableRivers;
    public bool useCustomImage;
    public Texture2D customImage;

    public GameObject riverLineRenderer;
    public bool usePerlinNoise;
    [MinMax(0,50)]
    public Vector2 nRivers;
    private bool flatIsland;

    IEnumerator delayStart()
    {
        print("Starting");
        yield return new WaitForSeconds(0);
        for (int i = 0; i < chunksPerEdge; i++)
        {
            for (int j = 0; j < chunksPerEdge; j++)
            {
                generateMesh(xsize * i, ysize * j);
            }
        }
        print("finishing");
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
        useCustomImage = settings.useCustomImage;
        enableSlowElevationGeneration = settings.useElevationSlow;
        enableRivers = settings.useRivers;
        nRivers.y = settings.nRivers;
        nRivers.x = Mathf.FloorToInt(nRivers.x);
        nRivers.y = Mathf.FloorToInt(nRivers.y);

        flatIsland = !enabledElevation;

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
        //setting up image pools to be selected from at runtime
        typesOfImages.Add(blobsOnlyMaps);
        typesOfImages.Add(genMaps);
        typesOfImages.Add(allOfTheImages);
    }


    

    void generateMesh(int xOffSet, int yOffSet)
    {
        #region Triangle.Net mesh Generation and trianglation
        elevations.Clear();
        polygon = new Polygon();
        mesh = null;
        for (int i = 0; i < randomPoints; i++)
        {
            polygon.Add(new Vertex(Random.Range(0.0f, xsize), Random.Range(0.0f, ysize)));
        }
        TriangleNet.Meshing.ConstraintOptions options =
            new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true };
        mesh = (TriangleNet.Mesh)polygon.Triangulate(options);
        boundedVoronoi = new TriangleNet.Voronoi.BoundedVoronoi(mesh);
        #endregion

        //setting up bounds to be used for generatating UVs 
        meshBounds = new Bounds(new Vector3((float)mesh.Bounds.Left - (float)mesh.Bounds.Right,
            (float)mesh.Bounds.Top - (float)mesh.Bounds.Bottom), new Vector3((float)mesh.Bounds.Width, (float)mesh.Bounds.Height));

        #region defining and sampling data from images or noise
        int randomMap = Random.Range(0, typesOfImages[selectedImagePoolIndex].Count);
        int min = 0;
        int maxW; 
        int maxH; 
        int maxX = 0;
        int maxY = 0;

        // setting width and height to be used for normalising the values to select the appropriate portion of the image
        if (useCustomImage)
        {
            circleGradient = customImage;
            maxW = circleGradient.width;
            maxH = circleGradient.height;
        }
        else
        {
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
        }
        //sampling image/noise and defining heights
        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            float sample;
            if (usePerlinNoise)
            {
                sample = Mathf.PerlinNoise((float)mesh.vertices[i].x + xOffSet, (float)mesh.vertices[i].y + yOffSet);
            }
            else
            {

                int x = Mathf.FloorToInt(HelperFunctions.normalise((float)mesh.vertices[i].x, min, xsize, min, maxW));
                int y = Mathf.FloorToInt(HelperFunctions.normalise((float)mesh.vertices[i].y, min, ysize, min, maxH));
                maxX = Mathf.Max(x, maxX);
                maxY = Mathf.Max(y, maxY);

                sample = circleGradient.GetPixel(x, y).grayscale;
            }
            sample = defineIsland(sample, mesh.vertices[i]);
            
            elevations.Add(sample);
        }
        #endregion

        MakeMesh(xOffSet, yOffSet);

    }

    float defineIslandHeights(float height, Vertex vertex)
    {
        if(vertex.biomeType == BiomeType.land)
        {
            float rand = Random.Range(1, 100);
            float sample = Mathf.PerlinNoise((float)vertex.x*rand, (float)vertex.y * rand);
            if(sample< secondaryIslandSampleHeight)
            {
                sample = 0;
            }
            height += maxMeshHeight * sample;
        }
        return height;
    }

    float defineIsland(float height, Vertex vertex)
    {
        if (height > islandHeight && !enforceWaterEdge(vertex))
        {
            height = maxMeshHeight;
            vertex.biomeType = BiomeType.land;
            height = defineIslandHeights(height, vertex);
        }
        else
        {
            height = 0;
            vertex.biomeType = BiomeType.water;
        }
        return height;
    }

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
            #region convert triangle.Net mesh into Unity Mesh
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
                Vector3 v0 = HelperFunctions.GetPoint3D(triangle.vertices[2].id, mesh, elevations);
                Vector3 v1 = HelperFunctions.GetPoint3D(triangle.vertices[1].id, mesh, elevations);
                Vector3 v2 = HelperFunctions.GetPoint3D(triangle.vertices[0].id, mesh, elevations);

                triangles.Add(vertices.Count);
                triangles.Add(vertices.Count + 1);
                triangles.Add(vertices.Count + 2);

                vertices.Add(v0);
                vertices.Add(v1);
                vertices.Add(v2);

                vertColors.Add(HelperFunctions.getVertColour(v0,maxMeshHeight));
                vertColors.Add(HelperFunctions.getVertColour(v1,maxMeshHeight));
                vertColors.Add(HelperFunctions.getVertColour(v2,maxMeshHeight));

                //custom data structure which will be used later to find adjanced vertices
                List<int> thisTri = new List<int>();
                thisTri.Add(vertices.Count);
                thisTri.Add(vertices.Count + 1);
                thisTri.Add(vertices.Count + 2);
                trisList.Add(thisTri);

                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);

                ////correctly calulates the UV's based on the position of the triangle within the mesh
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

            #endregion
            #region Mounatains and Rivers
            if (enabledElevation || enableRivers)
            {
                Dictionary<int, BiomeType> vertBiomes = new Dictionary<int, BiomeType>();
                vertBiomes = MeshSearching.setVertBiomes(chunkMesh, islandHeight);
                MeshSearching.VertexConnection[] vertCons = MeshSearching.FindAllOverLappingVert(chunkMesh);
                List<int> borderVerts = HelperFunctions.findBorderVerts(trisList, vertBiomes, chunkMesh, BiomeType.land, BiomeType.water);
                int mountainIndex = int.MaxValue;
                if (enabledElevation)
                {
                    if (enableSlowElevationGeneration)
                    {
                        mountainIndex = MeshSearching.findCentreOfIsland(chunkMesh, vertBiomes, borderVerts, vertCons, trisList);
                    }
                    else
                    {
                        int mountain = MeshSearching.findCentreOfIslandSimple(chunkMesh, vertBiomes, borderVerts, vertCons, trisList);
                    }
                }
                if(enableRivers)
                {
                    int nRiversMax = Random.Range((int)nRivers.x, (int)nRivers.y);
                    int maxIterationsOfRiverSearch = maximumRiverIterations;
                    int maxIter = maxIterationsOfRiverSearch;
                    List<int> allLandIndexs = new List<int>();
                    for (int i = 0; i < vertBiomes.Count; i++)
                    {
                        if (vertBiomes[i] == BiomeType.land)
                        {
                            allLandIndexs.Add(i);
                        }
                    }
                    for (int i = (int)nRivers.x; i < nRiversMax; i++)
                    {
                        defineRiversDownwards(chunkMesh, borderVerts, maxIterationsOfRiverSearch, mountainIndex, allLandIndexs);
                        maxIterationsOfRiverSearch -= riverIterationsDecrimentAmount;
                        if (maxIterationsOfRiverSearch <= 0)
                        {
                            maxIterationsOfRiverSearch = maxIter;
                        }
                    }
                }
            }
            #endregion
            //find tallest point so that it can be passed to the shader
            float currentMax = 0;
            for (int i = 0; i < chunkMesh.vertices.Length; i++)
            {
                if (chunkMesh.vertices[i].y > currentMax)
                {
                    currentMax = chunkMesh.vertices[i].y;
                }
            }
            Renderer ShaderRender = chunkPrefab.GetComponent<Renderer>();
            ShaderRender.sharedMaterial.SetFloat("MinHeight", 0);
            ShaderRender.sharedMaterial.SetFloat("MaxHeight", currentMax);

            //Spawning mesh into world
            GameObject rotationParent = new GameObject();
            GameObject chunk = Instantiate(chunkPrefab, new Vector3(transform.position.x + xOffSet, transform.position.y, transform.position.z + yOffSet), transform.rotation);
            chunk.GetComponent<MeshFilter>().mesh = chunkMesh;
            chunk.GetComponent<MeshCollider>().sharedMesh = chunkMesh;
            chunk.transform.parent = rotationParent.transform;
            Camera.main.gameObject.transform.position = new Vector3(chunk.transform.position.x + xsize/2 , Camera.main.transform.position.y, chunk.transform.position.z + ysize/2);
            rotationParent.transform.parent = transform;
            
        }
        
    }

   

    

    public void defineRiversDownwards(Mesh mesh, List<int> borderVerts, int maxIterations, int mountainIndex, List<int> allLandIndexs)
    {
        //randomly pick river desination
        int borderEdgeIndex = borderVerts[Random.Range(0, borderVerts.Count - 1)];
        List<int> riverVertIndex = new List<int>();

        //this is an error check to see if mountain index has been set, as I initialise it as int.MaxValue 
        //it if hasn't been selected pick a random land vert this will be used for the start position of the river
        if (mountainIndex == int.MaxValue)
        {
            int selectedLandIndex = allLandIndexs[Random.Range(0, allLandIndexs.Count)];
            riverVertIndex.Add(selectedLandIndex);
        }
        else
        {
            riverVertIndex.Add(mountainIndex);
        }

        for (int j = 0; j < maxIterations; j++)
        {
            float distance = float.MaxValue;
            int index = int.MaxValue;
            float distanceFromLastPointToTarget = HelperFunctions.sqrDistance(mesh.vertices[riverVertIndex.Last()], mesh.vertices[borderEdgeIndex]);
            for (int i = 0; i < allLandIndexs.Count; i++)
            {
                if (!riverVertIndex.Contains(allLandIndexs[i]))
                {
                    if (HelperFunctions.hasASmallerY(mesh.vertices[riverVertIndex.Last()], mesh.vertices[allLandIndexs[i]]) || flatIsland)
                    {
                        //distance between current point being checked and the last one added to the list
                        float distanceFromLastPoint = HelperFunctions.sqrDistanceWithoutY(mesh.vertices[allLandIndexs[i]], mesh.vertices[riverVertIndex.Last()]);
                        //distance between current point and destination
                        float possibleTargetDistance = HelperFunctions.sqrDistance(mesh.vertices[allLandIndexs[i]], mesh.vertices[borderEdgeIndex]);
                        
                        if (distanceFromLastPoint < distance && possibleTargetDistance < distanceFromLastPointToTarget && distanceFromLastPoint > 0)
                        {
                            distance = distanceFromLastPoint;
                            index = allLandIndexs[i];
                        }
                    }
                }
            }
            if (!riverVertIndex.Contains(index) && index != int.MaxValue) // this only checks index so duplicates could be coming from same position different vert
                riverVertIndex.Add(index);
        }


        //store the list of points
        List<Vector3> riverVertPositions = new List<Vector3>();
        for (int i = 0; i < riverVertIndex.Count; i++)
        {
            riverVertPositions.Add(mesh.vertices[riverVertIndex[i]]);
        }
        //add points to line renderer which will be the river
        GameObject lineRender = Instantiate(riverLineRenderer, riverVertPositions[0], Quaternion.identity);
        lineRender.GetComponent<LineRenderer>().positionCount = riverVertPositions.Count;
        lineRender.GetComponent<LineRenderer>().SetPositions(riverVertPositions.ToArray());
        lineRender.transform.parent = transform;
    }

   

    #region Custom Print Functions
    public void printList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            print(list[i]);
        }
    }
    public void printArray(Vector3[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            print(array[i]);
        }
    }
    public void printArrayIfY(Vector3[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if(array[i].y>0)
                print(array[i]);
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
