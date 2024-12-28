using UnityEngine;

public class UserFirebaseInformation : MonoBehaviour
{

    public string UserName { get; set; }
    public int Coin { get; set; }

    public int Gem {  get; set; }
    public int Win {  get; set; }
    public string Email {  get; set; } 
    public int Level {  get; set; }

    public bool removeAd {  get; set; }

    public string[] UsernameArrays { get; set; } = new string[10];

    public int[] WinArrays { get; set; } = new int[10];

    public static UserFirebaseInformation instance;

    private void Awake()
    {

        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(gameObject);

        }

        else
        {

            Destroy(gameObject);

        }
    }

}
