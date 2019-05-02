using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    public float FOVMin,FOVMax;
    public float FOVChangeAmount;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    //+z is up +x is right 
    //field of view to zoom??? seemed like it worked 
    //UI will need scaling depending on field of view. 
    // Update is called once per frame
    void Update()
    {

        if (Input.mouseScrollDelta.y > 0)
        {
            print("Y>0");
            FOVChange(-FOVChangeAmount);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            FOVChange(FOVChangeAmount);
        }


    }

    void FOVChange(float modifier)
    {
        if (camera.fieldOfView + modifier <= FOVMax && camera.fieldOfView + modifier >= FOVMin)
        {
            camera.fieldOfView += modifier;
        }
    }
}
