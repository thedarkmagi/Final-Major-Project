using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InputFieldController : MonoBehaviour
{
    public TMP_InputField inputField;

    public GameObject deleteButton;
    private Image buttonImage;
    public Sprite editIcon, doneIcon;
    private InputFieldVisualOnly shadow;
    private void Start()
    {
        //deleteButton.SetActive(false);
        
        buttonImage = GetComponent<Image>();

        //inputField = GetComponentInChildren<TMP_InputField>();
        inputField.interactable = true;
        shadow = GetComponentInChildren<InputFieldVisualOnly>();
        deleteButton.SetActive(true);

    }
    public void setReadOnly()
    {
        if (inputField.interactable)
        {
            inputField.interactable = false;
            buttonImage.sprite = editIcon;
            deleteButton.SetActive(false);
            shadow.setReadOnly(true);
        }
        else
        {
            inputField.interactable = true;
            buttonImage.sprite = doneIcon;
            deleteButton.SetActive(true);
            shadow.setReadOnly(false);

        }
    }
}
