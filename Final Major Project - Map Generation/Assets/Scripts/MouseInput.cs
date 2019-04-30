using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour
{
    public enum MouseState {none,ruler,customLabel, createMVP, moveMVP};
    private MouseState mouseState;


    //Ruler Variables
    private Vector3 firstClickPos;
    private Vector3 secondPos;
    private bool firstClickHasHappened;
    private LineRenderer lineRenderer;
    private Text distanceDisplay;

    //CustomLabel variables
    public GameObject customLabelPrefab;

    //MVP token variables 
    public GameObject token;
    private GameObject selectedToken;
    // Start is called before the first frame update
    void Start()
    {
        mouseState = MouseState.none;
        // ruler setup
        firstClickHasHappened = false;
        lineRenderer = GetComponent<LineRenderer>();
        distanceDisplay = GameObject.Find("Distance").GetComponent<Text>();

        selectedToken = null;
    }

    // Update is called once per frame
    void Update()
    {
        switch (mouseState)
        {
            case MouseState.none:
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        if(hit.collider.gameObject.tag == "MVPToken")
                        {
                            print("Ham");
                            hit.collider.gameObject.GetComponent<TokenController>().ActiveButtons();
                            setMouseState(MouseState.moveMVP);
                        }
                    }
                }

                break;
            case MouseState.ruler:
                #region ruler state
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        print("first click point "+hit.point);
                        if (!firstClickHasHappened)
                        {
                            firstClickHasHappened = true;
                            firstClickPos =hit.point;
                        }
                        else
                        {

                        }
                    }
                }
                else if(firstClickHasHappened)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        secondPos = hit.point;
                        distanceDisplay.transform.position = Input.mousePosition;
                        distanceDisplay.text = Vector3.Distance(firstClickPos, secondPos).ToString();
                        print(Vector3.Distance(firstClickPos, secondPos));
                        if (firstClickPos.y>secondPos.y)
                        {
                            secondPos.y = firstClickPos.y;
                        }
                        else
                        {
                            firstClickPos.y = secondPos.y;
                        }
                        lineRenderer.SetPosition(0, firstClickPos);
                        lineRenderer.SetPosition(1, secondPos);
                    }
                }
                if(Input.GetMouseButtonDown(1))
                {
                    lineRenderer.SetPosition(0, Vector3.zero);
                    lineRenderer.SetPosition(1, Vector3.zero);
                    distanceDisplay.text = "";
                    firstClickHasHappened = false;
                    resetMouseState();
                }
                #endregion
                break;
            case MouseState.customLabel:
                // click position
                // spawn textbox there 
                //type label
                //confirm with button
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        print("first click point " + hit.point);
                        if (!firstClickHasHappened)
                        {
                            firstClickPos = hit.point;
                            GameObject CustomLabel = Instantiate(customLabelPrefab, new Vector3( hit.point.x, hit.point.y+20, hit.point.z), Quaternion.identity);
                            CustomLabel.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                            //CustomLabel.transform.LookAt(CustomLabel.transform.position-transform.position);
                            CustomLabel.transform.rotation = Quaternion.Euler(-90,-180,0);
                            //CustomLabel.transform.LookAt(transform);
                            resetMouseState();
                        }
                    }
                }
                break;
            case MouseState.createMVP:
                //click button to enter state 
                // click somewhere on map 
                // spawn token 
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        print("first click point " + hit.point);
                        if (!firstClickHasHappened)
                        {
                            firstClickPos = hit.point;
                            GameObject tokenLocal = Instantiate(token, new Vector3(hit.point.x, hit.point.y + 20, hit.point.z), Quaternion.identity);
                            tokenLocal.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                            tokenLocal.GetComponent<TokenController>().mouseInputReference = this;
                            //CustomLabel.transform.LookAt(CustomLabel.transform.position-transform.position);
                            tokenLocal.transform.rotation = Quaternion.Euler(90, 0, 0);
                            //CustomLabel.transform.LookAt(transform);
                            resetMouseState();
                        }
                    }
                }
                break;
            case MouseState.moveMVP:
                //click token to enter this state  ( use tags or something ) 
                //have options, move, edit colour, delete

                if (selectedToken!=null)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        Vector3 newLocation = new Vector3(hit.point.x, selectedToken.transform.position.y, hit.point.z);
                        selectedToken.transform.position = newLocation;
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        selectedToken = null;
                        resetMouseState();
                    }
                }
                else
                {
                    resetMouseState();
                }
                break;
            default:
                break;
        }
        
    }

    public void setSelectedToken(GameObject token)
    {
        selectedToken = token;
    }

    public void setMouseToRuler()
    {
        setMouseState(MouseState.ruler);
    }
    public void setMouseToCustomLabel()
    {
        setMouseState(MouseState.customLabel);
    }
    public void setMouseToMVPCreate()
    {
        setMouseState(MouseState.createMVP);
    }
    public void resetMouseState()
    {
        setMouseState(MouseState.none);
    }

    public void setMouseState(MouseState state)
    {
        mouseState = state;
    }
}
