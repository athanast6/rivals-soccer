using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataCollector : MonoBehaviour
{
    [SerializeField] private SoccerGameManager soccerGameManager;
    [SerializeField] private Transform Ball;

    private StreamWriter writer;
    string filePath = Application.dataPath + "/MachineLearning/CollectedData.csv";
    public Gamepad gamepad = new Gamepad();
    private MixamoPlayerController playerController;

    private float[] lastDecision;


    string[] dataLabels = {
        "PosX", "PosY", "PosZ", 
        "VelX", "VelY", "VelZ", 
        "BallX", "BallZ", 
        "HorizontalInput", "VerticalInput", 
        "DecisionMade", "Reward"
    };

    
    void Start()
    {
        playerController = GetComponent<MixamoPlayerController>();

        gamepad = Gamepad.current;

        // Create a file to store the collected data
        writer = new StreamWriter(filePath);
        
        // Write the header line in the CSV file
       
        writer.WriteLine(string.Join(",", dataLabels));
    }

    public async void LogDecision(int decision, Vector2 moveDirection)
    {
        
        // Get the character's current position and velocity
        Vector3 position = transform.position;
        Vector3 velocity = GetComponent<Rigidbody>().velocity;

        lastDecision = new float[] {position.x, position.y, position.z, velocity.x, velocity.y, velocity.z, Ball.localPosition.x, Ball.localPosition.z, moveDirection.x, moveDirection.y, decision, 0};
        

        await Task.Delay(1000);

        CriticDecision();

    }


    public void CriticDecision(){

        float reward = 0.0f;

        if(playerController.isHomeTeam && soccerGameManager.homeTeamAttacking){
            reward += 0.1f;
        }
        lastDecision[11] = reward;

            // Use a StringBuilder for efficiency
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < lastDecision.Length; i++)
        {
            sb.Append(lastDecision[i]);

            // Add a comma if it's not the last element
            if (i < lastDecision.Length - 1)
                sb.Append(",");
        }

        // Write the final string
        writer.WriteLine(sb.ToString());
    }

    void OnApplicationQuit()
    {
        // Close the file when the game stops
        writer.Close();
    }

}
