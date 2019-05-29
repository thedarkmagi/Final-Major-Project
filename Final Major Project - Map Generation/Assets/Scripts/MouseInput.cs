using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour
{
    public enum MouseState {none,ruler,customLabel, createMVP, moveMVP};

    private const int heightModifier = 20;
    private const int rotationAmount = 90;
    private const float MaxDistance = 1000.0f;
    private MouseState mouseState;


    public List<ScaleIcon> allButtons = new List<ScaleIcon>();

    private GameObject gameCanvas;
    public GameObject MenuCanvas;
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

    // Start is called before the first frame update
    void Start()
    {
        mouseState = MouseState.none;
        // ruler setup
        firstClickHasHappened = false;
        lineRenderer = GetComponent<LineRenderer>();
        distanceDisplay = GameObject.Find("DistanceRuler").GetComponent<Text>();
        gameCanvas = GameObject.Find("GameCanvas");
        gameCanvas.SetActive(false);
        selectedToken = null;
    }
    

    // Update is called once per frame
    void Update()
    {
        switch (mouseState)
        {
            case MouseState.none:
                noneStateClick();
                noneStateHover();
                
                break;
            case MouseState.ruler:
                rulerState();
                break;
            case MouseState.customLabel:
                customLabelState();
                break;
            case MouseState.createMVP:
                createMVPTokenState();
                break;
            case MouseState.moveMVP:
                moveMVPTokenState();
                break;
            default:
                break;
        }
        
        if(gameCanvas.activeSelf)
        {
            MenuCanvas.SetActive(false);
        }
        else if (MenuCanvas.activeSelf)
        {
            gameCanvas.SetActive(false);
            
        }
        else
        {
            gameCanvas.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            var genManager = GameObject.Find("Generation Manager");
            foreach (Transform item in genManager.transform)
            {
                Destroy(item.gameObject);
            }
            gameCanvas.SetActive(false);
            MenuCanvas.SetActive(true);
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


    #region State Logic functions
    public void rulerState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HitType hit = raycast();
            if (hit.didHit)
            {
                if (!firstClickHasHappened)
                {
                    firstClickHasHappened = true;
                    firstClickPos = hit.hit.point;
                }
            }
        }
        else if (firstClickHasHappened)
        {
            HitType hit = raycast();
            if (hit.didHit)
            {
                secondPos = hit.hit.point;
                distanceDisplay.transform.position = Input.mousePosition;
                int distance = Mathf.FloorToInt(Vector3.Distance(firstClickPos, secondPos));
                distanceDisplay.text = distance.ToString() + "miles";
                print(Vector3.Distance(firstClickPos, secondPos));
                if (firstClickPos.y > secondPos.y)
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
        if (Input.GetMouseButtonDown(1))
        {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
            distanceDisplay.text = "";
            firstClickHasHappened = false;
            resetMouseState();
        }
    }
    private void noneStateClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HitType hit = raycast();
            if (hit.didHit)
            {
                if (hit.hit.collider.gameObject.tag == "MVPToken")
                {
                    hit.hit.collider.gameObject.GetComponent<TokenController>().ActiveButtons();
                }
                if (hit.hit.collider.gameObject.tag == "PinOnly")
                {
                    hit.hit.collider.gameObject.GetComponent<ToggleOnOff>().showLabel();
                }
            }
        }
    }
    private void noneStateHover()
    {
        HitType hit = raycast();
        if (hit.didHit)
        {
            if (hit.hit.collider.gameObject.tag == "PinOnly")
            {
                hit.hit.collider.gameObject.GetComponent<ToggleOnOff>().showLabelOnTimer();
            }
        }
    }
    private void customLabelState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HitType hit = raycast();
            if (hit.didHit)
            {
                if (!firstClickHasHappened)
                {
                    firstClickPos = hit.hit.point;
                    GameObject CustomLabel = Instantiate(customLabelPrefab, new Vector3(hit.hit.point.x, hit.hit.point.y + heightModifier, hit.hit.point.z), Quaternion.identity);
                    CustomLabel.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                    allButtons.Add(CustomLabel.GetComponentInChildren<ScaleIcon>());
                    CustomLabel.transform.localRotation = Quaternion.Euler(rotationAmount, 0, 0);
                    resetMouseState();
                }
            }
        }
    }
    private void createMVPTokenState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HitType hit = raycast();
            if (hit.didHit)
            {
                if (!firstClickHasHappened)
                {
                    firstClickPos = hit.hit.point;
                    GameObject tokenLocal = Instantiate(token, new Vector3(hit.hit.point.x, hit.hit.point.y + heightModifier, hit.hit.point.z), Quaternion.identity);
                    tokenLocal.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                    tokenLocal.GetComponent<TokenController>().mouseInputReference = this;
                    allButtons.Add(tokenLocal.GetComponent<ScaleIcon>());
                    tokenLocal.transform.rotation = Quaternion.Euler(rotationAmount, 0, 0);
                    resetMouseState();
                }
            }
        }

    }
    private void moveMVPTokenState()
    {
        if (selectedToken != null)
        {
            HitType hit = raycast();
            if (hit.didHit)
            {
                Vector3 newLocation = new Vector3(hit.hit.point.x, selectedToken.transform.position.y, hit.hit.point.z);
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
    }
    #endregion

    #region simplyflying raycast from screen for myself
    public struct HitType
    {
        public RaycastHit hit;
        public bool didHit;
    }
    public HitType raycast()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HitType hitType = new HitType();
        if (Physics.Raycast(ray, out hit, MaxDistance))
        {
            hitType.hit = hit;
            hitType.didHit = true;
        }
        else
        {
            hitType.hit = hit;
            hitType.didHit = false;
        }
        return hitType;
    }
    #endregion

    
}
