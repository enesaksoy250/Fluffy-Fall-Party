using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing;
using System;

public class IAPManager : MonoBehaviour, IStoreListener
{
    IStoreController controller;

    public string[] product;

    //public List<GameObject> canBePurchasedObjs;

    private void Start()
    {
        IAPStart();
    }

    public void IAPStart()
    {
        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
        foreach (string item in product)
        {
            builder.AddProduct(item, ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("OnInitializeFailed: " + error + " message = " + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product[0], StringComparison.Ordinal))
        {
            print("Buy ad");
            PlayerPrefs.SetInt("removeAd", 1);
            DataBaseManager.instance.UpdateFirebaseInfo("removeAd", true);       
            return PurchaseProcessingResult.Complete;
        }
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product[1], StringComparison.Ordinal))
        {
            print("coin_1000");
            DataBaseManager.instance.IncreaseFirebaseInfo("coin", 1000,() => { UIDataLoader.instance.SetStorePanelInfo(); });
            return PurchaseProcessingResult.Complete;
        }
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product[2], StringComparison.Ordinal))
        {
            print("coin_5000");
            DataBaseManager.instance.IncreaseFirebaseInfo("coin", 5000, () => { UIDataLoader.instance.SetStorePanelInfo(); });
            return PurchaseProcessingResult.Complete;
        }
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product[3], StringComparison.Ordinal))
        {
            print("coin_10000");
            DataBaseManager.instance.IncreaseFirebaseInfo("coin", 10000, () => { UIDataLoader.instance.SetStorePanelInfo(); });
            return PurchaseProcessingResult.Complete;
        }
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product[4], StringComparison.Ordinal))
        {
            print("gem_10");
            DataBaseManager.instance.IncreaseFirebaseInfo("gem", 10, () => { UIDataLoader.instance.SetStorePanelInfo(); });
            return PurchaseProcessingResult.Complete;
        }
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product[5], StringComparison.Ordinal))
        {
            print("gem_50");
            DataBaseManager.instance.IncreaseFirebaseInfo("gem", 50, () => { UIDataLoader.instance.SetStorePanelInfo(); });
            return PurchaseProcessingResult.Complete;
        }

        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product[6], StringComparison.Ordinal))
        {
            print("gem_100");
            DataBaseManager.instance.IncreaseFirebaseInfo("gem", 100, () => { UIDataLoader.instance.SetStorePanelInfo(); });
            return PurchaseProcessingResult.Complete;
        }

        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, product[7], StringComparison.Ordinal))
        {
            print("mix");
            PlayerPrefs.SetInt("removeAd", 1);
            DataBaseManager.instance.UpdateFirebaseInfo("removeAd", true);
            DataBaseManager.instance.IncreaseFirebaseInfo("coin", 10000, () => { UIDataLoader.instance.SetStorePanelInfo(); });
            DataBaseManager.instance.IncreaseFirebaseInfo("gem", 100, () => { UIDataLoader.instance.SetStorePanelInfo(); });
            return PurchaseProcessingResult.Complete;
        }

        else
        {
            return PurchaseProcessingResult.Pending;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("OnPurchaseFailed: " + failureReason);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
    }

    public void IAPButton(string id)
    {
        Product product = controller.products.WithID(id);
        if (product != null && product.availableToPurchase)
        {
            controller.InitiatePurchase(product);
            Debug.Log("Buying: " + id);
        }
        else
        {
            Debug.Log("Not Buying: " + id);
        }
    }

    private bool CheckFirstThreeCharacters(string text, string target)
    {
        if (string.IsNullOrEmpty(text) || text.Length < 3)
        {
            return false; // Metin boþ veya 3'ten kýsa ise false döner.
        }

        string firstThree = text.Substring(0, 3); // Ýlk üç harfi alýr.
        return firstThree == target; // Ýlk üç harf hedefle eþleþiyorsa true döner.
    }
}
