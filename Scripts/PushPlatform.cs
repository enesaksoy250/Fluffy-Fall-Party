using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlatform : MonoBehaviour
{

    public float pushForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        // Eðer çarpan nesne "Player" tag'ine sahipse
        if (collision.gameObject.CompareTag("Player"))
        {
            // Karakterin Rigidbody bileþenini al
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // X pozisyonlarýnýn farkýný hesapla
                float xDifference = collision.transform.position.x - transform.position.x;

                // Yönü belirle: Eðer xDifference pozitifse +x, negatifse -x yönünde kuvvet uygula
                Vector3 flingDirection = Vector3.zero;

                if (xDifference > 0)
                {
                    flingDirection = Vector3.right; // +x yönü
                }
                else if (xDifference < 0)
                {
                    flingDirection = Vector3.left; // -x yönü
                }

                // Y yönüne ekstra kuvvet ekle (yukarý doðru fýrlatma için)
                flingDirection.y = 1f;

                // Kuvveti uygula
                playerRb.AddForce(flingDirection.normalized * pushForce, ForceMode.Impulse);
            }
        }
    }
}
