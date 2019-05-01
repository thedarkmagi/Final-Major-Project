using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenController : MonoBehaviour
{
    public List<GameObject> uiButtons;
    public MouseInput mouseInputReference;
    public GameObject iconPanel;
    public GameObject IconPrefab;
    public Image Icon;
    public List<Sprite> IconList;

    private List<GameObject> currentIconsInPanel = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        setButtionsState(false);
        iconPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveButtons()
    {
        setButtionsState(true);
    }
    public void DeactiveButtons()
    {
        setButtionsState(false);
    }
    private void setButtionsState(bool state)
    {
        for (int i = 0; i < uiButtons.Count; i++)
        {
            uiButtons[i].SetActive(state);
        }
    }
    public void setMouseInputStateMoveToken()
    {
        mouseInputReference.setMouseState(MouseInput.MouseState.moveMVP);
        mouseInputReference.setSelectedToken(gameObject);
        DeactiveButtons();
    }
    public void enableEditMode()
    {
        DeactiveButtons();
        iconPanel.SetActive(true);
        for (int i = 0; i < IconList.Count; i++)
        {
            GameObject tempIcon = Instantiate(IconPrefab, iconPanel.transform);
            tempIcon.GetComponent<Image>().sprite = IconList[i];
            currentIconsInPanel.Add(tempIcon);
        }
    }
    public void SetIcon(Image newImage)
    {
        Icon.sprite = newImage.sprite;
        for (int i = 0; i < currentIconsInPanel.Count; i++)
        {
            Destroy(currentIconsInPanel[i]);
        }
        currentIconsInPanel.Clear();
        iconPanel.SetActive(false);
    }
    public void killToken()
    {
        Destroy(gameObject);
    }
}
