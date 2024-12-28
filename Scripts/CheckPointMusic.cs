using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckPointMusic : MonoBehaviourPun
{
    [SerializeField] AudioSource checkPointMusic;

    // Kontrol noktasý durumunu senkronize etmek için kullanýlan deðiþken.
    // Photon networkta senkronize edilecek.
    // Her oyuncu kendi kontrol noktasý durumunu belirleyecek.
    private int checkpointkontrol = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkpointkontrol == 0)
        {
            checkPointMusic.Play();
            // Yerel oyuncuda kontrol noktasý durumunu deðiþtir.
            photonView.RPC("SetCheckPoint", RpcTarget.AllBuffered, 1);
        }
    }

    // RPC metodu ile kontrol noktasý durumunu tüm oyunculara senkronize eder.
    [PunRPC]
    void SetCheckPoint(int value)
    {
        checkpointkontrol = value;
    }
}
