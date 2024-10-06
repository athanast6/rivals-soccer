using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] MenuManager menuManager;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject SquadUI;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] Transform itemList;


    private UiInteraction uiInteraction;

    public GameObject SellButton, DiscardButton, SquadButton;
    public TextMeshProUGUI moneyText;
    public int lastClickedIndex;



    public bool isAtShop;

    void Awake(){
        //transform.parent.TryGetComponent(out uiInteraction);
    }

    void OnEnable(){
        Debug.Log("Setting up UI inventory");

        menuManager.PauseGame();
        
        ShowInventory();


        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        //uiInteraction.Environment.SetActive(false);
        
    }
    
    void OnDisable(){
        isAtShop = false;



        SquadUI.SetActive(false);


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DiscardButton.SetActive(false);
        SellButton.SetActive(false);

        menuManager.ResumeGame();


        //uiInteraction.Environment.SetActive(true);
    }

    private void ShowInventory(){
        

    

        moneyText.text = $"Coins: {playerInventory.money.ToSafeString()}";


        foreach (Transform child in itemList)
        {
            // Deactivate the child object
            child.gameObject.SetActive(false);
        }

        var index = 0;
        foreach(var item in playerInventory.items){


            ShowItemCard(item, index);

            index++;



        }
    }

    private void ShowItemCard(ItemData item, int index){
        itemList.GetChild(index).gameObject.SetActive(true);

        itemList.GetChild(index).GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
        itemList.GetChild(index).transform.GetChild(1).GetComponent<Image>().sprite = itemManager.itemIcons[item.iconIndex];

        itemList.GetChild(index).GetComponent<Button>().onClick.AddListener(() => ItemClicked(index));
    }

    

    public void ItemClicked(int index){
        lastClickedIndex = index;

        if(isAtShop){
            //show sell button
            DiscardButton.SetActive(false);
            SellButton.SetActive(true);
        }else{
            //show discard button
            SellButton.SetActive(false);
            DiscardButton.SetActive(true);
        }
    }



    public void OnDiscardClick(){
        playerInventory.items.RemoveAt(lastClickedIndex);
        DiscardButton.SetActive(false);

        ShowInventory();
    }


    public void OnSellClick(){
        playerInventory.money += playerInventory.items[lastClickedIndex].saleValue;

        playerInventory.items.RemoveAt(lastClickedIndex);
        SellButton.SetActive(false);

        ShowInventory();
    }


    public void ShowSquad(){
        SquadUI.SetActive(true);
    }
}
