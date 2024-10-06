using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEmpty : MonoBehaviour
{
    [SerializeField] private GameObject CurrentField;
    [SerializeField] private Rigidbody ballRb;
    private MixamoPlayerController player;

    void Start(){
        player = transform.parent.GetComponent<MixamoPlayerController>();
    }
    public void OnTriggerExit(Collider other){
        if(!enabled){return;}
        if(other.gameObject.CompareTag("Ball")){
            other.transform.parent = CurrentField.transform;
            
            ballRb.isKinematic = false;

            player.playerHasBall = false;

            //player.transform.GetComponent<AudioSource>().Stop();
            
        }
    }
    
}
