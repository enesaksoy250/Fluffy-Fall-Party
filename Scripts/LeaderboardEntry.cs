using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    public string username;
    public int win;

    public LeaderboardEntry(string username, int win)
    {
        this.username = username;
        this.win = win;
    }
}
