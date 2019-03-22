using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingArrowController : ArrowController
{
    [Header("Special Values")]
    public float chargeDistance;
    public float laserTime;

    ShieldController shield;
    PlayerController player;
    SpriteRenderer chargingSpr;
    SpriteRenderer laserSpr;
    public GameObject charging;
    public GameObject laser;

    override protected void Awake ()
    {
        base.Awake();     
        chargingSpr = charging.GetComponent<SpriteRenderer>();
        laserSpr = laser.GetComponent<SpriteRenderer>();
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
        Vector3 startChargePosition = charging.transform.position;
        float chargeTime = 0f;
        while (chargeTime <= 1)
        {
            chargeTime += Time.deltaTime * secondSpeed;
            Color thisColor = Color.Lerp(startColor, Color.white, chargeTime);
            spr.color = thisColor;
            chargingSpr.color = thisColor;
            charging.transform.position = Vector3.Lerp(startChargePosition, transform.position + Vector3.forward, chargeTime);
            yield return null;
        }
        RaycastHit2D hit = Physics2D.Raycast(rb.position -rb.position.normalized * 0.2f, -rb.position.normalized);
        bool isShield = hit.collider.CompareTag("Shield");
        laser.transform.localScale = new Vector3(Mathf.Abs(transform.position.x + transform.position.y) * 2.1f, 1, 1);
        if (isShield) 
        {
            laser.transform.localPosition += Vector3.forward / 2;
        }
        else 
        {
            laser.transform.localPosition -= Vector3.forward / 4;
        }
        laserSpr.enabled = true;
        if (isShield) shield.Defend();
        else player.Hurt();
        yield return new WaitForSeconds(laserTime);
        laserSpr.enabled = false;
        DestroyArrow();
    }

    public override void MoveAndDestroy ()
    {
        laserSpr.enabled = false;
        if (visible)
        StartCoroutine("MoveToCenter");
        else
        {
            useAnim = false;
            DestroyArrow();
        }
    }

    public override void Stop ()
    {
        spr.color = stopColor;
        chargingSpr.color = stopColor;
        laserSpr.color = stopColor;
        StopAllCoroutines();
    }

    public override void DestroyArrow ()
    {
        mask.enabled = false;
        spr.sprite = chargingSpr.sprite;
        chargingSpr.enabled = false;
        box.enabled = false;
        if (useAnim) anim.SetTrigger("Destroy");
        anim.speed = speed / 2;
        StopAllCoroutines();
        Destroy(gameObject, 0.5f);
    }
}
