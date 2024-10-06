using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInventory : MonoBehaviour
{

    [SerializeField] GameObject InventoryUI;

    public List<ItemData> items = new List<ItemData>();
    public InputAction openInventory;
    private CinemachineFreeLook cameraPanning;


    public float money;


    void Awake(){
        
        cameraPanning = transform.GetComponentInChildren<CinemachineFreeLook>();

        
        openInventory.performed += context => ToggleInventory();
        openInventory.Enable();
    }
    
    
    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Item")){
            TryCollectItem(other.gameObject);
        }
    }
    void TryCollectItem(GameObject other)
    {
        
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            // Handle item collection, e.g., play sound, add to inventory
            items.Add(item.itemData);
            
            Destroy(other.gameObject);
        }
        
    }


    public void ToggleInventory(){
        if(!InventoryUI){

            ShowInventory();

        }else{
            InventoryUI.SetActive(false);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;

            cameraPanning.enabled = true;
        }
        
    }

    private void ShowInventory(){
        InventoryUI.SetActive(true);

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        cameraPanning.enabled = false;

        //PopulateItems();
    }

    private void PopulateItems(){
        for(int i=0;i<items.Count;i++){
            //itemSlots.GetChild(i).SetActive(true);

        }
    }
}
