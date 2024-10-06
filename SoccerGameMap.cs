using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public class SoccerGameMap : MonoBehaviour
{
    [SerializeField] private List<GameObject> awayPlayers = new List<GameObject>();
    [SerializeField] private List<GameObject> homePlayers = new List<GameObject>();

    [SerializeField] private List<GameObject> awayIcons;
    [SerializeField] private List<GameObject> homeIcons;

    [SerializeField] private GameObject Field;


    private GameObject UserPlayer;

    void OnEnable(){

        UserPlayer=GameObject.FindGameObjectWithTag("Home Player");
        InvokeRepeating("UpdateMiniMap",3.0f,0.05f);
    }

    void OnDisable(){
        CancelInvoke("UpdateMiniMap");
    }


    async void UpdateMiniMap(){
        //Debug.Log(Field.transform.position + " Field Position");
        //Debug.Log(UserPlayer.transform.position + " Field Position");

        for(int i=0;i<awayIcons.Count;i++){
            awayIcons[i].transform.localPosition = new Vector3(awayPlayers[i].transform.localPosition.x,awayPlayers[i].transform.localPosition.z,0f) * (-1.15f);
        }

        for(int i=0;i<homeIcons.Count - 1;i++){
            homeIcons[i].transform.localPosition = new Vector3(homePlayers[i].transform.localPosition.x,homePlayers[i].transform.localPosition.z,0f) * (-1.15f);
        }

        //The last home icon will be used for the user player
        var userPosition = new Vector3(Field.transform.position.x - UserPlayer.transform.position.x,Field.transform.position.z - UserPlayer.transform.position.z,0f) * 1.15f;
        homeIcons[homeIcons.Count-1].transform.localPosition = userPosition;

        await Task.CompletedTask;
    }
}
