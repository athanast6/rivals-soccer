using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoccerPlayerAudio : MonoBehaviour
{
    private AudioSource playerAudio;
    private EnemyAIController enemyAIController;

    [SerializeField] private SoccerGameAudio soccerGameAudio;


    void Start(){
        playerAudio = GetComponent<AudioSource>();
    }
    public async void KickBallSound(){
        if(playerAudio && !playerAudio.isPlaying){
            playerAudio.clip = soccerGameAudio.playerAudioClips[1];
            playerAudio.Play();
        }
        
        
        await Task.CompletedTask;
    }
}
