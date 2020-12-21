using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class SelectableObjectBehavior : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public UnityEvent clicked;
    private bool _clickEnabled;
    private bool mouseIsOver;
    public bool ClickEnabled
    {
        get { return _clickEnabled; }
        set
        {
            _clickEnabled = value;
            SetColorCorrectly();
        }
    }

    public void Awake()
    {
        //ClickEnabled = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetColorCorrectly();
        //   clicked = new UnityEvent();
    }

    void OnMouseOver()
    {
        mouseIsOver = true;
        SetColorCorrectly();
    }

    void OnMouseExit()
    {
        mouseIsOver = false;
        SetColorCorrectly();
    }
    private void SetColorCorrectly()
    {
        spriteRenderer.color = GetColorToUser(ClickEnabled, mouseIsOver);
    }
    private void OnMouseDown()
    {
    }

    private static Color GetColorToUser(bool clickEnabled, bool hovered)
    {
        if (!clickEnabled)
        {
            return new Color(0.5f,0.5f,0.5f);
        }

        if (hovered)
        {
            return new Color(1.0f, 0.50f, 0.50f);
        }
        else
        {
            return new Color(1.0f, 1.0f, 1.0f);
        }
    }

    private void OnMouseUp()
    {
        if (ClickEnabled)
        {
            clicked.Invoke();
        }
    }
}
