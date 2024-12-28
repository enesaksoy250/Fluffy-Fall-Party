using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FallingPlatform : MonoBehaviour
{
    public GameObject[] platforms;
    public Transform[] platformStartPosition;
    public Dictionary<int, int>  platformNumbers = new Dictionary<int, int>();
    private PhotonView pw;
    private bool setPlatformNumber = false;
    private bool masterClient = false;

    public static FallingPlatform instance;

    private void Awake()
    {
        instance = this;
        pw = GetComponent<PhotonView>();
    }

    private void Start()
    {
     
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            masterClient = true;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length == 2 && !setPlatformNumber && masterClient)
        {
            GeneratePlatformNumbers();
            pw.RPC("DistributePlatformNumbers", RpcTarget.OthersBuffered, SerializePlatformNumbers());
            setPlatformNumber = true;
        }
    }

    private void GeneratePlatformNumbers()
    {
        int num = 0;
        for (int i = 0; i < platforms.Length / 2; i++)
        {
            bool verdimi = false;
            for (int j = 0; j < 2; j++)
            {
                if (!verdimi && j == 1)
                {
                    platformNumbers.Add(num, 1);
                    verdimi = true;
                }
                else if (!verdimi)
                {
                    int randomNumber = Random.Range(0, 2);
                    platformNumbers.Add(num, randomNumber);                  
                    if (randomNumber == 1)
                    {
                        verdimi = true;
                    }
                }
                else
                {
                    platformNumbers.Add(num, 0);                 
                }
                num++;
            }
        }


        

    }

    [PunRPC]
    private void DistributePlatformNumbers(int[] serializedNumbers)
    {
        DeserializePlatformNumbers(serializedNumbers);
    }

    private int[] SerializePlatformNumbers()
    {
        int[] serializedNumbers = new int[platforms.Length];
        for (int i = 0; i < platforms.Length; i++)
        {
            serializedNumbers[i] = platformNumbers.ContainsKey(i) ? platformNumbers[i] : 0;
        }
        return serializedNumbers;
    }

    private void DeserializePlatformNumbers(int[] serializedNumbers)
    {
        platformNumbers.Clear();
        for (int i = 0; i < serializedNumbers.Length; i++)
        {
            platformNumbers[i] = serializedNumbers[i];
        }
    }

    public void SetPlatformPosition()
    {

        print("RPC çaðrýsý baþlatýlacak");
        pw.RPC("SetPlatformPositionRPC", RpcTarget.All);

    }

    [PunRPC]
    private void SetPlatformPositionRPC()
    {
        
        PlatformTrigger platformTrigger=null;
        platformNumbers.Clear();

        if (masterClient)
        {
            GeneratePlatformNumbers();
            pw.RPC("DistributePlatformNumbers", RpcTarget.OthersBuffered, SerializePlatformNumbers());
        }

        for (int i = 0; i < platforms.Length; i++)
        {

            platformTrigger = platforms[i].GetComponent<PlatformTrigger>();
            platformTrigger.ResetPosition();     
            platformTrigger.SetMaterial();
            platformTrigger.GetPlatformNumbers();        

        }

       

    }

}
