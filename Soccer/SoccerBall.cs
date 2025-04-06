using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.MLAgents;

public class SoccerBall : MonoBehaviour
{
    //References
    private Rigidbody rb;
    [SerializeField] private SoccerGameManager soccerGameManager;
    [SerializeField] private Agent agent;


    //Variables
    public bool trainingMode;
    public bool isHomeTeam = true;
    public bool isKicking;
    private Vector3 kickVector = new Vector3();



    void Start(){
        rb = GetComponent<Rigidbody>();
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Field Boundary")){
            
            ThrowIn(other.transform.position);

            if(trainingMode){
                agent.AddReward(-0.1f);
            }
        }

        if(other.gameObject.CompareTag("Enter Goal")){
            
            Debug.Log("Goal Scored.");
            other.transform.GetComponent<SoccerGoal>().ScoredGoal(trainingMode, isHomeTeam);

            ResetBallToStart();
            return;
        }
        
        if(other.gameObject.CompareTag("Goalline")){
            if(trainingMode){
                ResetBallToStart();
            }else{
                Debug.Log("Goal Kick.");
                soccerGameManager.GoalKick();
            }
            
        }
    }

    

    private async void ThrowIn(Vector3 position){
        //await Task.Delay(1500);
        //transform.position = new Vector3(transform.position.x,transform.position.y,(position.z + (-transform.localPosition.z/10.0f)));

        soccerGameManager.isThrowIn = true;
        soccerGameManager.ThrowIn(transform.localPosition);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Resets ball to original position in center of field.
    /// </summary>
    private void ResetBallToStart(){
        transform.localPosition = new Vector3(0f,0f,0f);
    }

}
