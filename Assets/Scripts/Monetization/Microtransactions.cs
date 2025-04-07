using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Microtransactions : MonoBehaviour
{
    [SerializeField] GameObject AdRemoveButton;
    [SerializeField] GameObject AmericaButton;

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void DisableAds()
    {
        int currentPremium = FindObjectOfType<Shop>().premiumMoney;
        if (currentPremium >= 2)
        {
            FindObjectOfType<Advertisements>().gameObject.SetActive(false);
            FindObjectOfType<Shop>().SpendPremiumCurrency(2);
            Destroy(AdRemoveButton);
        }
    }
    public void BuyAmerica()
    {
        int currentPremium = FindObjectOfType<Shop>().premiumMoney;
        if (currentPremium >= 7)
        {
            FindObjectOfType<Shop>().SpendPremiumCurrency(7);
            Destroy(AmericaButton);
        }
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }
}
