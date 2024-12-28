using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIDataLoader : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI[] usernamesArray, winArray;
    [SerializeField] TextMeshProUGUI nameText,levelText;
    [SerializeField] Image currentXpImage;
    [SerializeField] TMP_InputField usernameInputField, emailInputField;
    [SerializeField] TextMeshProUGUI profilUsernameText,profilEmailText,profilLevelText,profilWinText;
    [SerializeField] TextMeshProUGUI levelPanelUsernameText, levelPanelLevelText,levelPanelNextLevelText,levelPanelCurrentXpText;
    [SerializeField] Image levelPanelCurrentXpImage;
    [SerializeField] TextMeshProUGUI storePanelGemText, storePanelCoinText;
    [SerializeField] TextMeshProUGUI csPanelGemText, csPanelCoinText;

    public static UIDataLoader instance;

    private void Awake()
    {
        
        instance = this; 

    }

    private void Start()
    {

        PrintTopTenUsers();
        SetInfo();
       

    }

    public void SetLeaderBoard()
    {

        for (int i = 0; i < 10; i++)
        {
           
            usernamesArray[i].text = UserFirebaseInformation.instance.UsernameArrays[i];
            string wins = UserFirebaseInformation.instance.WinArrays[i].ToString();
            winArray[i].text = wins;

        }


    }

    public void SetInfo()
    {
        nameText.text = UserFirebaseInformation.instance.UserName;
        levelText.text = PlayerPrefs.GetInt("Level",1).ToString();
        currentXpImage.fillAmount = LevelSystem.instance.GetLevelProgress();
    }



    public void SetLevelPanelInfo()
    {

        int level = PlayerPrefs.GetInt("Level");
        int currentXp = LevelSystem.instance.currentXP;
        int xpToNextLevel = LevelSystem.instance.xpToNextLevel[level-1];
        levelPanelUsernameText.text = UserFirebaseInformation.instance.UserName;
        levelPanelLevelText.text = level.ToString();
        levelPanelNextLevelText.text = (level+1).ToString();
        levelPanelCurrentXpImage.fillAmount = LevelSystem.instance.GetLevelProgress();
        levelPanelCurrentXpText.text = currentXp.ToString() + " / " + xpToNextLevel;

    }

    public void SetProfilPanelInfo()
    {

        profilUsernameText.text = UserFirebaseInformation.instance.UserName;
        profilEmailText.text = UserFirebaseInformation.instance.Email;
        profilLevelText.text = PlayerPrefs.GetInt("Level").ToString();
        profilWinText.text = UserFirebaseInformation.instance.Win.ToString();

    }

    public void SetStorePanelInfo()
    {
  
        storePanelCoinText.text = UserFirebaseInformation.instance.Coin.ToString();
        storePanelGemText.text = UserFirebaseInformation.instance.Gem.ToString();

    }

    public void SetCharacterSelectionPanelInfo()
    {

        csPanelCoinText.text = UserFirebaseInformation.instance.Coin.ToString();
        csPanelGemText.text = UserFirebaseInformation.instance.Gem.ToString();

    }
    private void PrintTopTenUsers()
    {

        DataBaseManager.instance.PrintTopTenUsers();

    }


    public void UpdateUsername()
    {

        int totalGem = UserFirebaseInformation.instance.Gem;

        if(totalGem >= 10)
        {

            string username = usernameInputField.text;

            if (!string.IsNullOrEmpty(username))
            {

                DataBaseManager.instance.UpdateFirebaseInfo("username", username,() => { 
                    DataBaseManager.instance.UpdateUsernameOnLeaderboard(username); 
                    SetProfilPanelInfo(); });

            }

        }

     
    }

    public void UpdateEmail()
    {

        string email = emailInputField.text;

        if (!string.IsNullOrEmpty(email))
        {

            DataBaseManager.instance.UpdateFirebaseInfo("email", email);

        }

    }


    public void UpdateCurrentXpImage()
    {

        currentXpImage.fillAmount = LevelSystem.instance.GetLevelProgress();

    }
}
