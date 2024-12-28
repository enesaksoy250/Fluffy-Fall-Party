using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelection : MonoBehaviour
{

    public static MapSelection instance;


    [SerializeField] GameObject[] maps;
    [SerializeField] GameObject startFloor;

    PhotonView pw;

    private void Awake()
    {
        instance = this;
        pw = GetComponent<PhotonView>();
    }


    public void SelectMap()
    {

        int randomNumber = Random.Range(0, maps.Length);
        pw.RPC("SelectMapRPC", RpcTarget.All,randomNumber); //Burda randomSayi yazýcak

    }

    public void SelectMapForRestartGame()
    {

        int randomNumber = Random.Range(0, maps.Length);
        pw.RPC("SelectMapForRestartGameRPC", RpcTarget.All, randomNumber);

    }


    [PunRPC]
    private void SelectMapRPC(int mapIndex)
    {

       for(int i = 0; i < maps.Length; i++)
       {

            maps[i].SetActive(i==mapIndex);

       }

        if(PhotonNetwork.PlayerList.Length == 2)
           startFloor.SetActive(false);
   
    }

    [PunRPC]
      private void SelectMapForRestartGameRPC(int mapIndex)
      {

          for (int i = 0; i < maps.Length; i++)
          {

            maps[i].SetActive(i == mapIndex);

          }

        if (mapIndex == 1)
            FallingPlatform.instance.SetPlatformPosition();

        if (PhotonNetwork.PlayerList.Length == 2)
            startFloor.SetActive(false);

    }

}
