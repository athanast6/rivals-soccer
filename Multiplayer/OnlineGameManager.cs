using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.AI;
using System.IO.Pipes;
using Unity.Mathematics;
using UnityEngine.UI;
using Unity.Netcode;


public class OnlineGameManager : NetworkBehaviour
{   
    //References
    public List<OnlinePlayerController> UserPlayers;
    //private MixamoPlayerController Player;
    private PlayerInventory playerInventory;
    private SquadManager squadManager;
    private SaveManager saveManager;
    private Powerups powerups;




    [SerializeField] private GameObject Npcs;
    private List<GameObject> town;


    [SerializeField] public List<EnemyAIController> enemyPlayers;
    [SerializeField] public List<EnemyAIController> friendlyPlayers;
    [SerializeField] private GoalieController enemyGoalie, friendlyGoalie;

    [SerializeField] private GameObject Field;
    [SerializeField] private GameObject Ball;
    private Rigidbody ballRb;





    [SerializeField] private GameObject SoccerGameUI, OfferGameUI, ResultUI;
    [SerializeField] private TextMeshProUGUI homeScoreText, awayScoreText, timeLeftText, resultText;
    [SerializeField] private GameObject HomePlayers, AwayPlayers;
    [SerializeField] private List<GameObject> GoalScoredFX = new List<GameObject>();
    public GameObject Fences;
    public GameObject PassingToPlayer;


    public Material BlueShirt,RedShirt;


    






    private System.Random random = new System.Random();




    //Variables
    public float[] enemyDistancesToBall;
    public float[] friendlyDistancesToBall;
    public bool homeTeamAttacking;
    public bool goalieHasBall;
    public bool isGoalKick;

    public GameObject CurrentGoalie;
    public float fieldWidth;
    public float fieldLength;
    public Vector3 ballSpawnPos = new Vector3(518.95f,9.0f,609.7f);
    

    public int homeScore, awayScore;
    public int secondsLeft;
    public bool isHomeTeam;
    private float gamePrice;

    public float winnerCoins = 40.0f,loserCoins = 0f,drawCoins = 20.0f;





    void Start(){

        town = GameObject.FindGameObjectsWithTag("Town").ToList();

        squadManager = GetComponent<SquadManager>();
        saveManager = GetComponent<SaveManager>();
        powerups = GetComponent<Powerups>();

        ballRb = Ball.transform.GetComponent<Rigidbody>();

        //Player = GameObject.FindGameObjectWithTag("Player").transform.GetComponentInChildren<MixamoPlayerController>();
        //playerInventory = Player.transform.GetComponent<PlayerInventory>();

        resultText = ResultUI.GetComponentInChildren<TextMeshProUGUI>();


        //InvokeRepeating("CheckClosestToBall",0f,0.1f);

        
    }



    public void SpawnBall(){
        
        var newBall = Instantiate(Ball,ballSpawnPos,Quaternion.identity);
        newBall.SetActive(true);

        Ball = newBall;

        
    }


    public async void GoalScored(bool homeTeam){

        Debug.Log("Is Server: " + IsServer);
        Debug.Log("Is Client: " + IsClient);
        Debug.Log("Is Host: " + IsHost);

        if(!IsHost) return;

        goalieHasBall = false;
        var fx = GoalScoredFX[random.Next(0,GoalScoredFX.Count-1)];
        Instantiate(fx, Ball.transform.position,new Quaternion(90f,0f,90f,0f));


        if(homeTeam){
            homeScore+=1;
            homeScoreText.text = "Home: " + homeScore;
        }else{
            awayScore+=1;
            awayScoreText.text = "Away: " + awayScore;
        }

        Ball.SetActive(false);

        await Task.Delay(4000);

        
        
        //Reset all players.

        ResetPlayers();
        

        
        //Reset Ball.
        ResetBall();
        

        await Task.CompletedTask;
        

    }

    public async void CheckClosestToBall(){
        
        for(int i=0;i<enemyPlayers.Count;i++){
            enemyDistancesToBall[i] = (enemyPlayers[i].transform.position - Ball.transform.position).magnitude;
            enemyPlayers[i].closestToBall = false;
        }
        for(int i=0;i<friendlyPlayers.Count;i++){
            friendlyDistancesToBall[i] = (friendlyPlayers[i].transform.position - Ball.transform.position).magnitude;
            friendlyPlayers[i].closestToBall = false;
        }

        if(UserPlayers.Count == 2){
            enemyDistancesToBall[4] = (UserPlayers[1].transform.position - Ball.transform.position).magnitude;
        }

        var closestToBall = Array.IndexOf(enemyDistancesToBall, enemyDistancesToBall.Min());
        enemyPlayers[closestToBall].closestToBall = true;

        
        

        friendlyDistancesToBall[4] = (UserPlayers[0].transform.position - Ball.transform.position).magnitude;

        closestToBall = Array.IndexOf(friendlyDistancesToBall, friendlyDistancesToBall.Min());
        if(closestToBall!=4) friendlyPlayers[closestToBall].closestToBall = true;

        await Task.CompletedTask;

    }



    private void ResetPlayers(){
        /*


        AI PLAYERS


        for(int i=0;i<enemyPlayers.Count;i++){
            enemyPlayers[i].gameObject.transform.localPosition = Vector3.zero + new Vector3(-30.0f,-0.68f,UnityEngine.Random.Range(-20.0f,20.0f));
            //enemyPlayers[i].transform.GetComponent<NavMeshAgent>().isStopped = true;
        }
        for(int i=0;i<friendlyPlayers.Count;i++){
            friendlyPlayers[i].transform.localPosition =  Vector3.zero + new Vector3(30.0f,-0.68f,UnityEngine.Random.Range(-20.0f,20.0f));
            
        }
        */

        UserPlayers[0].transform.position = Field.transform.position + new Vector3(30.0f,0f,UnityEngine.Random.Range(-20.0f,20.0f));

        if(UserPlayers.Count == 2){
            UserPlayers[1].transform.position = Field.transform.position + new Vector3(-30.0f,0f,UnityEngine.Random.Range(-20.0f,20.0f));
        }
    }



    private async void ResetBall(){

        

        Ball.transform.localPosition = ballSpawnPos;

        Ball.SetActive(true);
        ballRb.WakeUp();
        
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        await Task.CompletedTask;
    }







    /*
    public async void StartGame(){

        //Set Users
        //UserPlayers = FindObjectsOfType<OnlinePlayerController>().ToList();

        //if(UserPlayers.Count == 2){
        //    friendlyPlayers[3].gameObject.SetActive(false);
        //}
        

        ToggleTown(false);

        saveManager.SaveGame();

        Npcs.SetActive(false);
        Fences.SetActive(true);

        if(playerInventory != null){
            playerInventory.money -= gamePrice;
        }

        secondsLeft = 300;
        //Fade to black

        LoadPlayersFromSquad();

        ResetPlayers();

        ResetBall();

        ResetScoreboard();

        //Fade in

        //Start countdown
        await Task.Delay(3000);

        StartClock();

        SoccerGameUI.SetActive(true);

        HomePlayers.SetActive(true);
        AwayPlayers.SetActive(true);

        InvokeRepeating("CheckClosestToBall",0f,0.05f);
        
        powerups.enabled = true;
        
        await Task.CompletedTask;
    }
    */

    public async void StartGame(){

        if(!IsHost){
            return;
        }

        UserPlayers = FindObjectsOfType<OnlinePlayerController>().ToList(); 

        UserPlayers[0].transform.position = Field.transform.position + new Vector3(30.0f,0f,UnityEngine.Random.Range(-20.0f,20.0f));
        UserPlayers[0].shirt.material = BlueShirt;

        if(UserPlayers.Count == 2){
            UserPlayers[1].transform.position = Field.transform.position + new Vector3(-30.0f,0f,UnityEngine.Random.Range(-20.0f,20.0f));

            UserPlayers[1].shirt.material = RedShirt;
        }
        

        
        

        ToggleTown(false);        
        
        secondsLeft = 300;
        ResetBall();
        ResetScoreboard();

        //Start countdown
        await Task.Delay(3000);

        StartClock();

        SoccerGameUI.SetActive(true);

        powerups.enabled = true;

        await Task.CompletedTask;
    }











    private void ResetScoreboard(){
        timeLeftText.text = secondsLeft.ToString();
        homeScoreText.text = "Home: 0";
        awayScoreText.text = "Away: 0";
    }

    private async void StartClock(){
        
        while(secondsLeft>0){

            await Task.Delay(1000);

            secondsLeft--; 

            var timeAmount = TimeSpan.FromSeconds(secondsLeft);
            timeLeftText.text = timeAmount.ToString();
        }

        EndGame();

        await Task.CompletedTask;
    }









    private async void EndGame(){
        
        for(int i=0;i<enemyPlayers.Count;i++){
            enemyPlayers[i].transform.GetComponent<NavMeshAgent>().isStopped = true;
        }
        for(int i=0;i<friendlyPlayers.Count;i++){
            friendlyPlayers[i].transform.GetComponent<NavMeshAgent>().isStopped = true;
        }


        HomePlayers.SetActive(false);
        AwayPlayers.SetActive(false);

        ToggleTown(true);


        
        await ShowResult();

        powerups.enabled = false;
        

        SoccerGameUI.SetActive(false);

        Fences.SetActive(false);

        Npcs.SetActive(true);

        saveManager.SaveGame();
        await Task.CompletedTask;
    }











    private async Task ShowResult(){
        ResultUI.SetActive(true);

        if(homeScore == awayScore){
            resultText.text = $"Draw. \n Won {drawCoins} coins.";
            playerInventory.money += drawCoins;

            saveManager.saveData.draws +=1;
        }
        else if((isHomeTeam && homeScore > awayScore) 
        || (!isHomeTeam && awayScore > homeScore)){
            resultText.text = $"Victory! \n Won {winnerCoins} coins!";
            playerInventory.money += winnerCoins;

            saveManager.saveData.wins +=1;

        }else if((!isHomeTeam && homeScore > awayScore)
        || (isHomeTeam && awayScore > homeScore)){
            resultText.text = $"Defeat. \n Won {loserCoins} coins.";
            playerInventory.money += loserCoins;

            saveManager.saveData.losses +=1;

        }

        var canvasGroup = resultText.GetComponent<CanvasGroup>();

        await FadeAsync(0,1,1.5f, canvasGroup);

        await Task.Delay(5000);

        await FadeAsync(1,0,1.5f, canvasGroup);


        ResultUI.SetActive(false);

        await Task.CompletedTask;
    }





    public async void OfferGame(GameObject Ref){
        
        var referee = Ref.GetComponent<Referee>();

        //Show referee message.
        referee.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);

        
        if(playerInventory == null || playerInventory.money >= referee.gamePrice){
            gamePrice = referee.gamePrice;
            OfferGameUI.SetActive(true);
        }

        await Task.Delay(5000);

        OfferGameUI.SetActive(false);

        referee.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);

        await Task.CompletedTask;
    }


    private void LoadPlayersFromSquad(){
        
        if(squadManager.players == null){return;}


        var index = 0;
        var goalieLoaded = false;

        foreach(var player in squadManager.players){

            //Load Goalie
            if(player.fieldPosition == PlayerAttributes.FieldPosition.Goalie && !goalieLoaded){
                goalieLoaded = true;
                friendlyGoalie.speed = player.runSpeed;
                friendlyGoalie.playerName = player.playerName;
                friendlyGoalie.transform.localScale = new Vector3(player.sizeScale,player.sizeScale,player.sizeScale);

                friendlyGoalie.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.playerName;

            }
            
            
            //Load Strikers, midfielders,defense

            else{

                friendlyPlayers[index].playerAttributes =  player;
                friendlyPlayers[index].transform.localScale = new Vector3(player.sizeScale,player.sizeScale,player.sizeScale);

                friendlyPlayers[index].transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.playerName;

                index++;

            }

            
            
        }
        
    }

    public async void ToggleTown(bool turnOn){


        foreach(var building in town){
        
            building.SetActive(turnOn);
            
        }

        await Task.CompletedTask;
    }


    public async void GoalKick(){
        
        isGoalKick = true;
        
        
        if(Vector3.Distance(Ball.transform.position,friendlyGoalie.transform.position) < 
        Vector3.Distance(Ball.transform.position,enemyGoalie.transform.position)){
            CurrentGoalie = friendlyGoalie.gameObject;

            goalieHasBall = true;
            await Task.Delay(2000);
            

            
            Ball.transform.position = friendlyGoalie.defendingGoal.position + friendlyGoalie.defendingGoal.transform.right*-10.0f;
            
            
            
            
        }else{
            CurrentGoalie = enemyGoalie.gameObject;

            goalieHasBall = true;
            await Task.Delay(2000);
            
            Ball.transform.position = enemyGoalie.defendingGoal.position + enemyGoalie.defendingGoal.transform.right*-10.0f;

        
            
        }

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        

        isGoalKick = false;

        await Task.CompletedTask;
    }


    
    private static async Task FadeAsync(float start, float end, float duration, CanvasGroup canvasGroup)
    {
        
        float time = 0f;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            await Task.Yield(); // Allow other asynchronous tasks to run
        }

        // Ensure the final alpha value is set
        canvasGroup.alpha = end;
    }

    
}
