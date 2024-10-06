using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class OnlineSoccerGoal : MonoBehaviour
{  
    //References
    [SerializeField] private OnlineGameManager onlineGameManager;




    //Variables
    public bool homeGoal;
    public int goalsScored;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Ball")){
            Debug.Log("Scored Goal");
            ScoredGoal();
        }
    }

   

    private void ScoredGoal(){
        
        
        onlineGameManager.GoalScored(homeGoal);
    }


}
