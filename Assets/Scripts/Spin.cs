using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    public float speed;

    IEnumerator StartSpin ()
    {
        Debug.Log("startspin");
        while (true)
        {
            transform.eulerAngles += new Vector3(0f, 0f, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RestarSpin ()
    {
        Debug.Log("restarspin");
        while (transform.eulerAngles.z != 0f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, speed * 20 * Time.deltaTime);
            yield return null;
        }
    }
}
