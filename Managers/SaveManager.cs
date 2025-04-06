using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SaveManager : MonoBehaviour
{   

    [SerializeField] Button NewGameButton, LoadGameButton;
    [SerializeField] GameObject NewGamePanel;
    [SerializeField] TMP_InputField PlayerName, TeamName, TeamLocation;



    //public InputAction enterButton;
    //private Vector2 mouseDirection;


    //Save and Load:
    //Player Inventory Items and Money
    //Squad

    private PlayerInventory playerInventory;
    private SquadManager squadManager;
    private List<SoccerPlayer> Npcs;
    private string saveFilePath;

    public bool mainMenuScene;
    
    [SerializeField] private Vector3[] trainStationPositions;










    public class SaveData{
        public List<ItemData> items = new List<ItemData>();
        public float money;
        public List<PlayerAttributes> squadPlayers = new List<PlayerAttributes>();
        public List<PlayerAttributes> clubhousePlayers = new List<PlayerAttributes>();
        public int wins,draws,losses;
        public PlayerAttributes playerAttributes;
        public string sceneName = "Smalltown";
        public Vector3 playerLocalPosition;
        public string playerName, teamName, teamLocation;

        public List<string> trophiesWon = new List<string>();
        public List<string> userPacks = new List<string>();

        public bool discoveredTemple = false;
    }

    public SaveData saveData = new SaveData();






    






    




    



    void Start(){

        

        Npcs = FindObjectsOfType<SoccerPlayer>().ToList();
        playerInventory = FindObjectOfType<PlayerInventory>();
        squadManager = transform.GetComponent<SquadManager>();

        saveFilePath = Application.persistentDataPath + "/gameSave.json";
        
        
        if(mainMenuScene){
            
            LoadGameButton.Select();
            //enterButton.Enable();

            //mouseMovement.Enable();

            //enterButton.performed += context => EnterButton();
            //backButton.performed += context => BackButton();

            if(!File.Exists(saveFilePath)){
                LoadGameButton.gameObject.SetActive(false);
            }

        }else{
            //enterButton.Disable();
            LoadGame();
        }
            

    }


    //void Update(){
    //    if(mainMenuScene){
    //        mouseDirection = mouseMovement.ReadValue<Vector2>();
    //            Mouse.current.WarpCursorPosition(new Vector2(Mouse.current.position.x.value + mouseDirection.x,Mouse.current.position.y.value + mouseDirection.y));
    //    }
    //}

    /*
    void EnterButton(){
        if(!mainMenuScene){
            return;
        }
        var selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        selectedButton.onClick.Invoke();
        
    }
    */




    public void NewGame(){
        //Enter player information
        
        NewGamePanel.SetActive(true);

        PlayerName.Select();
    }


    //Called on user selecting the enter new game button.
    public void EnterInfo(){
        saveData.playerName = PlayerName.text;
        saveData.teamName = TeamName.text;
        saveData.teamLocation = TeamLocation.text;

        saveData.sceneName = "Smalltown";

        saveData.playerAttributes = new PlayerAttributes();
        saveData.money = 100f;

        saveData.playerLocalPosition = new Vector3(100f,0f,100f);

        playerInventory = new PlayerInventory();

        //give player 4 bronze packs.
        saveData.userPacks.Add("Bronze");
        saveData.userPacks.Add("Bronze");
        saveData.userPacks.Add("Bronze");
        saveData.userPacks.Add("Bronze");
        SaveGame();

        SceneManager.LoadSceneAsync("Smalltown");

    }


    public void SaveGame(string scene = "", int destinationIndex = 0)
    {
           
        
        
        if(!mainMenuScene){

            saveData.money = playerInventory.money;

            saveData.sceneName = SceneManager.GetActiveScene().name;
            saveData.playerLocalPosition = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).localPosition;


            saveData.items.Clear();
            for(int i=0;i<playerInventory.items.Count;i++){
                saveData.items.Add(playerInventory.items[i]);
            }


            //Save squad players
            saveData.squadPlayers.Clear();
            for(int i=0;i<squadManager.players.Count;i++){
                saveData.squadPlayers.Add(squadManager.players[i]);
            }


            //Save clubhouse players
            saveData.clubhousePlayers.Clear();
            for(int i=0;i<squadManager.clubhousePlayers.Count;i++){
                saveData.clubhousePlayers.Add(squadManager.clubhousePlayers[i]);
            }


        } 
        

        if(scene != ""){
            saveData.sceneName = scene;
            saveData.playerLocalPosition = trainStationPositions[destinationIndex];
        }



        
        

        // Serialize to JSON
        string jsonData = JsonUtility.ToJson(saveData);

        // Save to file
        File.WriteAllText(saveFilePath, jsonData);



    }




    public void LoadGame(){
        

        Debug.Log("loading game..");
        // Load from file
        string jsonData = File.ReadAllText(saveFilePath);
        saveData = JsonUtility.FromJson<SaveData>(jsonData);

        Debug.Log(saveData.sceneName);

        if(mainMenuScene){
            SceneManager.LoadSceneAsync(saveData.sceneName);
            return;
        }

        var player =  GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
        //player.localPosition = saveData.playerLocalPosition;
        player.transform.GetChild(0).gameObject.SetActive(true);


        
        
       
        playerInventory.money = saveData.money;


        if(saveData.items != null){
            for(int i = 0; i<saveData.items.Count;i++){
                playerInventory.items.Add(saveData.items[i]);
            }
        }






        if(saveData.squadPlayers == null){return;}


        //Load Squad Players
        squadManager.players.Clear();

        for(int i=0;i<saveData.squadPlayers.Count;i++){

            squadManager.players.Add(saveData.squadPlayers[i]);

            //Set NPCS
            foreach(var npc in Npcs){
                if(npc.playerAttributes.playerName == saveData.squadPlayers[i].playerName){
                    npc.playerAttributes = saveData.squadPlayers[i];
                }
            }
        }


        //Load clubhouse players
        squadManager.clubhousePlayers.Clear();

        for(int i=0;i<saveData.clubhousePlayers.Count;i++){

            squadManager.clubhousePlayers.Add(saveData.clubhousePlayers[i]);
        }
        

        
    }


    public void PlayMultiplayer(string scene){
        SceneManager.LoadSceneAsync(scene);
    }
}
