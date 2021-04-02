using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
//using UnityEngine.Purchasing.Extension;

/// <summary>
/// Delegate of the type void not taking any parameters.
/// The methods referenced by an instance of this delegate are called when a purchase can't be made and should handle this information.
/// </summary>
public delegate void PurchaseFailedMethods();

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    /// <summary>
    /// Delegate containing the methods handling the situation that a purchase can't be made.
    /// </summary>
    PurchaseFailedMethods purchaseFailed;

    /// <summary>
    /// The product ID that is specified in the google play console.
    ///     HAS TO BE PASTED ON THE GOOGLE PLAY CONSOLE CORRECTLY! THAT IS MANDATORY!
    /// </summary>
    public static string productIDFullVersion = "com.strawberrystudios.snake.fullversion";

    //private IStoreCallback callback;
    //public void Initialize(IStoreCallback callback)
    //{
    //    this.callback = callback;
    //}

    //public void RetrieveProducts(System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Purchasing.ProductDefinition> products)
    //{
    //    // Fetch product information and invoke callback.OnProductsRetrieved();
    //}

    //public void Purchase(UnityEngine.Purchasing.ProductDefinition product, string developerPayload)
    //{
    //    // Start the purchase flow and call either callback.OnPurchaseSucceeded() or callback.OnPurchaseFailed()
    //}

    //public void FinishTransaction(UnityEngine.Purchasing.ProductDefinition product, string transactionId)
    //{
    //    // Perform transaction related housekeeping 
    //}

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

        //uncomment the following comment if you want to enable the 'Run API updater' function - Assets > Run API updater
        //this.animation.Play();
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());  

        // Add a product to sell 
        builder.AddProduct(productIDFullVersion, ProductType.NonConsumable);

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);

        print("initialization processing");
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    /// <summary>
    /// Method which should start the purchasing of the full version.
    /// </summary>
    public void BuyFullVersionNonConsumbable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        InitializePurchasing();
        BuyProductID(productIDFullVersion);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId">The product ID of the product you want to purchase. See 'Google Play Console' - section of your 
    /// product to determine it.</param>
    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                GetComponent<PurchaseFullVersionController>().ShowErrorMessageOnPanel("The purchase which you attempted to make failed. " +
                    "The product which you want to purchase is currently not available. Please try again later.");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
            GetComponent<PurchaseFullVersionController>().ShowErrorMessageOnPanel("The purchase which you attempted to make failed. " +
                    "The connection to the store couldn't be established. Please try again later.");
        }
    }

    // DON'T DELETE: ONLY NEEDED FOR THE APP STORE, CURRENTLY COMMENTED OUT:
    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    //public void RestorePurchases()
    //{
    //    // If Purchasing has not yet been set up ...
    //    if (!IsInitialized())
    //    {
    //        // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
    //        Debug.Log("RestorePurchases FAIL. Not initialized.");
    //        return;
    //    }

    //    // If we are running on an Apple device ... 
    //    if (Application.platform == RuntimePlatform.IPhonePlayer ||
    //        Application.platform == RuntimePlatform.OSXPlayer)
    //    {
    //        // ... begin restoring purchases
    //        Debug.Log("RestorePurchases started ...");

    //        // Fetch the Apple store-specific subsystem.
    //        var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
    //        // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
    //        // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
    //        apple.RestoreTransactions((result) =>
    //        {
    //            // The first phase of restoration. If no more responses are received on ProcessPurchase then 
    //            // no purchases are available to be restored.
    //            Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
    //        });
    //    }
    //    // Otherwise ...
    //    else
    //    {
    //        // We are not running on an Apple device. No work is necessary to restore purchases.
    //        Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
    //    }
    //}


      
    // --- IStoreListener


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, productIDFullVersion, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
            FullVersion.Instance.IsFullVersionUnlocked = FullVersionUnlocked.unlocked;
            GetComponent<BackToMenu>().ReturnToMenu();
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            GetComponent<PurchaseFullVersionController>().ShowErrorMessageOnPanel("The purchase which you attempted to make failed. " +
                    "The product which you want to purchase couldn't be recognized. Please try again later.");
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        GetComponent<PurchaseFullVersionController>().ShowErrorMessageOnPanel("The purchase which you attempted to make failed. " +
                    "The following error occured: " + failureReason);
    }
}
