using UnityEngine;
using UnityEngine.UI;

public class RestartGameButton : MonoBehaviour
{

    Button restartButton;

    private void Awake()
    {
        restartButton = GetComponent<Button>();
    }

    void Start()
    {
        restartButton.onClick.AddListener(delegate { RequestGameRestart(); });
    }

    
    private void RequestGameRestart()
    {

        OnlineGameManager.instance.RequestGameRestart();
        restartButton.interactable = false;

    }
    
}
