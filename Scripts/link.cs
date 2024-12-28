using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class link : MonoBehaviour
{
    public string url;

    public void instaURL()
    {
        Application.OpenURL(url);
    }
}
