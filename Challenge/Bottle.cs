using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public bool hitBall;
    private ChallengeController challengeController;

    public Vector3 startPosition;


    void Start(){
        startPosition = transform.localPosition;
        gameObject.SetActive(false);
    }
    void Awake(){
        transform.eulerAngles = Vector3.zero;
        challengeController = transform.parent.GetComponent<ChallengeController>();
    }

    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Ball")){
            if(hitBall){return;}
            hitBall=true;
            challengeController.BottleHit();
        }
    }
}
