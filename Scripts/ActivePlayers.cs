using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActivePlayers : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI activePlayersText;
  
    void Start()
    {
        activePlayersText.gameObject.SetActive(true);
    }

    
    void Update()
    {
        activePlayersText.text = PhotonNetwork.CountOfPlayers.ToString();
    }
}
