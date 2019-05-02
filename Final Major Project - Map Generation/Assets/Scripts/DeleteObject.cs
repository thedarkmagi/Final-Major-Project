using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    public void deleteObject()
    {
        if(GetComponent<ScaleIcon>())
        {
            FindObjectOfType<MouseInput>().deleteFromScaleableList(GetComponent<ScaleIcon>());
        }
        if(GetComponentInChildren<ScaleIcon>())
        {
            FindObjectOfType<MouseInput>().deleteFromScaleableList(GetComponentInChildren<ScaleIcon>());
        }

        Destroy(gameObject);
    }
}
