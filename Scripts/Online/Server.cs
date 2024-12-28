using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviourPunCallbacks
{

    [SerializeField] CharacterDatabase characterDatabase;

    public string KullaniciAdi;
    public string OdaAdi;

    //public bool devam = false;
    public bool started = false;

    public int time = 3;

    public bool onConnectPhoton=false;

    public static Server instance;

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


    void Start()
    {
       
        PhotonNetwork.ConnectUsingSettings();
        InvokeRepeating(nameof(CheckAndReconnect),5,3);
        //devam = false;
        started = false;
        time = 3;   
    }

    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length == 2 && !started)
        {
            UIElements.instance.waitTitleText.gameObject.SetActive(false);
            //devam = true;
            started = true;
        }
    }

    private void CheckAndReconnect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        
        PhotonNetwork.JoinLobby();
        onConnectPhoton = true;
        LoadingManager.instance.UpdateProgress();
  
    }

    public void OdaKur()
    {
       

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinRandomOrCreateRoom(null,0,MatchmakingMode.FillRoom,null,null,"RandomrRoom_",roomOptions);
    }

  
    public override void OnJoinedRoom()
    {

        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("TryScene1");
        Invoke(nameof(CloseLoadingPanel),3);
        //devam = false;
        started = false;
        time = 3;
        
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        PhotonNetwork.IsMessageQueueRunning = true;
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter");
        string playerName = characterDatabase.characters[selectedIndex].name;
        GameObject player = PhotonNetwork.Instantiate(playerName, Vector3.zero, Quaternion.identity, 0, null);

        
        //InvokeRepeating("bilgiKontrolEt", 0, 1f);

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            MapSelection.instance.SelectMap();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;

    }
    public override void OnLeftLobby()
    {
        //devam = false;
        started = false;
        time = 3;
    }

    public override void OnLeftRoom()
    {
        //devam = false;
        CharacterController.instance.countdownEnd = false;
        started = false;
        time = 3;
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("odaya girilemedi" + message);
        //CloseLoadingPanel();
        //MainMenuPanelManager.instance.LoadPanel("ErrorPanel");
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("odaya girilemedi" + message);
        //CloseLoadingPanel();
        //MainMenuPanelManager.instance.LoadPanel("ErrorPanel");
    }

   

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("odaya girilemedi" + message);
        //CloseLoadingPanel();
        //MainMenuPanelManager.instance.LoadPanel("ErrorPanel");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        print("odaya biri girdi");

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        string language = PlayerPrefs.GetString("Language");
        UIElements.instance.waitTitleText.gameObject.SetActive(true);
        UIElements.instance.waitTitleText.text = language == "English" ? "Other player left the room !" : "Diðer oyuncu odadan ayrýldý!";
      
        GameObject player = GameObject.FindWithTag("Player");
        bool countdownEnd = player.GetComponent<CharacterController>().countdownEnd;

        if (countdownEnd && OnlineGameManager.instance.isPlay)
        {

            PhotonNetwork.CurrentRoom.IsOpen = false;
            OnlineGameManager.instance.SetIsPlay(false);
            Player remainingPlayer = null;

            foreach (var p in PhotonNetwork.CurrentRoom.Players.Values)
            {

                remainingPlayer = p;
                break;

            }

            int actorNumber = remainingPlayer.ActorNumber;
            string winnerName = remainingPlayer.NickName;
            StartCoroutine(ShowEndGamePanel(actorNumber));
            OnlineGameManager.instance.FinishGame(winnerName);
            MoveButton.instance.ResetButtonPosition();
            DataBaseManager.instance.UpdateLeaderboard(winnerName, 1);

        }
        else
        {

            string language1 = PlayerPrefs.GetString("Language");
            UIElements.instance.waitTitleText.gameObject.SetActive(true);
            UIElements.instance.waitTitleText.text = language1 == "English" ? "Other player left the room !" : "Diðer oyuncu odadan ayrýldý!" ;
            StartCoroutine(LeaveRoom());

        }

        
    }


    IEnumerator LeaveRoom()
    {

        yield return new WaitForSeconds(3);
        PhotonNetwork.LeaveRoom();

    }

    IEnumerator ShowEndGamePanel(int actorNumber)
    {

        yield return new WaitForSeconds(2);
        OnlineGameManager.instance.ShowEndGamePanels(actorNumber);

    }

    
    private void CloseLoadingPanel()
    {

        GameObject.FindWithTag("LoadingPanel").SetActive(false);

    }
}
