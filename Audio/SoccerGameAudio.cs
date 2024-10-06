using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoccerGameAudio : MonoBehaviour
{
    [SerializeField] private GameObject fieldAudio;

    [SerializeField] public List<AudioClip> fieldAudioClips = new List<AudioClip>();

    [SerializeField] public List<AudioClip> playerAudioClips = new List<AudioClip>();





    [SerializeField] private AudioSource commentaryAudio;
    [SerializeField] public List<AudioClip> commentaryClips = new List<AudioClip>();


    public void StartGameAudio(){
        fieldAudio.SetActive(true);
    }

    public void EndGameAudio(){
        fieldAudio.SetActive(false);
    }

    public async void GoalScoredAudio(){
        if(commentaryAudio == null){return;}
        commentaryAudio.PlayOneShot(commentaryClips[Random.Range(0,commentaryClips.Count)]);
        await Task.CompletedTask;
    }
}
