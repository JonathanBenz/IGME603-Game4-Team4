using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
    public int premiumMoney = 0;
    public int money = 100;
    public int scouts = 5;
    public int flashlights;
    public int shovels;
    public int foodWater;

    public TMP_Text moneyText;
    public TMP_Text PremiumMoneyText;
    public TMP_Text availableScoutsText;
    public TMP_Text flashlightsText;
    public TMP_Text shovelsText;
    public TMP_Text foodWaterText;
    
    public Dictionary<string, int> shopItems = new Dictionary<string, int>();


    void Start()
    {
        UpdateText();

        shopItems.Add("Scout", 10);
        shopItems.Add("Flashlight", 5);
        shopItems.Add("Shovel", 5);
        shopItems.Add("Food/Water", 5);
        
    }

    public void BuyItem(string itemName)
    {
        if (shopItems.ContainsKey(itemName))
        {
            KeyValuePair<string, int> item = new KeyValuePair<string, int>(itemName, shopItems[itemName]);

            if (money >= item.Value)
            {
                Debug.Log("Bought: " + item.Key);

                switch (item.Key)
                {
                    case "Scout":
                        scouts++;
                        break;
                    case "Flashlight":
                        flashlights++;
                        break;
                    case "Shovel":
                        shovels++;
                        break;
                    case "Food/Water":
                        foodWater++;
                        break;
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

    void UpdateText()
    {
        moneyText.text = "Money: " + money;
        PremiumMoneyText.text = "Premium: " + premiumMoney;
        availableScoutsText.text = "In: " + scouts;
        flashlightsText.text = "Flashlights: " + flashlights;
        shovelsText.text = "Shovels: " + shovels;
        foodWaterText.text = "Food/Water: " + foodWater;
    }
}