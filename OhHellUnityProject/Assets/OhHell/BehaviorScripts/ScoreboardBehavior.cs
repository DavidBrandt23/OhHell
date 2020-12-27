using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePair : IComparable<ScorePair>
{
    public string playerName;
    public int score;
    public ScorePair(string name, int newScore)
    {
        playerName = name;
        score = newScore;
    }
    public int CompareTo(ScorePair other)
    {
        if(other.score > score)
        {
            return 1;

        }
        return -1;
    }
}
public class ScoreboardBehavior : MonoBehaviour
{
    public bool DebugMode;
    public GameObject ScoreBoardPlayerPrefab;
    public AudioClip halfTimeSound;
    public AudioClip endGameSound;
    public TextMeshPro title;
    public GameObject HalfTimeTitleObject;
    private List<GameObject> playerObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (DebugMode)
        {

            List<ScorePair> list = new List<ScorePair>()
            {
                new ScorePair("AAA",33),
                new ScorePair("AA3333333333A",133),
                new ScorePair("AAdfdfdfdfA",1),
                new ScorePair("bbb",1),
            };
            list.Sort();
            UpdateScores(list, true);
        }
    }
    public void UpdateScores(List<ScorePair> scoreList, bool isHalfTime)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            GameObject newPlayerOb = Instantiate(ScoreBoardPlayerPrefab, this.transform);
            playerObjects.Add(newPlayerOb);
            newPlayerOb.SetActive(false);
            ScoreBoardPlayerBehavior scoreboardPlayer = newPlayerOb.GetComponent<ScoreBoardPlayerBehavior>();
            scoreboardPlayer.SetUI(i + 1, scoreList[i].playerName, scoreList[i].score);
            newPlayerOb.transform.localPosition = new Vector3(0.0f, 25.0f + i * -5.0f, 0.0f);
        }

        HalfTimeTitleObject.SetActive(isHalfTime);

        string titleText = "Final Scores";
        AudioClip soundToPlay = endGameSound;

        if (isHalfTime)
        {
            titleText = "";
            soundToPlay = halfTimeSound;
        }
        title.text = titleText;
        GetComponent<AudioSource>().PlayOneShot(soundToPlay);

        this.StartCoroutine(() =>
        {
            RevealPlayer(scoreList.Count - 1);
        }, 0.5f);
    }

    private void RevealPlayer(int index)
    {
        playerObjects[index].SetActive(true);
        if(index != 0)
        {
            this.StartCoroutine(() =>
            {
                RevealPlayer(index - 1);
            }, 1.0f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
