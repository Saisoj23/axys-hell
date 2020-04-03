using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFixer : MonoBehaviour
{
    Material mat;
    Vector3 inputOffset;
    GameObject shadowObject;
    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<SpriteRenderer>().material;
        inputOffset = mat.GetVector("ShadowOffset");
        shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(transform);
        shadowObject.transform.localPosition = inputOffset;
    }

    // Update is called once per frame
    void Update()
    {
        shadowObject.transform.position = transform.position + inputOffset;
        Vector2 newOffset = shadowObject.transform.localPosition;
        mat.SetVector("ShadowOffset", newOffset);
    }
}
