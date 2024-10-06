using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GoaliePlayerController : MonoBehaviour
{
    [SerializeField] private SoccerGameManager soccerGameManager;


    private Animator animator;
    private Rigidbody rb;
    public InputAction goalieMovement, jump, pass, diveLeft, diveRight;



    [SerializeField] private CinemachineFreeLook lookCamera;
    private Rigidbody ballRb;
    [SerializeField] private Transform PlayerBallEmpty;
    [SerializeField] private Transform CurrentField;


    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public bool isGrounded = true; // To check if the goalie is on the ground
    private Vector2 moveDirection  = Vector2.zero;
    public float playerSpeed = 5.0f;
    public float sideMoveSpeed = 5.0f;

    private float blendValue = 0.0f;
    private float blendSpeed = 5.0f;
    public float kickForce = 75.0f;
    public float ballOffset = -1.4f;

    

    private bool playerHasBall;
    private bool isDiving;
    public float diveBoost;
    public float smoothTime = 0.3f; // The time it takes to reach the target
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Get the Animator and Rigidbody components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        goalieMovement.Enable();
        jump.Enable();
        pass.Enable();
        diveLeft.Enable();
        diveRight.Enable();

        jump.performed += context => {Jump();};
        pass.performed += context => {PassBall();};
        diveLeft.performed += context => {Dive("Dive Left");};
        diveRight.performed += context => {Dive("Dive Right");};
    }

    private void FixedUpdate(){
        
        
        
        if(isDiving){return;}
        moveDirection = goalieMovement.ReadValue<Vector2>();


        transform.Translate(Vector3.forward * playerSpeed * moveDirection.y * Time.deltaTime);
        
    
        //var viewDir = camera.transform.position - new Vector3(transform.position.x,transform.position.y,transform.position.z);
        //orientation.forward = viewDir.normalized;
        
        transform.localEulerAngles = new Vector3(0f,lookCamera.m_XAxis.Value,0f);
        transform.Translate(Vector3.right * sideMoveSpeed * moveDirection.x * Time.deltaTime);
        var move = Math.Max(0,(Math.Abs(moveDirection.y * 2.0f) - Math.Abs(moveDirection.x))/2.0f);


        if (moveDirection.magnitude > 0.1f)  // If there is movement input
        {
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Move");
            
            // Use Lerp to smoothly blend the value (instead of setting it directly)
            blendValue = Mathf.Lerp(blendValue, CalculateBlendValue(moveDirection), Time.deltaTime * blendSpeed);

            animator.SetFloat("Blend", blendValue);  // Update the blend parameter in the Animator
        }


        //No Movement Input
        else
        {
            animator.ResetTrigger("Move");
            animator.SetTrigger("Idle");
            // Smoothly return to idle
            blendValue = Mathf.Lerp(blendValue, 0.0f, Time.deltaTime * blendSpeed);
            animator.SetFloat("Blend", blendValue);
        }

        //animator.SetFloat("Blend", Math.Max(0,(Math.Abs(moveDirection.y * 2.0f) - Math.Abs(moveDirection.x))/2.0f));     
        
        
    }

    float CalculateBlendValue(Vector2 moveDirection)
    {
        // Optionally, normalize the movement input for smoother blending
        moveDirection.Normalize();

        // Calculate blend based on y and x movement, but avoid sharp changes with Lerp
        return (Mathf.Abs(moveDirection.y * 2.0f) - Mathf.Abs(moveDirection.x)) / 2.0f;
    }


    public void Jump(){
        animator.SetTrigger("Jump");
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    public void PassBall(){

        if(!playerHasBall){return;}

        animator.Play("Goalie Throw");
    }

    public void Pass(){

        Debug.Log(kickForce + ": Kickforce");
        ballRb.isKinematic = false;
        ballRb.transform.parent = CurrentField;
        
        ballRb.AddForce((transform.forward + 0.25f*transform.up) * kickForce * 1.25f,ForceMode.Impulse);

        //ballRb.AddTorque(new Vector3())

        playerHasBall = false;

        return;
    }

    public async void Dive(string side){

        

        if(!isDiving){
            isDiving = true;
            
            animator.ResetTrigger("Idle");
            animator.SetTrigger(side);

            if(side == "Dive Left"){
                
                rb.AddForce(transform.right * -diveBoost, ForceMode.Impulse);
            }else{
                
                rb.AddForce(transform.right * diveBoost, ForceMode.Impulse);
            }
            
        }

        await Task.Delay(1500);

        isDiving = false;

        await Task.CompletedTask;
    }


    public void OnCollisionEnter(Collision other){
        if(other.transform.CompareTag("Ball")){
            
            if(playerHasBall){return;}

            Debug.Log("Goalie touched ball");

            ballRb = other.transform.GetComponent<Rigidbody>();

            other.transform.parent = PlayerBallEmpty;
            other.transform.localPosition = new Vector3(0f,ballOffset,0f);
            other.transform.localEulerAngles = Vector3.zero;

            playerHasBall = true;

            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);

            
            //if(isHomeTeam)
            soccerGameManager.homeTeamAttacking = true;

            soccerGameManager.lastToTouchBall = name;

           

        }
    }




}
