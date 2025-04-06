using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PowerupItem : MonoBehaviour
{
    MixamoPlayerController player;
    EnemyAIController ai;
    GoalieController goalie;

    ParticleSystem particles;
    void Start(){
        //player =GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MixamoPlayerController>();
        particles = GetComponent<ParticleSystem>();
    }
    void OnTriggerEnter(Collider other){

        if(other.transform.CompareTag("Home Player") ||
        other.transform.CompareTag("Away Player")){
            
            
        ApplyPowerUp(other.transform);

            
            
                
            
        }
    }

    public async void ApplyPowerUp(Transform other){

        particles.Stop();

        if(other.TryGetComponent<MixamoPlayerController>(out player)){
            //player.HitPowerup();

        }else if(other.TryGetComponent<EnemyAIController>(out ai)){
            

        }else if(other.TryGetComponent<GoalieController>(out goalie)){
            
        }

        gameObject.SetActive(false);

        await Task.CompletedTask;

        

        
    }
}
