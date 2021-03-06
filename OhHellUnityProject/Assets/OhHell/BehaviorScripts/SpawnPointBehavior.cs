﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointBehavior : MonoBehaviour
{
    public List<Transform> Spawns;
    public List<Transform> CardTargets;
    public List<Transform> HandPoints;
    public Transform BidCenter;

    public Vector3 GetSpawnPoint(int numPlayers, int relativeIndex)
    {
        List<int> spawnsToUse = SpawnsToUse(numPlayers);
        int spawnNum = spawnsToUse[relativeIndex];
        return Spawns[spawnNum].position;
    }

    public Vector3 GetCardTarget(int numPlayers, int relativeIndex)
    {
        List<int> spawnsToUse = SpawnsToUse(numPlayers);
        int spawnNum = spawnsToUse[relativeIndex];
        return CardTargets[spawnNum].position;
    }

    //set relativeIndex to -1 for local player
    public Vector3 GetHandPoint(int numPlayers, int relativeIndex)
    {
        if(relativeIndex == -1)
        {
            return HandPoints[7].position;
        }
        List<int> spawnsToUse = SpawnsToUse(numPlayers);
        int spawnNum = spawnsToUse[relativeIndex];
        return HandPoints[spawnNum].position;
    }

    private List<int> SpawnsToUse(int numPlayers)
    {
        switch (numPlayers)
        {
            case (2):
                return new List<int>() { 3 };
            case (3):
                return new List<int>() { 2, 4 };
            case (4):
                return new List<int>() { 1, 3, 5 };
            case (5):
                return new List<int>() { 1, 2, 4, 5 };
            case (6):
                return new List<int>() { 1, 2, 3, 4, 5 };
            case (7):
                return new List<int>() { 0, 1, 2, 4, 5, 6 };
            case (8):
                return new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
        }
        return null;
    }
}
