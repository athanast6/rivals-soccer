using UnityEngine;

public class SmoothColorChange : MonoBehaviour
{
    public Material[] materials; // Reference to the material whose emission color will be changed
    public Gradient colorGradient; // Gradient to sample colors from
    public float transitionDuration = 1.0f; // Duration of each transition


    private bool forwardTransition = true;
    private float transitionTimer = 0.0f; // Timer for the current transition

    void Start()
    {
        if (materials == null || colorGradient == null)
        {
            enabled = false;
            Debug.LogError("Please assign a material and a color gradient in the inspector.");
            return;
        }
    }

    void Update()
    {   
        if(forwardTransition)   transitionTimer += Time.deltaTime;
        else transitionTimer -= Time.deltaTime;
        

        if (transitionTimer >= transitionDuration)
        {
            //transitionTimer = 0.0f;
            forwardTransition = false;
        }
        
        else if (transitionTimer <= 0.0f){
            forwardTransition = true;
        }

        float t = Mathf.Clamp01(transitionTimer / transitionDuration);
        Color lerpedColor = colorGradient.Evaluate(t);

        for(int i=0;i<materials.Length;i++){

            materials[i].SetColor("_EmissionColor", lerpedColor);
        }
    }
}
