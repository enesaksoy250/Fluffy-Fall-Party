using UnityEngine;
using Photon.Pun;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            CharacterController characterController = other.GetComponent<CharacterController>();
            if (characterController != null && characterController.gameObject.GetPhotonView().IsMine)
            {
                characterController.AddCheckpoint(transform.position);
            }
        }
    }
}
