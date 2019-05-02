using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleIcon : MonoBehaviour
{
    private Transform defaultTransfom;
    private Vector3 defaultScale;
    // Start is called before the first frame update
    void Start()
    {
        defaultTransfom = transform;
        defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateScale(float scaleModifier)
    {
        print(transform.localScale + "before Mod");
        transform.localScale = defaultScale * scaleModifier;
        print(transform.localScale);
    }
}
