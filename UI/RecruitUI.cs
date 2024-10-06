using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class RecruitUI : MonoBehaviour
{
    private UIDocument recruitUI;
    public SoccerPlayer recruit;

    public PlayerInventory playerInventory;

    private SquadManager squadManager;

    void OnEnable(){
        recruitUI = GetComponent<UIDocument>();

        ShowRecruitCost();

        recruitUI.rootVisualElement.Q<Button>("recruitButton").RegisterCallback<ClickEvent>(OnRecruitClick);

        transform.parent.GetComponent<UiInteraction>().currentUIDocument = recruitUI;

        
    }
    void Awake(){
        squadManager = FindObjectOfType<SquadManager>();
        
    }

    private async void ShowRecruitCost(){

        
        if(recruit.playerAttributes.isOnTeam || squadManager.players.Count > 5){ return;}
        


        var recruitButton = recruitUI.rootVisualElement.Q<Button>("recruitButton");

        if(playerInventory.money >= (float)recruit.playerAttributes.recruitCost){

            Debug.Log("Can recruit Player.");
            recruitButton.text = $"Recruit for {recruit.playerAttributes.recruitCost} coins.";
            recruitButton.style.display = DisplayStyle.Flex;
            recruitButton.BringToFront();

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

        }else{
            Debug.Log("Not enough coins.");
            recruitButton.style.display = DisplayStyle.None;
        }


        
        await Task.Delay(5000);

        gameObject.SetActive(false);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        await Task.CompletedTask;
        
    }

    public void OnRecruitClick(ClickEvent evt){

        //Add player to squad if player has enough coins.
        
        recruit.playerAttributes.isOnTeam = true;
        squadManager.players.Add(recruit.playerAttributes);

        playerInventory.money -= recruit.playerAttributes.recruitCost;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        gameObject.SetActive(false);

        
    }
}
