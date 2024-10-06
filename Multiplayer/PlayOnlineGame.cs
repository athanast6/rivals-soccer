using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnlineGame : MonoBehaviour
{
    public float gamePrice;
    [SerializeField] private OnlineGameManager onlineGameManager;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Home Player")){
            onlineGameManager.StartGame();
        }
    }

}
