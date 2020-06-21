using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour
{
    public float spritesDistance;
    public float spritesMinScale;
    public float spritesMinOpacity;
    public float fadeSpeed;
    public float duration;

    public Sprite sprite;
    List<SpriteRenderer> spr = new List<SpriteRenderer>();
    Vector2 firstPosition;

    public void Dash(Vector2 posA, Vector2 posB)
    {
        float distance = Vector2.Distance(posA, posB);
        int spritesCount = (int)(distance/spritesDistance);
        float normalicedDistance = 1f / spritesCount;
        int sprIndex = 0;
        for (float i = 0; i < 1; i += normalicedDistance)
        {
            print("sprites " + spritesCount);
            print("normaliced distance " + normalicedDistance);
            GameObject effect = new GameObject("effect" + sprIndex);
            effect.transform.SetParent(transform);
            effect.transform.SetPositionAndRotation(Vector3.Lerp(posA, posB, i), transform.rotation);
            spr.Add(effect.AddComponent<SpriteRenderer>());
            spr[sprIndex].sprite = sprite;
            spr[sprIndex].sortingOrder = -sprIndex - 1;
            spr[sprIndex].color = new Color(1, 1, 1, Mathf.Lerp(1, spritesMinOpacity, i));
            effect.transform.localScale = new Vector3(Mathf.Lerp(1, spritesMinScale, i), Mathf.Lerp(1, spritesMinScale, i), 1);
            
            sprIndex++;
        }
        firstPosition = transform.position;
        StartCoroutine(Fade());
    }

    IEnumerator Fade ()
    {
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            foreach (SpriteRenderer item in spr)
            {
                item.color -= new Color(0, 0, 0, fadeSpeed);
            }
            transform.position = firstPosition;
            yield return null;
        }
    }
}
