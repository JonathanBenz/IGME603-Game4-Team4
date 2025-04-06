using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Microtransactions : MonoBehaviour
{
    [SerializeField] GameObject transactionScreen;
    [SerializeField] GameObject transactionButton;
    [SerializeField] GameObject AdRemoveButton;
    [SerializeField] GameObject AmericaButton;

    bool isDisablingAds;
    bool isBuyingAmerica;
    bool isBuyingPremium;

    private void OnEnable()
    {
        Time.timeScale = 0;
    }
   
    public void CheckIfCanPurchase(int amount)
    {
        int currentPremium = FindObjectOfType<Shop>().premiumMoney;

        if (currentPremium >= amount) 
        { 
            transactionScreen.SetActive(true); 
            transactionButton.SetActive(true); 
        }
        else
        {
            isDisablingAds = false;
            isBuyingAmerica = false;
            return;
        }
    }

    public void IsDisablingAds(bool bValue)
    {
        isDisablingAds = bValue;
    }
    public void IsBuyingAmerica(bool bValue)
    {
        isBuyingAmerica = bValue;
    }
    public void IsBuyingPremium(bool bValue)
    {
        isBuyingPremium = bValue;
    }

    public void DisableAds()
    {
        if (isDisablingAds)
        {
            FindObjectOfType<Advertisements>().gameObject.SetActive(false);
            FindObjectOfType<Shop>().SpendPremiumCurrency(2);
            Destroy(AdRemoveButton);
            isDisablingAds = false;
        }
    }
    public void BuyAmerica()
    {
        if (isBuyingAmerica)
        {
            FindObjectOfType<Shop>().SpendPremiumCurrency(7);
            Destroy(AmericaButton);
            isBuyingAmerica = false;
        }
    }

    public void BuyPremium(int amount)
    {
        if (isBuyingPremium) FindObjectOfType<Shop>().AddPremiumCurrency(amount);
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }
}
