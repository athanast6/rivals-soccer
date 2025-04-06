using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{

    [SerializeField] public List<GameObject> itemPrefabs = new List<GameObject>();
    public List<Sprite> itemIcons = new List<Sprite>();




    //Placed at center of world
    [SerializeField] private Transform itemsEmpty;

    public float worldRadius;
    //public Vector3 worldCenter;

    void Start(){
        for(int i = 0; i<10;i++){
            SpawnItem();
        }
        InvokeRepeating("SpawnItem",60.0f,60.0f);
    }
    void SpawnItem(){
    
        if(itemsEmpty.childCount>40){return;}
        var spawnPoint = itemsEmpty.transform.position + new Vector3(Random.Range(-worldRadius,worldRadius),20.0f,Random.Range(-worldRadius,worldRadius));

        RaycastHit hit;
        if(Physics.Raycast(spawnPoint,Vector3.down,out hit,100.0f)){
            if(hit.transform.CompareTag("Ground")){

                spawnPoint.y = hit.transform.position.y;
                
            }
        }
        var randomIndex = Random.Range(0, itemPrefabs.Count);
        
        Instantiate(itemPrefabs[randomIndex], spawnPoint, Quaternion.identity,itemsEmpty);

        
        
    
    }
}
