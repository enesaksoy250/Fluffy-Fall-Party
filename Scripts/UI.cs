using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class UI : MonoBehaviour
{
    public void AnaMenuyeDon()
    {

        if(PhotonNetwork.InRoom)
           PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveLobby();
        //PhotonNetwork.Disconnect();
        //SceneManager.LoadScene("MainMenu");
    }



}
