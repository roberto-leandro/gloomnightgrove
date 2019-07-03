using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected bool set;
    public Sprite checkedSprite;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        set = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!set)
        {
            spriteRenderer.sprite = checkedSprite;
        }
    }
}
