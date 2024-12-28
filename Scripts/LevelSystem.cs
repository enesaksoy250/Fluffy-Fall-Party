using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{

    public int currentXP;
    public int currentLevel;
    public int[] xpToNextLevel;

    public static LevelSystem instance;

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

    private void Start()
    {
        currentXP = PlayerPrefs.GetInt("currentXP", 0);
        currentLevel = PlayerPrefs.GetInt("Level", 1);
    }

    public void AddXp(int xp)
    {

        currentXP += xp;
        PlayerPrefs.SetInt("currentXP", currentXP);
        CheckLevelUp();

    }

    public void CheckLevelUp()
    {

        if(currentLevel < xpToNextLevel.Length && currentXP >= xpToNextLevel[currentLevel-1])
        {

            currentXP -= xpToNextLevel[currentLevel-1];
            PlayerPrefs.SetInt("currentXP", currentXP);
            currentLevel++;
            PlayerPrefs.SetInt("Level", currentLevel);
            DataBaseManager.instance.UpdateFirebaseInfo("level", currentLevel);

        }

    }

    public float GetLevelProgress()
    {

        return (float)currentXP / xpToNextLevel[currentLevel-1];

    }

}
