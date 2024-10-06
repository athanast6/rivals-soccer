using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataCollector : MonoBehaviour
{
    private StreamWriter writer;
    [SerializeField] private Transform Ball;
    string filePath = Application.dataPath + "/MachineLearning/CollectedData.csv";
    public Gamepad gamepad;
    
    void Start()
    {

        gamepad = Gamepad.current;

        // Create a file to store the collected data
        writer = new StreamWriter(filePath);
        
        // Write the header line in the CSV file
        writer.WriteLine("PosX,PosY,PosZ,VelX,VelY,VelZ,BallX,BallZ,HorizontalInput,VerticalInput");
    }

    void Update()
    {
        // Collect the player's current inputs
        var moveDirection = gamepad.leftStick.ReadValue();

        // Get the character's current position and velocity
        Vector3 position = transform.position;
        Vector3 velocity = GetComponent<Rigidbody>().velocity;

        if(writer!=null){
            // Save the data to the CSV file
            writer.WriteLine($"{position.x},{position.y},{position.z},{velocity.x},{velocity.y},{velocity.z},{Ball.localPosition.x},{Ball.localPosition.z},{moveDirection.x},{moveDirection.y}");
        }

    }

    void OnApplicationQuit()
    {
        // Close the file when the game stops
        writer.Close();
    }

}
