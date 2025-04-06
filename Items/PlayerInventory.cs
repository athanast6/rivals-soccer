using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInventory : MonoBehaviour
{

    [SerializeField] private MixamoPlayerController playerController;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private Transform inventoryItems;




    public List<ItemData> items = new List<ItemData>();
    private CinemachineFreeLook cameraPanning;


    public float money;


    void Awake(){
        
        cameraPanning = transform.GetComponentInChildren<CinemachineFreeLook>();

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



    public void TryConsumeItem(int index){
        if(items[index].canConsume == false){
            return;
        }
        if(playerController.staminaRemaining < 100){
            playerController.staminaRemaining += items[index].staminaRestoration;

            //Limit to 100 stamina.
            playerController.staminaRemaining = Mathf.Clamp(playerController.staminaRemaining,0f,100f);
            Debug.Log($"Player ate {items[index].itemName} and gained {items[index].staminaRestoration}.");

            items.RemoveAt(index);

            //Update UI
            pauseManager.ShowPauseMenu();
        }
    }
}
