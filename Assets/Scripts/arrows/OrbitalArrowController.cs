using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalArrowController : ArrowController
{

    [Header("Special Values")]
    public float colisionTime;

    TrailRenderer trail;
    DashEffect dash;
    ShieldController shield;
    PlayerController player;

    bool onParent = false;

    override protected void Awake()
    {
        base.Awake();
        trail = GetComponentInChildren<TrailRenderer>();
        dash = GetComponentInChildren<DashEffect>();
        dash.sprite = spriteChange.sprites[0];
        trail.enabled = false;
        player = FindObjectOfType<PlayerController>();
        shield = FindObjectOfType<ShieldController>();
    }

    protected override IEnumerator Move () 
    {
        Vector2 target = rb.position.normalized * actionDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * 2 * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        GameObject pivot = new GameObject("Pivot");
        transform.parent = pivot.transform;
        onParent = true;
        actionDistance += (inicialPos.magnitude - actionDistance) / 2;
        float orbitalTime = 0f;
        float orbitalDistanceTime = 0f;
        while (orbitalTime < 1f)
        {
            orbitalTime = Mathf.InverseLerp(0f, actionDistance, orbitalDistanceTime);
            orbitalDistanceTime += speed * Time.fixedDeltaTime;
            pivot.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 360f, orbitalTime));
            yield return new WaitForFixedUpdate();
        }
        pivot.transform.eulerAngles = new Vector3(0f, 0f, 360f);
        yield return new WaitForFixedUpdate();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.position.normalized);
        box.enabled = false;
        Vector2 lastPosition = transform.position;
        transform.position = hit.point;
        dash.Dash(transform.position, lastPosition);
        for (float t = 0f; t < colisionTime; t += Time.fixedDeltaTime)
        {
            yield return new WaitForFixedUpdate();
        }
        if (hit.collider.CompareTag("Shield")) shield.Defend();
        else 
        {
            player.Hurt();
        }
        DestroyArrow();
        //trail.enabled = false;
    }

    protected override IEnumerator Destroy ()
    {
        for (float i = 0; i < 0.5f; i += Time.deltaTime)
        {
            yield return null;
        }
        if (onParent)
        {
            Destroy(transform.parent.gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
}
