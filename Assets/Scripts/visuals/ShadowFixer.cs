using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFixer : MonoBehaviour
{
    Vector3 inputOffset;
    Vector3 newOffset;

    void Start()
    {
        inputOffset = transform.localPosition / 2;
    }

    void Update()
    {
        newOffset = transform.parent.position + inputOffset;
        transform.position = newOffset;
    }
}
