using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
    public int premiumMoney = 0;
    public int money = 100;
    public int availableScouts = 5;
    public TMP_Text moneyText;
    public TMP_Text PremiumMoneyText;
    public TMP_Text availableScoutsText;

    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public int price;
    }

    public List<ShopItem> shopItems = new List<ShopItem>(); 

    void Start()
    {
        UpdateText();
    }

    public void BuyScout()
    {
        ShopItem scout = shopItems.Find(item => item.itemName == "Scout");
        if (scout != null)
        {
            BuyItem(scout);
            UpdateText();
        }
    }

    public void BuyItem(ShopItem item)
    {
        if (money >= item.price)
        {
            Debug.Log("Bought: " + item.itemName);

            switch (item.itemName)
            {
                case "Scout":
                    availableScouts++;
                    break;
                /*
                case "":
                    availableScouts++;
                    break;
                */
                default:
                    break;
            }

            money -= item.price;
            UpdateText();
        }
        else
        {
            Debug.Log("Not enough money!");
        }

        


    }

    void UpdateText()
    {
        moneyText.text = "Money: " + money;
        PremiumMoneyText.text = "Premium: " + premiumMoney;
        availableScoutsText.text = "Available: " + availableScouts;
    }
}