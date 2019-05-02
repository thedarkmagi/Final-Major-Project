using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleIcon : MonoBehaviour
{
    private Vector3 defaultTransfom;
    private Vector3 defaultScale;
    public bool scaleAndMove;
    // Start is called before the first frame update
    void Start()
    {
        defaultTransfom = transform.localPosition;
        defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateScale(float scaleModifier)
    {
        transform.localScale = defaultScale * scaleModifier;
        if(scaleAndMove)
        {
            transform.localPosition = new Vector3(defaultTransfom.x, defaultTransfom.y * scaleModifier, defaultTransfom.z);
        }
    }
}
