using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayAdButton : MonoBehaviour
{
    
    Button playButton;  

    void Start()
    {
        playButton=GetComponent<Button>();

        playButton.onClick.AddListener(delegate { PlayAd(); });

    }

    private void PlayAd()
    {

        MainMenuPanelManager.instance.LoadPanel("GameLoadingPanel");
        AdManager.instance.LoadRewardedAd();

    }

}
