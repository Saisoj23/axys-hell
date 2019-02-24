using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalArrowController : ArrowController
{

    public float orbitalDistance;
    public float rotationSpeed;

    protected override IEnumerator MoveTo ()
    {
        Vector2 target = rb.position.normalized * orbitalDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        GameObject pivot = Instantiate(new GameObject("Pivot"), Vector3.zero, Quaternion.identity);
        transform.parent = pivot.transform;
        float orbitalTime = 0f;
        do 
        {
            orbitalTime += Time.deltaTime * rotationSpeed;
            pivot.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 360f, orbitalTime));
            yield return null;
        } while (orbitalTime <= 1f);
        while (rb.position != Vector2.zero)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, Vector2.zero, speed * 2 * Time.deltaTime));
            yield return null;
        }
    }

    public override void DestroyArrow ()
    {
        Destroy(transform.parent.gameObject);
    }
}
