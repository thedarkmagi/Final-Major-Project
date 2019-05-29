using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera localCamera;
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
        localCamera = GetComponent<Camera>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    //+z is up +x is right 
    //field of view to zoom
    //UI scales depending on field of view. 
    // Update is called once per frame
    void Update()
    {
        #region Zooming in and out with scroll wheel
        if (Input.mouseScrollDelta.y > 0)
        {
            FOVChange(-FOVChangeAmount);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            FOVChange(FOVChangeAmount);
        }
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            updateScales();
        }
        #endregion
        #region Screen Scrolling
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
        #endregion
        
    }
    //This is used for zooming in
    void FOVChange(float modifier)
    {
        if (localCamera.fieldOfView + modifier <= FOVMax && localCamera.fieldOfView + modifier >= FOVMin)
        {
            localCamera.fieldOfView += modifier;
        }
    }

    float normalise(float number, float scaledMin=0.1f, float scaledMax =2)
    {
        float result = (scaledMax - scaledMin) * (number - FOVMin) / (FOVMax - FOVMin) + scaledMin;
        return result;
    }
    //update UI scales to ensure it can be seen
    void updateScales()
    {
        float scaleMod = normalise(localCamera.fieldOfView);
        for (int i = 0; i < mouseInput.allButtons.Count; i++)
        {
            mouseInput.allButtons[i].updateScale(scaleMod);
        }
    }
}
