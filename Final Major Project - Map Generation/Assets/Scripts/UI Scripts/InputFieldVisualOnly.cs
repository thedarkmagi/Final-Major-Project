using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InputFieldVisualOnly : MonoBehaviour
{
    private TMP_InputField inputField;
    private Text buttonText;
    public GameObject deleteButton;
    private Image buttonImage;
    public Sprite editIcon, doneIcon;
    
    private void Start()
    {
        deleteButton.SetActive(false);
        buttonImage = GetComponent<Image>();
    }
    public void setReadOnly()
    {
        if (inputField.interactable)
        {
            buttonImage.sprite = editIcon;
            deleteButton.SetActive(false);
        }
        else
        {
            buttonImage.sprite = doneIcon;
            deleteButton.SetActive(true);
        }
    }

    public void setReadOnly(bool input)
    {
        if (input)
        {
            buttonImage.sprite = editIcon;
            //deleteButton.SetActive(false);
        }
        else
        {
            buttonImage.sprite = doneIcon;
           // deleteButton.SetActive(true);
        }
    }
}
