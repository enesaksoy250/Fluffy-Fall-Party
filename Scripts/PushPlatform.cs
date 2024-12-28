using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlatform : MonoBehaviour
{

    public float pushForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        // E�er �arpan nesne "Player" tag'ine sahipse
        if (collision.gameObject.CompareTag("Player"))
        {
            // Karakterin Rigidbody bile�enini al
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // X pozisyonlar�n�n fark�n� hesapla
                float xDifference = collision.transform.position.x - transform.position.x;

                // Y�n� belirle: E�er xDifference pozitifse +x, negatifse -x y�n�nde kuvvet uygula
                Vector3 flingDirection = Vector3.zero;

                if (xDifference > 0)
                {
                    flingDirection = Vector3.right; // +x y�n�
                }
                else if (xDifference < 0)
                {
                    flingDirection = Vector3.left; // -x y�n�
                }

                // Y y�n�ne ekstra kuvvet ekle (yukar� do�ru f�rlatma i�in)
                flingDirection.y = 1f;

                // Kuvveti uygula
                playerRb.AddForce(flingDirection.normalized * pushForce, ForceMode.Impulse);
            }
        }
    }
}
