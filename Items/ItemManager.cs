using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{

    [SerializeField] public List<GameObject> itemPrefabs = new List<GameObject>();
    public List<Sprite> itemIcons = new List<Sprite>();





    [SerializeField] private Transform itemsEmpty;

    public float worldRadius;
    public Vector3 worldCenter;

    void Start(){
        for(int i = 0; i<5;i++){
            SpawnItem();
        }
        InvokeRepeating("SpawnItem",120.0f,60.0f);
    }
    void SpawnItem(){
    
        if(itemsEmpty.childCount>20){return;}
        var spawnPoint = worldCenter + new Vector3(Random.Range(-worldRadius,worldRadius),0.0f,Random.Range(-worldRadius,worldRadius));

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
