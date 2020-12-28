using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundScoreBehavior : MonoBehaviour
{
    public bool debugModeOn;
    public bool debugForLocal;
    public int debugPoints;
    
    public TextMeshPro PointText;
    public Color winColor;
    public Color failColor;

    // Start is called before the first frame update
    void Start()
    {
        if (debugModeOn)
        {
            Setup(debugPoints, debugForLocal);
        }
    }

    public void Setup(int points, bool forLocal)
    {
        string textString = "+" + points;
        bool isWin = points >= 10;
        Color colorToUse = isWin ? winColor : failColor;

        PointText.text = textString;
        PointText.color = colorToUse;
        if (forLocal)
        {
            PointText.fontSize += 20;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
