using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SquadUIManager : MonoBehaviour
{
    [SerializeField] private SquadManager squadManager;
    [SerializeField] Transform squadPlayers;
    public int lastClickedIndex;



    void OnEnable(){
        Debug.Log("Setting up UI inventory");
        
        ShowSquad();


        
        
    }


    private void ShowSquad(){
        


        foreach (Transform child in squadPlayers)
        {
            // Deactivate the child object
            child.gameObject.SetActive(false);
        }

        var index = 0;
        foreach(var player in squadManager.players){


            ShowPlayerCard(player, index);

            index++;



        }
    }

    private void ShowPlayerCard(PlayerAttributes player, int index){
        squadPlayers.GetChild(index).gameObject.SetActive(true);

        squadPlayers.GetChild(index).GetChild(0).GetComponent<TextMeshProUGUI>().text = player.playerName;
        squadPlayers.GetChild(index).GetChild(1).GetComponent<TextMeshProUGUI>().text = player.fieldPosition.ToString();
        squadPlayers.GetChild(index).GetChild(2).GetComponent<TextMeshProUGUI>().text = $"Age: "+ player.age.ToString();
        squadPlayers.GetChild(index).GetChild(3).GetComponent<TextMeshProUGUI>().text = $"Height: " + player.sizeScale.ToString();
        squadPlayers.GetChild(index).GetChild(4).GetComponent<TextMeshProUGUI>().text =  $"Speed: " + player.runSpeed.ToString();
        squadPlayers.GetChild(index).GetChild(5).GetComponent<TextMeshProUGUI>().text = $"Shooting: " + player.shotAccuracy.ToString();
        

        Addressables.LoadAssetAsync<Sprite>(player.m_LogoAddress).Completed += op => 
        {
            var sprite = op.Result;
            squadPlayers.GetChild(index).GetChild(6).GetComponent<Image>().sprite = sprite;
            Debug.Log("Loaded sprite.");

        };
        //squadPlayers.GetChild(index).GetChild(6).GetComponent<Image>().sprite = player.image;

        squadPlayers.GetChild(index).GetComponentInChildren<Button>().onClick.AddListener(() => ReleasePlayer(index));
    }

    

    



    public void ReleasePlayer(int index){
        Debug.Log(index);

        squadManager.players[index].isOnTeam = false;
        squadManager.players.RemoveAt(index);

        ShowSquad();
    }
}
