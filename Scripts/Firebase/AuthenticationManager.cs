using Firebase.Auth;
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;


public class AuthenticationManager : MonoBehaviour
{

    FirebaseAuth auth;
    DatabaseReference dbReference;

    [SerializeField] TMP_InputField saveUsernameInput,saveEmailInput,savePasswordInput;
    [SerializeField] TMP_InputField lateSaveUsernameInput,lateSaveEmailInput,lateSavePasswordInput;
    [SerializeField] TMP_InputField loginEmailInput,loginPasswordInput;

    private string userType;
    private string userID;

    public bool onConnectDatabase;

    public static AuthenticationManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoginControl();

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => 
        { 
        
           if(task.Result == DependencyStatus.Available) 
           {

                Debug.Log("Firebase'e baðlandý!");
                onConnectDatabase = true;
                auth = FirebaseAuth.DefaultInstance;
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                userType = PlayerPrefs.GetString("UserType");

                if(userType == "registered")
                {
                    userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
                }

                else if(userType == "guest")
                {
                    userID = SystemInfo.deviceUniqueIdentifier;
                }

                else
                {
                    return;
                }
                DataBaseManager.instance.Initialize(userID, userType,dbReference);
            } 
        
        
        });

    }

    public void SaveUser(bool isLateSave)
    {

        string username;
        string email;
        string password;

        if (isLateSave)
        {
            username = lateSaveUsernameInput.text;
            email = lateSaveEmailInput.text;
            password = lateSavePasswordInput.text;
        }

        else
        {
            username = saveUsernameInput.text;
            email = saveEmailInput.text;
            password = savePasswordInput.text;
        }

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowErrorPanel("ErrorPanel2");
            Debug.LogError("Tüm alanlarý doldurunuz!");
            return;
        }

        MainMenuPanelManager.instance.LoadPanel("GameLoadingPanel");

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            

            if (task.IsFaulted)
            {
                HandleAuthError(task.Exception);
            }
            else
            {
                string lastUserID=userID;
                FirebaseUser newUser = task.Result.User;
                userID = newUser.UserId;
                Debug.Log("Kullanýcý kaydý baþarýlý: " + newUser.UserId);

                if (isLateSave)
                    WriteNewUser(username, email, userID, true, lastUserID);
                else
                    WriteNewUser(username, email, userID, false);
            }
        });
    }

    private void WriteNewUser(string username, string email, string userId, bool isLateSave,string lastUserID=null)
    {
        string registrationDate = System.DateTime.UtcNow.ToString("dd-MM-yyyy");

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            {"username", username},
            {"email", email},
            {"coin", isLateSave ? UserFirebaseInformation.instance.Coin : 0},
            {"gem", isLateSave ? UserFirebaseInformation.instance.Gem : 0},
            {"level", isLateSave ? PlayerPrefs.GetInt("Level") : 1},
            {"removeAd", isLateSave && UserFirebaseInformation.instance.removeAd},
            {"date", registrationDate}
        };

        dbReference.Child("registered").Child(userId).SetValueAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Kayýt baþarýlý!");
                FinalizeSaveProcess(isLateSave,username,lastUserID);
            }
            else
            {
                Debug.LogError("Kayýt hatasý.");
            }
        });
    }

    private void FinalizeSaveProcess(bool isLateSave,string username,string lastUserId=null)
    {

        string panelName = isLateSave ? "SignUpPanel2" : "SignUpPanel";
        StartCoroutine(ClosePanel(2,panelName,"GameLoadingPanel"));
        PlayerPrefs.SetString("UserType", "registered");
        userType = "registered";
        DataBaseManager.instance.userType = userType;
        DataBaseManager.instance.userID = userID;

        UserFirebaseInformation.instance.UserName = username;
        CharacterSelection.instance.InitializeCharacters();
        DataBaseManager.instance.SaveCharacterDataToFirebase(CharacterSelection.instance.characters);
  
        DataBaseManager.instance.GetInfoFromFirebase(false);
        DataBaseManager.instance.GetWinOnLeaderboard();
        DataBaseManager.instance.PrintTopTenUsers();
        Invoke(nameof(SetInfo), 1);

        if (isLateSave)
        {
            DeleteUserByID("guest",lastUserId);
            DataBaseManager.instance.ChangeUserIdOnLeaderboard(lastUserId, userID,username);
        }
    }

    public void Login()
    {
        string enteredEmail = loginEmailInput.text;
        string enteredPassword = loginPasswordInput.text;

        if (string.IsNullOrEmpty(enteredEmail) || string.IsNullOrEmpty(enteredPassword))
        {
            MainMenuPanelManager.instance.LoadPanel("ErrorPanel2");
            Debug.LogError("Email ve þifre boþ býrakýlamaz!"); //Error2
            return;
        }

        MainMenuPanelManager.instance.LoadPanel("GameLoadingPanel");


        auth.SignInWithEmailAndPasswordAsync(enteredEmail, enteredPassword).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {

                MainMenuPanelManager.instance.ClosePanel("GameLoadingPanel");
                HandleLoginError(task.Exception);
            }
            else
            {

                FirebaseUser user = task.Result.User;
                Debug.Log("Giriþ baþarýlý! UserID: " + user.UserId);
                PlayerPrefs.SetString("UserType", "registered");
                userType = "registered";
                userID = user.UserId;
                DataBaseManager.instance.userType = userType;
                DataBaseManager.instance.userID = userID;
                StartCoroutine(ClosePanel(2, "GameLoadingPanel","LoginPanel"));          
                DataBaseManager.instance.GetInfoFromFirebase(false);
                DataBaseManager.instance.LoadCharacterDataFromFirebase(CharacterSelection.instance.characters);

            }
        });
    }

    public void GuestLogin()
    {

        MainMenuPanelManager.instance.LoadPanel("GameLoadingPanel");
        userID =SystemInfo.deviceUniqueIdentifier;
        string randomUsername = "guest" + UnityEngine.Random.Range(1, 99999);
        string randomEmail = randomUsername + "@gmail.com";
        string registrationDate = System.DateTime.UtcNow.ToString("dd-MM-yyyy");

        Dictionary<string, object> userData = new Dictionary<string, object>
        {

            {"username",randomUsername},
            {"email",randomEmail},
            {"coin",0},
            {"gem",0},
            {"level",1},
            {"removeAd",false},
            {"date",registrationDate}

        };


        //User guestUser = new User(randomUsername, randomEmail, 0, 0, 1, false);
        //string json = JsonUtility.ToJson(guestUser);

        dbReference.Child("guest").Child(userID).SetValueAsync(userData).ContinueWithOnMainThread(task =>
        {

            if (task.IsCompleted)
            {

                Debug.Log("Misafir kayýt baþarýlý");
                PlayerPrefs.SetString("UserType", "guest");
                userType = "guest";
                DataBaseManager.instance.userType = userType;
                DataBaseManager.instance.userID = userID;
                StartCoroutine(ClosePanel(2, "GameLoadingPanel", "ChoosePanel"));
                CharacterSelection.instance.InitializeCharacters();
                DataBaseManager.instance.GetInfoFromFirebase(false);
                DataBaseManager.instance.PrintTopTenUsers();

            }
            else
            {
                Debug.Log("Misafir kayýt baþarýsýz");

            }

        });
    }

    private void LoginControl()
    {

        if (PlayerPrefs.GetString("UserType", "null") == "null")
        {

            MainMenuPanelManager.instance.ClosePanel("LoadingPanel");
            MainMenuPanelManager.instance.LoadPanel("LanguagePanel");

        }

        userType = PlayerPrefs.GetString("UserType");
    }

    private void DeleteUserByID(string userType,string userId)
    {

        DatabaseReference userToDelete = dbReference.Child(userType).Child(userId);

        userToDelete.RemoveValueAsync().ContinueWith(task =>
        {
            
            if (task.IsCompleted)
            {

                print("Silme iþlemi tamamlandý");

            }
            else
            {

                print("Silme iþleminde hata oluþtu");

            }

        });

    }

    private void SetInfo()
    {

        UIDataLoader.instance.SetInfo();

    }

    private void HandleAuthError(AggregateException exception)
    {
        foreach (var innerException in exception.Flatten().InnerExceptions)
        {
            if (innerException is FirebaseException firebaseEx)
            {
                switch (firebaseEx.ErrorCode)
                {
                    case (int)AuthError.EmailAlreadyInUse:
                        ShowErrorPanel("ErrorPanel3");
                        Debug.LogError("Bu e-posta adresi zaten kullanýlýyor.");
                        return;

                    case (int)AuthError.InvalidEmail:
                        ShowErrorPanel("ErrorPanel4");
                        Debug.LogError("Email formatý yanlýþ.");
                        return;

                    default:
                        ShowErrorPanel("ErrorPanel");
                        Debug.LogError("Bilinmeyen bir hata oluþtu.");
                        return;
                }
            }
        }
        Debug.LogError("Kullanýcý kaydýnda hata: " + exception);
    }

    private void HandleLoginError(AggregateException exception)
    {
        foreach (var innerException in exception.Flatten().InnerExceptions)
        {
            if (innerException is FirebaseException firebaseEx)
            {
                switch (firebaseEx.ErrorCode)
                {
                    case (int)AuthError.InvalidEmail:
                        ShowErrorPanel("ErrorPanel4");
                        return;

                    case (int)AuthError.WrongPassword:
                        ShowErrorPanel("ErrorPanel5");
                        return;

                    case (int)AuthError.UserNotFound:
                        ShowErrorPanel("ErrorPanel6");
                        return;

                    default:
                        ShowErrorPanel("ErrorPanel");
                        return;
                }
            }
        }
        Debug.LogError("Giriþ hatasý: " + exception);
    }
  
    private void ShowErrorPanel(string panelName)
    {
        MainMenuPanelManager.instance.LoadPanel(panelName);
    }

    IEnumerator ClosePanel(int time,params string [] panelName)
    {

        yield return new WaitForSeconds(time);

        foreach(string panel in panelName)
        {

            MainMenuPanelManager.instance.ClosePanel(panel);

        }

    }

}
