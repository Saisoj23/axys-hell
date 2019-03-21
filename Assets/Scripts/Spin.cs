using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    public float speed;

    IEnumerator StartSpin ()
    {
        while (true)
        {
            transform.eulerAngles += new Vector3(0f, 0f, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RestarSpin ()
    {
        Quaternion target = Quaternion.Euler(0f, 0f, Mathf.Round(transform.eulerAngles.z / 90) * 90);
        while (transform.rotation != target)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, speed * 20 * Time.deltaTime);
            yield return null;
        }
    }
}
