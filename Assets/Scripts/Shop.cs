using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class Shop : MonoBehaviour
{
    public int premiumMoney = 0;
    public int money = 200;
    public int kangaroos = 1;
    public int bloominOnions;
    //public int occupiedScouts;
    //public int flashlights;
    //public int foodWater;

    public TMP_Text moneyTextMain;
    public TMP_Text moneyTextWorld;
    public TMP_Text premiumMoneyTextMain;
    public TMP_Text premiumMoneyTextWorld;
    //public TMP_Text availableScoutsText;
    //public TMP_Text kangaroosText;
    //public TMP_Text flashlightsText;
    //public TMP_Text bloominOnionText;
    //public TMP_Text foodWaterText;

    public Transform kangarooLayoutGroup;
    public GameObject kangarooImg;
    public Transform onionLayoutGroup;
    public GameObject onionImg;
    public GameObject LosePanel;

    public Dictionary<string, int> shopItems = new Dictionary<string, int>();

    void Start()
    {
        UpdateText();

        shopItems.Add("Kangaroos", 20);
        shopItems.Add("Bloomin' Onions", 5);
        //shopItems.Add("Shovel", 5);
        //shopItems.Add("Food/Water", 5);
        Instantiate(kangarooImg, kangarooLayoutGroup); // Start with 1 kangaroo
        Instantiate(onionImg, onionLayoutGroup); // Start with 1 bloomin' onion
    }

    public void BuyItem(string itemName)
    {
        if (shopItems.ContainsKey(itemName))
        {
            KeyValuePair<string, int> item = new KeyValuePair<string, int>(itemName, shopItems[itemName]);

            if (money >= item.Value)
            {
                if(item.Key.Equals("Kangaroos") && kangaroos >= 3)
                {
                    Debug.Log("Kangaroo cap limit already reached. Cannot buy anymore.");
                    return;
                }
                if (item.Key.Equals("Bloomin' Onions") && bloominOnions >= 6)
                {
                    Debug.Log("Bloomin' Onions cap limit already reached. Cannot buy anymore.");
                    return;
                }

                Debug.Log("Bought: " + item.Key);

                switch (item.Key)
                {
                    case "Kangaroos":
                        BuyKangaroo();
                        break;
                    /*case "Flashlight":
                        flashlights++;
                        break;*/
                    case "Bloomin' Onions":
                        BuyOnion();
                        break;
                    /*case "Food/Water":
                        foodWater++;
                        break;*/
                    /*
                    case "":
                        availableScouts++;
                        break;
                    */
                    default:
                        break;
                }

                money -= item.Value;
                UpdateText();
            }
            else
            {
                Debug.Log("Not enough money!");
            }
        }
        else
        {
            Debug.Log("Item not found!");
        }
    }

    private void Update()
    {
        UpdateText();

        if(money <= 0)
        {
            LoseCondition();
        }
    }

    private void LoseCondition()
    {
        LosePanel.SetActive(true);
        LosePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
            "Game Over! \nYou ran out of coupon funds! \nWe have to take you out back and... \nNo more Bloomin' Onions for you!";
    }

    public void UpdateText()
    {
        moneyTextMain.text = "Money: $" + money;
        moneyTextWorld.text = moneyTextMain.text;
        premiumMoneyTextMain.text = "Premium: " + premiumMoney;
        premiumMoneyTextWorld.text = premiumMoneyTextMain.text;

        //availableScoutsText.text = "In: " + kangaroos;
        //occupiedScoutsText.text = "Out: " + occupiedScouts;
        //flashlightsText.text = "Flashlights: " + flashlights;
        //bloominOnionText.text = "Bloomin' Onions: " + bloominOnions;
        //foodWaterText.text = "Food/Water: " + foodWater;
    }

    public void DeductMoney(int amountToDeduct)
    {
        money -= amountToDeduct;
        if (money <= 0) money = 0;
    }

    void BuyKangaroo()
    {
        if (kangaroos >= 3) return; // Don't allow more than 3 kangaroos at a time
        kangaroos++;
        Instantiate(kangarooImg, kangarooLayoutGroup);
    }

    void BuyOnion()
    {
        if (bloominOnions >= 6) return; // Don't allow more than 6 onions at a time
        bloominOnions++;
        Instantiate(onionImg, onionLayoutGroup);
    }

    public void DecrementKangaroo(int kangs)
    {
        kangaroos -= kangs;
        int kangarooImagesCount = kangarooLayoutGroup.childCount;
        for(int i = kangarooImagesCount; i > kangarooImagesCount - kangs; i--)
        {
            Destroy(kangarooLayoutGroup.GetChild(i-1).gameObject);
        }
    }

    public void DecrementOnion(int onions)
    {
        bloominOnions -= onions;
        int onionImagesCount = onionLayoutGroup.childCount;
        for (int i = onionImagesCount; i > onionImagesCount - onions; i--)
        {
            Destroy(onionLayoutGroup.GetChild(i - 1).gameObject);
        }
    }
}