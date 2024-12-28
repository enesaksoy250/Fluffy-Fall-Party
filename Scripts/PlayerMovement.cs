using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    CharacterController controller;
    PhotonView pw;
 
  

    private void Awake()
    {
        pw = GetComponent<PhotonView>();

    }

  
    public void SetFindControllerScript(CharacterController controller)
    {
             
        this.controller = controller;

    }

   

    public void Run()
    {

        controller.isRunning=true;

    }

    public void Stop()
    {
        
        controller.isRunning =false;

    }

    public void Jump()
    {

        controller.isJumping=true;

    }

    public void NoJump()
    {

        controller.isJumping=false;

    }
}
