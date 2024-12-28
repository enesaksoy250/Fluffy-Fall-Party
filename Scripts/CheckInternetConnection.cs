using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CheckInternetConnection : MonoBehaviour
{


    private int sceneIndex;

    void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(CheckInternet());
    }

    IEnumerator CheckInternet()
    {

        while (true)
        {

            if(Application.internetReachability == NetworkReachability.NotReachable)
            {

                if(sceneIndex == 0)
                {

                    MainMenuPanelManager.instance.LoadPanel("InternetErrorPanel");

                }

            }

            else
            {

                if (sceneIndex == 0)
                {

                    MainMenuPanelManager.instance.ClosePanel("InternetErrorPanel");

                }

            }

            yield return new WaitForSeconds(1);
        }

    }
    
}
