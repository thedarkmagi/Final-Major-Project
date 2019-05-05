using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour
{
    public enum MouseState {none,ruler,customLabel, createMVP, moveMVP};
    private MouseState mouseState;


    public List<ScaleIcon> allButtons = new List<ScaleIcon>();

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

    //radial variables
    private List<GameObject> buttonList = new List<GameObject>();
    public GameObject buttonPrefab;
    private bool open;

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
                        if (hit.collider.gameObject.tag == "MVPToken")
                        {
                            hit.collider.gameObject.GetComponent<TokenController>().ActiveButtons();
                            //spawnButtons(hit);
                        }
                        if (hit.collider.gameObject.tag == "PinOnly")
                        {
                            hit.collider.gameObject.GetComponent<ToggleOnOff>().showLabelOnTimer();
                            //spawnButtons(hit);
                        }
                    }
                }
                if (true) // this is me cheating. this script needs refactioring for a more efficent raycast checking system. 
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, 1000.0f))
                    {
                        if (hit.collider.gameObject.tag == "PinOnly")
                        {
                            hit.collider.gameObject.GetComponent<ToggleOnOff>().showLabelOnTimer();
                            
                            //spawnButtons(hit);
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
                        int distance =Mathf.FloorToInt( Vector3.Distance(firstClickPos, secondPos));
                        distanceDisplay.text = distance.ToString()+"miles";
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
                            allButtons.Add(CustomLabel.GetComponentInChildren<ScaleIcon>());
                            //allButtons.Add(CustomLabel.GetComponent<ScaleIcon>());
                            //CustomLabel.transform.LookAt(CustomLabel.transform.position-transform.position);
                            CustomLabel.transform.localRotation= Quaternion.Euler(90,0,0);
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
                            allButtons.Add(tokenLocal.GetComponent<ScaleIcon>());
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
    #region mouse state functions for buttons
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
    #endregion
    public void deleteFromScaleableList(ScaleIcon icon)
    {
        if(allButtons.Contains(icon))
        {
            allButtons.Remove(icon);
        }
    }

#region Radial button code unsure if I want to use it
    public void spawnButtons(RaycastHit hit)
    {
        //hit.collider.gameObject.;
        //print(hit.point);
        //if (hit.collider.gameObject.GetComponent<PlotType>())
        //{
        //    if (hit.collider.gameObject.GetComponent<PlotType>().plotType == PlotType.type.available)
        //    {
        // print("ham");

        Canvas canvas = hit.collider.GetComponentInChildren<Canvas>();
        float distance = 50;
        open = true;
        //waitAFrame = true;
        //var foo = System.Enum.GetValues(typeof(PlotType.type)).Cast<PlotType.type>().Max();
        //var enumLength = System.Enum.GetNames(typeof(PlotType.type)).Length;
        var enumLength = 4;
        //print(foo);
        for (int i = 1; i < enumLength; i++)
        {

            float angle = (2 * Mathf.PI / (enumLength - 1)) * i - 1;
            float xPos = Mathf.Sin(angle);
            float yPos = Mathf.Cos(angle);
            //make the thing
            GameObject newButton = Instantiate(buttonPrefab, new Vector3(hit.point.x, hit.point.y + 20, hit.point.z), Quaternion.identity) as GameObject;
            newButton.transform.rotation = Quaternion.Euler(-90, 0, 0);
            newButton.transform.SetParent(canvas.transform);
            //newButton.transform.position = Input.mousePosition;
            newButton.transform.localPosition = new Vector3((xPos * distance), (yPos * distance));

            //TEMP DISABLE FOR EASIER DEBUG
            //newButton.GetComponent<Image>().sprite = BuildingInfo[i - 1].GetComponent<Building>().icon;


            //newButton.GetComponent<TestClick>().inputSetting = i;
            //newButton.GetComponent<TestClick>().owner(gameObject);

            buttonList.Add(newButton);

         }
        //create text box for hover text to be displayed with.
        //GameObject newText = Instantiate(TextBox, canvas.transform);
        //newText.GetComponent<Text>().text = "";
        //newText.transform.localPosition = new Vector3(Input.mousePosition.x - (Screen.width / 2), Input.mousePosition.y - (Screen.height / 2));
        ////float angle2 = (2 * Mathf.PI / (enumLength - 1)) *  0;
        ////float xPos2 = Mathf.Sin(angle2);
        ////float yPos2 = Mathf.Cos(angle2);
        ////newText.transform.localPosition = new Vector3((xPos2 * 1) + Input.mousePosition.x - (Screen.width / 2), (yPos2 * 1) + Input.mousePosition.y - (Screen.height / 2));
        //buttonList.Add(newText);


        //// THIS WAS MY JACK ATTEMPT  IT'll probably need to be here
        //float xDistance = 0;
        //float xPosition = 0;
        //float yDistance = 0;
        //float yPosition = 0;
        //float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        //float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        //// get the furthest out button
        //foreach (GameObject button in buttonList)
        //{
        //    if (Mathf.Abs(xPosition) < Mathf.Abs(button.GetComponent<RectTransform>().localPosition.x))
        //    {
        //        xPosition = button.GetComponent<RectTransform>().localPosition.x;
        //    }

        //    if (Mathf.Abs(yPosition) < Mathf.Abs(button.GetComponent<RectTransform>().localPosition.y))
        //    {
        //        yPosition = button.GetComponent<RectTransform>().localPosition.y;
        //    }
        //}
        ////if the button is positive on the x, move the piece left
        //if (xPosition > 0)
        //{
        //    xDistance = (canvas.GetComponent<RectTransform>().rect.width / 2) - (xPosition + (buttonPrefab.GetComponent<RectTransform>().rect.width / 2));
        //}
        ////if the button is negative on the x, move the piece right
        //else
        //{
        //    xDistance = Mathf.Abs((canvas.GetComponent<RectTransform>().rect.width / 2) - Mathf.Abs((xPosition - (buttonPrefab.GetComponent<RectTransform>().rect.width / 2))));
        //}

        ////if the button is positive on the y, move the piece down
        //if (yPosition > 0)
        //{
        //    yDistance = (canvas.GetComponent<RectTransform>().rect.height / 2) - (yPosition + (buttonPrefab.GetComponent<RectTransform>().rect.height / 2));
        //}
        ////if the button is negative on the y, move the piece up
        //else
        //{
        //    yDistance = Mathf.Abs((canvas.GetComponent<RectTransform>().rect.height / 2) - Mathf.Abs((yPosition - (buttonPrefab.GetComponent<RectTransform>().rect.height / 2))));
        //}

        ////move all the buttons into position
        //foreach (GameObject button in buttonList)
        // {
        //     button.GetComponent<RectTransform>().localPosition += new Vector3(xDistance, yDistance);
        // }

         //bool insideScreen = false;
         //GameObject furthestAway;
         //foreach(GameObject button in buttonList)
         //{
         //    Mathf.Abs( button.transform.localPosition.x  )
         //}
            //}

        //}

    }

    public int ClearButtons()
    {
        int result = 0;
        open = false;
        //selection = events.GetComponent<UnityEngine.EventSystems.EventSystem>().currentSelectedGameObject.gameObject;
        // do selection code somewhere here
        for (int i = 0; i < buttonList.Count; i++)
        {
            //if (buttonList[i] == selection)
            //{
            //    result = i;
            //}
            Destroy(buttonList[i]);
        }
        buttonList.Clear();
        //Time.timeScale = 1;

        return result;
    }

    #endregion
}
