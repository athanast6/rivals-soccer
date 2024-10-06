using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class OnlinePlayerController : NetworkBehaviour
{


    //References
    private Animator playerAnimator;
    private Rigidbody playerRb;
    [SerializeField] private BoxCollider boxC;
    private GameObject camera;
    private CinemachineFreeLook lookCamera;


    [SerializeField] private Rigidbody ballRb;


    [SerializeField] private OnlineGameManager onlineGameManager;
    [SerializeField] private Transform CurrentField;
    [SerializeField] private Transform PlayerBallEmpty;
    [SerializeField] private GameObject SpeedBoost;
    [SerializeField] private GameObject MagicSpell;
    [SerializeField] public SkinnedMeshRenderer shirt;








    //Variables
    private float forwardInput;
    private float sideInput;
    public bool playerOnGround;
    public float walkSpeed = 8.0f;
    public float runSpeed = 14.0f;
    public float playerRotation = 60.0f;
    public float sideMoveSpeed = 2.0f;
    public Vector3 jumpForce;
    private float playerSpeed;
    //private Vector2 moveDirection  = Vector2.zero;

    private bool combatCamera;
    private bool runningButtonHeld;
    public bool playerHasBall;
    public float kickPower;
    public float touchPower;
    public bool isKicking;
    public float kickTime;
    public float clearPower;
    public bool isHomeTeam = true;

    private bool isBoosting = false;
    
    public Vector3 spawnPosition = new Vector3(357.8f,9.2f,499.3f);


    //INPUTS
    public InputActionMap playerControls;
    
    

    private InputAction movement, run, jump, kick, clearBall, switchCamera, slideTackle, punch, block, boost;

    
    

    

    void OnEnable(){

        onlineGameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<OnlineGameManager>();
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
        lookCamera = transform.GetChild(0).gameObject.GetComponent<CinemachineFreeLook>();
        CurrentField = GameObject.FindGameObjectWithTag("Field").transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    
        InitializeInputs();
        
        SpawnBall();

        InitializePlayer();
        
    }
    private void OnDisable(){
        playerControls.Disable();
    }





    

    void Update()
    {
        
        if(!IsOwner){return;}
        

        var moveDirection = movement.ReadValue<Vector2>();

        if(isKicking){
            kickTime += Time.deltaTime;
        }
        
        

        var isRunning = 0f;
        if(!runningButtonHeld){
            isRunning = run.ReadValue<float>();
        }
        
        
       


        
        if(moveDirection.x == 0 && moveDirection.y == 0)
        {
            playerAnimator.SetTrigger("Idle");
            playerAnimator.ResetTrigger("Walk");
            playerAnimator.ResetTrigger("Run");
        }else
        {
            if(runningButtonHeld || isRunning >0f){

                playerAnimator.SetTrigger("Run");
                playerAnimator.ResetTrigger("Idle");
                playerAnimator.ResetTrigger("Walk");

                playerSpeed = runSpeed;

               
            }else{
                playerSpeed = walkSpeed;
                playerAnimator.SetTrigger("Walk");
                playerAnimator.ResetTrigger("Idle");
                playerAnimator.ResetTrigger("Run");
            }

            playerAnimator.SetFloat("LeftRight",moveDirection.x);
            playerAnimator.SetFloat("ForwardBackward",moveDirection.y);
            
        }

       


        
        
    }

    private void FixedUpdate(){

        if(!IsOwner){return;}

        var moveDirection = movement.ReadValue<Vector2>();

        transform.Translate(Vector3.forward * playerSpeed * moveDirection.y * Time.deltaTime);
        
        //var viewDir = camera.transform.position - new Vector3(transform.position.x,transform.position.y,transform.position.z);
        //orientation.forward = viewDir.normalized;
        
        if(combatCamera){
            transform.localEulerAngles = new Vector3(0f,lookCamera.m_XAxis.Value,0f);
            transform.Translate(Vector3.right * sideMoveSpeed * moveDirection.x * Time.deltaTime);
        }
        
        else{
            if(moveDirection.x == 0f){return;}
            transform.Rotate(Vector3.up * playerRotation * moveDirection.x * Time.deltaTime);
        }

        
        
    }






    private async void InitializePlayer(){

        await Task.Delay(50);

        transform.position = spawnPosition;

        //Disable camera and controls if not local player
        if(!IsOwner){
            lookCamera.gameObject.SetActive(false);
            playerControls.Disable();
        }

        
        ballRb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>();

        SetBallEmptyServerRpc();
        
    }




    private void SpawnBall(){
        if(!GameObject.FindGameObjectWithTag("Ball")){
            onlineGameManager.SpawnBall();
        }

        var ballEmpty = Instantiate(PlayerBallEmpty);
        PlayerBallEmpty = ballEmpty;
    }

    [ServerRpc]
    void SetBallEmptyServerRpc(){
        PlayerBallEmpty.transform.SetParent(transform);
    }
    [ClientRpc]
    void SetBallEmptyClientRpc(){
        PlayerBallEmpty.transform.SetParent(transform);
    }





    private void InitializeInputs(){

        playerControls.Enable();
        
        movement = playerControls.FindAction("Movement");
        run = playerControls.FindAction("Run");
        jump = playerControls.FindAction("Jump");
        jump.Disable();
        boost = playerControls.FindAction("Boost");

        kick = playerControls.FindAction("Kick");
        clearBall = playerControls.FindAction("Clear Ball");
        slideTackle = playerControls.FindAction("Slide Tackle");

        switchCamera = playerControls.FindAction("Switch Camera");
        

        switchCamera.started += 
            context => {SwitchCamera();};

        run.performed += 
            context => {runningButtonHeld = true;};

        movement.canceled +=
            context => {runningButtonHeld = false;};

    
        kick.started +=
            context => {StartKick();};

        kick.canceled +=
            context => {CancelKick();};

        kick.performed +=
            context => {OnKick();};

        clearBall.started +=
            context => {ClearBall();};


        slideTackle.performed +=
            context => {SlideTackle();};

        boost.performed +=
            context => {Boost();};


        

        
    }




    public void OnCollisionEnter(Collision other){

        if(!IsOwner){return;}

        if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide Tackle") && other.gameObject.CompareTag("Away Player")){

            other.gameObject.transform.GetComponent<EnemyAIController>().SlideTackleAnimations();
        }


        
        if(other.gameObject.CompareTag("Ground")){
            playerOnGround = true;
        }
        if(other.gameObject.CompareTag("Ball")){
            if(playerHasBall){return;}

            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);

            playerHasBall = true;

            //if(isHomeTeam)
            //onlineGameManager.homeTeamAttacking = true;
            
            ballRb.Sleep();

            GetComponent<OnlinePickUpBall>().ChangeParentServerRpc(true);
        }
    }

    public void OnCollisionExit(Collision other){
        if(!IsOwner){return;}
        if(other.gameObject.CompareTag("Ball")){
            //other.transform.parent = null;
            ballRb.WakeUp();
            playerHasBall = false;

            GetComponent<OnlinePickUpBall>().ChangeParentServerRpc(false);
            
        }
    }


   
    public void OnJump(){
        if(!playerOnGround){return;}

        playerAnimator.Play("Header");
        playerRb.AddForce(jumpForce, ForceMode.Impulse);
        playerOnGround = false;
        
    }

    private void StartKick(){
        //Update timer.
        isKicking = true;
        
    }

    private void CancelKick(){
        if(playerHasBall){
            OnKick();
            return;
        }else{
            Debug.Log("Requesting pass.");
            RequestPass();
            
        }
        isKicking = false;
        kickTime = 0f;
    }

    public void OnKick(){
        isKicking = false;
       
        if(!playerHasBall){
            kickTime = 0f;

            
           
            
            return;
        }
        


        Debug.Log("Kicked Ball");
        playerAnimator.Play("Kick");

        var kickForce = Math.Clamp(kickPower * kickTime,60.0f,300.0f);
        ballRb.AddForce((transform.forward + (0.25f * transform.up)) * kickForce,ForceMode.Impulse);
        //ballRb.transform.parent = CurrentField;
        playerHasBall = false;
        kickTime = 0f;
        
        GetComponent<OnlinePickUpBall>().ChangeParentServerRpc(false);
        
    }

    public void ClearBall(){
        if(!playerHasBall){
            return;
        }

        Debug.Log("Clearing Ball.");
        playerAnimator.Play("Kick");

        var force = (transform.up * 2.0f + transform.forward) + new Vector3(UnityEngine.Random.Range(-4.0f,4.0f),0f,UnityEngine.Random.Range(-4.0f,4.0f));
        ballRb.AddForce(force * clearPower,ForceMode.Impulse);
        //ballRb.transform.parent = CurrentField;
        transform.Rotate(0f,180f,0f);

        GetComponent<OnlinePickUpBall>().ChangeParentServerRpc(false);
        
    }


    private async void SlideTackle(){
        if(playerHasBall || isBoosting){return;}

        Debug.Log("Trying Slide Tackle");

        playerAnimator.Play("Slide Tackle");

        playerRb.AddForce(transform.forward*400.0f,ForceMode.Impulse);
        isBoosting = true;

        await Task.Delay(4000);
        isBoosting = false;

        await Task.CompletedTask;
    }

    private async void Boost(){
        if(playerHasBall || isBoosting){return;}

        MagicSpell.SetActive(false);
        MagicSpell.SetActive(true);

        playerRb.AddForce(transform.forward*300.0f,ForceMode.Impulse);
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
        runSpeed -= 4.0f;

        SpeedBoost.SetActive(false);

        await Task.CompletedTask;
    }

    

    public void RequestPass(){
        for(int i =0;i<onlineGameManager.friendlyPlayers.Count;i++){
            var player = onlineGameManager.friendlyPlayers[i];
            if(player.closestToBall){
                player.PlayerToPassTo = gameObject;
                player.nextAction = EnemyAIController.NextAction.WillPass;
                player.CancelInvoke("MakeDecision");
                return;
            }
        }
    }

    private void SwitchCamera(){
        combatCamera = !combatCamera;
    }

    private void StartSoccerGame(){
        jump.Disable();
        kick.Enable();
    }

    private void EndSoccerGame(){
        jump.Enable();
        kick.Disable();
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

    
    

    

    

}
