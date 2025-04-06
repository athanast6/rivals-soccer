using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private SquadManager squadManager;

    [SerializeField] private TextMeshProUGUI statsText, standingsText, inventoryText, myTeamText;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerAttributes playerAttributes;
    [SerializeField] private MixamoPlayerController singlePlayer;
    [SerializeField] private Transform inventoryUIRoot, myTeamUIRoot, clubhouseUIRoot;

    public void ShowPauseMenu(){
        statsText.text = "";
        statsText.text += "W: " + saveManager.saveData.wins + ", D: " + saveManager.saveData.draws + ", L: " + saveManager.saveData.losses +"\n";
        statsText.text += "Player Name: " + saveManager.saveData.playerName + "\n";
        statsText.text += "Team Name: " + saveManager.saveData.teamName +"\n";
        statsText.text += "Team Location: " + saveManager.saveData.teamLocation +"\n";
        statsText.text += "Money: " + playerInventory.money + " coins" +"\n"+"\n";
        statsText.text += "Stamina Remaining: " + singlePlayer.staminaRemaining + "" +"\n";

        UpdateInventoryUI(inventoryUIRoot);

        ShowCardsUI(myTeamUIRoot, squadManager.players);
    }

    public void UpdateInventoryUI(Transform inventoryUIRoot){
        for(int i=0;i<inventoryUIRoot.childCount;i++){
            inventoryUIRoot.GetChild(i).gameObject.SetActive(false);
        }
        var index = 0;
        foreach(var item in playerInventory.items){
            //Debug.Log("Updating UI For " + item.itemName);

            var itemCard = inventoryUIRoot.GetChild(index).gameObject;
            itemCard.transform.GetChild(0).GetComponent<Image>().sprite = itemManager.itemIcons[item.iconIndex];
            itemCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.itemName;
            itemCard.SetActive(true);

            


            index++;

            if(index == 24){
                Debug.Log("Filled Inventory UI");
                break;
            }
        }
    }


    private void ShowCardsUI(Transform UIRoot, List<PlayerAttributes> players){

        var numCards = UIRoot.childCount;
        for(int j=0; j<numCards; j++){
            var playerCard = UIRoot.GetChild(j).gameObject;
            playerCard.SetActive(false);
        }
        for(int i=0; i<players.Count;i++){

            var player = players[i];

            UIRoot.GetChild(i).transform.GetComponent<PlayerCard>().DisplayPlayerCard(player);


            
        }
    }

    

    public void ShowClubhouse(){
        

        ShowCardsUI(clubhouseUIRoot, squadManager.clubhousePlayers);
        
    }

    

    

   
}
