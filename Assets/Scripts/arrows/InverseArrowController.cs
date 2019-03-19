using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseArrowController : ArrowController
{

    public float turnDistance;

    void Start ()
    {
        inicialDir = rb.transform.position.normalized;
        rb.MoveRotation(Vector2.SignedAngle(Vector2.right, inicialDir));
        StartCoroutine("Move");
    }

    override protected IEnumerator Move ()
    {
        inicialDir = -inicialDir;
        box.enabled = false;
        Vector2 target = rb.position.normalized * turnDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        secondSpeed = speed * 3;
        target = -target;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, secondSpeed * Time.deltaTime));
            yield return null;
        }
        box.enabled = true;
        while (rb.position != Vector2.zero)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, (secondSpeed * 1.1f) * Time.deltaTime));
            yield return null;
        }
    }
}
