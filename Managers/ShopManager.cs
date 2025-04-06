using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    [SerializeField] GameObject ShopUI;
    [SerializeField] private Transform SellItems, BuyItems;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private GameObject SellButton, BuyButton;
    [SerializeField] private TextMeshProUGUI moneyText;

    private int selectedItemIndex;

    [SerializeField] public List<Item> itemsForSale = new List<Item>();




    


    public void OpenShopUI(){
        ShopUI.SetActive(true);

        SetShopUI();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        menuManager.PauseGame();
        
    }


    public void CloseShopUI(){
        menuManager.ResumeGame();

        ShopUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetShopUI(){
        pauseManager.UpdateInventoryUI(SellItems);

        moneyText.text = $"Coins: {playerInventory.money}";

        //Set Sell Buttons
        for (int i = 0; i < SellItems.childCount; i++)
        {
            int index = i; // Capture the current index
            var button = SellItems.GetChild(i).GetComponent<Button>();

            // Add listener to the button's onClick event
            button.onClick.AddListener( () => OnSellItemClicked(index));
        }


        //Set Buy Buttons
        for (int i = 0; i < itemsForSale.Count; i++)
        {
            int index = i; // Capture the current index
            var button = BuyItems.GetChild(i).GetComponent<Button>();
            button.gameObject.SetActive(true);

            // Add listener to the button's onClick event
            button.onClick.AddListener( () => OnBuyItemClicked(index));
        }
    }

    public async void OnSellItemClicked(int itemIndex){

        BuyButton.SetActive(false);
        
        selectedItemIndex = itemIndex;

        if(playerInventory.items[selectedItemIndex].saleValue > 0){
            SellButton.SetActive(true);
        }

        await Task.CompletedTask;
    }

    public void SellItem(){

        SellButton.SetActive(false);

        var item = playerInventory.items[selectedItemIndex];

        playerInventory.money += item.saleValue;

        playerInventory.items.Remove(item);

        SetShopUI();

        
    }

    public async void OnBuyItemClicked(int itemIndex){

        SellButton.SetActive(false);
        
        selectedItemIndex = itemIndex;

        //If user can afford the item, show the buy button.
        if(playerInventory.money >= itemsForSale[itemIndex].itemData.purchaseValue){
            BuyButton.SetActive(true);
            BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy for {itemsForSale[itemIndex].itemData.purchaseValue} coins.";
        }

        await Task.CompletedTask;
    }

    public void BuyItem(){

        BuyButton.SetActive(false);

        var item = itemsForSale[selectedItemIndex];

        playerInventory.money -= item.itemData.purchaseValue;

        playerInventory.items.Add(item.itemData);

        SetShopUI();

        
    }

}
