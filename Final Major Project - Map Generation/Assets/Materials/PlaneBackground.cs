using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBackground : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        loopThroughMeshSettingVertColours();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void loopThroughMeshSettingVertColours()
    {
        var mesh = GetComponent<MeshFilter>();
        List<Color> vertColours= new List<Color>();
        for (int i = 0; i < mesh.mesh.vertices.Length; i++)
        {
            vertColours.Add(Color.blue);
        }

        mesh.mesh.colors = vertColours.ToArray();
    }
}
