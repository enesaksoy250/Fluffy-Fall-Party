using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    public float bounceForce; 

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                
                Vector3 bounce = Vector3.up * bounceForce;
                playerRb.velocity = Vector3.zero; 
                playerRb.AddForce(bounce, ForceMode.Impulse);
            }
        }
    }
}
