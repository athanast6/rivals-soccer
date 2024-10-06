using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChallenge : MonoBehaviour
{

    private ChallengeController challengeController;
    void Start(){
        challengeController = transform.parent.GetComponent<ChallengeController>();
    }
    void OnTriggerEnter(Collider other){
        if(other.transform.parent.CompareTag("Player")){
            Debug.Log("start challenge");
            challengeController.StartChallenge(gameObject);
            this.gameObject.SetActive(false);
        }
    }
}

