using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowieSprite : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        var oldColor = spriteRenderer.color;
        spriteRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, Mathf.Abs(Mathf.Cos(Time.fixedTime)));
    }
}
