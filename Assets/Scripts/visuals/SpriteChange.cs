using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChange : MonoBehaviour
{
    public Sprite[] sprites;
    public int sprite;
    public int lastSprite;
    public bool cheking;

    SpriteRenderer spr;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        if (!cheking) this.enabled = false;
    }

    void Update()
    {
        if (sprite != lastSprite)
        {
            lastSprite = sprite;
            ChangeSprite(sprite);
        }
    }

    public void ChangeSprite (int sprite)
    {
        if (sprite < sprites.Length)
        {
            spr.sprite = sprites[sprite];
        }
    }

    public void ChangeOrder (int order)
    {
        spr.sortingOrder = order;
    }
}
