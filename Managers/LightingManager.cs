using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OccaSoftware.SuperSimpleSkybox.Runtime;
using System.Linq;
using System.Threading.Tasks;
using System;

public class LightingManager : MonoBehaviour
{

    [SerializeField] private Moon Moon;
    private List<GameObject> NightLighting = new List<GameObject>();
    public float maxLight = 1.2f;
    private Material snow;

    
    void Start()
    {
        NightLighting = GameObject.FindGameObjectsWithTag("Night Lighting").ToList();

        ToggleLightingOff();

        if(GameObject.FindGameObjectWithTag("Snow")){
            snow = GameObject.FindGameObjectWithTag("Snow").transform.GetComponent<MeshRenderer>().material;
        }
    }

    void OnEnable(){
        Moon.OnRise += ToggleLightingOn;
        Moon.OnSet += ToggleLightingOff;
    }

    


    void OnDisable(){
        Moon.OnRise -= ToggleLightingOn;
        Moon.OnSet -= ToggleLightingOff;
    }

    async void ToggleLightingOn(){
        foreach(var light in NightLighting){
            light.SetActive(true);
            TurnLightOn(light);
            await Task.Delay(100);
            
        }

        if(snow != null){
            snow.EnableKeyword("_EMISSION");
            snow.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        }

        await Task.CompletedTask;
    }
    async void ToggleLightingOff(){
        await Task.Delay(5000);
        foreach(var light in NightLighting){
            light.transform.GetComponent<Light>().intensity = 0f;
            light.SetActive(false);
        }

        if(snow != null){
            snow.DisableKeyword("_EMISSION");
            snow.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        }

        await Task.CompletedTask;
    }

    void TurnLightOn(GameObject light){
        light.GetComponent<Light>().intensity = maxLight;
        //await Task.Delay(5000);
    }
}
