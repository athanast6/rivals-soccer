using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public GameObject Player;
    [SerializeField] private GameObject Ball;
    public float camDistance = 32.0f;

    
    void Update(){
        var newPos = new Vector3(Ball.transform.position.x,transform.position.y,((Player.transform.position.z + Ball.transform.position.z)/2.0f) + camDistance);

        transform.position = Vector3.Lerp(transform.position,newPos,Time.deltaTime);
    }

    public void SetPlayer(GameObject playerToSet)
    {
        Player = playerToSet;
    }
}
