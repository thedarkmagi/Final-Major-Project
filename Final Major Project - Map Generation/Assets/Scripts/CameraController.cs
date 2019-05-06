using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    private MouseInput mouseInput;
    public float FOVMin,FOVMax;
    public float FOVChangeAmount;

    private int screenWidth, screenHeight;
    public float minBoundary, maxBoundary;
    public float speed;
    private Vector3 modifyPosition;
    // Start is called before the first frame update
    void Start()
    {
        mouseInput = GetComponent<MouseInput>();
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
            FOVChange(-FOVChangeAmount);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            FOVChange(FOVChangeAmount);
        }

        modifyPosition = Vector3.zero;
        if (Input.mousePosition.x > screenWidth * maxBoundary)
        {
            modifyPosition.x = speed * Time.deltaTime;
        }
        else if (Input.mousePosition.x < minBoundary * screenWidth)
        {
            modifyPosition.x = -(speed * Time.deltaTime);
        }

        if (Input.mousePosition.y > screenHeight * maxBoundary)
        {
            modifyPosition.z = speed * Time.deltaTime;
        }
        else if (Input.mousePosition.y < minBoundary * screenHeight )
        {
            modifyPosition.z = -(speed * Time.deltaTime);
        }

        gameObject.transform.position += modifyPosition;
        if(Input.mouseScrollDelta != Vector2.zero)
        {
            updateScales();
        }
    }

    void FOVChange(float modifier)
    {
        if (camera.fieldOfView + modifier <= FOVMax && camera.fieldOfView + modifier >= FOVMin)
        {
            camera.fieldOfView += modifier;
        }
    }

    float normalise(float number, float scaledMin=0.1f, float scaledMax =2)
    {
        float result = (scaledMax - scaledMin) * (number - FOVMin) / (FOVMax - FOVMin) + scaledMin;
        return result;
    }

    void updateScales()
    {
        float scaleMod = normalise(camera.fieldOfView);
        for (int i = 0; i < mouseInput.allButtons.Count; i++)
        {
            mouseInput.allButtons[i].updateScale(scaleMod);
        }
    }
}
