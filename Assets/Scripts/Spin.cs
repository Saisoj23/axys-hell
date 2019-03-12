using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    public float speed;

    void FixedUpdate ()
    {
        transform.eulerAngles += new Vector3(0f, 0f, speed * Time.deltaTime);
    }
}
