using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
public class EnemyAIController : MonoBehaviour
{
    //References
    [SerializeField] private SoccerGameManager soccerGameManager;
    private SoccerPlayerAudio soccerPlayerAudio;
    private Rigidbody aiRb;
    private NavMeshAgent agent;
    public Transform ball;
    private Rigidbody ballRb;

    [SerializeField] private Slider ShotPowerSlider;
    [SerializeField] private Transform CurrentField;
    [SerializeField] public Transform attackingGoal;
    [SerializeField] public Transform defendingGoal;
    public Animator animator;
    [SerializeField] private List<GameObject> teamMates;
    public List<GameObject> otherTeam;

    public LayerMask layerToIgnore;


    
    

    [SerializeField] public PlayerAttributes playerAttributes = new PlayerAttributes();


    public enum NextAction{
        WillPass,
        WillShoot,
        WillTouch,
        WillClear,
        MakeTackle,
        GoToCorner
    }



    //Variables
    public float groundHeight = 8.55f;
    public bool isHomeTeam;
    public bool closestToBall;
    public bool touchingBall;
    public float speed;
    
    public NextAction nextAction;
    public bool nextDecisionMade;
  
    public GameObject PlayerToPassTo;

    public float fieldOfView;

    public float staminaRemaining = 99.0f;

    
    void Awake(){
        animator = GetComponent<Animator>();
        ball = GameObject.FindGameObjectWithTag("Ball").transform;
        ballRb = ball.GetComponent<Rigidbody>();
        
    }
    

    void Start()
    {
        
        soccerPlayerAudio = GetComponent<SoccerPlayerAudio>();


        aiRb = GetComponent<Rigidbody>();
        
        agent = GetComponent<NavMeshAgent>();
        //InvokeRepeating("UpdateDestination",0.0f,0.05f);
        

        speed = playerAttributes.runSpeed;

        if(isHomeTeam){  layerToIgnore = 10;}
        else if(!isHomeTeam) {layerToIgnore = 9;}


        
    }

    void OnEnable(){
        GetTeammates();

        staminaRemaining = 99.0f;
        
        animator.SetFloat("LeftRight",0f);
        animator.SetFloat("ForwardBackward",1f);
        InvokeRepeating("MakeDecision",0.0f,0.1f);
        
        var staminaTime = (float)(Math.Round((playerAttributes.stamina/10.0f),1));
        InvokeRepeating("DeductStamina",0.0f,staminaTime);
    }

    void OnDisable(){
        CancelInvoke("MakeDecision");
        speed = playerAttributes.runSpeed;
    }


    
    private void Update()
    {
        if(soccerGameManager.isGoalKick){return;}
        if(touchingBall){
            SelectAction();
            touchingBall = false;
        }
        
        if(speed == 0f){animator.SetTrigger("Idle");}

        //If the player is closest to the ball.
        // Set its location to the ball.
        // With an offset to simulate defense.
        // Set agents speed to depend on stamina.
        if(closestToBall){
            
            if(soccerGameManager.isGoalKick){
                agent.ResetPath();
                return;
            }
            
            SetClosestToBall();

            return;
        }
        

        else switch(playerAttributes.fieldPosition){
            case PlayerAttributes.FieldPosition.Striker:
                //Debug.Log("Setting position for striker.");
                SetStrikerPosition();
                break;

            case PlayerAttributes.FieldPosition.Defense:
                //Debug.Log("Setting position for defense.");
                SetDefenderPosition();
                break;
            
            case PlayerAttributes.FieldPosition.LeftWing:
                SetStrikerPosition();
                break;

            case PlayerAttributes.FieldPosition.CenterMid:
                SetDefenderPosition();
                break;

            case PlayerAttributes.FieldPosition.RightWing:
                SetStrikerPosition();
                break;


        }
        
        

        
    }

    


    public void OnCollisionEnter(Collision other){

        if(!enabled){return;}

        if(other.collider.gameObject.transform.tag == "Goalie"){
            Debug.Log("Hit Goalie.");
            aiRb.velocity = Vector3.zero;
            aiRb.AddForce(other.collider.gameObject.transform.forward * 4000.0f, ForceMode.Impulse);
            return;
        }

        if(other.gameObject.CompareTag("Ball")){

            ballRb.isKinematic = false;

            touchingBall=true;
            
            
            other.transform.parent = CurrentField;
            

            
            
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            
            
            if(isHomeTeam) {soccerGameManager.homeTeamAttacking = true;}
            else {soccerGameManager.homeTeamAttacking = false;}

            
            soccerGameManager.lastToTouchBall = playerAttributes.playerName;
            
        }

        if(other.gameObject.CompareTag("Away Player") && touchingBall && isHomeTeam){
            SlideTackleAnimations();
        }else if(other.gameObject.CompareTag("Home Player") && touchingBall && !isHomeTeam){
            SlideTackleAnimations();
        }
    }
    
    public void OnCollisionStay(Collision other){
        if(!enabled){return;}
        if(other.gameObject.CompareTag("Ball")){
            touchingBall = true;
        }
    }

    

    

    public void OnCollisionExit(Collision other){
        if(!enabled){return;}
        if(other.gameObject.CompareTag("Ball")){
            touchingBall = false;
            nextDecisionMade = false;
        }
    }


    public async void SlideTackleAnimations(){
        Debug.Log("Enemy tackled");
        

        animator.Play("AI_Tackled1");
        agent.speed = 0f;
        agent.isStopped = true;

        await Task.Delay(3000);
    
        agent.speed = playerAttributes.originalRunSpeed;
        agent.isStopped = false;
        
        await Task.CompletedTask;
    }


   


    private void MakeDecision(){
        
        
        if(!closestToBall){return;}
        
        
        
        //CHECK CLEAR BALL
        if(Vector3.Distance(defendingGoal.position,transform.position) < 15.0f){
            //Debug.Log("Clearing next.");
            nextAction = NextAction.WillClear;
            nextDecisionMade = true;
            return;
        }

        //CHECK IF NEAR CORNER
        //if(((ball.position - attackingGoal.position).x < -50.0f || (ball.position - attackingGoal.position).x > 50.0f)
        //&& ((ball.position - attackingGoal.position).z < -80.0f || (ball.position - attackingGoal.position).z > 80.0f)){
        //    Debug.Log("Ball In Corner.");
        //    nextAction = NextAction.WillPass;
        //    nextDecisionMade = true;
        ///    PlayerToPassTo = teamMates.OrderBy(go => Vector3.Distance(go.transform.position,attackingGoal.position))
        //    .LastOrDefault();
        //    return;
        //}   

        
        
        //CHECK FOR DEFENDER CLOSE
        for(int i=0;i<otherTeam.Count;i++){
            var directionToTarget = (otherTeam[i].transform.position - transform.position  + Vector3.up).normalized;
            //Debug.Log("Direction: " + direction);

            if(IsTargetInFieldOfView(directionToTarget)){
                // Cast a ray to check for obstacles
                if(DetectObstacle(otherTeam[i])){
                    //Debug.Log("Obstacle detected. Will Pass.");
                    return;
                }
            }

      
        }

        //CHECK FOR TEAMMATE UPFIELD
        for(int i=0;i<teamMates.Count;i++){

            //Check if teammate closer to attacking goal.
            
            if(Vector3.Distance(transform.position,attackingGoal.position) > Vector3.Distance(teamMates[i].transform.position,attackingGoal.position) + 10.0f){
                if(IsTeammateOpenAhead(teamMates[i])){return;}
            }
            
            
        }

        //CHECK SHOT
        if(Vector3.Distance(attackingGoal.position,transform.position) < playerAttributes.shotDistance ){
            //Debug.Log("Shooting next.");
            nextAction = NextAction.WillShoot;
            nextDecisionMade = true;
            return;
        }


        //ELSE TOUCH BALL
        //Debug.Log("Touching next.");
        nextAction = NextAction.WillTouch;
        nextDecisionMade = true;
        return;
        
       
        
        
            

        
    }
    
    private void SelectAction(){
        switch(nextAction){
            case NextAction.WillTouch:
                TakeTouch(false);
                break;

            case NextAction.WillPass:
                MakePass();
                break;

            case NextAction.WillShoot:
                ShootBall();
                break;

            case NextAction.WillClear:
                ClearBall();
                break;

            
        }
    }




    //Check if teammate is open ahead of ball.

    //A: Nobody between player and teammate
    //OR
    //B: Nobody between teammate and goal
    bool IsTeammateOpenAhead(GameObject teamMate){

        

        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward*2.0f + Vector3.up, (teamMate.transform.position - transform.position + Vector3.up), out hit)){
            if(hit.transform.gameObject.layer == gameObject.layer && hit.transform != transform){
                //Debug.Log("Nobody between player and teammate");
                //Debug.DrawRay(transform.position + transform.forward*2.0f + Vector3.up, (teamMate.transform.position - transform.position + Vector3.up), Color.magenta,3.0f);
                PlayerToPassTo = teamMate;
                nextAction = NextAction.WillPass;
                nextDecisionMade = true;
                return true;
            }
        }
        
        if(Physics.Raycast(teamMate.transform.position + transform.forward*2.0f + Vector3.up, (attackingGoal.position - teamMate.transform.position + Vector3.up), out hit)){
            //Debug.DrawRay(teamMate.transform.position + transform.forward*2.0f + Vector3.up, (attackingGoal.position - teamMate.transform.position + Vector3.up), Color.cyan,3.0f);
            if((hit.transform.CompareTag("Away Player") && isHomeTeam)
            || (hit.transform.CompareTag("Home Player") && !isHomeTeam)){
                //Debug.Log("Somebody between teammate and goal.");
                return false;
            }else{
                //Debug.Log("Nobody between teammate and goal.");
                PlayerToPassTo = teamMate;
                nextAction = NextAction.WillPass;
                nextDecisionMade = true;
                return true;
            }
            
        }else{
            return false;
        }
    }

    bool IsTargetInFieldOfView(Vector3 directionToTarget)
    {
        // Calculate the angle between the player's forward direction and the direction to the target
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        //Debug.Log(angleToTarget);

        // Check if the angle is within the field of view
        return angleToTarget <= fieldOfView * 0.5f;
    }

    


    bool DetectObstacle(GameObject otherPlayer)
    {
        
        //Debug.DrawRay(transform.position + transform.forward*2.0f + Vector3.up, (otherPlayer.transform.position - transform.position + Vector3.up), Color.yellow,2.0f);
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position + transform.forward*2.0f + Vector3.up, (otherPlayer.transform.position - transform.position + Vector3.up), out hit, playerAttributes.passDetectRange))
        {

            //Debug.DrawRay(transform.position + transform.forward*2.0f + Vector3.up, ((otherPlayer.transform.position - transform.position) + Vector3.up)*playerAttributes.passDetectRange, Color.red,1.0f);
            
            
            nextAction = NextAction.WillPass;
            nextDecisionMade = true;
            return true;
            
            
            
        }else{
            return false;
        }
    }












    private async void TakeTouch(bool isGuarded){

        soccerPlayerAudio.KickBallSound();

        /*
        //Check if facing attacking goal.
        RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up + 2.0f*transform.forward,attackingGoal.position - transform.position,out hit)){
            if(hit.transform.CompareTag("Goal")){
                Debug.Log("Goal straight ahead.");
                ballRb.AddForce(transform.forward * touchPower,ForceMode.Impulse);
                return;
            }
        }
        */

        ballRb.isKinematic = false;

        //If not guarded, touch towards goal.
        if(!isGuarded){
            
            //If striker, defense or center mid, touch towards goal
            if(playerAttributes.fieldPosition == PlayerAttributes.FieldPosition.Striker
            || playerAttributes.fieldPosition == PlayerAttributes.FieldPosition.Defense
            || playerAttributes.fieldPosition == PlayerAttributes.FieldPosition.CenterMid){
                transform.LookAt(attackingGoal);

                ballRb.velocity = Vector3.zero;
                ballRb.AddForce(transform.forward * playerAttributes.touchPower,ForceMode.Impulse);
                return;
            }

            //If winger, go to corner.
            else{
                //Debug.Log("Go to corner.");

                Vector3 nearestCornerPosition;
                if(transform.localPosition.z >= 0f){
                    nearestCornerPosition = attackingGoal.position + new Vector3(0f,0f,soccerGameManager.fieldWidth/2.0f);
                }else{
                    nearestCornerPosition = attackingGoal.position - new Vector3(0f,0f,soccerGameManager.fieldWidth/2.0f);
                }

                transform.LookAt(nearestCornerPosition);
                ballRb.velocity = Vector3.zero;
                ballRb.AddForce(transform.forward * playerAttributes.touchPower,ForceMode.Impulse);
                return;
            }
            
            
            
        }

        //If almost out of bounds, touch towards center of field
        else if(transform.localPosition.x > (soccerGameManager.fieldLength/2.0f) - 5.0f ||
        transform.localPosition.x < (-soccerGameManager.fieldLength/2.0f) + 5.0f){
            
            //Debug.Log("Touch to center of field");
            transform.LookAt((attackingGoal.position-defendingGoal.position)/2.0f);

            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.AddForce(transform.forward * playerAttributes.touchPower,ForceMode.Impulse);

        }

        

        //If sidelines, touch in z direction of goal.
        else if(transform.localPosition.z > (soccerGameManager.fieldWidth/2.0f) - 10.0f ||
        transform.localPosition.z < (-soccerGameManager.fieldWidth/2.0f) + 10.0f){

            //Debug.Log("Touch in direction of goal");

            var targetZ = (attackingGoal.position - transform.position).normalized.z;
            transform.LookAt(transform.right * targetZ);
            

            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;

            ballRb.AddForce(transform.right * targetZ * playerAttributes.touchPower,ForceMode.Impulse);
        }



        
        //If Center of the field, take touch to nearest corner.

        //Closer to attacking goal, go to corner

        else if(Vector3.Distance(attackingGoal.position,transform.position) < Vector3.Distance(defendingGoal.position,transform.position) ){

            //Debug.Log("Go to corner.");

            Vector3 nearestCornerPosition;
            if(transform.localPosition.z >= 0f){
                nearestCornerPosition = attackingGoal.position + new Vector3(0f,0f,soccerGameManager.fieldWidth/2.0f);
            }else{
                nearestCornerPosition = attackingGoal.position - new Vector3(0f,0f,soccerGameManager.fieldWidth/2.0f);
            }

            transform.LookAt(nearestCornerPosition);
            ballRb.velocity = Vector3.zero;
            ballRb.AddForce(transform.forward * playerAttributes.touchPower,ForceMode.Impulse);
            
        }
        
        //Closer to defending goal
        else{

            //Debug.Log("Go to corner.");

            Vector3 nearestCornerPosition;
            if(transform.localPosition.z >= 0f){
                nearestCornerPosition = defendingGoal.position + new Vector3(0f,0f,soccerGameManager.fieldWidth/2.0f);
            }else{
                nearestCornerPosition = defendingGoal.position - new Vector3(0f,0f,soccerGameManager.fieldWidth/2.0f);
            }
            transform.LookAt(nearestCornerPosition);
            ballRb.velocity = Vector3.zero;
            ballRb.AddForce(transform.forward * playerAttributes.touchPower,ForceMode.Impulse);
        }


        UpdateShotSlider(0.1f);

        
        
        
        
        await Task.CompletedTask;
    }


    private async void ShootBall(){


        soccerPlayerAudio.KickBallSound();

        //Debug.Log("AI Shot Ball.");
        animator.Play("Kick");
        transform.LookAt(attackingGoal);
        //ball.position = transform.position + transform.forward;

        ballRb.isKinematic = false;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;


        //Smaller accuracy value equals better shot
        var shotVector = new Vector3(0f,0f,UnityEngine.Random.Range(-playerAttributes.shotAccuracy,playerAttributes.shotAccuracy));

        var force = (transform.forward + (0.2f * transform.up) + shotVector)*(playerAttributes.shotPower)*(UnityEngine.Random.Range(0.5f,0.8f));
        ballRb.AddForce(force,ForceMode.Impulse);

        //Debug.Log(force);


        UpdateShotSlider(0.9f);



        await Task.CompletedTask;
    }

    private async void ClearBall(){

        soccerPlayerAudio.KickBallSound();


        animator.Play("Kick");
        ballRb.AddForce(((attackingGoal.position-transform.position).normalized + transform.up*playerAttributes.clearPower),ForceMode.Impulse);

        UpdateShotSlider(0.5f);

        await Task.CompletedTask;
    }

    


    public void MakePass(){

        

        ballRb.isKinematic = false;
        
        //If theres a pre-determined player to pass to, pass to him.
        
        if(PlayerToPassTo != null){

            transform.LookAt(PlayerToPassTo.transform);
            ball.position = transform.position + transform.forward;

            Debug.DrawRay(transform.position + Vector3.up,(PlayerToPassTo.transform.position + Vector3.up - transform.position), Color.green,3.0f);
            
            soccerGameManager.PassingToPlayer = PlayerToPassTo;

            ExecutePass(PlayerToPassTo.transform);
            PlayerToPassTo = null;
            return;
        }

        //Else find an open teammate to pass to.

        for(int i=0;i<teamMates.Count;i++){
            var player = teamMates[i];
            //Debug.DrawRay(transform.position + Vector3.up,(player.transform.position + Vector3.up - transform.position), Color.blue,3.0f);


            //Check if the teammate is in your field of view and at least 5 meters away

            var directionToTarget = (teamMates[i].transform.position - transform.position  + Vector3.up).normalized;
            if(IsTargetInFieldOfView(directionToTarget) && Vector3.Distance(teamMates[i].transform.position,transform.position)>5.0f){
                
                RaycastHit hit;
                
                if (Physics.Raycast(transform.position + transform.forward*2.0f + Vector3.up, (player.transform.position - transform.position + Vector3.up), out hit, 40.0f)){  //passRange
                    
                    if(hit.transform.gameObject.layer==gameObject.layer && hit.transform!=transform){
                        
                        Debug.DrawRay(transform.position + Vector3.up,(player.transform.position + Vector3.up - transform.position), Color.green,3.0f);
                        //Debug.Log(hit.transform.name);
                        transform.LookAt(hit.transform);

                        //var timeOfPass = Vector3.Distance(player.transform.position,transform.position)/19.0f;
                        soccerGameManager.PassingToPlayer = player;
                        ExecutePass(player.transform);
                        
                        return;
                    }
                }
            }
            
        }

        //IF CANT FIND ANYONE
        //TRY TO TAKE TOUCH AROUND DEFENDER
       
        
        
        TakeTouch(true);

        

    }

    //MODELED CUSTOM EQUATION FOR PASS POWER
    
    private void ExecutePass(Transform otherPlayer){
        var speed = otherPlayer.GetComponent<Rigidbody>().velocity.magnitude;
        var a0 = -0.000009m;
        var a1 = 0.001367m;
        var a2 = -0.075419m;
        var a3 = 2.087854m;

        var distance = Vector3.Distance(otherPlayer.transform.position,transform.position);
        var d = Mathf.Round(distance);
        
        var x0 = (decimal)(d * d * d) * a0;
        var x1 = (decimal)(d * d) * a1;
        var x2 = (decimal)(d) * a2;
        
        var power = x0 + x1 + x2 + a3 + 0.2m;
        power = Math.Clamp(power,0.25m,1.4m);
        

        var passVector = (otherPlayer.transform.position + (otherPlayer.transform.forward) - transform.position);
        ballRb.velocity = (float)(power)*passVector;

        soccerPlayerAudio.KickBallSound();

        UpdateShotSlider(0.5f);

        
       
    }


    private async void GetTeammates(){
        await Task.Delay(500);
        if(isHomeTeam){
            teamMates = GameObject.FindGameObjectsWithTag("Home Player").ToList();
            otherTeam = GameObject.FindGameObjectsWithTag("Away Player").ToList();
        }
        else{
            teamMates = GameObject.FindGameObjectsWithTag("Away Player").ToList();
            otherTeam = GameObject.FindGameObjectsWithTag("Home Player").ToList();
        }
        for(int i=0;i<teamMates.Count;i++){
            if(teamMates[i].transform == this){
                teamMates.RemoveAt(i);
            }
        }
        await Task.CompletedTask;
    }


    //IF GOAL SCORED OR OUT OF BOUNDS, NO LONGER ATTACKING.


    public async void MovePlayerGoalKick(){

        Vector3 direction = (defendingGoal.position - attackingGoal.position).normalized;

        // Calculate the distance between the attacking goal and the defending goal
        float totalDistance = Vector3.Distance(attackingGoal.position, defendingGoal.position);

        // Calculate the point 17.5% away from the attacking goal
        float distance = totalDistance * 0.75f; // 82.5% of the total distance
        Vector3 point = attackingGoal.position + direction * distance;


        transform.position = new Vector3(point.x,transform.position.y,transform.position.z);

        await Task.Delay(2000);
    }








    /// <summary>
    /// Sets the position of an attacking player,
    /// if the player is not closest to the goal.
    /// </summary>
    private void SetStrikerPosition(){


        //ATTACK:

        //If ball closer to attack goal.

        if(Vector3.Distance(ball.position,attackingGoal.position) - 5.0f <= Vector3.Distance(ball.position,defendingGoal.position)){


            //If ball wide
            if(ball.localPosition.x < -(soccerGameManager.fieldWidth/2.0f) || ball.localPosition.x > (soccerGameManager.fieldWidth/2.0f)){
                //Debug.Log("Ball is wide");
                //if(ball.transform.localPosition.x < -30.0f){

                //Stay Central
                //var setDes = ((attackingGoal.position - defendingGoal.position).normalized * 20.0f) - new Vector3(0f,0f,(attackingGoal.position - ball.position).normalized.x * 20.0f);
                //agent.SetDestination(ball.transform.position + new Vector3(spacingDistance,0f,0f));
                var setDes = attackingGoal.position
                    + ((attackingGoal.position + defendingGoal.position).normalized * 10f)
                    + new Vector3(ball.position.x,0f,0f)
                    + new Vector3(UnityEngine.Random.Range(-10.0f,10.0f),0f,UnityEngine.Random.Range(-10.0f,10.0f));
                agent.SetDestination(setDes);

                if(agent.remainingDistance !=0f){
                    animator.SetTrigger("Run");
                    agent.speed = playerAttributes.runSpeed;
                    //await Task.CompletedTask;
                }else{
                    animator.SetTrigger("Idle");
                    //await Task.CompletedTask;
                }

            }

            //Ball Central
            //If left central go wide right
            //If right central go wide left.
            else{

                if(otherTeam.Count == 0){return;}

                var lastDefender = otherTeam
                .OrderBy(go => Vector3.Distance(go.transform.position, attackingGoal.transform.position))
                .ToList()[1];

                if((attackingGoal.position - ball.position).x < 0){
                    var setDes = attackingGoal.position
                    + ((attackingGoal.position + defendingGoal.position).normalized * 10f)
                    + new Vector3(0f,0f,playerAttributes.spacingDistance)
                    + new Vector3(UnityEngine.Random.Range(-5f,5f),0f,UnityEngine.Random.Range(-5f,5f));

                    //setDes.clamp(fieldwidth, fieldlength)

                    agent.SetDestination(setDes);
                }else{
                    var setDes = lastDefender.transform.position - new Vector3(0f,0f,playerAttributes.spacingDistance) + new Vector3(UnityEngine.Random.Range(-5f,5f),0f,UnityEngine.Random.Range(-5f,5f));
                    agent.SetDestination(setDes);
                }
                

                if(agent.remainingDistance !=0f){
                    animator.SetTrigger("Run");
                    agent.speed = playerAttributes.runSpeed;
                    //await Task.CompletedTask;
                }else{
                    animator.SetTrigger("Idle");
                    //await Task.CompletedTask;
                }
                
            }






        //BALL CLOSER TO DEFENDING GOAL. STAY UP FIELD.
        }else{
            //DEFENSE
            var setDes = ball.position + (defendingGoal.position - attackingGoal.position).normalized*playerAttributes.defenderDistance + new Vector3(0f,0f,playerAttributes.spacingDistance);
            agent.SetDestination(setDes);
            if(agent.remainingDistance !=0f){
                animator.SetTrigger("Run");
                agent.speed = playerAttributes.runSpeed;
                //await Task.CompletedTask;
            }else{
                animator.SetTrigger("Idle");
                //await Task.CompletedTask;
            }
        }

        
    }
    private async void SetDefenderPosition(){

        //If ball closer to attacking goal. stay short of goal.

        if(Vector3.Distance(ball.position,attackingGoal.position) <= Vector3.Distance(ball.position,defendingGoal.position)){
            agent.SetDestination((attackingGoal.position - (attackingGoal.position - defendingGoal.position).normalized*playerAttributes.strikerDistance) + new Vector3(0f,0f,playerAttributes.spacingDistance));

            if(agent.remainingDistance !=0f){
                animator.SetTrigger("Run");
                agent.speed = playerAttributes.runSpeed;
                //await Task.CompletedTask;
            }else{
                animator.SetTrigger("Idle");
                //await Task.CompletedTask;
            }
            await Task.CompletedTask;
        }


        //If ball closer to defending goal, stay between ball and goal.

        else{
            //Debug.Log((defendingGoal.position - ball.position).normalized);
            agent.SetDestination(defendingGoal.position - (defendingGoal.position - ball.position).normalized*playerAttributes.defenderDistance);

            if(agent.remainingDistance !=0f){
                animator.SetTrigger("Run");
                agent.speed = playerAttributes.runSpeed;
                //await Task.CompletedTask;
            }else{
                animator.SetTrigger("Idle");
                //await Task.CompletedTask;
            }
        
            await Task.CompletedTask;
        }
    }


    private void SetClosestToBall(){
            
        var vectorToBall = (ball.transform.position - transform.position).normalized;
        var newLocation = new Vector3(ball.position.x + vectorToBall.x,groundHeight,ball.position.z + vectorToBall.z);

        
        agent.SetDestination(newLocation);
        transform.LookAt(newLocation);
        animator.SetTrigger("Run");

        agent.speed = speed * (staminaRemaining/100.0f);

        //TRY BICYCLE KICK IF BALL IS CLOSE AND IN AIR
        if(ball.localPosition.y >= 3.0f && (Vector3.Distance(ball.transform.position, transform.position) < 5.0f)){
            agent.speed = 0f;
            aiRb.AddForce(Vector3.up * 3.0f, ForceMode.Impulse);
            animator.Play("ScissorKick");
        }
    }


    private void UpdateShotSlider(float power){
        if(ShotPowerSlider == null){return;}
        ShotPowerSlider.value = power;
    }


    private void DeductStamina(){
        staminaRemaining -= 0.5f;
    }
}
