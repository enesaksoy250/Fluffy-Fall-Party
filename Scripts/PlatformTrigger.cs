using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class PlatformTrigger : MonoBehaviour
{

    public int sequenceNumber;
    public int number;
    Rigidbody rb;
    MeshRenderer mr;
    PhotonView pw;

    public Material[] mat;

    private Vector3 startPosition;

    private void Awake()
    {
        pw = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
    }

    private void Start()
    {

        startPosition = transform.position;
        StartCoroutine(WaitForNumber());
    }

    private IEnumerator WaitForNumber()
    {
        while (!FallingPlatform.instance.platformNumbers.ContainsKey(sequenceNumber))
        {
            yield return null; 
        }

        number = FallingPlatform.instance.platformNumbers[sequenceNumber];
        Debug.Log($"{sequenceNumber}. Platform number: {number}");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (number == 0)
            {
                CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
                StartCoroutine(characterController.SetReadyForJump(2));           
                Material[] materials = mr.materials;
                materials[0] = mat[0];
                mr.materials = materials;
                rb.isKinematic = false;
                StartCoroutine(FreezePositionY());
            }
            else if (number == 1)
            {
                Material[] materials = mr.materials;
                materials[0] = mat[1];
                mr.materials = materials;
                mr.materials[0] = null;
                mr.materials[0] = mat[1];
            }
        }
    }

 /*   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(number == 0)
            {
                other.gameObject.GetComponent<CharacterController>().SetReadyForJump(2);
                Material[] materials = mr.materials;
                materials[0] = mat[0];
                mr.materials = materials;
                rb.isKinematic = false;                
                StartCoroutine(FreezePositionY());
            }
            else if (number == 1)
            {
                Material[] materials = mr.materials;
                materials[0] = mat[1];
                mr.materials = materials;
                mr.materials[0] = null;
                mr.materials[0] = mat[1];
            }
        }
    }
*/

   IEnumerator FreezePositionY()
   {
       yield return new WaitForSeconds(5);
       rb.isKinematic = true;
   }

  
    public void SetMaterial()
    {

        Material[] materials = mr.materials;
        materials[0] = mat[2];
        mr.materials = materials;

    }
 
    public void ResetPosition()
    {

        transform.position = startPosition;

    }

    public void GetPlatformNumbers()
    {

        number = FallingPlatform.instance.platformNumbers[sequenceNumber];

    }

}
