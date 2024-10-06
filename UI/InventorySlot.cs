using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : MonoBehaviour
{



    public Button button;
    public ItemData itemData;
    private InventoryUI inventoryUI;
    private VisualElement discardButton;
    


    public InventorySlot(ItemData itemData, VisualTreeAsset template, InventoryUI inventoryUI, Sprite icon){
        
        var itemButtonContainer = template.Instantiate();

        this.inventoryUI = inventoryUI;

        discardButton = inventoryUI.discardButton;

        button = itemButtonContainer.Q<Button>();
        this.itemData = itemData;


        button.Q("itemImage").style.backgroundImage = new StyleBackground(icon);
        button.style.backgroundColor = new Color(145,46,46,46);
        button.text = itemData.itemName;

        button.RegisterCallback<ClickEvent>(OnClick);
    }

    public void OnClick(ClickEvent evt){
        
        Debug.Log(itemData.itemName + " has been clicked.");

        inventoryUI.ItemClicked(itemData);
    }
}
