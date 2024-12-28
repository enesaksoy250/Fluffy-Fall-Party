using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanelManager : MonoBehaviour
{

    [SerializeField] GameObject[] mainPanels;

    private bool closeLoadingPanel=false;

    public static MainMenuPanelManager instance;

    private void Awake()
    {
       
        instance = this;

    }

    private void Start()
    {
      
        if (!PlayerPrefs.HasKey("Info"))
        {
            LoadPanel("InfoPanel");
            PlayerPrefs.SetInt("Info", 1);
        }
            
        InvokeRepeating(nameof(ConnectControl), 0, 1);
    }

   
    private void ConnectControl()
    {

        if (Server.instance.onConnectPhoton && DataBaseManager.instance.onConnectDatabase && DataBaseManager.instance.dataExtractionFinished)
        {
 
            Invoke(nameof(CloseLoadingPanel), 2);
            CancelInvoke(nameof(ConnectControl));
            UIDataLoader.instance.SetInfo();

        }

    }

    public void LoadPanel(string panelName)
    {

        foreach (var panel in mainPanels)
        {

            if(panel.name == panelName)
            {

                panel.gameObject.SetActive(true);
                break;
            }

        }


    }

    public void ClosePanel(string panelName)
    {


        foreach (GameObject gameObject in mainPanels)
        {

            if (gameObject.name == panelName)
            {

                gameObject.SetActive(false);
                break;


            }

        }

    }

    public void LoadSpecialPanel(string panelName)
    {

        foreach (GameObject gameObject in mainPanels)
        {

            if (gameObject.name == panelName)
            {

                GameObject.FindWithTag("MainCamera").transform.Find("MainCamera").gameObject.SetActive(false);
                GameObject.FindWithTag("MainCamera2").transform.Find("MainCamera2").gameObject.SetActive(true);
                GameObject.FindWithTag("Canvas1").transform.Find("Canvas1").gameObject.SetActive(false);
                GameObject.FindWithTag("Canvas2").transform.Find("Canvas2").gameObject.SetActive(true);
                gameObject.SetActive(true);
                break;


            }

        }

    }


    public void CloseSpecialPanel(string panelName)
    {

        foreach (GameObject gameObject in mainPanels)
        {

            if (gameObject.name == panelName)
            {

                gameObject.SetActive(false);
                GameObject.FindWithTag("MainCamera").transform.Find("MainCamera").gameObject.SetActive(true);     
                GameObject.FindWithTag("MainCamera2").transform.Find("MainCamera2").gameObject.SetActive(false);            
                GameObject.FindWithTag("Canvas1").transform.Find("Canvas1").gameObject.SetActive(true);
                GameObject.FindWithTag("Canvas2").transform.Find("Canvas2").gameObject.SetActive(false);            
                break;


            }

        }

    }

    public void SelectLanguageButton(string buttonName)
    {

        PlayerPrefs.SetString("Language", buttonName);

        LanguageManager.instance.LoadLocalizedText(PlayerPrefs.GetString("Language"));

        UILanguageManager[] uýLanguageManagers = FindObjectsOfType<UILanguageManager>();

        foreach (UILanguageManager manager in uýLanguageManagers)
        {

            manager.UpdateText();

        }

     

    }

    private void CloseLoadingPanel()
    {

        ClosePanel("LoadingPanel");

    }

    public static void Quit()
    {

        Application.Quit();

    }
}
