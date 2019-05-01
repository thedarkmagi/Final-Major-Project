using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InputFieldController : MonoBehaviour
{
    private TMP_InputField inputField;
    private Text buttonText;
    public GameObject deleteButton;
    private Image buttonImage;
    public Sprite editIcon, doneIcon;
    private InputFieldVisualOnly shadow;
    private void Start()
    {
        deleteButton.SetActive(false);
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();
        inputField = GetComponentInChildren<TMP_InputField>();
        shadow = GetComponentInChildren<InputFieldVisualOnly>();
    }
    public void setReadOnly()
    {
        if (inputField.interactable)
        {
            inputField.interactable = false;
            //buttonText.text = "Edit";
            buttonImage.sprite = editIcon;
            deleteButton.SetActive(false);
            shadow.setReadOnly(true);
        }
        else
        {
            inputField.interactable = true;
            //buttonText.text = "Done";
            buttonImage.sprite = doneIcon;
            deleteButton.SetActive(true);
            shadow.setReadOnly(false);

        }
    }
}
