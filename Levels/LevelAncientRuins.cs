using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelAncientRuins : MonoBehaviour
{

    private SaveManager saveManager;
    [SerializeField] SoccerGameManager soccerGameManager;
    [SerializeField] Transform ruins;


    //Keep track of the current original positions
    // of the ruins fully built.
    [SerializeField]
    private List<Vector3> ruinOriginalPositions = new List<Vector3>();

    [SerializeField]
    private List<Vector3> ruinRandomPositions = new List<Vector3>();



    //Store all components that need to be activated/deactivated
    // Other Team Enemy AI Controllers
    // Other Team Animators

    [SerializeField] private List<GameObject> enemyPlayers;
    [SerializeField] private GameObject enemyGoalie;
    //[SerializeField] private List<Vector3> enemyPositions = new List<Vector3>();

    void Awake(){
        saveManager = FindObjectOfType<SaveManager>();

        saveManager.saveData.discoveredTemple = false;

        if(!saveManager.saveData.discoveredTemple){
            SetScene();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }


    //Disperses gameobjects,
    //sets other objects active/inactive
    void SetScene(){


        //Loop through enemy players
        //Deactivate their components
        //Position them in the scene
        //Set active
        for(int i=0; i<enemyPlayers.Count;i++){
            enemyPlayers[i].transform.GetComponent<EnemyAIController>().enabled = false;
            enemyPlayers[i].transform.GetComponent<Animator>().enabled = false;
            //enemyPlayers[i].transform.localPosition = enemyPositions[i];
        }

        //Goalie
        enemyGoalie.transform.GetComponent<GoalieController>().enabled = false;
        enemyGoalie.transform.GetComponent<Animator>().enabled = false;
        //enemyGoalie.transform.localPosition = enemyPositions[5];

        //Set away team true
        soccerGameManager.AwayPlayers.SetActive(true);

        //Loop through ruins and set them to random locations.
        //Debug.Log(ruins.childCount);
        for(int i=0;i<ruins.childCount;i++){
            //Debug.Log(ruins.childCount);

            var ruin_position = ruins.GetChild(i).localPosition;

            ruinOriginalPositions.Add(ruin_position);

            ruins.GetChild(i).localPosition = ruin_position + new Vector3(UnityEngine.Random.Range(-5.0f,5.0f),0f,UnityEngine.Random.Range(-5.0f,5.0f));

            ruins.GetChild(i).localEulerAngles = new Vector3(UnityEngine.Random.Range(-17.0f,17.0f),
            UnityEngine.Random.Range(-180.0f,180.0f),
            UnityEngine.Random.Range(87.0f,93.0f));

           
        }
    }


    public async void DiscoveredTemple(){

        Debug.Log("Discovered Temple");
        GetComponent<BoxCollider>().enabled = false;

        

        //Start Cutscene

       

        //Loop through ruins and set them to the original locations.
        for(int i=0;i<ruinOriginalPositions.Count;i++){
            Debug.Log($"ruin {i}");
            //var startPosition = ruins.GetChild(i).localPosition;

            //var endPosition = ruinOriginalPositions[i];

            var duration = UnityEngine.Random.Range(0.1f,0.2f);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Calculate the interpolated position
                ruins.GetChild(i).transform.localPosition = ruinOriginalPositions[i];

                ruins.GetChild(i).transform.localEulerAngles = new Vector3(0f,90f,0f);

                elapsedTime += Time.deltaTime; // Accumulate time
                await Task.Yield();           // Wait until the next frame
            }
            await Task.Delay(50);           // Wait until the next frame

        }

        soccerGameManager.StartGame();

        soccerGameManager.HomePlayers.SetActive(true);

        saveManager.saveData.discoveredTemple = true;

        //Loop through enemy players
        //Activate their components
        for(int i=0; i<enemyPlayers.Count;i++){
            enemyPlayers[i].transform.GetComponent<EnemyAIController>().enabled = true;
            enemyPlayers[i].transform.GetComponent<Animator>().enabled = true;
        }

        enemyPlayers[0].transform.localScale = new Vector3(2f,2f,2f);
        enemyPlayers[1].transform.localScale = new Vector3(2f,2f,2f);

        enemyGoalie.transform.GetComponent<GoalieController>().enabled = true;
        enemyGoalie.transform.GetComponent<Animator>().enabled = true;




        

        
    }



    public void OnCollisionEnter(Collision other){
        Debug.Log("Collision");
            
        DiscoveredTemple();

            
        

    }
}
