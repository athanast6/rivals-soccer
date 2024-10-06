using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    //References
    private Rigidbody rb;

    [SerializeField] private SoccerGameManager soccerGameManager;



    void Start(){
        rb = GetComponent<Rigidbody>();
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Field Boundary")){
            
            ThrowIn(other.transform.position);
        }

        if(other.gameObject.CompareTag("Enter Goal")){
            
            Debug.Log("Goal Scored.");
            other.transform.GetComponent<SoccerGoal>().ScoredGoal();
            return;
        }
        
        if(other.gameObject.CompareTag("Goalline")){
            
            Debug.Log("Goal Kick.");
            soccerGameManager.GoalKick();
        }
    }

    

    private async void ThrowIn(Vector3 position){
        await Task.Delay(1500);
        transform.position = new Vector3(transform.position.x,transform.position.y,(position.z + (-transform.localPosition.z/10.0f)));
        await Task.CompletedTask;
    }
}
