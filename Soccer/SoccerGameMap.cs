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


    [SerializeField] private List<GameObject> UserPlayers;

    void OnEnable(){

        //UserPlayer=GameObject.FindGameObjectWithTag("Home Player");
        InvokeRepeating("UpdateMiniMap",3.0f,0.05f);
    }

    void OnDisable(){
        CancelInvoke("UpdateMiniMap");
    }


    async void UpdateMiniMap(){
        //Debug.Log(Field.transform.position + " Field Position");
        //Debug.Log(UserPlayer.transform.position + " Field Position");

        for(int i=0;i<awayIcons.Count - 1;i++){
            awayIcons[i].transform.localPosition = new Vector3(awayPlayers[i].transform.localPosition.x,awayPlayers[i].transform.localPosition.z,0f) * (-1.15f);
        }

        for(int i=0;i<homeIcons.Count - 1;i++){
            homeIcons[i].transform.localPosition = new Vector3(homePlayers[i].transform.localPosition.x,homePlayers[i].transform.localPosition.z,0f) * (-1.15f);
        }

        //The last home icons will be used for the user players
        for(int i=0;i<UserPlayers.Count;i++){
            var userPosition = new Vector3(Field.transform.position.x - UserPlayers[i].transform.position.x,Field.transform.position.z - UserPlayers[i].transform.position.z,0f) * 1.15f;
            //homeIcons[homeIcons.Count-1].transform.localPosition = userPosition;
            if(i==0){
                homeIcons[homeIcons.Count-1].transform.localPosition = userPosition;
            }
            else if(i==1){
                awayIcons[awayIcons.Count-1].transform.localPosition = userPosition;
            }
        }

        await Task.CompletedTask;
    }
}
