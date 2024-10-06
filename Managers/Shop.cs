using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Shop : MonoBehaviour
{

    [SerializeField] InventoryUI inventoryUI;
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Home Player")||other.gameObject.CompareTag("Away Player")){
            OpenShopUI();
        }
    }


    private void OpenShopUI(){
        inventoryUI.isAtShop = true;
        inventoryUI.gameObject.SetActive(true);
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
        
    }


    private void CloseShopUI(){
        inventoryUI.isAtShop = false;
        inventoryUI.gameObject.SetActive(false);

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }
}
