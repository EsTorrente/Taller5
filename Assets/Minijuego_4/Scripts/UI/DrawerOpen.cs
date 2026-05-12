using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawerOpen : MonoBehaviour
{
    private bool open = false;

    [SerializeField] private Animator anim;
    [SerializeField] private Image sprite;
    private Button button;

    [Header("Sprites")]
    [SerializeField] private Sprite spriteClosed;
    [SerializeField] private Sprite spriteOpen;

    void Awake()
    {
        button = GetComponent<Button>();

        if (button != null && sprite != null)
        {
            button.targetGraphic = sprite;
        }
    }

    public void Open()
    {
        open = !open;

        if (open)
        {
            anim.SetTrigger("Open");
            sprite.sprite = spriteOpen;
        }
        else
        {
            anim.SetTrigger("Close");
            sprite.sprite = spriteClosed;
        }

        if (button != null)
        {
            button.targetGraphic = sprite;
        }
    }
}