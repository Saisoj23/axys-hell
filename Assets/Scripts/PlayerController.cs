using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float rotateSpeed;
    public float minAxisSensibility;
    public float minTouchSpeed;

    Vector2 direccion;
    Vector3 lastMousePosition;
    bool lastPresed;
    float timeTurning;
    float lastAngle;
    Rigidbody2D rb;
    Animator anim;
    ShieldController shield;

    void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shield = GetComponentInChildren<ShieldController>();
    }

    void Update ()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > minAxisSensibility) direccion.x = horizontal > 0 ? 1 : -1;
        else direccion.x = 0;
        if (Mathf.Abs(vertical) > minAxisSensibility) direccion.y = vertical > 0 ? 1 : -1;
        else direccion.y = 0;

        #if UNITY_ANDROID
        Vector3 touch;
        if (Input.touchCount > 0) 
        {
            touch = Input.GetTouch(0).deltaPosition;
            if (touch.magnitude * Time.deltaTime > minTouchSpeed)
            {
                direccion = touch.normalized; 
            }
        }
        #endif

        #if UNITY_EDITOR
        Vector3 actualMousePosition;
        if (Input.GetMouseButton(0)) 
        {
            actualMousePosition = Input.mousePosition;
            if (lastMousePosition != null && lastPresed && Vector2.Distance(actualMousePosition, lastMousePosition) * Time.deltaTime > minTouchSpeed)
            {
                direccion = -(lastMousePosition - actualMousePosition).normalized; 
            }
            lastMousePosition = actualMousePosition;
            lastPresed = true;
        }
        else lastPresed = false;
        #endif
    }

    void FixedUpdate ()
    {
        Vector2 finalRot = new Vector2(Mathf.Round(direccion.x), Mathf.Round(direccion.y)).normalized;
        if (finalRot.y != 0 || finalRot.x != 0)
        {
            lastAngle = Vector2.SignedAngle(Vector2.right, finalRot);
        }
        if (rb.rotation != lastAngle)
        {
            rb.MoveRotation(Mathf.MoveTowardsAngle(rb.rotation, lastAngle, rotateSpeed * Time.deltaTime));
        }
    }

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.gameObject.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);
            Hurt();
        }
        if (col.gameObject.CompareTag("Anti Bullet"))
        {
            Destroy(col.gameObject);
            shield.Defend();
        }
    }

    public void Hurt ()
    {
        anim.SetTrigger("Hurted");
        Debug.Log("perdiste");
    }
}
