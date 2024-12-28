using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class OnlineGameManager : MonoBehaviourPun
{

    public static OnlineGameManager instance;


    [SerializeField] TextMeshProUGUI resultText;
    public GameObject winPanel, losePanel,exitButton;
    [SerializeField] Button[] restartButtons;
    public GameObject borders; 
    PhotonView pw;

    public bool isPlay = true;

    private bool isGameRestarted = false;
    private int playersReadyToRestart = 0;

    private const string restartGameTextTurkish="Oyun yeniden baslýyor...";
    private const string restartGameTextEnglish= "The game starts again";
    private string language;

    private double startTime;
    private bool countdown;

    public bool removeAdControl { get; private set; }

    private bool isButtonHidden = false;

    private void Awake()
    {
        instance = this;
        pw = GetComponent<PhotonView>();
    }

    private void Start()
    {

        language = PlayerPrefs.GetString("Language");
        InvokeRepeating(nameof(CheckPlayerCount), 0f, 0.1f); //Burayý aç
    }

    private void CheckPlayerCount()
    {
        
        if (isButtonHidden)
        {
            CancelInvoke(nameof(CheckPlayerCount));
            return;
        }

      
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            exitButton.SetActive(false); 
            isButtonHidden = true; 
        }
    }


    [PunRPC]
    public void ShowEndGamePanelsRPC(int winnerPlayer)
    {


        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {

            if(player.GetPhotonView().Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {

                CharacterController characterController = player.GetComponent<CharacterController>();
                removeAdControl = characterController.removeAdControl;
                print("removeAdControl deðeri = " + removeAdControl);

            }

        }


        if (winnerPlayer == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            winPanel.SetActive(true);
            HandleAdAndRewards(100, 100);
        }

        else
        {
            losePanel.SetActive(true);
            HandleAdAndRewards(50, 50);   
        }


    }

    private void HandleAdAndRewards(int coinReward, int xpReward)
    {
        if (!removeAdControl)
        {
            AdManager.instance.ShowInterstitialAd();
        }

        DataBaseManager.instance.IncreaseFirebaseInfo("coin", coinReward);
        LevelSystem.instance.AddXp(xpReward);
        StartCoroutine(Countdown());
    }

    public void RequestGameRestart()
    {

        photonView.RPC("RPC_RequestGameRestart", RpcTarget.All);

    }

    [PunRPC]
    public void RPC_RequestGameRestart()
    {
     
        playersReadyToRestart++;

        
        if (playersReadyToRestart == 2)
        {
            if (!isGameRestarted)
            {

                countdown = false;
                StopCoroutine(Countdown());
                isGameRestarted = true;
                playersReadyToRestart = 0;
                pw.RPC("SetButtonInteractable", RpcTarget.All);
                SetButtonInteractable();
                winPanel.SetActive(false);
                losePanel.SetActive(false);              
                StartCoroutine(RestartGame());
             
            }
        }
    }


    private IEnumerator RestartGame()
    {

        string message = language == "English" ? restartGameTextEnglish : restartGameTextTurkish;

        pw.RPC("ShowRestartMessage", RpcTarget.All,message);

        yield return new WaitForSeconds(3);

        pw.RPC("ShowRestartMessage", RpcTarget.All,"");

        UIElements.instance.loadingPanel.SetActive(true);

        SetPlayerPosition();

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
           MapSelection.instance.SelectMapForRestartGame();

        yield return new WaitForSeconds(3);

        UIElements.instance.loadingPanel.SetActive(false);
      
        GameStartCountdown();

        pw.RPC("LoadAd", RpcTarget.All);
         
    }

    [PunRPC]
    private void LoadAd()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                CharacterController characterController = player.GetComponent<CharacterController>();
                bool adControl = characterController.removeAdControl;

                if (!adControl) { AdManager.instance.LoadInterstitialAd(); }
            }
          
        }
            
    }

    [PunRPC]
    private void CloseLoadingPanelRPC()
    {

        UIElements.instance.loadingPanel.SetActive(false);

    }

    private void GameStartCountdown()
    {

       startTime = PhotonNetwork.Time + 3;
       pw.RPC("GameStartCountdownRPC", RpcTarget.All);

    }

     [PunRPC]
     private void GameStartCountdownRPC()
     {

         StartCoroutine(SynchronizedCountdown());

     }

    IEnumerator SynchronizedCountdown()
    {
        while (true)
        {
            double timeLeft = startTime - PhotonNetwork.Time;

            if (timeLeft > 0)
            {
                resultText.text = "THE GAME BEGINS";
            }
            else
            {
                break;
            }

            yield return null;
        }


        for (int i = 3; i > 0; i--)
        {
            resultText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }


        resultText.text = "START";
        yield return new WaitForSeconds(1f);
        resultText.text = "";
        SetIsPlay(true);
        isGameRestarted = false;
    
    }

    public void ShowEndGamePanels(int actorNumber)
    {

        pw.RPC("ShowEndGamePanelsRPC", RpcTarget.All,actorNumber);

    }

    public void SetIsPlay(bool isPlay)
    {
        pw.RPC("RPC_SetIsPlay", RpcTarget.All, isPlay);
    }

    [PunRPC]
    private void RPC_SetIsPlay(bool state)
    {
        isPlay = state;
    }


    public void FinishGame(string winnerName)
    {

        pw.RPC("FinishGameRPC", RpcTarget.All, winnerName);

    }

    [PunRPC]
    private void FinishGameRPC(string winnerName)
    {

        resultText.text = $"{winnerName} kazandý!";

    }

    [PunRPC]
    private void ShowRestartMessage(string message)
    {

        resultText.text = message;

    }

    [PunRPC]     
    IEnumerator Countdown()
    {
        countdown = true;

        for (int i = 15; i >= 0; i--)
        {
            yield return new WaitForSeconds(1f);
        }

        if (PhotonNetwork.InRoom && countdown)
            PhotonNetwork.LeaveRoom();

    }

    private void SetPlayerPosition()
    {

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        CharacterController characterController;

          foreach (GameObject player in players)
          {

             characterController = player.GetComponent<CharacterController>();
             characterController.SetStartPosition();
             characterController.ResetCheckpoints();
           
          }
         

    }

    [PunRPC]
    private void SetButtonInteractable()
    {

        foreach(Button button in restartButtons)
        {

            button.interactable = true;

        }

    }
}
