using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreBoardPlayerBehavior : MonoBehaviour
{
    public TextMeshPro playerText;
    public TextMeshPro scoreText;
    // Use this for initialization
    void Start()
    {

    }
    public void SetUI(int placement, string player, int score)
    {
        string playerTextString = placement + ". " + player;
        playerText.text = playerTextString;
        scoreText.text = "" + score;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
