using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{

    [SerializeField]CharacterDatabase characterDatabase;
    public GameObject[] characters;
    [SerializeField] TextMeshProUGUI nameText, priceText,currentIndexText,lockText;
    [SerializeField] Button selectButton, buyButton;
    [SerializeField] Image lockImage;
    [SerializeField] Image [] speedImages, jumpImages;
    [SerializeField] Image infoPanelImage,mainPanel,payImage;
    [SerializeField] Sprite coinSprite, gemSprite;
    private int currentIndex,playerLevel;

    private Color32 legendColor = new (177,0,255,213);
    private Color32 legendBackgroundColor = new(255,160,210,255);
    private Color32 normalColor = new (0,137,255,213);
  

    public static CharacterSelection instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        if (PlayerPrefs.HasKey("Login"))
        {

            LoadCharacterData();

        }

    

        currentIndex = PlayerPrefs.GetInt("SelectedCharacter");
     
       
    }

    public void InitializeCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {

            CharacterInfo characterInfo = characters[i].GetComponent<CharacterInfo>();

            if (i < 6)
            {
                characterInfo.isUnlocked = true;
                characterInfo.isPurchased = true;
            }
            else if (i < 8 || i > 35)
            {
                characterInfo.isUnlocked = true;
                characterInfo.isPurchased = false;
            }
            else
            {
                characterInfo.isUnlocked = false;
                characterInfo.isPurchased = false;
            }
        }
    
        SaveCharacterData();
  
    }

    public void SaveCharacterData()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            CharacterInfo characterInfo = characters[i].GetComponent<CharacterInfo>();
            PlayerPrefs.SetInt("Character_" + i + "_Unlocked", characterInfo.isUnlocked ? 1 : 0);
            PlayerPrefs.SetInt("Character_" + i + "_Purchased", characterInfo.isPurchased ? 1 : 0);
     
        }


        PlayerPrefs.SetInt("Login", 1);
    }

    void LoadCharacterData()
    {
        playerLevel = PlayerPrefs.GetInt("Level", 1);

        for (int i = 0; i < characters.Length; i++)
        {
            bool isUnlocked = false;

           
            if (i < 8 || i >= characters.Length-5)
            {
                isUnlocked = true;
            }
                     
            else
            {
                isUnlocked = i < playerLevel + 7;
            }

         
            isUnlocked = isUnlocked || PlayerPrefs.GetInt("Character_" + i + "_Unlocked", 0) == 1;

  
            characters[i].GetComponent<CharacterInfo>().isUnlocked = isUnlocked;
            characters[i].GetComponent<CharacterInfo>().isPurchased = PlayerPrefs.GetInt("Character_" + i + "_Purchased", 0) == 1;
        }
    }


    public void SelectCharacter()
    {

        if (!characters[currentIndex].GetComponent<CharacterInfo>().isPurchased) return;

        PlayerPrefs.SetInt("SelectedCharacter", currentIndex);
        SetSelectButtonText();

    }

    public void BuyCharacter()
    {
      
        if (!characters[currentIndex].GetComponent<CharacterInfo>().isUnlocked) return;

        int characterPrice = characters[currentIndex].GetComponent<CharacterInfo>().price;
        int playerCoin = UserFirebaseInformation.instance.Coin;
        int playerGem = UserFirebaseInformation.instance.Gem;

       
        if (playerCoin >= characterPrice && currentIndex < 36)
        {
            ProcessPurchase("coin", -characterPrice);
        }
        else if (playerGem >= characterPrice && currentIndex > 35)
        {
            ProcessPurchase("gem", -characterPrice);
        }
        else
        {
            print($"Yetersiz kaynak! Altýn miktarýnýz = {playerCoin}, Elmas miktarýnýz = {playerGem}");
        }
    }

   
    private void ProcessPurchase(string currency, int amount)
    {
        DataBaseManager.instance.IncreaseFirebaseInfo(currency, amount, () =>
        {
            PlayerPrefs.SetInt("Character_" + currentIndex + "_Purchased", 1);
            characters[currentIndex].GetComponent<CharacterInfo>().isPurchased = true;
            DataBaseManager.instance.UpdateCharacterField(currentIndex, "isPurchased", true);
            UIDataLoader.instance.SetCharacterSelectionPanelInfo();
            SaveCharacterData();
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
        });
    }





    public void UnlockCharacterOnLevelUp()
    {
        int unlockIndex = playerLevel + 7;
        if (unlockIndex < characters.Length-5)
        {
            characters[unlockIndex].GetComponent<CharacterInfo>().isUnlocked = true;
            PlayerPrefs.SetInt("Character_" + unlockIndex + "_Unlocked", 1);
            DataBaseManager.instance.UpdateCharacterField(unlockIndex, "isUnlocked", true);
            SaveCharacterData();
        }
    }

    public void LevelUp() //LEVEL ARTINCA ÇAÐRILACAK
    {
        playerLevel++;
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        DataBaseManager.instance.UpdateFirebaseInfo("level", playerLevel);
        UnlockCharacterOnLevelUp();
        Debug.Log("Level Up! Current level: " + playerLevel);
    }

    public void NextCharacter()
    {
        characters[currentIndex].SetActive(false);
        characters[currentIndex].transform.rotation = Quaternion.Euler(0, 200, 0);
        currentIndex = (currentIndex + 1) % characters.Length;
        characters[currentIndex].SetActive(true);
        currentIndexText.text = currentIndex.ToString();

        if (currentIndex > 35) { infoPanelImage.color = legendColor; mainPanel.color = legendBackgroundColor; payImage.sprite = gemSprite; }

        else { infoPanelImage.color = normalColor; mainPanel.color = Color.white; payImage.sprite = coinSprite; }


        UpdateCharacterPanel();
        UpdateCharacterInfo();
    }

    public void PreviousCharacter()
    {
        characters[currentIndex].SetActive(false);
        characters[currentIndex].transform.rotation = Quaternion.Euler(0, 200, 0);
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        characters[currentIndex].SetActive(true);
        currentIndexText.text = currentIndex.ToString();

        if (currentIndex > 35) { infoPanelImage.color = legendColor; mainPanel.color = legendBackgroundColor; payImage.sprite = gemSprite; }

        else { infoPanelImage.color = normalColor; mainPanel.color = Color.white; payImage.sprite = coinSprite; }

        UpdateCharacterPanel();
        UpdateCharacterInfo();
    }


    public void UpdateCharacterPanel()
    {

        CharacterInfo characterInfo = characters[currentIndex].GetComponent<CharacterInfo>();

        if (!characterInfo.isUnlocked)
        {
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
            lockText.text = PlayerPrefs.GetString("Language") == "English" ? "Level " + (currentIndex - 6) + " required!" : "Seviye " + (currentIndex - 6) + " gerekli!";
            return; 
        }

       
        lockImage.gameObject.SetActive(false);
      
        if (!characterInfo.isPurchased)
        {
            buyButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
        }
        else
        {
          
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
            SetSelectButtonText();
            
        }


    }

   
    private void SetSelectButtonText()
    {

        if (currentIndex == PlayerPrefs.GetInt("SelectedCharacter"))
        {

          
            selectButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString("Language") == "English" ? "SELECTED"
                                                                                                 : "SECÝLÝ";
        

        }

        else
        {

            selectButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString("Language") == "English" ? "SELECT"
                                                                                                        : "SEC";

        }

    }


    public void LoadSelectedCharacter()
    {

        currentIndex = PlayerPrefs.GetInt("SelectedCharacter");
        currentIndexText.text = currentIndex.ToString();
        UpdateCharacterInfo();

    }


    public void UpdateCharacterInfo()
    {

        CharacterInfo characterInfo = characterDatabase.characters[currentIndex].GetComponent<CharacterInfo>();
        nameText.text = characterInfo.name;
        int speed = characterInfo.speed;
     
        for(int i = 0; i < 5; i++)
        {
            if(i < speed)
              speedImages[i].gameObject.SetActive(true);

            else
              speedImages[i].gameObject.SetActive(false);
        }

        int jump = characterInfo.jumpPower;
        
        for(int i = 0;i < 5; i++)
        {

            if (i < jump)
                jumpImages[i].gameObject.SetActive(true);
            else
                jumpImages[i].gameObject.SetActive(false); 

        }

               
        priceText.text = characterInfo.price.ToString();

    }


    public void ShowCharacterImage()
    {

        for (int i = 0; i < characters.Length; i++)
        {

            characters[i].gameObject.SetActive(i == currentIndex);

            if (i == currentIndex)
            {

                characters[i].gameObject.transform.localPosition = new Vector3(0.96f, -1.65f, -7.51f);
                characters[i].gameObject.transform.rotation = Quaternion.Euler(0, 200, 0);

            }

        }

    }


    public void ShowProfilCharacterImage()
    {

        for (int i = 0; i < characters.Length; i++)
        {

            characters[i].gameObject.SetActive(i == currentIndex);

            if (i == currentIndex)
            {

                characters[i].gameObject.transform.localPosition = new Vector3(-3f, -1.74f, -7.21f);
                characters[i].gameObject.transform.rotation = Quaternion.Euler(0, 150, 0);

            }


        }

    }
}
