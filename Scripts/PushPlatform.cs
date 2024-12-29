using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlatform : MonoBehaviour
{

    public float pushForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
               
                float xDifference = collision.transform.position.x - transform.position.x;

               
                Vector3 flingDirection = Vector3.zero;

                if (xDifference > 0)
                {
                    flingDirection = Vector3.right; 
                }
                else if (xDifference < 0)
                {
                    flingDirection = Vector3.left; 
                }

                flingDirection.y = 1f;

            
                playerRb.AddForce(flingDirection.normalized * pushForce, ForceMode.Impulse);
            }
        }
    }
}
