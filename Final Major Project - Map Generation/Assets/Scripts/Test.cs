using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Geometry;
public class Test : MonoBehaviour
{
    TriangleNet.Configuration c;
    TriangleNet.Mesh m;
    TriangleNet.Voronoi.StandardVoronoi V;
    // Start is called before the first frame update
    void Start()
    {
        c = new TriangleNet.Configuration();
        m = new TriangleNet.Mesh(c);
        m.bounds = new Rectangle(50, 50, 50, 50);
       V = new TriangleNet.Voronoi.StandardVoronoi(m);
        m.MakeVertexMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
