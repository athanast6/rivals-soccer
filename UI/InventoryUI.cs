using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    private UIDocument inventoryUI;
    public VisualTreeAsset itemButtonTemplate;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private MixamoPlayerController playerController;
    [SerializeField] private GameObject SquadUI;
    [SerializeField] private ItemManager itemManager;



    private UiInteraction uiInteraction;

    public Button discardButton;
    private Button sellButton;
    private Label moneyText;
    public ItemData lastClicked;



    public bool isAtShop;

    void Awake(){
        uiInteraction = transform.parent.GetComponent<UiInteraction>();
    }

    void OnEnable(){
        Debug.Log("Setting up UI inventory");

        uiInteraction.enabled = true;

        //FindObjectOfType<MenuManager>().PauseGame();
        playerController.playerControls.Disable();
        
        ShowInventory();

        transform.parent.GetComponent<UiInteraction>().currentUIDocument = inventoryUI;
        
    }
    
    void OnDisable(){

        uiInteraction.enabled = false;

        playerController.playerControls.Enable();

        isAtShop = false;

        SquadUI.SetActive(false);
        
        //FindObjectOfType<MenuManager>().ResumeGame();
        //uiInteraction.Environment.SetActive(true);
    }

    private void ShowInventory(){
        inventoryUI = GetComponent<UIDocument>();
        sellButton = inventoryUI.rootVisualElement.Q<Button>("sellButton");
        sellButton.RegisterCallback<ClickEvent>(OnSellClick);
        sellButton.style.display=DisplayStyle.None;

        discardButton = inventoryUI.rootVisualElement.Q<Button>("discardButton");
        discardButton.RegisterCallback<ClickEvent>(OnDiscardClick);
        discardButton.style.display=DisplayStyle.None;

        moneyText = inventoryUI.rootVisualElement.Q<Label>("moneyText");

        
        inventoryUI.rootVisualElement.Q<Button>("squadButton").RegisterCallback<ClickEvent>(ShowSquad);

        moneyText.text = $"Coins: {playerInventory.money.ToSafeString()}";

        foreach(var item in playerInventory.items){
            var newSlot = new InventorySlot(item, itemButtonTemplate, this, itemManager.itemIcons[0]);

            inventoryUI.rootVisualElement.Q("itemRow").Add(newSlot.button);
        }
    }

    

    public void ItemClicked(ItemData item){
        lastClicked = item;

        if(isAtShop){
            //show sell button
            sellButton.style.display=DisplayStyle.Flex;
        }else{
            //show discard button
            discardButton.style.display=DisplayStyle.Flex;
        }
    }



    private void OnDiscardClick(ClickEvent evt){
        playerInventory.items.Remove(lastClicked);
        discardButton.style.display=DisplayStyle.None;

        inventoryUI.rootVisualElement.Q("itemRow").Clear();
        ShowInventory();
    }


    private void OnSellClick(ClickEvent evt){
        playerInventory.money += lastClicked.saleValue;

        playerInventory.items.Remove(lastClicked);
        sellButton.style.display=DisplayStyle.None;

        inventoryUI.rootVisualElement.Q("itemRow").Clear();
        ShowInventory();
    }


    public void ShowSquad(ClickEvent evt){
        Debug.Log("Show Squad");
        SquadUI.SetActive(true);

    }
}
