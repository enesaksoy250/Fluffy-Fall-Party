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


    public string userID;
    private DatabaseReference dbReference;


    public bool dataExtractionFinished = false;

    public string userType;

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

 
    public void Initialize(string userId, string userType,DatabaseReference dbReference)
    {
        userID = userId;
        this.userType = userType;
        this.dbReference = dbReference;
        GetInfoFromFirebase(true);
        GetWinOnLeaderboard();
        Invoke(nameof(PrintTopTenUsers), 2);
    }

    public void GetUserDataFromFirebase(string key, Action<object> onSuccess)
    {
    
        dbReference.Child(userType).Child(userID).Child(key).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Veritabanýndan {key} alýnýrken bir hata oluþtu: " + task.Exception);
                print("UserType deðeri=" + userType);
                GetUserDataFromFirebase(key, onSuccess);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    onSuccess(snapshot.Value);
                }
                else
                {
                    Debug.LogError($"{key} deðeri alýnamadý!");
                    GetUserDataFromFirebase(key, onSuccess);
                }
            }
        });
    }

    public void GetInfoFromFirebase(bool login, Action onComplete = null)
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
        dbReference.Child(userType).Child(userID).Child(statName).GetValueAsync().ContinueWithOnMainThread(task =>
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
                    dbReference.Child(userType).Child(userID).Child(statName).SetValueAsync(newValue);
                    UpdateUserInfo(statName, newValue);
                    onComplete?.Invoke();

                }

            }
        });
    }

    public void UpdateFirebaseInfo(string statName, int newValue)
    {
        dbReference.Child(userType).Child(userID).Child(statName).SetValueAsync(newValue).ContinueWithOnMainThread(task =>
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
        dbReference.Child(userType).Child(userID).Child(statName).SetValueAsync(newValue).ContinueWithOnMainThread(task =>
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

    public void UpdateFirebaseInfo(string statName, string newValue, Action onComplete = null)
    {
        dbReference.Child(userType).Child(userID).Child(statName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Veritabanýndan {statName} alýnýrken bir hata oluþtu: " + task.Exception);
                UpdateFirebaseInfo(statName, newValue);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                dbReference.Child(userType).Child(userID).Child(statName).SetValueAsync(newValue);
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


        if (userType == "registered")
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
                Debug.LogError("Kullanýcý oturum açmamýþ!");
                return;
            }


        }

        DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");


        leaderboardRef.Child(userID).RunTransaction(mutableData =>
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
                GetWinOnLeaderboard();
            }
        });

    }

    public void ChangeUserIdOnLeaderboard(string lastUserID,string newUserID,string username)
    {

        DatabaseReference leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");

        leaderboardRef.Child(lastUserID).GetValueAsync().ContinueWith(task => 
        {

            if (task.IsCompleted && task.Result.Exists)
            {

                int win =int.Parse(task.Result.Child("win").Value.ToString());

                Dictionary<string, object> newUserData = new Dictionary<string, object>
                {

                    {"username",username},
                    {"win",win}

                };

                leaderboardRef.Child(newUserID).SetValueAsync(newUserData).ContinueWith(setTask =>
                {

                    if (setTask.IsCompleted)
                    {

                        leaderboardRef.Child(lastUserID).RemoveValueAsync().ContinueWith(removeTask =>
                        {

                            if (removeTask.IsCompleted)
                            {
                                Debug.Log("Leaderboard silme iþlemi tamamlandý");
                            }

                            else
                            {
                                Debug.LogError("Leaderboard silme iþlemi hatasý");
                            }

                        });

                    }

                    else
                    {

                        Debug.LogError("Yeni leaderboard verisi kaydedilemedi");

                    }


                });

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

    public void GetWinOnLeaderboard()
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

       
            if (snapshot.Exists)
            {
                
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
            .Child("registered")
            .Child(userID)
            .Child("characters");


        for (int i = 0; i < characters.Length; i++)
        {
            CharacterInfo characterInfo = characters[i].GetComponent<CharacterInfo>();


            var characterData = new Dictionary<string, object>
            {
               
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
            .Child("registered")
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
               
                characterInfo.isPurchased = bool.Parse(child.Child("isPurchased").Value.ToString());
            }

            CharacterSelection.instance.SaveCharacterData();
            Debug.Log("Character data loaded successfully.");
        });
    }

    public void UpdateCharacterField(int characterIndex, string fieldName, bool value)
    {

        DatabaseReference databaseRef = FirebaseDatabase.DefaultInstance.RootReference
            .Child("registered")
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

  
}
