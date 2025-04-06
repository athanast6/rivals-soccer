using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TrainStationUI : MonoBehaviour
{


    [SerializeField] private Transform Player;
    [SerializeField] private TrainController trainController;
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private List<GameObject> DestinationButtons = new List<GameObject>();
    [SerializeField] private UiInteraction uiInteraction;

    [SerializeField] private string[] destinations;
    [SerializeField] private int[] destinationCosts;

    
    private string currentScene;
    private int destinationIndex;

    [SerializeField] SaveManager saveManager;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] private GameObject PurchaseButton;
    [SerializeField] GameObject trainStationUI;


    void Awake(){

        currentScene = SceneManager.GetActiveScene().name;

        ShowStationUI();
    }
    private void ShowStationUI(){
        //Go through each button and set it to take the index of the button
        //Then use the index of the button to send the player to another scene
        for(int i=0;i<DestinationButtons.Count;i++){
            DestinationButtons[i].SetActive(false);
        }

        for(int i=0;i<destinations.Length;i++){
           DestinationButtons[i].SetActive(true);
        }
    }




    public async void ChooseStation(int index){

        destinationIndex = index;
        
        if(PurchaseButton != null){ PurchaseButton.SetActive(true);}
        
        await Task.Delay(5000);

        if(PurchaseButton != null){PurchaseButton.SetActive(false);}
        
        await Task.CompletedTask;
    }



    public async void PurchaseTicket(){
        //Close Station
        CloseStationUI();
        trainController.enabled = true;

        playerInventory.money -= destinationCosts[destinationIndex];

        //Set player on train.
        PlayerRidingTrain();
        
        await FadeAsync(0,1,2.5f,fadeCanvas);


        var destinationScene = destinations[destinationIndex];
        //Go to destination
        SceneManager.LoadSceneAsync(destinationScene);
        SceneManager.UnloadSceneAsync(currentScene);

        saveManager.SaveGame(destinationScene, destinationIndex);

        
        
        await Task.Delay(12500);
        
        

        await Task.CompletedTask;

    }

    private void PlayerRidingTrain(){
        Player.GetComponentInChildren<MixamoPlayerController>().enabled=false;


        Player.transform.parent = trainController.gameObject.transform;
        Player.transform.localPosition = new Vector3(0f,0.337000012f,-2.97099996f);


        Player.GetChild(0).transform.localPosition = Vector3.zero;
        
        Player.GetChild(0).transform.GetComponent<Rigidbody>().isKinematic = true;
        Player.GetChild(0).transform.GetComponent<BoxCollider>().enabled = false;
    }


    public void CloseStationUI(){
        trainStationUI.SetActive(false);
        FindObjectOfType<MenuManager>().ResumeGame();
        
    }



    private static async Task FadeAsync(float start, float end, float duration, CanvasGroup canvasGroup)
    {
        
        float time = 0f;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            await Task.Yield(); // Allow other asynchronous tasks to run
        }

        // Ensure the final alpha value is set
        canvasGroup.alpha = end;
    }
}
