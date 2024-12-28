using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour,IPointerDownHandler
{

    GameObject player;
    CharacterController characterController;
   
    private void Start()
    {



        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {

            if (p.GetPhotonView().Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {

                player = p;
                characterController = p.GetComponent<CharacterController>();

            }

           
        }

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        characterController.Jump();
    }

   
}
