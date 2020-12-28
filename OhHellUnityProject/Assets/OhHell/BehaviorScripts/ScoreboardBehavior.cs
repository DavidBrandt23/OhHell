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
    public GameObject HalfTimeTitleObject;
    public GameObject FinalScoreTitleObject;

    private List<GameObject> playerObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (DebugMode)
        {
            List<ScorePair> list = new List<ScorePair>()
            {
                new ScorePair("Sal",33),
                new ScorePair("Steve",133),
                new ScorePair("Dave",12),
                new ScorePair("Bob",1),
                new ScorePair("Bruce",33),
                new ScorePair("Sara",131),
                new ScorePair("Dakota",4),
                new ScorePair("Terry",1),
            };
            list.Sort();
            UpdateScores(list, false);
        }
    }

    private int GetPlacement(int index, List<ScorePair> scoreList)
    {
        int place = index + 1;
        
        for (int i = index - 1; i >= 0; i--)
        {
            if(scoreList[i].score == scoreList[index].score)
            {
                place--;
            }
            else
            {
                break;
            }
        }
        return place;
    }

    public void UpdateScores(List<ScorePair> scoreList, bool isHalfTime)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            GameObject newPlayerOb = Instantiate(ScoreBoardPlayerPrefab, this.transform);
            playerObjects.Add(newPlayerOb);
            newPlayerOb.SetActive(false);
            ScoreBoardPlayerBehavior scoreboardPlayer = newPlayerOb.GetComponent<ScoreBoardPlayerBehavior>();
            int placement = GetPlacement(i, scoreList);
            scoreboardPlayer.SetUI(placement, scoreList[i].playerName, scoreList[i].score);
            newPlayerOb.transform.localPosition = new Vector3(0.0f, 16.0f + i * -5.0f, 0.0f);
        }

        HalfTimeTitleObject.SetActive(isHalfTime);
        FinalScoreTitleObject.SetActive(!isHalfTime);
        
        AudioClip soundToPlay = endGameSound;

        if (isHalfTime)
        {
            soundToPlay = halfTimeSound;
        }
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
}
