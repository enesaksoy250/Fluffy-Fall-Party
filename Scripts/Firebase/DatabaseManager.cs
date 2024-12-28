using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;
using System.Linq;


public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager instance;

    public TMP_InputField UserNameInput, EmailInput, passwordInput;

    private string userID;
    private DatabaseReference dbReference;


    [SerializeField] TMP_InputField loginEmailField, loginPasswordField;

    private string username;
    private string email;

    public bool onConnectDatabase, dataExtractionFinished = false;
    private bool closeLoadingPanel = false;

    FirebaseAuth auth;

 

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
    void Start()
    {

        LoginControl();

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    Debug.Log("Firebase'e baþarýyla baðlanýldý!");
                    onConnectDatabase = true;
                    auth = FirebaseAuth.DefaultInstance;
                    dbReference = FirebaseDatabase.DefaultInstance.RootReference;

                    if (PlayerPrefs.HasKey("Login"))
                    {
                        userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
                        GetInfoFromFirebase(true);
                        GetWinOnLeaderboard();
                        Invoke(nameof(PrintTopTenUsers),2);

                    }
                }
                else
                {
                    Debug.LogError($"Firebase baðlantý hatasý: {task.Result}");
                }
            });

      
    }


    private void LoginControl()
    {

        if (!PlayerPrefs.HasKey("Login"))
        {

            MainMenuPanelManager.instance.ClosePanel("LoadingPanel");
            MainMenuPanelManager.instance.LoadPanel("LanguagePanel");

        }

    }

    public void Save()
    {
        string username = UserNameInput.text;
        string email = EmailInput.text;
        string password = passwordInput.text; // Yeni alan

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Tüm alanlarý doldurunuz!");
            MainMenuPanelManager.instance.LoadPanel("ErrorPanel2");//Error2
            return;
        }

        MainMenuPanelManager.instance.LoadPanel("GameLoadingPanel");


        CheckIfUsernameExists(username, (exists) =>
        {
            if (exists)
            {
                MainMenuPanelManager.instance.ClosePanel("GameLoadingPanel");
                MainMenuPanelManager.instance.LoadPanel("ErrorPanel3");
                Debug.LogError("Kullanýcý adý daha önce alýnmýþ."); //Error3
            }
            else
            {

                auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        MainMenuPanelManager.instance.ClosePanel("GameLoadingPanel");
                        MainMenuPanelManager.instance.LoadPanel("ErrorPanel4"); //Email formatý yanlýþ
                        Debug.LogError("Kullanýcý kaydýnda hata: " + task.Exception); //Error1
                    }
                    else
                    {

                        FirebaseUser newUser = task.Result.User;
                        userID = newUser.UserId;
                        Debug.Log("Kullanýcý kaydý baþarýlý: " + newUser.UserId);

                        // Yeni kullanýcý verilerini Firebase Realtime Database'e kaydet
                        WriteNewUser(username, email, userID);
                    }
                });
            }
        });
    }


    private void CheckIfUsernameExists(string username, Action<bool> callback)
    {
        dbReference.Child("users").OrderByChild("username").EqualTo(username)
          .GetValueAsync().ContinueWithOnMainThread(task =>
          {
              if (task.IsFaulted)
              {
                  //MainMenuPanelManager.instance.ClosePanel("GameLoadingPanel");
                  //MainMenuPanelManager.instance.LoadPanel("ErrorPanel");
                  Debug.LogError("Kullanýcý adý kontrol edilirken hata oluþtu: " + task.Exception); //Error1
                  callback(false);
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  callback(snapshot.Exists);
              }
          });
    }

    private void WriteNewUser(string username, string email, string userId)
    {
        User user = new User(username, email, 0, 0, 1,false);
        string json = JsonUtility.ToJson(user);

        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Kayýt baþarýlý!");
                Invoke(nameof(CloseGameLoadingPanel), 2);
                Invoke(nameof(CloseSignUpPanel), 3);
                PlayerPrefs.SetInt("Login", 1);
                UserFirebaseInformation.instance.UserName = username;
                CharacterSelection.instance.InitializeCharacters();
                SaveCharacterDataToFirebase(CharacterSelection.instance.characters);
                GetInfoFromFirebase(false);
                GetWinOnLeaderboard();
                PrintTopTenUsers();
                Invoke(nameof(SetInfo), 1);
            }
            else
            {
                //MainMenuPanelManager.instance.ClosePanel("GameLoadingPanel");
                //MainMenuPanelManager.instance.LoadPanel("ErrorPanel");
                Debug.LogError("Kayýt hatasý."); //Error1
            }
        });
    }

    public void Login()
    {
        string enteredEmail = loginEmailField.text;
        string enteredPassword = loginPasswordField.text;

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
                Debug.LogError("Giriþ hatasý: " + task.Exception);
                MainMenuPanelManager.instance.ClosePanel("GameLoadingPanel");
                MainMenuPanelManager.instance.LoadPanel("ErrorPanel");

            }
            else
            {

                FirebaseUser user = task.Result.User;
                Debug.Log("Giriþ baþarýlý! UserID: " + user.UserId);
                PlayerPrefs.SetInt("Login", 1);
                userID = user.UserId;
                Invoke(nameof(CloseGameLoadingPanel), 2);
                Invoke(nameof(CloseLoginPanel), 2);
                GetInfoFromFirebase(false);
                LoadCharacterDataFromFirebase(CharacterSelection.instance.characters);

            }
        });
    }


    public void GetUserDataFromFirebase(string key, Action<object> onSuccess)
    {

        dbReference.Child("users").Child(userID).Child(key).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Veritabanýndan {key} alýnýrken bir hata oluþtu: " + task.Exception);
                GetUserDataFromFirebase(key, onSuccess);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    onSuccess(snapshot.Value);
                    LoadingManager.instance.UpdateProgress();
                }
                else
                {
                    Debug.LogError($"{key} deðeri alýnamadý!");
                    GetUserDataFromFirebase(key, onSuccess);
                }
            }
        });
    }

    public void GetInfoFromFirebase(bool login,Action onComplete=null)
    {


        var dataKeys = new Dictionary<string, Action<object>>
       {
        { "username", value => UserFirebaseInformation.instance.UserName = value.ToString() },
        { "coin", value => UserFirebaseInformation.instance.Coin = int.Parse(value.ToString()) },
        { "email", value => UserFirebaseInformation.instance.Email = value.ToString() },
        { "gem", value => UserFirebaseInformation.instance.Gem = int.Parse(value.ToString())},
        { "level", value => UserFirebaseInformation.instance.Level = int.Parse(value.ToString())},
        {  "removeAd", value => UserFirebaseInformation.instance.removeAd = Convert.ToBoolean(value) }
       };

        int remainingKeys = dataKeys.Count;

        foreach (var entry in dataKeys)
        {
            GetUserDataFromFirebase(entry.Key, value =>
            {
                entry.Value(value);
                print(value);
                remainingKeys--;

                if (remainingKeys == 0)
                {
                 
                    dataExtractionFinished = true;                              
                    onComplete?.Invoke();

                    if (!login)
                    {
                        PlayerPrefs.SetInt("Level", UserFirebaseInformation.instance.Level);
                        LevelSystem.instance.currentLevel = PlayerPrefs.GetInt("Level");

                    }

                }
            });
        }
    }

    public void IncreaseFirebaseInfo(string statName, int incrementValue, Action onComplete = null)
    {
        dbReference.Child("users").Child(userID).Child(statName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Veritabanýndan {statName} alýnýrken bir hata oluþtu: " + task.Exception);
            }
            else if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {

                    int currentValue = int.Parse(snapshot.Value.ToString());
                    int newValue = currentValue + incrementValue;
                    dbReference.Child("users").Child(userID).Child(statName).SetValueAsync(newValue);
                    UpdateUserInfo(statName, newValue);
                    onComplete?.Invoke();

                }

            }
        });
    }

   

    public void UpdateFirebaseInfo(string statName, int newValue)
    {
        dbReference.Child("users").Child(userID).Child(statName).SetValueAsync(newValue).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Veritabanýnda {statName} güncellenirken bir hata oluþtu: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"{statName} baþarýyla {newValue} olarak güncellendi.");
                UpdateUserInfo(statName, newValue);
            }
        });
    }

    public void UpdateFirebaseInfo(string statName, bool newValue)
    {
        dbReference.Child("users").Child(userID).Child(statName).SetValueAsync(newValue).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Veritabanýnda {statName} güncellenirken bir hata oluþtu: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"{statName} baþarýyla {newValue} olarak güncellendi.");
                UpdateUserInfo(statName, newValue);
            }
        });
    }

    public void UpdateFirebaseInfo(string statName, string newValue,Action onComplete = null)
    {
        dbReference.Child("users").Child(userID).Child(statName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Veritabanýndan {statName} alýnýrken bir hata oluþtu: " + task.Exception);
                UpdateFirebaseInfo(statName, newValue);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                dbReference.Child("users").Child(userID).Child(statName).SetValueAsync(newValue);
                UpdateUserInfo(statName, newValue);
                onComplete?.Invoke();
            }
        });
    }

    private void UpdateUserInfo(string statName, int newValue)
    {

        switch (statName)
        {

            case "coin":
                UserFirebaseInformation.instance.Coin = newValue;
                break;

            case "win":
                UserFirebaseInformation.instance.Win = newValue;
                break;

            case "level":
                UserFirebaseInformation.instance.Level = newValue;
                break;

            case "gem":
                UserFirebaseInformation.instance.Gem = newValue;
                break;

        }

    }

    private void UpdateUserInfo(string statName, string newValue)
    {

        switch (statName)
        {

            case "email":
                UserFirebaseInformation.instance.Email = newValue;
                break;

            case "username":
                UserFirebaseInformation.instance.UserName = newValue;
                UIDataLoader.instance.SetInfo();
                MainMenuPanelManager.instance.ClosePanel("ChangeUsernamePanel");
                IncreaseFirebaseInfo("gem", -10);
                break;

        }

    }

    private void UpdateUserInfo(string statName, bool newValue)
    {

        switch (statName)
        {

            case "removeAd":
                UserFirebaseInformation.instance.removeAd = newValue;
                break;

        }

    }


    public void PrintTopTenUsers()
    {
        DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");
        leaderboardRef.OrderByChild("win").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve leaderboard data: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            List<KeyValuePair<string, int>> leaderboard = new List<KeyValuePair<string, int>>();

            foreach (var childSnapshot in snapshot.Children)
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int win = int.Parse(childSnapshot.Child("win").Value.ToString());
                leaderboard.Add(new KeyValuePair<string, int>(username, win));
            }


            leaderboard = leaderboard.OrderByDescending(pair => pair.Value).ToList();

            int i = 0;
            foreach (var entry in leaderboard)
            {
                Debug.Log($"{entry.Key}: {entry.Value} wins");
                UserFirebaseInformation.instance.UsernameArrays[i] = entry.Key;
                UserFirebaseInformation.instance.WinArrays[i] = entry.Value;
                i++;
            }

            
        });
    }


    public void UpdateLeaderboard(string username, int win)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.LogError("Kullanýcý oturum açmamýþ!");
            return;
        }

        DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");

        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // Mevcut 'win' deðerini almak için
        leaderboardRef.Child(userId).RunTransaction(mutableData =>
        {
            if (mutableData.Value == null)
            {
                mutableData.Value = new Dictionary<string, object>
        {
            { "username", username },
            { "win", 1 }
        };
            }
            else
            {
                Dictionary<string, object> currentData = (Dictionary<string, object>)mutableData.Value;
                currentData["win"] = int.Parse(currentData["win"].ToString()) + win;
                mutableData.Value = currentData;
            }

            return TransactionResult.Success(mutableData);
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Transaction baþarýsýz: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Transaction baþarýyla tamamlandý.");
            }
        });

    }


    public void UpdateUsernameOnLeaderboard(string newUsername)
    {
        DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");

     
        leaderboardRef.Child(userID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Veri alýnýrken hata oluþtu: " + task.Exception);
            }
            else if (task.IsCompleted && task.Result.Exists)
            {
                
                Dictionary<string, object> currentData = (Dictionary<string, object>)task.Result.Value;

                if (currentData.ContainsKey("username"))
                {
                  
                    currentData["username"] = newUsername;

               
                    leaderboardRef.Child(userID).SetValueAsync(currentData).ContinueWithOnMainThread(updateTask =>
                    {
                        if (updateTask.IsFaulted)
                        {
                            Debug.LogError("Kullanýcý adý güncellenirken hata oluþtu: " + updateTask.Exception);
                        }
                        else
                        {
                            Debug.Log("Kullanýcý adý baþarýyla güncellendi.");
                            PrintTopTenUsers();
                        }
                    });
                }
                else
                {
                    Debug.LogError("Veri içinde 'username' alaný bulunamadý.");
                }
            }
            else
            {
                Debug.LogError("Belirtilen userID bulunamadý.");
            }
        });
    }



    private void GetWinOnLeaderboard()
    {



        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference
            .Child("leaderboard")
            .Child(userID);


        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Kullanýcý verisi alýnamadý.");
                GetWinOnLeaderboard();
                return;
            }

            DataSnapshot snapshot = task.Result;

            // Veritabanýnda verinin olup olmadýðýný kontrol et
            if (snapshot.Exists)
            {
                // "win" deðerini al ve integer'a çevir
                int userWins = int.Parse(snapshot.Child("win").Value.ToString());

                UserFirebaseInformation.instance.Win = userWins;

            }
            else
            {
                Debug.Log("Kullanýcý verisi bulunamadý.");
            }
        });

    }


    public void SaveCharacterDataToFirebase(GameObject[] characters)
    {


        DatabaseReference databaseRef = FirebaseDatabase.DefaultInstance.RootReference
            .Child("users")
            .Child(userID)
            .Child("characters");


        for (int i = 0; i < characters.Length; i++)
        {
            CharacterInfo characterInfo = characters[i].GetComponent<CharacterInfo>();


            var characterData = new Dictionary<string, object>
            {
                { "isUnlocked", characterInfo.isUnlocked },
                { "isPurchased", characterInfo.isPurchased }
            };


            databaseRef.Child(i.ToString()).SetValueAsync(characterData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Character {i} data saved to Firebase.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError($"Failed to save Character {i} data: {task.Exception}");
                }
            });
        }
    }

    public void LoadCharacterDataFromFirebase(GameObject[] characters)
    {


        DatabaseReference databaseRef = FirebaseDatabase.DefaultInstance.RootReference
            .Child("users")
            .Child(userID)
            .Child("characters");


        databaseRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load character data.");
                LoadCharacterDataFromFirebase(characters);
                return;
            }

            DataSnapshot snapshot = task.Result;


            foreach (var child in snapshot.Children)
            {
                int characterIndex = int.Parse(child.Key);
                CharacterInfo characterInfo = characters[characterIndex].GetComponent<CharacterInfo>();


                characterInfo.isUnlocked = bool.Parse(child.Child("isUnlocked").Value.ToString());
                characterInfo.isPurchased = bool.Parse(child.Child("isPurchased").Value.ToString());
            }

            CharacterSelection.instance.SaveCharacterData();
            Debug.Log("Character data loaded successfully.");
        });
    }


    public void UpdateCharacterField(int characterIndex, string fieldName, bool value)
    {

        DatabaseReference databaseRef = FirebaseDatabase.DefaultInstance.RootReference
            .Child("users")
            .Child(userID)
            .Child("characters")
            .Child(characterIndex.ToString())
            .Child(fieldName);


        databaseRef.SetValueAsync(value).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Character {characterIndex} {fieldName} updated to {value}.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError($"Failed to update Character {characterIndex}: {task.Exception}");
            }
        });
    }

    private void CloseSignUpPanel()
    {

        MainMenuPanelManager.instance.ClosePanel("SignUpPanel");

    }

    private void CloseGameLoadingPanel()
    {

        MainMenuPanelManager.instance.ClosePanel("GameLoadingPanel");

    }

    private void CloseLoginPanel()
    {

        MainMenuPanelManager.instance.ClosePanel("LoginPanel");

    }

    private void SetInfo()
    {

        UIDataLoader.instance.SetInfo();

    }
}
