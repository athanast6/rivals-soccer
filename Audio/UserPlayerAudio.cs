using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UserPlayerAudio : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    private AudioSource audioSource;

    void Start(){
        audioSource = GetComponent<AudioSource>();
    }
    public async void WalkingAudio(){
        audioSource.clip = audioClips[0];
        audioSource.UnPause();
        await Task.CompletedTask;
    }

    public async void IdleAudio(){
        audioSource.Pause();
        await Task.CompletedTask;
    
    }
}
