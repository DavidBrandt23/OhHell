using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class SelectableObjectBehavior : MonoBehaviour
{
    public UnityEvent clicked;
    public bool enabledOnAwake;
    public bool useSpecifiedDefaultColor;
    public Color defaultColor;

    private bool _clickEnabled;
    private bool mouseIsOver;
    private SpriteRenderer spriteRenderer;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        ClickEnabled = enabledOnAwake;
        SetColorCorrectly();
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
        spriteRenderer.color = GetColorToUse(ClickEnabled, mouseIsOver);
    }

    private void OnMouseDown()
    {
    }

    private Color GetColorToUse(bool clickEnabled, bool hovered)
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
            Color white = new Color(1.0f, 1.0f, 1.0f);
            return useSpecifiedDefaultColor ? defaultColor : white;
            //return new Color(1.0f, 1.0f, 1.0f);
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
