using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using System.Linq;
public static class MeshSearching 
{
    //this was 100 earlier
    private const int mounatinYMod = 40;
    private const int slowGenerationMounatinHeight = 60;

    public static bool findVertIndexOfAdjenctVerts(Mesh mesh, int vertIndex, List<List<int>> triList, Dictionary<int, BiomeType> vertBiomes, BiomeType targetBiome, BiomeType secondTargetBiome)
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
    public static List<int> findVertIndexOfAdjenctVerts(Mesh mesh, int vertIndex)
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
    public static Dictionary<int, BiomeType> setVertBiomes(Mesh mesh, float islandHeight)
    {
        Dictionary<int, BiomeType> result = new Dictionary<int, BiomeType>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            result[i] = setBiomeType(mesh.vertices[i].y, islandHeight);

        }
        return result;
    }
    public static BiomeType setBiomeType(float height, float islandHeight)
    {
        if (height > islandHeight)
            return BiomeType.land;
        else
            return BiomeType.water;
    }


    public static bool findTransitionVerts(List<int> adjecentVertIndexs, Dictionary<int, BiomeType> vertBiomes, int vertIndex, BiomeType targetBiome, BiomeType secondTargetBiome)
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
                if (temp == secondTargetBiome)
                {
                    biomeTwo = true;
                }
            }
            if (vertBiomes[vertIndex] == secondTargetBiome)
            {
                biomeTwo = true;
                BiomeType temp = vertBiomes[adjecentVertIndexs[i]];
                if (temp == targetBiome)
                {
                    biomeOne = true;
                }
            }

            if (biomeOne && biomeTwo)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    public static int findCentreOfIslandSimple(Mesh mesh, Dictionary<int, BiomeType> vertBiomes, List<int> borderVerts, VertexConnection[] vertCons, List<List<int>> triList)
    {
        int borderVert1, borderVert2;
        borderVert1 = borderVerts[Random.Range(0, borderVerts.Count / 2)];
        borderVert2 = borderVerts[Random.Range(borderVerts.Count / 2, borderVerts.Count)];
        Vector3 midpoint = midpointFormula(mesh.vertices[borderVert1], mesh.vertices[borderVert2]);
        int selectedIndex = 0;

        List<int> allLandIndexs = new List<int>();
        for (int i = 0; i < vertBiomes.Count; i++)
        {
            if (vertBiomes[i] == BiomeType.land)
            {
                allLandIndexs.Add(i);
            }
        }
        
        selectedIndex = allLandIndexs[ Random.Range(0, allLandIndexs.Count - 1)];

        List<int> samePositionMountainVerts = findVertsOfTheSamePosition(vertCons, selectedIndex);
        for (int i = 0; i < samePositionMountainVerts.Count; i++)
        {
            mesh.colors = HelperFunctions.setVertToColor(mesh, Color.magenta, samePositionMountainVerts[i]);
        }

        mesh.vertices = updateVertPositionsFromList(samePositionMountainVerts, mesh, 0, mounatinYMod, 0);
        mesh.vertices = updateElevationSimple(mesh, vertBiomes, borderVerts, vertCons, triList, selectedIndex);

        return selectedIndex;
    }

    public static Vector3 midpointFormula(Vector3 one, Vector3 two)
    {
        Vector3 result = new Vector3(one.x + two.x / 2, one.y + two.y / 2, one.z + two.z / 2);
        return result;
    }

    public static int findCentreOfIsland(Mesh mesh, Dictionary<int, BiomeType> vertBiomes, List<int> borderVerts, VertexConnection[] vertCons, List<List<int>> triList)
    {
        Dictionary<int, float> islandVertDistancesFromBorder = new Dictionary<int, float>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (vertBiomes[i] == BiomeType.land)
            {
                islandVertDistancesFromBorder[i] = distanceBetweenBorderVertandAnyOther(mesh, i, borderVerts);
            }
        }
        int indexOfCentreVertex = 0;
        float distanceOfCurrentCentre = 0;
        foreach (var item in islandVertDistancesFromBorder)
        {
            if (item.Value > distanceOfCurrentCentre)
            {
                distanceOfCurrentCentre = item.Value;
                indexOfCentreVertex = item.Key;
            }
        }
        List<int> samePositionMountainVerts = findVertsOfTheSamePosition(vertCons, indexOfCentreVertex);
        for (int i = 0; i < samePositionMountainVerts.Count; i++)
        {
            mesh.colors = HelperFunctions.setVertToColor(mesh, Color.magenta, samePositionMountainVerts[i]);
        }
        mesh.vertices = updateVertPositionsFromList(samePositionMountainVerts, mesh, 0, slowGenerationMounatinHeight, 0);
        //Instantiate(new GameObject(), mesh.vertices[indexOfCentreVertex], Quaternion.identity);
        mesh.vertices = updateElevationSimple(mesh, vertBiomes, borderVerts, vertCons, triList, indexOfCentreVertex);
        return indexOfCentreVertex;
    }

    public static Vector3[] updateVertPositionsFromList(List<int> vertIndexs, Mesh mesh, int xMod = 0, int yMod = 0, int zMod = 0)
    {
        Vector3[] templist = mesh.vertices;
        for (int i = 0; i < vertIndexs.Count; i++)
        {
            templist[vertIndexs[i]] = new Vector3(mesh.vertices[vertIndexs[i]].x + xMod, mesh.vertices[vertIndexs[i]].y + yMod, mesh.vertices[vertIndexs[i]].z + zMod);
        }

        return templist;
    }

    public static Vector3 updateVertPositions(int vertIndex, Mesh mesh, int xMod = 0, float yMod = 0, int zMod = 0)
    {
        Vector3 temp;
        temp = new Vector3(mesh.vertices[vertIndex].x + xMod, mesh.vertices[vertIndex].y + yMod, mesh.vertices[vertIndex].z + zMod);
        return temp;
    }
    public static Vector3 setVertY(int vertIndex, Mesh mesh, float yMod = 0)
    {
        Vector3 temp;
        temp = new Vector3(mesh.vertices[vertIndex].x, yMod, mesh.vertices[vertIndex].z );
        return temp;
    }

    public static Vector3[] updateElevationSimple(Mesh mesh, Dictionary<int, BiomeType> vertBiomes, List<int> borderVerts, VertexConnection[] vertCons, List<List<int>> triList, int mountainPeakVert)
    {
        Vector3[] templist = mesh.vertices;

        float minScaledHeight = mesh.vertices[0].y;
        float maxScaledHeight = mesh.vertices[mountainPeakVert].y;
        float minDistance = 0;
        float maxDistance = float.MinValue;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (vertBiomes[i] == BiomeType.land)
            {
                float checkedDistance = distanceBetweenAnyVertandAnyOther(mesh, mesh.vertices[mountainPeakVert], i, float.MaxValue);
                if (checkedDistance > maxDistance)
                {
                    maxDistance = checkedDistance;
                }
            }
        }


        List<int> excludedVerts = new List<int>();
        for (int i = 0; i < borderVerts.Count; i++)
        {
            excludedVerts.AddRange(findVertsOfTheSamePosition(vertCons, borderVerts[i]));
        }
        excludedVerts.AddRange(findVertsOfTheSamePosition(vertCons, mountainPeakVert));
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (i != mountainPeakVert)
            {
                if (!excludedVerts.Contains(i))
                {
                    if (vertBiomes[i] == BiomeType.land)
                    {

                        float checkedDistance = distanceBetweenAnyVertandAnyOther(mesh, mesh.vertices[mountainPeakVert], i, float.MaxValue);
                        float newHeight = HelperFunctions.normalise(checkedDistance, maxDistance, minDistance, minScaledHeight, maxScaledHeight);
                        templist[i] = setVertY(i, mesh, newHeight);
                    }
                }
            }
        }
        return templist;
    }


    public static float distanceBetweenBorderVertandAnyOther(Mesh mesh, int vertIndex, List<int> borderVerts)
    {
        float result = float.MaxValue;

        for (int i = 0; i < borderVerts.Count; i++)
        {
            if (HelperFunctions.sqrDistance(mesh.vertices[vertIndex], mesh.vertices[borderVerts[i]]) < result)
            {
                result = HelperFunctions.sqrDistance(mesh.vertices[vertIndex], mesh.vertices[borderVerts[i]]);
            }
        }
        return result;
    }
    public static float distanceBetweenAnyVertandAnyOther(Mesh mesh, Vector3 point, int vertexIndex, float currentSmallest)
    {
        float result = currentSmallest;
        if (HelperFunctions.sqrDistance(point, mesh.vertices[vertexIndex]) < result)
        {
            result = HelperFunctions.sqrDistance(point, mesh.vertices[vertexIndex]);
        }
        return result;
    }

    #region helper Class from https://answers.unity.com/questions/371115/is-there-an-easy-way-to-find-connected-vertices.html 
    public class VertexConnection
    {
        public List<int> connections = new List<int>();
    }

    public static VertexConnection[] FindAllOverLappingVert(Mesh mesh)
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
    public static List<int> findVertsOfTheSamePosition(VertexConnection[] vertCons, int vertIndex)
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


    #region unused Code

    public static Vector3[] updateElevationOfMap(Mesh mesh, Dictionary<int, BiomeType> vertBiomes, List<int> borderVerts, VertexConnection[] vertCons, List<List<int>> triList, int mountainPeakVert)
    {
        Vector3[] templist = mesh.vertices;

        List<int> adjecentVertIndexs = new List<int>();
        Dictionary<int, bool> hasBeenAltered = new Dictionary<int, bool>();
        for (int k = 0; k < borderVerts.Count; k++)
        {
            float distance = HelperFunctions.sqrDistance(mesh.vertices[borderVerts[k]], mesh.vertices[mountainPeakVert]);
            for (int i = 0; i < triList.Count; i++)
            {
                for (int j = 0; j < triList[i].Count; j++)
                {
                    if (triList[i].Contains(borderVerts[k]))
                    {
                        if (triList[i][j] != borderVerts[k])
                        {
                            hasBeenAltered.Add(triList[i][j], true);
                            if (distance > HelperFunctions.sqrDistance(mesh.vertices[triList[i][j]], mesh.vertices[mountainPeakVert]))
                            {
                                adjecentVertIndexs.Add(triList[i][j]);
                                adjecentVertIndexs.AddRange(findVertsOfTheSamePosition(vertCons, triList[i][j]));
                            }
                        }
                    }
                }
            }
        }

        mesh.vertices = updateVertPositionsFromList(adjecentVertIndexs, mesh, 0, 100, 0);

        return templist;
    }

    #endregion
}
