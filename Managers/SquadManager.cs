using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

[System.Serializable]
public class SquadManager : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private UiInteraction uiInteraction;
    [SerializeField] private GameObject RecruitUI;
    private TextMeshProUGUI recruitText;
    public List<PlayerAttributes> players = new List<PlayerAttributes>();
    public List<PlayerAttributes> clubhousePlayers = new List<PlayerAttributes>();
    private SoccerPlayer currentRecruit;



    void Start(){
        recruitText = RecruitUI.GetComponentInChildren<TextMeshProUGUI>();
    }


    public async void RecruitToSquad(SoccerPlayer recruit){

        if(playerInventory.money < recruit.playerAttributes.recruitCost){return;}

        if(recruit.playerAttributes.isOnTeam){return;}

        currentRecruit = recruit;

        //Show the ui panel for options to recruit him to squad.
        RecruitUI.SetActive(true);
        uiInteraction.enabled = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        recruitText.text = $"Recruit For {currentRecruit.playerAttributes.recruitCost} Coins.";

        await Task.Delay(5000);

        RecruitUI.SetActive(false);
        uiInteraction.enabled=false;  
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;     

        await Task.CompletedTask;

    }
    
    public void RecruitPlayer(){

        if(playerInventory.money < currentRecruit.playerAttributes.recruitCost){return;}

        if(currentRecruit.playerAttributes.isOnTeam){return;}

        playerInventory.money -= currentRecruit.playerAttributes.recruitCost;

        players.Add(currentRecruit.playerAttributes);

        RecruitUI.SetActive(false);
        uiInteraction.enabled=false;  
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

    }




   


    

}
