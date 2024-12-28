using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedTrap : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();

            if(characterController != null && characterController.gameObject.GetPhotonView().IsMine)
            {

                characterController.Die();

            }

        }
    }


}
