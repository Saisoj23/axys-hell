using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    Rigidbody2D rb;

    public float speed;

    void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start ()
    {
        rb.MoveRotation(Vector2.SignedAngle(Vector2.right, -rb.position.normalized));
        Invoke("Acelerate", 0.01f);
    }

    void Acelerate ()
    {
        rb.velocity = transform.right * speed;
    }
}
