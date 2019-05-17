using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using System.Linq;
public static class HelperFunctions
{

    public static float normalise(float number, float min, float max, float scaledMin, float scaledMax)
    {
        float result = (scaledMax - scaledMin) * (number - min) / (max - min) + scaledMin;
        return result;
    }

    public static float sqrDistance(Vector3 One, Vector3 Two)
    {
        //float result = 0;
        Vector3 tempOne = One;
        Vector3 tempTwo = Two;

        float result = ((tempTwo.x - tempOne.x) * (tempTwo.x - tempOne.x)) + ((tempTwo.y - tempOne.y) * (tempTwo.y - tempOne.y)) + ((tempTwo.z - tempOne.z) * (tempTwo.z - tempOne.z));
        return result;
    }
    public static List<int> findBorderVerts(List<List<int>> triList, Dictionary<int, BiomeType> vertBiomes, Mesh mesh, BiomeType targetBiome, BiomeType secondTargetBiome)
    {
        List<int> borderVerts = new List<int>();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (MeshSearching.findVertIndexOfAdjenctVerts(mesh, i, triList, vertBiomes, targetBiome, secondTargetBiome))
            {
                //print("inside the loop " + i);
                borderVerts.Add(i);
            }
        }

        return borderVerts;
    }

    /* Returns a point's local coordinates. */
    public static Vector3 GetPoint3D(int index, TriangleNet.Mesh mesh, List<float> elevations)
    {
        Vertex vertex = mesh.vertices[index];
        float elevation = elevations[index];
        return new Vector3((float)vertex.x, elevation, (float)vertex.y);
    }

    public static Color getVertColour(Vector3 point, float maxMeshHeight)
    {
        if (point.y <= 0)
        {
            return Color.blue;
        }
        else if (point.y <= maxMeshHeight)
        {
            return Color.green;
        }
        else
        {
            return Color.gray;
        }
    }
    public static Color getBiomeType(Vertex vert)
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
}
