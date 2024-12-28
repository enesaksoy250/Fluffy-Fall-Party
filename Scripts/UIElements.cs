using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIElements : MonoBehaviour
{

    public static UIElements instance;

    public TextMeshProUGUI waitTitleText, timerText;
    public GameObject loadingPanel;

    private void Awake()
    {
        
        instance = this;

    }

}
