using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class SelectableObjectBehavior : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public UnityEvent clicked;
    public bool ClickEnabled;

    public void Awake()
    {
        //ClickEnabled = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
     //   clicked = new UnityEvent();
    }

    void OnMouseOver()
    {
        if (ClickEnabled)
        {
            spriteRenderer.color = new Color(1.0f, 0.50f, 0.50f);
        }
    }

    void OnMouseExit()
    {
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f);
    }
    private void OnMouseDown()
    {
    }

    private void OnMouseUp()
    {
        if (ClickEnabled)
        {
            Debug.Log("sel on mouse up");
            clicked.Invoke();
        }
    }
}
