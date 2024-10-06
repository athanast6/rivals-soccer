using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Referee : MonoBehaviour
{
    public float gamePrice;
    [SerializeField] private SoccerGameManager soccerGameManager;

    public void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Play Game")){

            

            soccerGameManager.OfferGame(gameObject);

        }
    }

}
