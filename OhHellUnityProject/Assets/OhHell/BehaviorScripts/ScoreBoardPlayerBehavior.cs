using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreBoardPlayerBehavior : MonoBehaviour
{
    public TextMeshPro playerText;
    public TextMeshPro scoreText;

    public void SetUI(int placement, string player, int score)
    {
        string playerTextString = placement + ". " + player;
        playerText.text = playerTextString;
        scoreText.text = "" + score;
    }
}
