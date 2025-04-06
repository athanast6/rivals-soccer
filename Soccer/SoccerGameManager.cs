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
using Unity.VisualScripting;


public class SoccerGameManager : MonoBehaviour
{   
    //References
    //public List<OnlinePlayerController> UserPlayers;
    private MenuManager menuManager;
    //Multiplayer: Need to change to an array of players.
    [SerializeField] private MixamoPlayerController Player;
    public bool multiplayerMode;
    [SerializeField] private List<MixamoPlayerController> multiplayers;
    private PlayerInventory playerInventory;
    private SquadManager squadManager;
    private SaveManager saveManager;
    private Powerups powerups;
    private SoccerGameAudio soccerGameAudio;

    [SerializeField] private GameObject GameCamera;
    




    [SerializeField] private GameObject Npcs;
    private List<GameObject> town;


    [SerializeField] public List<EnemyAIController> enemyPlayers;
    [SerializeField] public List<EnemyAIController> friendlyPlayers;
    [SerializeField] private GoalieController enemyGoalie, friendlyGoalie;

    [SerializeField] private GameObject Field;
    [SerializeField] private GameObject Ball;
    private Rigidbody ballRb;





    [SerializeField] private GameObject SoccerGameUI, OfferGameUI, ResultUI;
    private TextMeshProUGUI offerGameText;
    [SerializeField] private TextMeshProUGUI homeScoreText, awayScoreText, timeLeftText, resultText, activePlayerText, goalScoredText;
    [SerializeField] public GameObject HomePlayers, AwayPlayers;
    [SerializeField] private List<GameObject> GoalScoredFX = new List<GameObject>();
    public GameObject Fences;
    public GameObject PassingToPlayer;
    public Transform friendlyNearestBall;





    






    private System.Random random = new System.Random();




    //Variables
    public float[] enemyDistancesToBall;
    public float[] friendlyDistancesToBall;
    public bool homeTeamAttacking = false;
    public bool goalieHasBall;
    public bool isGoalKick;
    public bool isThrowIn;

    public GameObject CurrentGoalie;
    public float fieldWidth;
    public float fieldLength;
    

    public int homeScore, awayScore;
    public int secondsLeft;
    public bool isHomeTeam;
    private float gamePrice;
    public int gameTime = 120;

    public float winnerCoins = 40.0f,loserCoins = 0f,drawCoins = 20.0f;

    public string lastToTouchBall;
    public List<string> goalsScored;

    public Vector3 gameCameraPosition;

    [SerializeField] GameObject Boundaries;


    public enum GameState{
        Normal,
        GoalKick,
        ThrowIn,
        Corner
    }





    void Start(){
        transform.GetComponent<PackManager>().GivePack("Gold");

        menuManager = GetComponent<MenuManager>();

        town = GameObject.FindGameObjectsWithTag("Town").ToList();

        squadManager = GetComponent<SquadManager>();
        saveManager = GetComponent<SaveManager>();
        powerups = GetComponent<Powerups>();
        soccerGameAudio = GetComponent<SoccerGameAudio>();

        ballRb = Ball.transform.GetComponent<Rigidbody>();

        playerInventory = Player.transform.GetComponent<PlayerInventory>();

        resultText = ResultUI.GetComponentInChildren<TextMeshProUGUI>();

        offerGameText = OfferGameUI.GetComponentInChildren<TextMeshProUGUI>();


        //InvokeRepeating("CheckClosestToBall",0f,0.1f);
    }



    public async void GoalScored(bool homeTeam){

        //Set User Attacking To False. User is always home team for now.
        homeTeamAttacking = false;

        
        goalScoredText.transform.parent.gameObject.SetActive(true);

        ballRb.velocity=Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        soccerGameAudio.GoalScoredAudio();

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

        var goalInfo = "Goal Scored By: " + lastToTouchBall + "! Home: " + homeScore + ", Away: " + awayScore;
        goalsScored.Add(goalInfo);
        goalScoredText.text = goalInfo;

        Ball.SetActive(false);

        PausePlayers(false);

        await Task.Delay(7000);

        
        ResetPlayers();
        ResetBall();

        PausePlayers(true);

        
        await Task.Delay(5000);
        goalScoredText.transform.parent.gameObject.SetActive(false);
        

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

        //if(UserPlayers.Count == 2){
        //    enemyDistancesToBall[4] = (UserPlayers[1].transform.position - Ball.transform.position).magnitude;
        //}

        var closestToBall = Array.IndexOf(enemyDistancesToBall, enemyDistancesToBall.Min());
        enemyPlayers[closestToBall].closestToBall = true;

        
        

        //friendlyDistancesToBall[4] = (Player.transform.position - Ball.transform.position).magnitude;

        closestToBall = Array.IndexOf(friendlyDistancesToBall, friendlyDistancesToBall.Min());
        friendlyPlayers[closestToBall].closestToBall = true;

        friendlyNearestBall = friendlyPlayers[closestToBall].transform;
        //IF closest to ball changes, player control changes
        //if(friendlyNearestBall != friendlyPlayers[closestToBall].transform &&
        //friendlyNearestBall != null && homeTeamAttacking){
            
            //friendlyNearestBall = friendlyPlayers[closestToBall].transform;
            //Player to change is whichever character has a mixamoplayercontroller active.
            
            //var playersChange = FindObjectsOfType<MixamoPlayerController>();
            //foreach(var player in playersChange){
            //    if(player.enabled){
            //        Debug.Log("Changing from player: " + player.name);
            //        player.ChangePlayers();
            //    }
            //}

            //var playerToChange = friendlyNearestBall;

            //friendlyNearestBall = friendlyPlayers[closestToBall].transform;

            //playerToChange.GetComponent<MixamoPlayerController>().ChangePlayers();
                       

        //}

        //friendlyNearestBall = friendlyPlayers[closestToBall].transform;

        



        

        await Task.CompletedTask;
        
        

    }

    public async void PausePlayers(bool canMove){
        
        if(canMove){
            Player.playerControls.Enable();
        }
        else{
            Player.playerControls.Disable();
        }
        
        
        for(int i=0;i<enemyPlayers.Count;i++){
            Debug.Log("Enemy Player " + i + ". " + enemyPlayers[i]);
            var controller = enemyPlayers[i].GetComponent<MixamoPlayerController>();

            if (controller != null && controller.isActiveAndEnabled){}

            else{
                enemyPlayers[i].enabled = canMove;
                var agent=enemyPlayers[i].transform.GetComponent<NavMeshAgent>();

                if(agent.enabled && agent.gameObject.activeInHierarchy){agent.SetDestination(agent.transform.position);}
                
                agent.enabled = canMove;
            }
            
        }
        for(int i=0;i<friendlyPlayers.Count;i++){

            if(friendlyPlayers[i].transform.GetComponent<MixamoPlayerController>().isActiveAndEnabled){}

            else{
                friendlyPlayers[i].enabled = canMove;
                var agent=friendlyPlayers[i].transform.GetComponent<NavMeshAgent>();
                
                if(agent.enabled && agent.gameObject.activeInHierarchy){agent.SetDestination(agent.transform.position);}

                agent.enabled = canMove;
            }

            
        }

        await Task.CompletedTask;
    }

    private void ResetPlayers(){
        for(int i=0;i<enemyPlayers.Count;i++){
            enemyPlayers[i].gameObject.transform.localPosition = Vector3.zero + new Vector3(-30.0f,-0.68f,UnityEngine.Random.Range(-20.0f,20.0f));
            //enemyPlayers[i].transform.GetComponent<NavMeshAgent>().isStopped = true;
        }
        for(int i=0;i<friendlyPlayers.Count;i++){
            friendlyPlayers[i].transform.localPosition =  Vector3.zero + new Vector3(30.0f,-0.68f,UnityEngine.Random.Range(-20.0f,20.0f));

            if(friendlyPlayers[i].transform.GetComponent<MixamoPlayerController>().isActiveAndEnabled){
                friendlyPlayers[i].transform.position = Field.transform.position + new Vector3(30.0f,0f,UnityEngine.Random.Range(-20.0f,20.0f));
            }
            
        }
        
        

        //if(UserPlayers.Count == 2){
        //    UserPlayers[1].transform.position = Field.transform.position + new Vector3(-30.0f,0f,UnityEngine.Random.Range(-20.0f,20.0f));
        //}
    }

    private async void ResetBall(){

        ballRb.isKinematic = false;

        Ball.transform.parent = Field.transform;

        Ball.transform.localPosition = Vector3.zero;
        Ball.SetActive(true);
        ballRb.WakeUp();
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        await Task.CompletedTask;
    }








    public async void StartGame(){

        //Set Users
        //UserPlayers = FindObjectsOfType<OnlinePlayerController>().ToList();

        //if(UserPlayers.Count == 2){
        //    friendlyPlayers[3].gameObject.SetActive(false);
        //}
        
        if(multiplayerMode){
            for(int i=0; i<multiplayers.Count;i++){
                multiplayers[i].GetComponent<MixamoPlayerController>().StartSoccerGame();
                multiplayers[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }else{
            Player.GetComponent<MixamoPlayerController>().StartSoccerGame();
            Player.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        
        //Change camera
        
        GameCamera.SetActive(true);

        //Set Camera Height
        GameCamera.transform.localPosition = gameCameraPosition;

        ToggleTown(false);

        if(!multiplayerMode) saveManager.SaveGame();

        Npcs.SetActive(false);
        Fences.SetActive(true);

        if(playerInventory != null){
            playerInventory.money -= gamePrice;
        }

        secondsLeft = gameTime;
        //Fade to black

        LoadPlayersFromSquad();

        ResetPlayers();

        ResetBall();

        ResetScoreboard();

        UpdatePlayerUI(saveManager.saveData.playerName);

        //Fade in

        //Start countdown
        await Task.Delay(3000);

        StartClock();

        soccerGameAudio.StartGameAudio();

        SoccerGameUI.SetActive(true);

        HomePlayers.SetActive(true);
        AwayPlayers.SetActive(true);

        enemyGoalie.transform.localPosition = new Vector3(-85.9f,-0.69f,2.93f);
        friendlyGoalie.transform.localPosition = new Vector3(62.2f,-0.69f,2.17f);

        GameObject.FindWithTag("Play Game").SetActive(false);

        InvokeRepeating("CheckClosestToBall",0f,0.05f);
        
        powerups.enabled = true;
        
        await Task.CompletedTask;
    }











    private void ResetScoreboard(){
        homeScore = 0;
        awayScore = 0;
        timeLeftText.text = secondsLeft.ToString();
        homeScoreText.text = "Home: 0";
        awayScoreText.text = "Away: 0";
    }

    private async void StartClock(){
        
        while(secondsLeft>0){

            
                await Task.Delay(1000);
                if(menuManager.isPaused == false){
                    secondsLeft--; 

                    var timeAmount = TimeSpan.FromSeconds(secondsLeft);
                    timeLeftText.text = timeAmount.ToString(@"m\:ss");
                }      

            
        }

        EndGame();

        await Task.CompletedTask;
    }









    private async void EndGame(){

        
        
        soccerGameAudio.EndGameAudio();

        CancelInvoke("CheckClosestToBall");

        for(int i=0;i<enemyPlayers.Count;i++){
            NavMeshAgent cur_agent;
            enemyPlayers[i].transform.TryGetComponent<NavMeshAgent>(out cur_agent);
            if(cur_agent.isActiveAndEnabled){
                cur_agent.isStopped = true;
            }
            
        }
        //for(int i=0;i<friendlyPlayers.Count;i++){
        //    if(friendlyPlayers[i].transform != Player.transform){
                //friendlyPlayers[i].transform.GetComponent<MixamoPlayerController>().DisablePlayer();
                //friendlyPlayers[i].transform.GetComponent<NavMeshAgent>().isStopped = true;
        //    }
            
        // }




        
        


        HomePlayers.SetActive(false);
        AwayPlayers.SetActive(false);

        ToggleTown(true);


        
        await ShowResult();

        powerups.enabled = false;

        //Change camera
        Player.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        GameCamera.SetActive(false);

        //Player.GetComponent<MixamoPlayerController>().enabled = true;
        if(multiplayerMode){
            for(int i=0; i<multiplayers.Count;i++){
                multiplayers[i].GetComponent<MixamoPlayerController>().EndSoccerGame();
            }
        }else{
            Player.GetComponent<MixamoPlayerController>().EndSoccerGame();
        }


        //Change Players
        //Player.EnablePlayer(Player.transform);
        

        SoccerGameUI.SetActive(false);

        Fences.SetActive(false);

        Npcs.SetActive(true);

        if(!multiplayerMode) saveManager.SaveGame();

        GameObject.FindWithTag("Play Game").SetActive(true);

        
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

            if(saveManager.saveData.wins == 1){
                transform.GetComponent<PackManager>().GivePack("Gold");
            }

        }else if((!isHomeTeam && homeScore > awayScore)
        || (isHomeTeam && awayScore > homeScore)){
            resultText.text = $"Defeat. \n Won {loserCoins} coins.";
            playerInventory.money += loserCoins;

            saveManager.saveData.losses +=1;

        }

        transform.GetComponent<PackManager>().GivePack("Gold");
            

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


            offerGameText.text = $"Play Game For {referee.gamePrice} Coins.";

            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = true;
            MenuManager.Instance.ShowCursor();
            //Mouse.current.WarpCursorPosition(new Vector2(960f,100f));
        }

        //await Task.Delay(5000);

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        MenuManager.Instance.HideCursor();

        //OfferGameUI.SetActive(false);

        referee.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);

        await Task.CompletedTask;
    }


    private void LoadPlayersFromSquad(){
        
        if(squadManager.players == null){return;}


        //Index 1 because of the user player
        var index = 1;
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

                var playerController = friendlyPlayers[index].transform.GetComponent<MixamoPlayerController>();
                playerController.runSpeed = player.runSpeed;
                playerController.walkSpeed = 5.0f;
                playerController.shotAccuracy = player.shotAccuracy;
                playerController.originalRunSpeed = player.runSpeed;

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

        await Task.Delay(3000);
        Ball.SetActive(false);
        
        var friendlyDistance = Vector3.Distance(Ball.transform.position,friendlyGoalie.transform.position);
        var enemyDistance = Vector3.Distance(Ball.transform.position,enemyGoalie.transform.position);
        
        if(friendlyDistance < enemyDistance){

            CurrentGoalie = friendlyGoalie.gameObject;
            
            
            //Ball.transform.position = friendlyGoalie.defendingGoal.position + friendlyGoalie.defendingGoal.transform.right*-10.0f;
            
        }else if(enemyDistance < friendlyDistance){

            CurrentGoalie = enemyGoalie.gameObject;
            
            //Ball.transform.position = enemyGoalie.defendingGoal.position + enemyGoalie.defendingGoal.transform.right*-10.0f;
        }

        Debug.Log(CurrentGoalie);

        
        
        var curGoalControl = CurrentGoalie.GetComponent<GoalieController>();
        curGoalControl.GoalKickColliders.SetActive(true);
        
        
        


        //If user is home team, set hometeamattacking false
        if(Player.isHomeTeam){
            homeTeamAttacking = false;
        }else{
            homeTeamAttacking = true;
        }

        var activePlayer = Player.GetComponentInChildren<MixamoPlayerController>();
        if(!multiplayerMode){
            activePlayer.GetComponent<EnemyAIController>().enabled = true;
            activePlayer.GetComponent<NavMeshAgent>().enabled = true;
            activePlayer.enabled = false;
        }
        
    
       

        

    
        //Move all players outside of the goalkeeper box.
        for(int i=0;i<friendlyPlayers.Count;i++){
            var index = i;
            if(friendlyPlayers[index].transform.localPosition.x < -71.0f || friendlyPlayers[index].transform.localPosition.x > 71.0f){

                friendlyPlayers[index].MovePlayerGoalKick();

            }
            if(enemyPlayers[index].transform.localPosition.x < -71.0f || enemyPlayers[index].transform.localPosition.x > 71.0f){

                enemyPlayers[index].MovePlayerGoalKick();

            }
            
        }


        await Task.Delay(3000);

        Ball.SetActive(true);

        Ball.transform.position = curGoalControl.defendingGoal.position + curGoalControl.defendingGoal.transform.right*-10.0f;

        CurrentGoalie.transform.position = new Vector3(Ball.transform.position.x,transform.position.y,Ball.transform.position.z);

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        

        await Task.Delay(2000);

        if(!multiplayerMode){
            activePlayer.GetComponent<EnemyAIController>().enabled = false;
            activePlayer.GetComponent<NavMeshAgent>().enabled = false;
            activePlayer.enabled = true;
        }
        

       

       
        

        isGoalKick = false;

        await Task.CompletedTask;
    }


    /// <summary>
    /// Called when ball goes across touch line.
    /// Pause players momentarily.
    /// Send Nearest Player To Ball. (from opposing team as last to touch ball).
    /// Make Them throw it in.
    /// Or Allow the user to throw in.
    /// </summary>
    /// <param name="playerName"></param>
    public async void ThrowIn(Vector3 ballPositionForThrowIn){

        PausePlayers(false);

        Player.playerControls.Disable();

        //Temporarily disable the sidelines.
        Boundaries.SetActive(false);

        await Task.Delay(2500);

        ballRb.transform.localPosition = new Vector3(ballPositionForThrowIn.x,0f,ballPositionForThrowIn.z) + new Vector3(0f,0f,1.0f);

        for(int i=0;i<friendlyPlayers.Count;i++){
            if(friendlyPlayers[i].closestToBall){
                friendlyPlayers[i].transform.position = ballRb.transform.position + new Vector3(-3.0f,0f,0f);
            }
        }

        
       
        
        ballRb.velocity = Vector3.zero;

        await Task.Delay(2000);
        
        Player.playerControls.Enable();

        //wait for ball to get thrown in
        await Task.Delay(1000);

        PausePlayers(true);

        Boundaries.SetActive(true);

        await Task.CompletedTask;
    }

    

    public async void UpdatePlayerUI(string playerName){
        activePlayerText.text = playerName;
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
