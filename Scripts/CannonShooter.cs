using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    public GameObject cannonballPrefab; 
    public Transform firePoint; 
    public float fireForce;
    private float fireInterval; 

    private void Start()
    {

        FireCannon();
    }

    private void FireCannon()
    {
        
        GameObject cannonball = Instantiate(cannonballPrefab, firePoint.position, firePoint.rotation);

     
        Rigidbody rb = cannonball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);
        }

      
        Destroy(cannonball, 5f);

        fireInterval = Random.Range(1f, 3f);
        Invoke(nameof(FireCannon), fireInterval);
   
    }
}
