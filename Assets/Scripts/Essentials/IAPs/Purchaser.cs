using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

/// <summary>
/// Delegate of the type void not taking any parameters.
/// The methods referenced by an instance of this delegate are called when a purchase can't be made and should handle this information.
/// </summary>
public delegate void PurchaseFailedMethods();

public class Purchaser : MonoBehaviour, IStoreListener
{
    /// <summary>
    /// The game object in the scene which has a component implementing the 'ShowErrorMessage' method
    /// </summary>
    IPurchase debugger;

    private static IStoreController m_StoreController;         
    private static IExtensionProvider m_StoreExtensionProvider; 

    /// <summary>
    /// Delegate containing the methods handling the situation that a purchase can't be made.
    /// </summary>
    PurchaseFailedMethods purchaseFailed;

    /// <summary>
    /// The product ID that is specified in the all consoles.
    ///     HAS TO BE PASTED ON ALL CONSOLES CORRECTLY! THAT IS MANDATORY!
    /// </summary>
    public static string productIDFullVersion;

    void Awake()
    {
        productIDFullVersion = StaticValues.productIDFullVersion;
    }

    void Start()
    {
        PurchaseFullVersionController pc = GetComponent<PurchaseFullVersionController>();
        RestoreFullVersionController pr = GetComponent<RestoreFullVersionController>();

        if (pc != null)
            debugger = pc;
        else if (pr != null)
            debugger = pr;
        else
            Debug.Log("The debugger interface wasn't implemented. This will result in errors. Check the code.");

        if (m_StoreController == null)
            InitializePurchasing();

        //DONT DELETE: uncomment the following comment if you want to enable the 'Run API updater' function - Assets > Run API updater
       //this.GetComponent<Animation>().Play();
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(productIDFullVersion, ProductType.NonConsumable, 
            new IDs {
            {productIDFullVersion, GooglePlay.Name },
            {productIDFullVersion,  AmazonApps.Name}
        });

        UnityPurchasing.Initialize(this, builder);

        print("initialization processing");
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    /// <summary>
    /// Method which should start the purchasing of the full version.
    /// </summary>
    public void BuyFullVersionNonConsumbable()
    {
        // Buy the non-consumable. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(productIDFullVersion);
    }

    /// <summary>
    /// If the full version was already unlocked, it can be restored with the following method. (If the file holding it was deleted
    /// or the device on which the game is installed switched.)
    /// </summary>
    public void RestoreFullVersion()
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productIDFullVersion);
            if (product != null && product.hasReceipt)
            {
                PurchaseFullVersionReward();
            }
            else
            {
                //debugger.ShowErrorMessageOnPanel("The Full Version couldn't be restored. It wasn't unlocked on this account. " +
                //    "If you are certain that you unlocked the Full Version, make sure to install this game with the same" +
                //    " account with which you purchased the Full Version.");

                if (GetComponent<RestoreFullVersionController>() != null)
                {
                    GetComponent<RestoreFullVersionController>().ShowRestoreFullVersionFailed();
                }
                else
                    Debug.Log("The 'restore full version controller' object wasn't attached to the controller of the 'restore full version" +
                        "scene'. Make sure that this script is attached, otherwise the player can't be informed if the full version can't " +
                        "be restored.");
            }
        }
        else
        {
            debugger.ShowErrorMessageOnPanel("The Full Version couldn't be restored. " +
                "The connection to the store couldn't be established. Check your internet connection and try again later.");
            InitializePurchasing();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId">The product ID of the product you want to purchase. See 'Google Play Console' - section of your 
    /// product to determine it.</param>
    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                debugger.ShowErrorMessageOnPanel("The purchase which you attempted to make failed. " +
                    "The product which you want to purchase is currently not available. Please try again later.");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
            debugger.ShowErrorMessageOnPanel("The purchase which you attempted to make failed. " +
                    "The connection to the store couldn't be established. Please try again later.");
            InitializePurchasing();
        }
    }

    // DON'T DELETE: ONLY NEEDED FOR THE APP STORE, CURRENTLY COMMENTED OUT:
    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    //public void RestorePurchases()
    //{
    //    if (!IsInitialized())
    //    {
    //        Debug.Log("RestorePurchases FAIL. Not initialized.");
    //        return;
    //    }
    //
    //    if (Application.platform == RuntimePlatform.IPhonePlayer ||
    //        Application.platform == RuntimePlatform.OSXPlayer)
    //    {
    //        Debug.Log("RestorePurchases started ...");
    //        var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
    //        apple.RestoreTransactions((result) =>
    //        {
    //            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
    //        });
    //    }
    //    else
    //    {
    //        Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
    //    }
    //}


      
    // --- IStoreListener


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        debugger.ShowErrorMessageOnPanel("The connection to the store couldn't be established. " +
            "The following error occured:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, productIDFullVersion, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

            PurchaseFullVersionReward();
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            debugger.ShowErrorMessageOnPanel("The purchase which you attempted to make failed. " +
                    "The product which you want to purchase couldn't be recognized. Please try again later.");
        }
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// The player is rewarded the full version.
    /// FullVersionUnlocked is set to true and the menu is opened again.
    /// </summary>
    void PurchaseFullVersionReward()
    {
        FullVersion.Instance.IsFullVersionUnlocked = FullVersionUnlocked.unlocked;
        Time.timeScale = 1.0f; //makes sure that the timeScale is set to 1 (means: real time)
        SceneManager.LoadScene("Menu");
        //GetComponent<BackToMenu>().ReturnToMenu();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        debugger.ShowErrorMessageOnPanel("The purchase which you attempted to make failed. " +
                    "The following error occured: " + failureReason);
    }
}
