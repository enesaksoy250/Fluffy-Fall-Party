using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCollider : MonoBehaviour
{
    public static groundCollider instance;

    public bool isGrounded;
    private LayerMask groundLayer;
   
    private float checkDistance; 
    private float rayWidth;
    private float forwardOffset;
    private float backwardOffset;

    private void Awake()
    {
        checkDistance = .5f;
        rayWidth = .3f;
        forwardOffset = .1f;
        backwardOffset = .07f;
    }

    private void Start()
    {
        instance = this;
        groundLayer = LayerMask.GetMask("Ground");
    }




    private void Update()
    {
                
        isGrounded |= Physics.Raycast(transform.position + Vector3.up * 0.1f + Vector3.forward * forwardOffset, Vector3.down, checkDistance, groundLayer);    
        isGrounded |= Physics.Raycast(transform.position + Vector3.right * rayWidth + Vector3.up * 0.1f + Vector3.forward * forwardOffset, Vector3.down, checkDistance, groundLayer);   
        isGrounded |= Physics.Raycast(transform.position + Vector3.left * rayWidth + Vector3.up * 0.1f + Vector3.forward * forwardOffset, Vector3.down, checkDistance, groundLayer);   
        isGrounded |= Physics.Raycast(transform.position + Vector3.up * 0.1f - Vector3.forward * backwardOffset, Vector3.down, checkDistance, groundLayer);      
        isGrounded |= Physics.Raycast(transform.position + Vector3.right * rayWidth + Vector3.up * 0.1f - Vector3.forward * backwardOffset, Vector3.down, checkDistance, groundLayer);      
        isGrounded |= Physics.Raycast(transform.position + Vector3.left * rayWidth + Vector3.up * 0.1f - Vector3.forward * backwardOffset, Vector3.down, checkDistance, groundLayer);

    }


}

