using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowButton : MonoBehaviour
{

    public Vector3 position;
    public static FollowButton instance;
    PlayerMovement playerMovement;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
  
    private void Update()
    {

        position = GetComponent<RectTransform>().localPosition;

        if (position != Vector3.zero)
            playerMovement.Run();

        else
            playerMovement.Stop();

    }



}
