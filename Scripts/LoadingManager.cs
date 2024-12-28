using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingManager : MonoBehaviour
{

    [SerializeField] Image progressBar;

    private int totalSteps = 6;
    private int completedSteps = 0;

    private float targetProgress = 0f;
    private float fillSpeed = 0.75f;

    public static LoadingManager instance;

    private void Awake()
    {
       
        instance = this;
        
    }

    private void Update()
    {

        if (progressBar.fillAmount < targetProgress)
        {
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, targetProgress, fillSpeed * Time.deltaTime);

        }

    }


    public void UpdateProgress()
    {

        completedSteps++;
        targetProgress = (float)completedSteps / totalSteps;


        if (completedSteps == totalSteps)
        {

            Destroy(gameObject, 1);

        }

    }

}
