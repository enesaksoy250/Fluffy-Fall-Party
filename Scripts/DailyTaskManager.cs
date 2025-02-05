using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyTaskManager : MonoBehaviour
{

    private const string keyPlayMatches = "PlayMatches";
    private const string keyWinMatches = "WinMatches";       // 5 Maç Kazan
    private const string keyWinStreak = "WinStreak";         // 5 Maç Üst Üste Kazan
    private const string keyMapWins = "MapWins";             // Tüm Haritalarda Galibiyet Al
    private const string lastResetKey = "LastResetDate";

    private const int requiredMatches = 5;

    public static DailyTaskManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CheckDailyReset();
        InvokeRepeating(nameof(CheckDailyReset), 0, 60);
    }


    void CheckDailyReset()
    {
        string lastReset = PlayerPrefs.GetString(lastResetKey, "");
        if (string.IsNullOrEmpty(lastReset) || IsNewDay(lastReset))
        {
            ResetTasks();
        }
    }

    bool IsNewDay(string lastResetDate)
    {
        DateTime lastReset = DateTime.Parse(lastResetDate);
        return DateTime.Now.Date > lastReset.Date;
    }

 
    void ResetTasks()
    {
        PlayerPrefs.SetInt(keyPlayMatches, 0);
        PlayerPrefs.SetInt(keyWinMatches, 0);
        PlayerPrefs.SetInt(keyWinStreak, 0);
        PlayerPrefs.SetInt(keyMapWins, 0);
        PlayerPrefs.SetInt(keyPlayMatches+"Reward",0);
        PlayerPrefs.SetInt(keyWinMatches+"Reward",0);
        PlayerPrefs.SetInt(keyMapWins+"Reward",0);
        PlayerPrefs.SetInt(keyWinStreak+"Reward",0);   
        PlayerPrefs.SetString(lastResetKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
        Debug.Log("Daily tasks reset!"); 
    }
   
    public void IncrementPlayMatches()
    {

        int current = PlayerPrefs.GetInt(keyPlayMatches, 0);
        
        if(current == 5)      
            return;
       
        current++;
        PlayerPrefs.SetInt(keyPlayMatches, current);

      

    }

    public void IncrementWinMatches()
    {

        int current = PlayerPrefs.GetInt(keyWinMatches, 0);

        if (current == 5)
            return;

        current++;
        PlayerPrefs.SetInt(keyWinMatches, current);

      

    }

    public void IncrementWinStreak(bool isWin)
    {

        if (isWin)
        {

            int current = PlayerPrefs.GetInt(keyWinStreak, 0);

            if (current == 5)
                return;

            current++;
            PlayerPrefs.SetInt(keyWinStreak, current);

        
        }

        else
        {

            PlayerPrefs.SetInt(keyWinStreak, 0);

        }

    }

    public void IncrementMapWins(int mapIndex)
    {

        string mapKey = $"{keyMapWins}_Map{mapIndex}";

        if(PlayerPrefs.GetInt(mapKey,0) == 0)
        {
            PlayerPrefs.SetInt(mapKey, 1);
            int totalMapsWon = PlayerPrefs.GetInt(keyMapWins, 0) + 1;

            PlayerPrefs.SetInt(keyMapWins, totalMapsWon);

        }

    }

}
