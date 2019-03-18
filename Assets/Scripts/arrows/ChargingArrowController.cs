using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingArrowController : ArrowController
{
    [Header("Special Values")]
    public float chargeDistance;
    public float laserTime;

    LineRenderer line;
    ShieldController shield;
    PlayerController player;

    override protected void Awake ()
    {
        base.Awake();
        line = GetComponent<LineRenderer>();
        player = FindObjectOfType<PlayerController>();
        shield = FindObjectOfType<ShieldController>();
    }

    protected override IEnumerator Move ()
    {
        Vector2 target = rb.position.normalized * chargeDistance;
        while (rb.position != target)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
            yield return null;
        }
        Color startColor = spr.color;
        float colorTime = 0f;
        while (colorTime <= 1)
        {
            colorTime += Time.deltaTime * secondSpeed;
            spr.color = Color.Lerp(startColor, Color.white, colorTime);
            yield return null;
        }
        RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
        bool isShield = hit.collider.CompareTag("Shield");
        if (isShield) line.SetPositions(new Vector3[] {transform.position, new Vector3 (0f, 0f, 1f)});
        else line.SetPositions(new Vector3[] {transform.position, new Vector3 (0f, 0f, 0.1f)});
        line.enabled = true;
        if (isShield) shield.Defend();
        else player.Hurt();
        yield return new WaitForSeconds(laserTime);
        line.enabled = false;
        DestroyArrow();
    }

    public override void MoveAndDestroy ()
    {
        line.enabled = false;
        StartCoroutine("MoveToCenter");
    }

    public override void Stop ()
    {
        spr.color = Color.grey;
        line.startColor = Color.grey;
        line.endColor = Color.grey;
        StopAllCoroutines();
    }
}
