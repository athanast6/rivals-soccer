using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackManager : MonoBehaviour
{
    private SquadManager squadManager;
    [SerializeField] SaveManager saveManager;
    [SerializeField] GameObject MyPacksUI;
    [SerializeField] GameObject packsEmpty;
    [SerializeField] GameObject PackUI;

    private List<string> playerNamesList = new List<string>();

    private List<PlayerAttributes> players = new List<PlayerAttributes>();

    private System.Random random = new System.Random();


    void Start()
    {


        squadManager = GetComponent<SquadManager>();



        // Load the CSV file as a TextAsset
        var playerNamesCsv = Resources.Load<TextAsset>("British_Male_Names_1900s");

        if (playerNamesCsv != null)
        {
            // Split the file content into lines
            string[] lines = playerNamesCsv.text.Split('\n');

            // Create a list to store names
            //List<string> names = new List<string>();

            // Start parsing from the second line to skip the header
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (!string.IsNullOrEmpty(line))
                {
                    // Split by comma and combine first and last names
                    string[] parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        string fullName = parts[0].Trim() + " " + parts[1].Trim();
                        playerNamesList.Add(fullName);
                    }
                }
            }
        }
    }






    public void ConfirmPack(){

    }
    public void CancelPack(){
        
    }

    public void DisplayPacks(){
        for(int i=0;i<12;i++){
            packsEmpty.transform.GetChild(i).gameObject.SetActive(false);
        }
        for(int i=0;i<saveManager.saveData.userPacks.Count;i++){
            packsEmpty.transform.GetChild(i).gameObject.SetActive(true);
        }
    }


    public void GivePack(string type){
        
        saveManager.saveData.userPacks.Add(type);
    }
    
    /// <summary>
    /// Called when the user clicks confirm to open a pack.
    /// Generates 5 random players that the user can select from.
    /// </summary>
    public void OpenPack(int index){

        var pack = saveManager.saveData.userPacks[index];

        players.Clear();


        

        //Generate 5 random players        
        for(int i =0; i<5; i++){
            
            
            var player = GenerateRandomPlayer(pack);

            players.Add(player);

        }


        //Open Pack UI
        PackUI.SetActive(true);


        //Display Cards in UI
        for(int i =0; i<5; i++){
            PackUI.transform.GetChild(i).transform.GetComponent<PlayerCard>().DisplayPlayerCard(players[i]);
        }


        //Delete pack from user's inventory
        saveManager.saveData.userPacks.RemoveAt(index);
    }


    /// <summary>
    /// Generate a random player with attributes.
    /// </summary>
    /// <returns></returns>
    private PlayerAttributes GenerateRandomPlayer(string type){


        //Bronze Pack
        var maxSpeed = 10f;
        if(type == "Bronze"){
            maxSpeed = 9f;
        }
        if(type == "Silver"){
            maxSpeed = 10f;
        }
        if(type == "Gold"){
            maxSpeed = 11f;
        }
        var player = new PlayerAttributes();

        var random_look = random.Next(1,25);
        player.m_LogoAddress = "Assets/Sprites/CartoonGuy ("+random_look+").png";

        player.playerName = playerNamesList[random.Next(0,playerNamesList.Count)];
        player.touchPower = 58;
        player.shotPower = random.Next(150,200);
        player.passPower = 0.5f;
        player.clearPower = 6;
        player.shotDistance = random.Next(30,50);
        var shotAcc = UnityEngine.Random.Range(0.3f,0.6f);
        player.shotAccuracy = (float)Math.Round(shotAcc,2);
        player.stamina = random.Next(60,99);

        var spd = UnityEngine.Random.Range(7.0f, maxSpeed);
        player.runSpeed = (float)Math.Round(spd,2);
        player.originalRunSpeed = player.runSpeed;
        player.dribbleSpeed = 5;


        //Depends on position
        player.spacingDistance = random.Next(-30, 30);
        player.defenderDistance = random.Next(-20, 60);
        player.strikerDistance = random.Next(-20, 20);

        player.passDetectRange = random.Next(5,8);

        player.occupation = "Townsperson";

        player.age = random.Next(15,40);
        player.sizeScale = (float)Math.Round(UnityEngine.Random.Range(0.9f,1.1f),2);

        player.fieldPosition = PlayerAttributes.FieldPosition.Striker;

        player.isOnTeam = true;

        return player;
    }

    
    


    ///Called when the user selects the card to keep.
    public void SelectCard(int index){

        

        //If squad doesnt have 4 players, add player to squad
        if(squadManager.players.Count < 4){
            squadManager.players.Add(players[index]);
        }

        //Else Add player to clubhouse
        else{
            squadManager.clubhousePlayers.Add(players[index]);
        }
        

        saveManager.SaveGame();
        PackUI.SetActive(false);
        
        DisplayPacks();
        
    }












    
    
}
