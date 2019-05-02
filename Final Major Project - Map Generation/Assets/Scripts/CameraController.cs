using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    public float FOVMin,FOVMax;
    public float FOVChangeAmount;

    private int screenWidth, screenHeight;
    public int boundary;
    public float speed;
    private Vector3 modifyPosition;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;
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

        modifyPosition = Vector3.zero;
        if (Input.mousePosition.x > screenWidth - boundary)
        {
            modifyPosition.x = speed * Time.deltaTime;
        }
        else if (Input.mousePosition.x < boundary)
        {
            modifyPosition.x = -(speed * Time.deltaTime);
        }

        if (Input.mousePosition.y > screenHeight - boundary)
        {
            modifyPosition.z = speed * Time.deltaTime;
        }
        else if (Input.mousePosition.y < boundary)
        {
            modifyPosition.z = -(speed * Time.deltaTime);
        }

        gameObject.transform.position += modifyPosition;

    }

    void FOVChange(float modifier)
    {
        if (camera.fieldOfView + modifier <= FOVMax && camera.fieldOfView + modifier >= FOVMin)
        {
            camera.fieldOfView += modifier;
        }
    }
}
