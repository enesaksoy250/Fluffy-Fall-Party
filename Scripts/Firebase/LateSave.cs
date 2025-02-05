using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LateSave : MonoBehaviour
{
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(delegate { Save(); });
    }

    private void Save()
    {

        AuthenticationManager.instance.SaveUser(true);

    }
}
