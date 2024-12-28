using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruzgar : MonoBehaviour
{
    [SerializeField] private float forceSpeed;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            rigidbody.AddForce(Vector3.up * forceSpeed);
        }
    }
}
