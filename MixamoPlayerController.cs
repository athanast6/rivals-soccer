using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor.Animations;

public class MixamoPlayerController : MonoBehaviour
{


    //References
    public Animator playerAnimator;
    [SerializeField] private AnimatorController normalAnim;

    private Rigidbody playerRb;
    [SerializeField] private BoxCollider boxC;
    private GameObject camera;
    public GameCamera gameCamera;
    private CinemachineFreeLook lookCamera;
    public Gamepad gamepad;

    //private UserPlayerAudio userAudio;


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
    public float runSpeed = 14.0f;
    public float playerRotation = 60.0f;
    public float sideMoveSpeed = 2.0f;
    public Vector3 jumpForce;
    public float playerSpeed;
    private Vector2 moveDirection  = Vector2.zero;

    public bool isPlayingGame;
    public bool playerHasBall;
    public float kickPower;
    public float touchPower;
    public bool isKicking;
    public float kickTime;
    public float clearPower;
    public bool isHomeTeam = true;

    private bool isBoosting = false;

    public float goalieMoveForce = 1000.0f;
    
    



    //INPUTS
    public InputActionMap playerControls;
    
    

    private InputAction fpsMovement, thirdMovement, run, jump, kick, powerKick, switchCamera, slideTackle, punch, block, boost, requestPass, changePlayer, loftedKick;

    
    

    


    void OnEnable(){
        
       
        
        
    }

    void OnDisable(){

        //playerControls.Disable();

        //runSpeed = originalRunSpeed;

    }
    
    void Awake(){
        playerAnimator = GetComponent<Animator>();

        soccerGameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<SoccerGameManager>();

        playerRb = GetComponent<Rigidbody>();

        aiController =  GetComponent<EnemyAIController>();
        navAgent = GetComponent<NavMeshAgent>();

        playerName = aiController.playerAttributes.playerName;

        playerAnimator.runtimeAnimatorController = normalAnim;

        playerControls.Enable();

       

    }
    
    void Start()
    {

       

        InitializeInputs();
        originalRunSpeed = runSpeed;
        
        ballRb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>();
        

        CurrentField = GameObject.FindGameObjectWithTag("Field").transform;


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        lookCamera = transform.GetChild(0).gameObject.transform.GetComponent<CinemachineFreeLook>();

        
        
        
        
        
        gamepad = Gamepad.current;
        
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
            context => {OnPass();};

        powerKick.started +=
            context => {StartKick();};

        powerKick.canceled +=
            context => {CancelPowerKick();};

        powerKick.performed +=
            context => {OnPowerKick();};

        loftedKick.started +=
            context => {StartKick();};

        loftedKick.canceled +=
            context => {CancelLofted();};

        loftedKick.performed +=
            context => {OnLofted();};


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
       
        


        if(isKicking){
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

            //Debug.Log(moveDirection);
           
            if (gamepad != null && playerControls.enabled)
            {
                moveDirection = gamepad.leftStick.ReadValue();
            }
            
            var dir = (gameCamera.transform.right * moveDirection.x) + (-gameCamera.transform.forward * moveDirection.y);
            
            
        
            //var viewDir = camera.transform.position - new Vector3(transform.position.x,transform.position.y,transform.position.z);
            //orientation.fFmoveorward = viewDir.normalized;
            
            //transform.LookAt(dir);

            if (moveDirection.magnitude >= 0.1f)
            {
                 
                // Calculate rotation angle in radians
                float targetAngle = ((Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg) + 90.0f);

                // Smoothly rotate towards the target angle (based on movement direction)
                Quaternion newRot = Quaternion.Euler(0f, targetAngle, 0f);

                // Smooth rotation when moving normally
                transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 0.15f);
                

                transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime);




                
            }

            

            return;

        }else{

            moveDirection = fpsMovement.ReadValue<Vector2>();

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

        if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide Tackle") && other.gameObject.CompareTag("Away Player")){

            other.gameObject.transform.GetComponent<EnemyAIController>().SlideTackleAnimations();
        }


        
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Parkour")){
            playerOnGround = true;
        }


        if(other.gameObject.CompareTag("Play Game")){
            soccerGameManager.OfferGame(other.gameObject);

        }
        
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
        
    }

    private void CancelPass(){
        if(playerHasBall){
            OnPass();
            return;
        }
        isKicking = false;
        kickTime = 0f;
    }

    public async void OnPass(){
        
        isKicking = false;
       
        if(!playerHasBall){
            kickTime = 0f;
            return;
        }

        ballRb.transform.localPosition = new Vector3(0f,0.23f,0.5f);
        


        //Debug.Log("Kicked Ball");
        

        kickForce = Math.Clamp(kickPower * kickTime,60.0f,300.0f);

        if(kickForce>200.0f){
            playerAnimator.Play("Strike");
        }else{
            playerAnimator.Play("Kick");
            
        }
        PassTheBall();

        await Task.CompletedTask;

        
        
        
        
    }

    public async void PassTheBall(){

        
        
        Debug.Log(kickForce + ": Kickforce");
        ballRb.isKinematic = false;
        ballRb.transform.parent = CurrentField;
        
        ballRb.AddForce((transform.forward) * kickForce * 1.25f,ForceMode.Impulse);

        //ballRb.AddTorque(new Vector3())

        

        
        playerHasBall = false;
        kickTime = 0f;

        await Task.CompletedTask;
    }

    private void CancelPowerKick(){
        if(playerHasBall){
            OnPowerKick();
            return;
        }
        isKicking = false;
        kickTime = 0f;
    }

    private async void OnPowerKick(){
        

        isKicking = false;

        playerAnimator.Play("Strike");
        
        await Task.Delay(shotDelayTime);

        ballRb.transform.localPosition = ballShootPosition;
        
        if(!playerHasBall){
            kickTime = 0f;
            return;
        }
        
        
        kickForce =Mathf.Clamp(kickPower * kickTime,60.0f,300.0f);

        ballRb.isKinematic = false;
        ballRb.transform.parent = CurrentField;

        var force = ((AttackingGoal.transform.position - transform.position).normalized + Vector3.up*0.3f + (transform.right*UnityEngine.Random.Range(-shotAccuracy,shotAccuracy)));
        ballRb.AddForce(force * kickForce * shotForce, ForceMode.Impulse);

        playerHasBall = false;
        kickTime = 0f;

        await Task.CompletedTask;
    }

    private void CancelLofted(){
        if(playerHasBall){
            OnLofted();
            return;
        }
        isKicking = false;
        kickTime = 0f;
    }

    public async void OnLofted(){
        
        isKicking = false;
       
        if(!playerHasBall){
            kickTime = 0f;
            return;
        }

        
        ballRb.transform.localPosition = new Vector3(0f,0.25f,0.5f);


        Debug.Log("Kicked Ball");
        

        kickForce = Math.Clamp(kickPower * kickTime,60.0f,300.0f);

        if(kickForce>200.0f){
            playerAnimator.Play("Strike");
        }else{
            playerAnimator.Play("Kick");
            
        }
        LoftedPass();

        await Task.CompletedTask;

        
        
        
        
    }

    public async void LoftedPass(){

        Debug.Log(kickForce + ": Kickforce");
        ballRb.isKinematic = false;
        ballRb.transform.parent = CurrentField;
        
        ballRb.AddForce((transform.forward + 0.3f*transform.up) * kickForce * 2.0f,ForceMode.Impulse);

        

        //ballRb.AddTorque(new Vector3())
        
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

    private async void Boost(){
        if(playerHasBall || isBoosting){return;}

        MagicSpell.SetActive(false);
        MagicSpell.SetActive(true);

        playerRb.AddForce(transform.forward*1000.0f,ForceMode.Impulse);
        isBoosting = true;

        await Task.Delay(2000);
        isBoosting = false;

        await Task.CompletedTask;
    }

    public async void HitPowerup(){

        //Player hit powerup
        //Speed Boost

        SpeedBoost.SetActive(true);

        runSpeed += 4.0f;
        await Task.Delay(30000);
        runSpeed = originalRunSpeed;

        SpeedBoost.SetActive(false);

        await Task.CompletedTask;
    }

    

    public async void RequestPass(){

        //Play wave hand animation

        playerAnimator.Play("Hand Raising");
        StallPlayer(1);
        

        //Debug.Log("Requesting Pass.");

        for(int i =0;i<soccerGameManager.friendlyPlayers.Count;i++){

            var player = soccerGameManager.friendlyPlayers[i];

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
        soccerGameManager.UpdatePlayerUI(nextPlayer.GetComponent<EnemyAIController>());

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


    private void StartSoccerGame(){
        jump.Disable();
        kick.Enable();
    }

    private void EndSoccerGame(){
        jump.Enable();
        kick.Disable();
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



    private async void EnterFightMode(){
        GetComponent<FightPlayerController>().enabled = true;
        GetComponent<MixamoPlayerController>().enabled = false;
        await Task.CompletedTask;
    }

    

    

    

}
