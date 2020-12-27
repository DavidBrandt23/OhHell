using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerInfoBoxBehavior : MonoBehaviour
{
    public TextMeshPro TricksText;
    public TextMeshPro ScoreText;
    public TextMeshPro BidText;
    public TextMeshPro NameText;
    public GameObject MyTurnBG;
    public GameObject TrickWinBG;

    public void UpdateUI(DataNeededForPlayerUI data)
    {
        UpdateUI(data.playerName, data.currentTricks, data.currentScore, data.currentBid, data.isMyTurn, data.isTrickWinner);
    }
    public void UpdateUI(string name, int? tricks, int? score, int? bid, bool isMyTurn, bool isTrickWinner)
    {
        NameText.text = name;
        BidText.text = BidDisplayString(bid);
        TricksText.text = TricksDisplayString(tricks);
        ScoreText.text = ScoreDisplayString(score);
        MyTurnBG.SetActive(isMyTurn);
        TrickWinBG.SetActive(isTrickWinner);
    }
    
    public static string BidDisplayString(int? bid)
    {
        return "Bid: " + NullIntToString(bid);
    }
    public static string TricksDisplayString(int? tricks)
    {
        return "Tricks: " + NullIntToString(tricks);
    }
    public static string ScoreDisplayString(int? score)
    {
        return "" + NullIntToString(score);
        // return "Score: " + NullIntToString(score);
    }
    private static string NullIntToString(int? input)
    {
        if (input == null)
        {
            input = -1;
        }
        string text = input.ToString();
        if (input < 0)
        {
            text = "";
        }
        return text;
    }
}
