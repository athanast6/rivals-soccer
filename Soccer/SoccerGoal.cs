using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class SoccerGoal : MonoBehaviour
{  
    //References
    [SerializeField] private SoccerGameManager soccerGameManager;

    [SerializeField] private Agent agent;



    //Variables
    public bool homeGoal;
    public int goalsScored;

    public void OnCollisionEnter(Collision other){
        //if(other.gameObject.CompareTag("Ball")){
        //    Debug.Log("Scored Goal");
        //    ScoredGoal();
        //}
    }

   

    public void ScoredGoal(bool isTrainingMode, bool isHomeTeam){
        
        if(isTrainingMode){

            //Goal Scored
            if(isHomeTeam == homeGoal){
                agent.AddReward(5.0f);
            }

            //Own Goal Scored
            else if(isHomeTeam != homeGoal){
                agent.AddReward(-5.0f);
            }

            return;
        }

        else{
            soccerGameManager.GoalScored(homeGoal);
        }
        
    }


}
