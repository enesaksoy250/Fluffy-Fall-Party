using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DailyRewardSystem : MonoBehaviour
{
   
    private const string LastRewardTimeKey = "LastRewardTime";
    private const int RewardCooldownHours = 24;
    
    [SerializeField] GameObject remainingPanel;
  

    public void SetRemainingPanel()
    {
     
        remainingPanel.SetActive(!IsRewardAvailable());

    }

    public void ClaimReward()
    {
        if (IsRewardAvailable())
        {
          
            Debug.Log("Ödül verildi!");
            GiveReward();
             
        }
        else
        {
           
            Debug.Log("Ödül almak için 24 saat daha bekleyin.");
        }
    }

    private bool IsRewardAvailable()
    {
        
        if (!PlayerPrefs.HasKey(LastRewardTimeKey))
            return true; 

        
        string lastRewardTimeString = PlayerPrefs.GetString(LastRewardTimeKey);
        DateTime lastRewardTime = DateTime.Parse(lastRewardTimeString);
       
        TimeSpan timeSinceLastReward = DateTime.Now - lastRewardTime;
        print(timeSinceLastReward);
        return timeSinceLastReward.TotalHours >= RewardCooldownHours;
  
    }

    private void GiveReward()
    {

        int randomNumber = Random.Range(0, 10);

        if (randomNumber < 1)
        {
            DataBaseManager.instance.IncreaseFirebaseInfo("gem", 1, () => { UIDataLoader.instance.SetStorePanelInfo(); MainMenuPanelManager.instance.LoadPanel("DailyGemPanel"); });       
        }

        else if (randomNumber < 5)
        {
            DataBaseManager.instance.IncreaseFirebaseInfo("coin",100, () => { UIDataLoader.instance.SetStorePanelInfo(); MainMenuPanelManager.instance.LoadPanel("DailyCoinPanel"); });     
        }

        else
        {
            LevelSystem.instance.AddXp(100);
            UIDataLoader.instance.SetInfo();
            MainMenuPanelManager.instance.LoadPanel("DailyXpPanel");
        }

        remainingPanel.SetActive(true);
        PlayerPrefs.SetString(LastRewardTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();

    }


}
