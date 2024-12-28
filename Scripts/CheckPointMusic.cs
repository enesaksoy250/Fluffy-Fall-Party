using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckPointMusic : MonoBehaviourPun
{
    [SerializeField] AudioSource checkPointMusic;

    // Kontrol noktas� durumunu senkronize etmek i�in kullan�lan de�i�ken.
    // Photon networkta senkronize edilecek.
    // Her oyuncu kendi kontrol noktas� durumunu belirleyecek.
    private int checkpointkontrol = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkpointkontrol == 0)
        {
            checkPointMusic.Play();
            // Yerel oyuncuda kontrol noktas� durumunu de�i�tir.
            photonView.RPC("SetCheckPoint", RpcTarget.AllBuffered, 1);
        }
    }

    // RPC metodu ile kontrol noktas� durumunu t�m oyunculara senkronize eder.
    [PunRPC]
    void SetCheckPoint(int value)
    {
        checkpointkontrol = value;
    }
}
