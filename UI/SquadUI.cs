using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SquadUI : MonoBehaviour
{

    [SerializeField] private SquadManager squadManager;
    private UIDocument squadUI, inventoryUI;
    public VisualTreeAsset playerCardTemplate;

    private Label nameText, speedText, positionText, ageText;
    private VisualElement playerImage;



    





    void OnEnable(){
        
        Debug.Log("Setting up squad UI");
        squadUI = GetComponent<UIDocument>();

        transform.parent.GetComponent<UiInteraction>().currentUIDocument = squadUI;

       

        inventoryUI = transform.parent.GetChild(1).transform.GetComponent<UIDocument>();

        var closeButton = squadUI.rootVisualElement.Q<Button>("closeSquad");
        closeButton.RegisterCallback<ClickEvent>(CloseSquad);
        
        


        
        ShowSquad();

        //inventoryUI.gameObject.SetActive(false);
       


    }


    

    void ShowSquad(){
        Debug.Log("Show squad players.");
        if(squadManager.players.Count == 0){return;}
        for(int i=0;i<squadManager.players.Count;i++){
            
            
            var player = squadManager.players[i];
            var playerCard = playerCardTemplate.Instantiate();

            var cardElement = playerCard.Q<Button>("playerCard");
            

            cardElement.Q<Label>("playerName").text = player.playerName;

            //STATS
            var stats = cardElement.Q<GroupBox>("stats");
            stats.Q<Label>("playerSpeed").text = $"Speed: " + player.runSpeed;
            stats.Q<Label>("playerPosition").text = $"Pos: " + player.fieldPosition;
            stats.Q<Label>("playerAge").text = $"Age: " + player.age;
            stats.Q<Label>("playerShooting").text = $"Shooting: " + player.shotAccuracy;
            stats.Q<Label>("playerHeight").text = $"Height: " + player.sizeScale;

            var index = i;
            Addressables.LoadAssetAsync<Sprite>(player.m_LogoAddress).Completed += op => 
            {
                var sprite = op.Result;
                cardElement.Q<VisualElement>("playerImage").contentContainer.style.backgroundImage = new StyleBackground(sprite);
                Debug.Log("Loaded sprite.");

            };





            

            

            //var index = i;
            //playerCard.Q<Button>("releaseButton").RegisterCallback<ClickEvent>(ev => ReleasePlayer(cardElement));
            var cardRow = squadUI.rootVisualElement.Q<VisualElement>("cardRow");

            cardRow.Add(cardElement);

            AnimateCard(cardElement);
            
        }

    }


    async void AnimateCard(VisualElement ve){
        
        ve.AddToClassList(".playerCard");
        ve.AddToClassList(".playerCard-out");
        await Task.Delay(100);
        ve.RemoveFromClassList(".playerCard-out");
        await Task.CompletedTask;
    }

    void CloseSquad(ClickEvent evt){
        transform.parent.GetComponent<UiInteraction>().currentUIDocument = inventoryUI;
        gameObject.SetActive(false);

        //inventoryUI.gameObject.SetActive(true);

    }

    public void ReleasePlayer(VisualElement element){

        var index = squadUI.rootVisualElement.Q("cardRow").IndexOf(element);
        Debug.Log(index);

        squadManager.players[index].isOnTeam = false;
        squadManager.players.RemoveAt(index);
        
        squadUI.rootVisualElement.Q("cardRow").Remove(element);


    }



    


}
