using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SoccerGoal : MonoBehaviour
{  
    //References
    [SerializeField] private SoccerGameManager soccerGameManager;




    //Variables
    public bool homeGoal;
    public int goalsScored;

    public void OnCollisionEnter(Collision other){
        //if(other.gameObject.CompareTag("Ball")){
        //    Debug.Log("Scored Goal");
        //    ScoredGoal();
        //}
    }

   

    public void ScoredGoal(){
        
        
        soccerGameManager.GoalScored(homeGoal);
    }


}
