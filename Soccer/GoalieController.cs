using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GoalieController : MonoBehaviour
{
    [SerializeField] public Transform attackingGoal, defendingGoal;
    [SerializeField] private SoccerGameManager soccerGameManager;
    private Rigidbody goalieRb;
    private Transform ball;
    private Rigidbody ballRb;
    public Animator animator;
    [SerializeField] private Transform GoalieEmpty;
    [SerializeField] private GameObject HasBallColliders;
    [SerializeField] public GameObject GoalKickColliders;

    private NavMeshAgent agent;



    //Variables
    public string playerName;
    public bool isHomeTeam;
    public float speed = 10f;
    public float jumpForce = 8f;
    public float clearPower = 10f;
    public bool isBallPickedUp = false;
    public bool isGrounded = true;
    private bool isPassing;
    private float groundPos = 8.5f;
    public float goalieDistance;
    public float velocityPredict = 1.0f;
    public float detectJumpDistance = 6.5f;
    public float detectJumpHeight = 3.5f;

    public float goalWidth = 15.0f;

    void Start()
    {   
        animator = GetComponent<Animator>();
        goalieRb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        
        GameObject ballObject = GameObject.FindGameObjectWithTag("Ball");

        if (ballObject != null)
        {
            ball = ballObject.transform;
            ballRb = ballObject.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("Ball not found. Make sure to tag your ball GameObject with 'Ball'.");
        }

    }

    void Update()
    {
        
        if(isBallPickedUp){
            return;
        }

        if(!isGrounded){return;}

        if(soccerGameManager.isGoalKick){return;}
        
        
        

        //IF BALL CLOSE AND ABOVE PLAYER, JUMP
        //if(Vector3.Distance(transform.position,ball.position)<3.0f && ball.position.y > 11.3f){
        //    Jump();
        //}

        //(Math.Abs(defendingGoal.position.x-transform.position.x) <= 4.0f)
        //&& (ball.position.y > transform.position.y + detectJumpHeight)

        if(Vector3.Distance(transform.position,ball.position) < detectJumpDistance && (ball.position.y > transform.position.y + detectJumpHeight))
        {
            if(isGrounded ){
                Debug.Log("Jump");
                Jump();
            }
        }

        if(isGrounded && Vector3.Distance(defendingGoal.position,ball.position) <= 20.0f){
            GoToBall();
            return;
        }

        
        //STAY BETWEEN BALL AND GOAL IF BALL IS TOO FAR AWAY.
        if(isGrounded && !isBallPickedUp && Vector3.Distance(transform.position,ball.position)>15.0f){

            StayBetweenBallAndGoal();
            

        }
        
        //BLOCK BALL PATH
        else if(isGrounded && !isBallPickedUp && Vector3.Distance(transform.position,ball.position)<15.0f
        ){

            animator.SetTrigger("Sidestep");
        
            BlockPath();

            


            


        }


        
        
        

        
        
    }

    private async void StayBetweenBallAndGoal(){
        animator.SetTrigger("Idle");

        //transform.LookAt(new Vector3(ball.position.x,groundPos+1.0f,ball.position.z));
    
        var newPosition = defendingGoal.position - (defendingGoal.position - ball.position).normalized * 8.0f;
        newPosition = new Vector3(newPosition.x,groundPos,newPosition.z);
        
        agent.SetDestination(newPosition);

        await Task.CompletedTask;
    }

    private async void BlockPath()
    {
        
        var direction = (defendingGoal.position - ball.position).normalized;
        var newPosition = defendingGoal.position - direction*goalieDistance;
        
        

        //Debug.Log((defendingGoal.position - newPosition).x + " Home team: " + isHomeTeam);
        //var distanceZ = transform.position.z - ball.position.z;
        //Debug.Log("Distance z: " + distanceZ);

        if(ball.position.z - defendingGoal.position.z > -goalWidth ||
        ball.position.z - defendingGoal.position.z < goalWidth){
            if((defendingGoal.position - newPosition).x < 0.2f){
                newPosition.x = defendingGoal.position.x - direction.x*1.0f;
            }
            newPosition.z = ball.position.z + ballRb.velocity.z*(velocityPredict);
        }

        if(isGrounded){newPosition.y = groundPos;}
        
        

        agent.SetDestination(newPosition);

        await Task.CompletedTask;
        
        
    }
    private async void GoToBall(){
        
        
        agent.SetDestination(new Vector3(ball.position.x,groundPos,ball.position.z));
        //transform.position = Vector3.Slerp(transform.position, newPosition,Time.deltaTime*speed);
        //transform.LookAt(new Vector3(ball.position.x,groundPos+1.0f,ball.position.z));

        await Task.CompletedTask;
    }
    private async void Jump(){

        BlockPath();
        
        animator.Play("Jump");
        isGrounded = false;
        //var jumpPos = new Vector3(transform.position.x,transform.position.y + 2.0f,transform.position.z);
        //transform.position = Vector3.Slerp(transform.position, jumpPos,0.5f);

        //await Task.Delay(500);

        //var newPosition = new Vector3(transform.position.x,groundPos,transform.position.z);
        //transform.position= Vector3.Slerp(transform.position,newPosition,0.5f);

        GetComponent<Rigidbody>().AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
        await Task.CompletedTask;
        
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Ball") && !isBallPickedUp)
        {
            PickUpBall();
        }
        
    }

    

    async void PickUpBall()
    {

        Debug.Log("Goalie picked up ball.");

        animator.SetTrigger("Catch");

        soccerGameManager.PausePlayers(false);

        //Set The Mixamo Player Controller inactive
        //Make the player controls inactive
        //Direct players outside box
        //Set goal kick colliders
        //Reset when goalie throws ball
        //soccerGameManager.GoalKick();
        soccerGameManager.goalieHasBall = true;
        isBallPickedUp = true;


        agent.SetDestination(defendingGoal.position + defendingGoal.right*-10.0f);


        
        ballRb.velocity= Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        //ballRb.MovePosition(GoalieEmpty.transform.position);

        agent.speed = 0f;
        goalieRb.velocity = Vector3.zero;
        goalieRb.angularVelocity = Vector3.zero;

        //GetComponent<BoxCollider>().enabled = false;
        //HasBallColliders.SetActive(true);
        
        soccerGameManager.CurrentGoalie = this.gameObject;

        transform.position = new Vector3(transform.position.x,groundPos,transform.position.z);
        
        
        

        
        
        //ball.GetComponent<SphereCollider>().isTrigger = true;
        ball.parent = GoalieEmpty;
        ball.localPosition = new Vector3(0f,0f,0f);
        ball.rotation = new Quaternion(0f,0f,0f,0f);
        
        goalieRb.velocity = Vector3.zero;
        goalieRb.angularVelocity = Vector3.zero;
        
        
        ballRb.useGravity = false;
        transform.LookAt(attackingGoal);

        //animator.SetTrigger("Idle");
        animator.SetTrigger("Throw");

        agent.speed = speed;

        await Task.CompletedTask;
        
    }

    public async void PassBall(){

      

        
        Debug.Log("Throwing ball.");

        //HasBallColliders.SetActive(false);
        //GetComponent<BoxCollider>().enabled = true;

        isPassing = true;
        transform.LookAt(attackingGoal);
        
        ballRb.useGravity = true;
        //ballRb.isKinematic = false;
        //ball.GetComponent<SphereCollider>().isTrigger = false;
        ball.rotation = new Quaternion(0f,0f,0f,0f);

        ballRb.velocity= Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        var force = (transform.forward + (0.5f*Vector3.up)) * clearPower;
        ballRb.AddForce(force, ForceMode.Impulse);

        
        ball.parent = GameObject.FindGameObjectWithTag("Field").transform;
        
        

        await Task.Delay(1000);

        GoalKickColliders.SetActive(false);
        soccerGameManager.PausePlayers(true);

        

        isBallPickedUp = false;

        soccerGameManager.goalieHasBall = false;
        //soccerGameManager.CurrentGoalie = null;
        await Task.CompletedTask;
        

    }


    
}


