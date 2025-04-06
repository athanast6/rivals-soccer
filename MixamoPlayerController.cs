using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Unity.VisualScripting;
//using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
using Unity.Burst.CompilerServices;

public class MixamoPlayerController : MonoBehaviour
{
    //To be used in multiplayer.
    public int playerIndex;
    private DataCollector dataCollector;

    //References
    public Animator playerAnimator;
    //[SerializeField] private AnimatorController normalAnim;

    private Rigidbody playerRb;
    [SerializeField] private BoxCollider boxC;
    private GameObject camera;
    public GameCamera gameCamera;
    [SerializeField] private CinemachineFreeLook lookCamera;
    public Gamepad gamepad;

    //private UserPlayerAudio userAudio;
    private Coroutine staminaCoroutine;


    [SerializeField] private Rigidbody ballRb;

    private EnemyAIController aiController;
    private NavMeshAgent navAgent;

    [SerializeField] private SoccerGameManager soccerGameManager;
    [SerializeField] private Transform CurrentField;
    [SerializeField] private Transform PlayerBallEmpty;
    [SerializeField] private GameObject SpeedBoost;
    [SerializeField] private GameObject MagicSpell;
    [SerializeField] private Slider ShotPowerSlider;
    [SerializeField] private GameObject AttackingGoal;
    [SerializeField] private Vector3 ballShootPosition = new Vector3(0.37f,0.23f,0.4f);
    [SerializeField] private int shotDelayTime = 300;




    [SerializeField] private SoccerBall soccerBall;

    [SerializeField] private Transform head;







    //Variables
    private string playerName;
    private float kickForce;
    public float shotForce = 2.5f;
    public float shotAccuracy = 0.5f;
    private float forwardInput;
    private float sideInput;
    public bool playerOnGround;
    public float walkSpeed = 8.0f;
    public float originalRunSpeed;
    public float runSpeed = 10.0f;
    public float playerRotation = 60.0f;
    public float sideMoveSpeed = 2.0f;
    public Vector3 jumpForce;
    public float playerSpeed;
    private Vector2 moveDirection  = Vector2.zero;

    public bool isPlayingGame;
    public bool playerHasBall;
    public float kickPower;
    public float touchPower;
    public float touchPowerAddBoost;
    public bool isKicking;
    public float kickTime;
    public bool isKickTimer;
    public float clearPower;
    public string kickType;
    public float loftedKickForce;
    public float loftedAngle = 0.4f;

    public float touchDistance = 1.5f;
    
    public bool isHomeTeam = true;

    private bool isBoosting = false;

    public float goalieMoveForce = 1000.0f;

    public float stamina = 70.0f;
    public float staminaRemaining = 99.0f;
    public float movePlayerOnTouchDistance;

    public float acceleration = 20.0f;
    public float deceleration = 10.0f;

    public float lowerPassPower, upperPassPower;


    
  
    
    



    //INPUTS
    public InputActionMap playerControls;

    //Joystick Functionality
    public Joystick jsMovement;
    public Vector3 direction;
    
    

    private InputAction fpsMovement, thirdMovement, run, jump, kick, powerKick, switchCamera, slideTackle, punch, block, boost, requestPass, changePlayer, loftedKick;
    

    

    void OnEnable(){
        
        playerAnimator = GetComponent<Animator>();
        
        playerRb = GetComponent<Rigidbody>();
    }

    void OnDisable(){

        playerControls.Disable();

        //runSpeed = originalRunSpeed;

    }
    
    void Awake(){
        

        soccerGameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<SoccerGameManager>();

        
        

        aiController =  GetComponent<EnemyAIController>();
        navAgent = GetComponent<NavMeshAgent>();

        playerName = aiController.playerAttributes.playerName;

        //playerAnimator.runtimeAnimatorController = normalAnim;

        playerControls.Enable();

       

    }
    
    void Start()
    {

        if(!isHomeTeam)isPlayingGame=true;

        dataCollector = GetComponent<DataCollector>();

        InitializeInputs();
        originalRunSpeed = runSpeed;
        
        //ballRb = GameObject.FindGameObjectWithTag("Ball").transform.GetComponent<Rigidbody>();
        

        CurrentField = GameObject.FindGameObjectWithTag("Field").transform;


        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        //lookCamera = transform.GetChild(0).gameObject.transform.GetComponent<CinemachineFreeLook>();

        
        
        foreach (var gamepad in Gamepad.all)
        {
            Debug.Log("Detected Gamepad: " + gamepad.name);
        }
        
        
        // Assign gamepads manually based on player index
        if (Gamepad.all.Count > playerIndex)
        {
            Debug.Log(Gamepad.all);
            gamepad = Gamepad.all[playerIndex];

            // Manually assign gamepad to InputActions
            playerControls.devices = new InputDevice[] { gamepad };
        }
        
        //userAudio = GetComponent<UserPlayerAudio>();
    
        

    
       
    }


    private void InitializeInputs(){


        fpsMovement = playerControls.FindAction("First Person Movement");
        thirdMovement = playerControls.FindAction("Third Person Movement");
        run = playerControls.FindAction("Run");
        jump = playerControls.FindAction("Jump");
        boost = playerControls.FindAction("Boost");

        kick = playerControls.FindAction("Kick");
        powerKick = playerControls.FindAction("Power Kick");
        loftedKick = playerControls.FindAction("Lofted Pass");
        slideTackle = playerControls.FindAction("Slide Tackle");

        requestPass = playerControls.FindAction("Request Pass");

        changePlayer = playerControls.FindAction("Change Player");

        //changePlayer.performed +=
        //    context => {ChangePlayers();};


        
        jump.performed += 
            context => {OnJump();};


        

    
        kick.started +=
            context => {StartKick();};

        kick.canceled +=
            context => {CancelPass();};

        kick.performed +=
            context => {CancelPass();};

        powerKick.started +=
            context => {StartKick();};

        powerKick.canceled +=
            context => {CancelPowerKick();};

        powerKick.performed +=
            context => {CancelPowerKick();};

        loftedKick.started +=
            context => {StartKick();};

        loftedKick.canceled +=
            context => {CancelLofted();};

        loftedKick.performed +=
            context => {CancelLofted();};


        slideTackle.performed +=
            context => {SlideTackle();};

        boost.performed +=
            context => {Boost();};

        requestPass.performed +=
            context => {RequestPass();};

        playerControls.FindAction("Emote").performed +=
            context => {Emote();};

        //playerControls.FindAction("Fight").performed +=
        //    context => {EnterFightMode();};


        playerControls.Enable();

        
    }


    void Update()
    {
        if(isKickTimer){
            kickTime += Time.deltaTime;

            if(ShotPowerSlider!=null){
                ShotPowerSlider.value = kickTime*3.0f;
            }
            
        }
        
        var isRunning = run.ReadValue<float>();
        
        if(moveDirection.x == 0 && moveDirection.y == 0)
        {
            playerAnimator.SetTrigger("Idle");
            playerAnimator.ResetTrigger("Walk");
            playerAnimator.ResetTrigger("Run");

            //userAudio.IdleAudio();

        }else
        {
            if(isRunning >0f){

                playerAnimator.SetTrigger("Run");
                playerAnimator.ResetTrigger("Idle");
                playerAnimator.ResetTrigger("Walk");

                playerSpeed = runSpeed;

                //userAudio.WalkingAudio();

               
            }else{
                playerSpeed = walkSpeed;
                playerAnimator.SetTrigger("Walk");
                playerAnimator.ResetTrigger("Idle");
                playerAnimator.ResetTrigger("Run");

                //userAudio.WalkingAudio();
            }

            if(isPlayingGame){
                playerAnimator.SetFloat("ForwardBackward",1.0f);
            }else{
                playerAnimator.SetFloat("LeftRight",moveDirection.x);
                playerAnimator.SetFloat("ForwardBackward",moveDirection.y);
            }
            
            
        }
    }

    private void FixedUpdate(){
        
        
        if(isPlayingGame){

            Vector3 toBall = (ballRb.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toBall);

            if (angle < 90.0f) { 
                head.LookAt(ballRb.transform.position); // Adjust this as per your setup
            }

            //Debug.Log(moveDirection);
           
            if (gamepad != null && playerControls.enabled)
            {
                moveDirection = gamepad.leftStick.ReadValue();
            }
            
            //var dir = (gameCamera.transform.right * moveDirection.x) + (-gameCamera.transform.forward * moveDirection.y);
            var dir = (gameCamera.transform.right * direction.x) + (-gameCamera.transform.forward * direction.y);
            
            
        
            //var viewDir = camera.transform.position - new Vector3(transform.position.x,transform.position.y,transform.position.z);
            //orientation.fFmoveorward = viewDir.normalized;
            
            //transform.LookAt(dir);

            //Make a soccer player in unity spherically loop around the ball
            // to the opposite side of the direction the controller stick is facing
            // in order to make him touch the ball in the desired direction?

            //Player chase ball condition
            if (moveDirection.magnitude >= 0.1f)
            {
                //if ((soccerGameManager.lastToTouchBall == playerName && soccerGameManager.friendlyNearestBall == transform) || 
                //(soccerGameManager.lastToTouchBall != playerName && soccerGameManager.homeTeamAttacking && soccerGameManager.friendlyNearestBall == transform))
                if ((soccerGameManager.lastToTouchBall == playerName) || (Vector3.Distance(ballRb.transform.position, transform.position) < 5.0f))
                //OR IF THE BALL IS CLOSE ENOUGH

                {
                    // Get the direction to the ball
                    Vector3 ballDirection = ballRb.transform.position - transform.position;
                    ballDirection.y = 0f;

                    // Calculate the target rotation towards the ball
                    Quaternion targetRotation = Quaternion.LookRotation(ballDirection);

                    // Smoothly rotate towards the ball
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
                }

                else{
                    

                    // Calculate rotation angle in radians
                    float targetAngle = ((Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg) + 90.0f);

                    // Smoothly rotate towards the target angle (based on movement direction)
                    Quaternion newRot = Quaternion.Euler(0f, targetAngle, 0f);

                    // Smooth rotation when moving normally
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRot, 0.15f);
                    
                }



                
                
                    // Apply force for acceleration
                    playerRb.AddForce(transform.forward * acceleration * (staminaRemaining/80.0f), ForceMode.Acceleration);
            }
                    
            else
            {
                // Apply force for deceleration
                playerRb.velocity = Vector3.Lerp(playerRb.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            }

            // Limit max speed
            if (playerRb.velocity.magnitude > runSpeed)
            {
                playerRb.velocity = playerRb.velocity.normalized * runSpeed;
            }
                
    

                //transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime * (staminaRemaining/80.0f));

            if(isBoosting){
                playerRb.AddForce(transform.forward*10000.0f,ForceMode.Impulse);
                isBoosting=false;
            }


            
                

                
            

            

            return;

        }else{
            moveDirection = gamepad.leftStick.ReadValue();
            transform.Translate(Vector3.forward * playerSpeed * moveDirection.y * Time.deltaTime);
        
            //var viewDir = camera.transform.position - new Vector3(transform.position.x,transform.position.y,transform.position.z);
            //orientation.forward = viewDir.normalized;
            
            transform.localEulerAngles = new Vector3(0f,lookCamera.m_XAxis.Value,0f);
            transform.Translate(Vector3.right * sideMoveSpeed * moveDirection.x * Time.deltaTime);


        }
        
        
        return;
        
        
    }


    


    public void OnCollisionEnter(Collision other){

        if(enabled==false){return;}

        //Debug.Log(other.collider.gameObject.transform.tag);

       

        if(other.collider.gameObject.transform.tag == "Goalie"){
            Debug.Log("Hit Goalie.");
            playerRb.velocity = Vector3.zero;
            playerRb.AddForce(other.collider.gameObject.transform.forward * goalieMoveForce, ForceMode.Impulse);
            return;
        }
        
        if(other.transform.CompareTag("Ball")){
            soccerGameManager.homeTeamAttacking = true;

            soccerGameManager.lastToTouchBall = playerName;
            if(isKicking){
                playerHasBall = true;
                
                //If animation is playing for slide tackle,
                // Just take a touch.
                var stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Slide Tackle"))
                {
                    TouchBall();
                    return;
                }

                if(kickType == "Pass"){
                    PassTheBall();
                }else if(kickType == "Lofted"){
                    LoftedPass();
                }else if(kickType == "Shoot"){
                    OnPowerKick();
                }

                
                
                return;
            }

            TouchBall();
            return;
            
            
            if(playerHasBall){return;}

           

            ballRb = other.transform.GetComponent<Rigidbody>();

            other.transform.parent = PlayerBallEmpty;
            other.transform.localPosition = new Vector3(0f,0.23f,0f);
            other.transform.localEulerAngles = Vector3.zero;
            ballRb.isKinematic = true;

            playerHasBall = true;

            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);

            
            //if(isHomeTeam)
            soccerGameManager.homeTeamAttacking = true;

            soccerGameManager.lastToTouchBall = playerName;

            

           

        }

        
        if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide Tackle") && ((isHomeTeam && other.gameObject.CompareTag("Away Player") || (!isHomeTeam && other.gameObject.CompareTag("Home Player"))))){

            var enemyAI = other.gameObject.GetComponent<EnemyAIController>();

            if (enemyAI != null && enemyAI.isActiveAndEnabled) {
                enemyAI.SlideTackleAnimations();  // Call method if EnemyAIController exists
            } else {
                var otherPlayer = other.gameObject.GetComponent<MixamoPlayerController>();

                if (otherPlayer != null) {
                    otherPlayer.SlideTackleAnimations(); // Fallback behavior
                } else {
                    Debug.LogWarning("No suitable component found on " + other.gameObject.name);
                }
            }
        }


        
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Parkour")){
            playerOnGround = true;
        }


        if(other.gameObject.CompareTag("Play Game")){
            soccerGameManager.OfferGame(other.gameObject);

        }
        
    }


    /// <summary>
    /// Called by default on collision with ball.
    /// Kicks ball in forward direction.
    /// </summary>
    public async void TouchBall(){

        var curTouchPower = touchPower;

        //ballRb.isKinematic = false;
        //ballRb.transform.parent = CurrentField;

        var dir = (gameCamera.transform.right * moveDirection.x) + (gameCamera.transform.forward * moveDirection.y);
        dir.y=0f;

        var stick_mag = (float)moveDirection.sqrMagnitude;
        stick_mag = Math.Clamp(stick_mag,0.2f,1.0f);



        //Check the direction the player is facing and compare to direction of stick
        // If Angle Between Two is greater than 60 degrees,move player behind ball.
        // Spherically interpolate his position so he runs into the ball in the touch direction.
        if(Vector3.Angle(transform.forward,dir) > 50.0f){

            playerRb.AddForce(transform.up * 1.5f, ForceMode.Impulse);

            var targetPosition = ballRb.transform.position - (dir).normalized * movePlayerOnTouchDistance + Vector3.up * 0.05f;

            float elapsedTime = 0f;
            Vector3 startDir = (transform.position - ballRb.transform.position).normalized;
            Vector3 targetDir = (targetPosition - ballRb.transform.position).normalized;

            while (elapsedTime < 0.1f)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / 0.1f;

                // Smoothly interpolate along the sphere's surface
                Vector3 newPosition = Vector3.Slerp(startDir, targetDir, t);
                transform.position = ballRb.transform.position + newPosition * 2.0f;

                await Task.Yield(); // Wait until the next frame
            }


            Debug.Log("Moved player behind ball.");
        }



        //touchPower = staminaRemaining*0.2846f + 14.3462f;

        Debug.Log("Ridigbody velocity: " + playerRb.velocity.sqrMagnitude);
        if(playerRb.velocity.sqrMagnitude>30.0f){
            curTouchPower = touchPower + touchPowerAddBoost;
        }else if(playerRb.velocity.sqrMagnitude<5.0f){
            curTouchPower = touchPower - 15.0f;
        }

        

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        var kickVector = (dir*curTouchPower*stick_mag*(playerSpeed/5.0f));

        //ballRb.transform.position = transform.position + transform.forward*touchDistance;

        //soccerBall.Kick(kickVector);
        ballRb.AddForce(dir*curTouchPower*stick_mag*(playerSpeed/5.0f), ForceMode.Impulse);
        

        //dataCollector.LogDecision(0, moveDirection);
        
        
        playerHasBall = false;

        await Task.CompletedTask;
    }





   
    public void OnJump(){
        if(!playerOnGround){return;}
        if(playerHasBall){
            playerAnimator.Play("Soccer Spin");
            return;
        }

        playerAnimator.Play("Header");
        playerRb.AddForce(jumpForce, ForceMode.Impulse);
        playerOnGround = false;
        
    }

    private void StartKick(){
        //Update timer.
        isKicking = true;
        isKickTimer = true;
        
    }

    private async void CancelPass(){
        //if(playerHasBall){
        //    OnPass();
        //    return;
        //}
        isKickTimer = false;

        kickType = "Pass";
        
        await Task.Delay(1500);
        
        //Debug.Log("Cancelled Pass");
        isKicking = false;
        kickTime = 0f;
        await Task.CompletedTask;
    }

    public async void OnPass(){
        
        //isKicking = false;
       
        //if(!playerHasBall){
        //    kickTime = 0f;
        //    return;
        //}

        //ballRb.transform.localPosition = new Vector3(0f,0.23f,0.5f);
        


        //Debug.Log("Kicked Ball");
        

        kickForce = Math.Clamp(kickPower * kickTime,60.0f,300.0f);

        if(kickForce>200.0f){
            playerAnimator.Play("Strike");
        }else{
            playerAnimator.Play("Kick");
            
        }
        //PassTheBall();

        await Task.CompletedTask;

        
        
        
        
    }

    /// <summary>
    /// Called in order to provide force to pass the ball
    /// to the closest player the user is looking for.
    /// </summary>
    public async void PassTheBall(){

        var passVector = GetPassVector();

        ballRb.velocity = Vector3.zero;


        if(Vector3.Angle(transform.forward,passVector) > 50.0f){

            playerRb.AddForce(transform.up * 1.5f, ForceMode.Impulse);
            transform.position = ballRb.transform.position - (passVector).normalized * movePlayerOnTouchDistance;


            Debug.Log("Moved player behind ball.");
        }


        //need to calculate the power based on the distance to intended player.
        //kickPower = UnityEngine.Random.Range(lowerPassPower, upperPassPower);
        //Debug.Log(passVector);
        //Debug.Log(kickPower);

        //var kickVector = passVector * kickPower * kickTime;

        //ballRb.AddForce(kickVector, ForceMode.Impulse);

        ballRb.velocity = passVector;

        isKicking=false;
        playerHasBall = false;
        kickTime = 0f;

        await Task.CompletedTask;
    }

    private Vector3 GetPassVector(){
        var current_dir = (gameCamera.transform.right * moveDirection.x) + (gameCamera.transform.forward * moveDirection.y);
        current_dir.y=0f;
        current_dir = current_dir.normalized;

        Debug.Log("Direction of stick: " + current_dir);

        Transform PlayerToPassTo = null;

        var currentTeamList = new List<EnemyAIController>();
        
        if(isHomeTeam){
            currentTeamList = soccerGameManager.friendlyPlayers;
        }else{
            currentTeamList = soccerGameManager.enemyPlayers;
        }

        for(int i=0; i<currentTeamList.Count; i++){
            var cur_player = currentTeamList[i];
            if(cur_player.transform == transform){
                continue;
            }

            var vectorBetweenPlayers = (cur_player.transform.position - transform.position).normalized;
            Debug.Log("Vector between players: " + vectorBetweenPlayers);
            Debug.Log("Angle Between Passing Players: " + Vector3.Angle(current_dir, vectorBetweenPlayers));
            
            if((Vector3.Angle(current_dir, vectorBetweenPlayers) < 25.0f || Vector3.Angle(current_dir, vectorBetweenPlayers) > 335.0f)){
                PlayerToPassTo = cur_player.transform;

                var passVector = (PlayerToPassTo.transform.position + (PlayerToPassTo.transform.forward) - transform.position);

                var passPower = PassPower(PlayerToPassTo);

                passVector = (float)(passPower) * passVector;

                Debug.Log($"Found player to pass to: {passVector}");

                return passVector;
            }
        }

        Debug.Log($"Direction of Pass: {current_dir}");

        return current_dir;
    }

    /// <summary>
    /// Calculates the pass power for the other player.
    /// </summary>
    /// <param name="otherPlayer"></param>
    private double PassPower(Transform otherPlayer){
        var speed = otherPlayer.GetComponent<Rigidbody>().velocity.magnitude;
        var a0 = -0.000009d;
        var a1 = 0.001367d;
        var a2 = -0.075419d;
        var a3 = 2.087854d;

        var distance = Vector3.Distance(otherPlayer.transform.position,transform.position);
        var d = Mathf.Round(distance);
        
        var x0 = (double)(d * d * d) * a0;
        var x1 = (double)(d * d) * a1;
        var x2 = (double)(d) * a2;
        
        var power = x0 + x1 + x2 + a3 + 0.2d;
        power = Math.Clamp(power,0.25d,1.4d);
        
        return power;
        
        //ballRb.velocity = (float)(power)*passVector;

        //soccerPlayerAudio.KickBallSound();

        //UpdateShotSlider(0.5f);

        
       
    }



    private async void CancelPowerKick(){
        isKickTimer = false;

        kickType = "Shoot";
        await Task.Delay(1500);
        isKicking = false;
        kickTime = 0f;
        await Task.CompletedTask;
    }
    

    private async void OnPowerKick(){
        
        
        

        playerAnimator.Play("Strike");
        
        await Task.Delay(shotDelayTime);

        //ballRb.transform.localPosition = ballRb.transform.localPosition + ballShootPosition;
        
        //if(!playerHasBall){
        //    kickTime = 0f;
        //    return;
        //}
        
        
        kickForce =Mathf.Clamp(kickPower * kickTime,60.0f,300.0f);

        ballRb.isKinematic = false;
        ballRb.transform.parent = CurrentField;

        //var force = ((AttackingGoal.transform.position - transform.position).normalized + Vector3.up*0.3f + (transform.right.normalized*UnityEngine.Random.Range(-shotAccuracy,shotAccuracy)));
        var dir = (gameCamera.transform.right * moveDirection.x) + (gameCamera.transform.forward * moveDirection.y);
        dir.y=0f;

        var stick_mag = (float)moveDirection.sqrMagnitude;
        stick_mag = Math.Clamp(stick_mag,0.5f,1.0f);

        

        Debug.Log("Kick Direction: " + dir);

        var force = ((dir + Vector3.up*0.3f) + (transform.right.normalized*UnityEngine.Random.Range(-shotAccuracy,shotAccuracy)) * stick_mag);

        var kickVector = force * kickForce * shotForce;

        //soccerBall.Kick(kickVector);
        ballRb.AddForce(kickVector, ForceMode.Impulse);

        isKicking = false;

        //dataCollector.LogDecision(2, moveDirection);

        playerHasBall = false;
        kickTime = 0f;

        await Task.CompletedTask;
    }

    private async void CancelLofted(){
        isKickTimer = false;

        kickType = "Lofted";
        await Task.Delay(1500);
        isKicking = false;
        kickTime = 0f;
        await Task.CompletedTask;
    }
        
    

    public async void OnLofted(){

        
        
        
        //if(!playerHasBall){
        //    kickTime = 0f;
        //    return;
        //}

        
        //ballRb.transform.localPosition = new Vector3(0f,0.25f,0.5f);

        


        Debug.Log("Kicked Ball");
        

        kickForce = Math.Clamp(kickPower * kickTime,200.0f,500.0f);

        if(kickForce>500.0f){
            playerAnimator.Play("Strike");
        }else{
            playerAnimator.Play("Kick");
            
        }

        
        LoftedPass();

        await Task.CompletedTask;

        
        
        
        
    }

    public async void LoftedPass(){

        

        //Debug.Log(kickForce + ": Kickforce");
        ballRb.isKinematic = false;
        ballRb.transform.parent = CurrentField;

        kickForce =Mathf.Clamp(kickPower * kickTime,60.0f,300.0f);

        ballRb.isKinematic = false;
        ballRb.transform.parent = CurrentField;

        var dir = (gameCamera.transform.right * moveDirection.x) + (gameCamera.transform.forward * moveDirection.y);
        dir.y=0f;

        var force = ((dir + Vector3.up*0.3f));

        var kickVector = force * kickForce * loftedKickForce;

        //soccerBall.Kick(kickVector);
        ballRb.AddForce(kickVector, ForceMode.Impulse);

        isKicking = false;

        //ballRb.AddTorque(new Vector3())

        //dataCollector.LogDecision(3, moveDirection);
        
        playerHasBall = false;
        kickTime = 0f;

        await Task.CompletedTask;
    }

    public void ClearBall(){
        if(!playerHasBall){
            return;
        }

        Debug.Log("Clearing Ball.");
        playerAnimator.Play("Kick");

        var force = (transform.up * 2.0f + transform.forward) + new Vector3(UnityEngine.Random.Range(-4.0f,4.0f),0f,UnityEngine.Random.Range(-4.0f,4.0f));

        ballRb.isKinematic = false;
        ballRb.AddForce(force * clearPower,ForceMode.Impulse);
        ballRb.transform.parent = CurrentField;
        playerHasBall = false;
        transform.Rotate(0f,180f,0f);
        
    }

    private async void SlideTackle(){
        if(playerHasBall || isBoosting){return;}

        Debug.Log("Trying Slide Tackle");

        playerAnimator.Play("Slide Tackle");

        playerRb.AddForce(transform.forward*500.0f,ForceMode.Impulse);
        isBoosting = true;

        await Task.Delay(4000);
        isBoosting = false;

        await Task.CompletedTask;
    }

    public async void Boost(){
        if(playerHasBall || isBoosting){return;}

        if(MagicSpell != null){
            MagicSpell.SetActive(false);
            MagicSpell.SetActive(true);
        }

        

        
        isBoosting = true;

        await Task.CompletedTask;
    }

    public async void HitPowerup(){

        //Player hit powerup
        //Speed Boost

        SpeedBoost.SetActive(true);

        playerSpeed += 3.0f;
        await Task.Delay(20000);
        playerSpeed -= 3.0f;

        SpeedBoost.SetActive(false);

        await Task.CompletedTask;
    }

    

    public async void RequestPass(){

        //Play wave hand animation

        playerAnimator.Play("Hand Raising");
        StallPlayer(1);
        

        //Debug.Log("Requesting Pass.");
        var currentTeamList = new List<EnemyAIController>();
        
        if(isHomeTeam){
            currentTeamList = soccerGameManager.friendlyPlayers;
        }else{
            currentTeamList = soccerGameManager.enemyPlayers;
        }

        for(int i =0;i<currentTeamList.Count;i++){

            var player = currentTeamList[i];

            if(player.closestToBall){

                player.PlayerToPassTo = gameObject;
                player.nextAction = EnemyAIController.NextAction.WillPass;
                player.nextDecisionMade = true;
                player.CancelInvoke("MakeDecision");

                //Set indicator active.


                //Reset player Make Decison
                await Task.Delay(5000);
                player.InvokeRepeating("MakeDecision",0.0f,0.1f);


                await Task.CompletedTask;

                return;
            }
        }
    }

    private async void StallPlayer(int seconds){
        walkSpeed = 0f;
        runSpeed = 0f;
        await Task.Delay(seconds*1000);
        walkSpeed = 5.0f;
        runSpeed = originalRunSpeed;
        await Task.CompletedTask;
    }



    public async void ChangePlayers(){

        Debug.Log("Change Players");

        if(soccerGameManager.friendlyNearestBall == null || soccerGameManager.friendlyNearestBall == transform){return;}

        EnablePlayer(soccerGameManager.friendlyNearestBall);


        await Task.CompletedTask;
    }

    public void EnablePlayer(Transform nextPlayer){
        nextPlayer.GetComponent<EnemyAIController>().enabled = false;
        nextPlayer.GetComponent<NavMeshAgent>().enabled = false;
        var rb = nextPlayer.GetComponent<Rigidbody>();
        rb.angularDrag = 0f;
        rb.drag = 0.05f;

        nextPlayer.GetComponent<MixamoPlayerController>().enabled = true;
        
        var ballEmpT = nextPlayer.GetChild(9);
        ballEmpT.gameObject.SetActive(true);
        ballEmpT.GetComponent<BoxCollider>().enabled = true;
        ballEmpT.GetComponent<BallEmpty>().enabled = true;

        nextPlayer.GetChild(12).gameObject.SetActive(true);


        gameCamera.SetPlayer(nextPlayer.gameObject);
        //soccerGameManager.UpdatePlayerUI(nextPlayer.GetComponent<EnemyAIController>());

        DisablePlayer();

        return; 

    }

    
    public void DisablePlayer(){
    
        playerRb.angularDrag = 200f;
        playerRb.drag = 200f;

        aiController.enabled = true;
        navAgent.enabled = true;

        var prevPlayerBallEmpty = transform.GetChild(9);
        
        prevPlayerBallEmpty.GetComponent<BoxCollider>().enabled = false;
        prevPlayerBallEmpty.GetComponent<BallEmpty>().enabled = false;
        prevPlayerBallEmpty.gameObject.SetActive(false);

        transform.GetChild(12).gameObject.SetActive(false);

        this.enabled = false;

        return;
    }


    public void StartSoccerGame(){
        //jump.Disable();
        //kick.Enable();
        isPlayingGame = true;
        var staminaTime = stamina/10.0f;
        staminaCoroutine = StartCoroutine(DeductStamina(staminaTime));
    }

    public void EndSoccerGame(){
        if (staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
            staminaCoroutine = null;
        }

        playerSpeed = 8.0f;

        //jump.Enable();
        //kick.Disable();
        //enabled = true;
        //GetComponent<EnemyAIController>().enabled = false;
        //GetComponent<NavMeshAgent>().enabled = false;
        isPlayingGame = false;

        
    }

    private async void Emote(){
        playerAnimator.Play("Salsa Dancing");
        await Task.CompletedTask;
    }
    
    
    
    /*
    async void SpawnMagic(){
        MagicSpell.SetActive(true);

        var spellRb = MagicSpell.transform.GetComponent<Rigidbody>();
        
        spellRb.AddForce(transform.forward * 50.0f,ForceMode.Impulse);

        await Task.Delay(5000);

        MagicSpell.transform.GetComponent<ParticleSystem>().Play();

        MagicSpell.SetActive(false);

        MagicSpell.transform.localPosition = new Vector3(0f,1.27f,0.8f);

        spellRb.velocity = Vector3.zero;
        spellRb.angularVelocity = Vector3.zero;
        
        await Task.CompletedTask;
    }
    */

    private IEnumerator DeductStamina(float waitTime)
    {
        while (true)
        {

            staminaRemaining -= UnityEngine.Random.Range(1,3);

            ShotPowerSlider.value = staminaRemaining/100.0f;

            yield return new WaitForSeconds(waitTime);
        }
    }

    public async void SlideTackleAnimations(){
        Debug.Log("User player tackled.");
        
        playerAnimator.Play("AI_Tackled1");
        runSpeed = 0f;
        walkSpeed = 0f;

        await Task.Delay(3000);

        runSpeed = originalRunSpeed;
        walkSpeed = originalRunSpeed - 2.0f;
    
        
        await Task.CompletedTask;
    }


    private async void EnterFightMode(){
        GetComponent<FightPlayerController>().enabled = true;
        GetComponent<MixamoPlayerController>().enabled = false;
        await Task.CompletedTask;
    }

    

    

    

}
