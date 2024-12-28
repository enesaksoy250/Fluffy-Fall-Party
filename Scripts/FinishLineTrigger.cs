using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{


    GameObject winnerPlayer;
    string winnerName;
    int actorNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && OnlineGameManager.instance.isPlay)
        {

            OnlineGameManager.instance.SetIsPlay(false);
            winnerPlayer = other.gameObject;
            Animator animator = winnerPlayer.GetComponent<Animator>();
            animator.SetBool("IsRunning", false);
            animator.SetTrigger("Win");
            PhotonView pw = winnerPlayer.GetPhotonView();
            winnerName = pw.Owner.NickName;
            actorNumber = pw.Owner.ActorNumber;
            OnlineGameManager.instance.FinishGame(winnerName);
            winnerPlayer.GetComponent<CharacterController>().UpdateLeaderboard(winnerName, 1);
            MoveButton.instance.ResetButtonPosition();
            Invoke(nameof(ShowEndGamePanels), 2);
            

        }
    }

    private void ShowEndGamePanels()
    {

        OnlineGameManager.instance.ShowEndGamePanels(actorNumber);

    }

}
