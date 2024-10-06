using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineSoccerBall : NetworkBehaviour
{
    //References
    private Rigidbody rb;

    private OnlineGameManager onlineGameManager;



    void Awake(){
        rb = GetComponent<Rigidbody>();

        onlineGameManager = FindFirstObjectByType<OnlineGameManager>();
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Sideline")){
            
            //transform.localPosition = Vector3.zero;
            //soccerGameManager.ThrowIn();
        }
        if(other.gameObject.CompareTag("Goalline")){
            

            //soccerGameManager.GoalKick();
        }
    }
}
