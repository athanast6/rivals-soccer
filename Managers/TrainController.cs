using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TrainController : MonoBehaviour
{

    private Rigidbody trainRb;
    private GameObject[] wheels;

    public float horsePower = 0.5f;
    public float rotationSpeed = 50.0f;
    public float maxSpeed = 60.0f;


    void Start(){
        trainRb = GetComponent<Rigidbody>();
        wheels = GameObject.FindGameObjectsWithTag("Wheel");
    }
    void Update(){
        
        RotateWheels();
    }


    void FixedUpdate(){
        if(trainRb.velocity.magnitude < maxSpeed){
            trainRb.AddForce(transform.forward * horsePower, ForceMode.Acceleration);
        }
    }


    void RotateWheels()
    {
        // Assuming wheels are child objects of the train
        foreach (var wheel in wheels)
        {
            
            // Rotate wheels based on translation
            wheel.transform.Rotate(Vector3.right, Time.deltaTime * rotationSpeed * trainRb.velocity.magnitude);
            
        }
    }
}
