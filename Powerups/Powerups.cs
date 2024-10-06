using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    [SerializeField] private List<GameObject> powerups;
    
    void OnEnable(){
        InvokeRepeating("SpawnPowerup", 0.0f, 40.0f);
    }
    void OnDisable(){
        CancelInvoke("SpawnPowerup");
    }

    public async void SpawnPowerup(){
        
        var random = Random.Range(0,powerups.Count);

        powerups[random].transform.localPosition = new Vector3(Random.Range(-80,80),-0.25f,Random.Range(-40,40));
        powerups[random].SetActive(true);

        await Task.Delay(35000);

        powerups[random].SetActive(false);

        await Task.CompletedTask;
    }
}
