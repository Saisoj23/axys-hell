using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFixer : MonoBehaviour
{
    Vector3 inputOffset;
    Vector3 newOffset;
    // Start is called before the first frame update
    void Start()
    {
        inputOffset = transform.localPosition / 2;
    }

    // Update is called once per frame
    void Update()
    {
        newOffset = transform.parent.position + inputOffset;
        transform.position = newOffset;
    }
}
