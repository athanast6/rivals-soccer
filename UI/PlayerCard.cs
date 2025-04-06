using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;

public class PlayerCard : MonoBehaviour
{
    ///
    /// This script controls a player from their card
    ///

    private SquadManager squadManager;
    

    private int index;

    void Start(){
        index = transform.GetSiblingIndex();
        squadManager = FindObjectOfType<SquadManager>();
    }

    //Release
    public void ReleasePlayer(){
        Debug.Log(index);

        squadManager.clubhousePlayers[index].isOnTeam = false;
        squadManager.clubhousePlayers.RemoveAt(index);

        //ShowPauseMenu();
    }

    //Send to Clubhouse
     public void SendToClubhouse(){
        squadManager.clubhousePlayers.Add(squadManager.players[index]);

        squadManager.players.RemoveAt(index);
        

    }

    //Can only send to squad if there's less than 5 players
    public void SendToSquad(){

        if(squadManager.players.Count < 4){

            squadManager.players.Add(squadManager.clubhousePlayers[index]);

            squadManager.clubhousePlayers.RemoveAt(index);
        }
    }

    public void DisplayPlayerCard(PlayerAttributes player){


        transform.gameObject.SetActive(true);

        
        Addressables.LoadAssetAsync<Sprite>(player.m_LogoAddress).Completed += op => 
            {
                var sprite = op.Result;
                transform.GetChild(0).GetComponent<Image>().sprite = sprite;
                Debug.Log("Loaded sprite.");

            };
        

            
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.playerName;
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = player.fieldPosition.ToString();
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "SPD" + player.runSpeed.ToString();
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "FIT" + player.stamina.ToString();
        transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "SHOT" + player.shotAccuracy.ToString();
        transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = "HGT" + player.sizeScale.ToString();
    }
}
