using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardTimer : MonoBehaviour
{

    private TextMeshProUGUI timerText;


    private void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {

        if (PlayerPrefs.HasKey("LastRewardTime"))
        {
            string lastRewardTimeString = PlayerPrefs.GetString("LastRewardTime");
            DateTime lastRewardTime = DateTime.Parse(lastRewardTimeString);
            TimeSpan timeRemaining = (lastRewardTime.AddHours(24) - DateTime.Now);

            if (timeRemaining.TotalSeconds > 0)
            {

                timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                    timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
            }
            else
            {
                timerText.text = "Ödül alýnabilir!";
            }
        }
        else
        {
            timerText.text = "Ödül alýnabilir!";
        }
    }


}
