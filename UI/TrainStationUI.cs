using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.UIElements;

public class TrainStationUI : MonoBehaviour
{


    [SerializeField] private Transform Player;
    [SerializeField] private TrainController trainController;
    [SerializeField] private CanvasGroup fadeCanvas;

    private UIDocument trainStationUI;
    private UiInteraction uiInteraction;

    [SerializeField] private string[] destinations;

    
    private string currentScene;
    private int destinationIndex;

    [SerializeField] SaveManager saveManager;
    [SerializeField] PlayerInventory playerInventory;


    
    void Awake(){
        uiInteraction = transform.parent.GetComponent<UiInteraction>();

        currentScene = SceneManager.GetActiveScene().name;
    }
    async void OnEnable(){
        trainStationUI = GetComponent<UIDocument>();


        FindObjectOfType<MenuManager>().PauseGame();
        
       

        var closeButton = trainStationUI.rootVisualElement.Q<Button>("ExitButton");
        closeButton.RegisterCallback<ClickEvent>(CloseStationUI);
        closeButton.BringToFront();

        var purchaseButton = trainStationUI.rootVisualElement.Q<Button>("PurchaseButton");
        purchaseButton.RegisterCallback<ClickEvent>(PurchaseTicket);
        
        uiInteraction.currentUIDocument = trainStationUI;

        ShowStationUI();


        await Task.CompletedTask;
    }

    void OnDisable(){
        FindObjectOfType<MenuManager>().ResumeGame();
    }


    async void ShowStationUI(){
        //Go through each button and set it to take the index of the button
        //Then use the index of the button to send the player to another scene

        for(int i=0;i<destinations.Length;i++){
            var index = i;
            var buttonName = "stationButton" + index;

            trainStationUI.rootVisualElement.Q<GroupBox>("stations").Q<Button>(buttonName).text = $"Ticket to {destinations[index]}" + "\n" + " 50 coins"; 


            trainStationUI.rootVisualElement.Q<GroupBox>("stations").Q<Button>(buttonName).RegisterCallback<ClickEvent>(ev => ChooseStation(index));
        }
        await Task.CompletedTask;
    }




    public async void ChooseStation(int index){

        //Set Purchase Button to show
        var purchaseButton = trainStationUI.rootVisualElement.Q("PurchaseButton");
        purchaseButton.style.display = DisplayStyle.Flex;

        //Save index of button.
        destinationIndex = index;

        await Task.Delay(7000);

        purchaseButton.style.display = DisplayStyle.None;

        await Task.CompletedTask;
    }



    public async void PurchaseTicket(ClickEvent evt){
        trainController.enabled = true;

        playerInventory.money -= 50;

        //Set player on train.
        PlayerRidingTrain();
        

        await Task.Delay(12500);

        await FadeAsync(0,1,2.5f,fadeCanvas);


        var destinationScene = destinations[destinationIndex];
        //Go to destination
        SceneManager.LoadSceneAsync(destinationScene);
        SceneManager.UnloadSceneAsync(currentScene);

        //Close Station
        trainStationUI.gameObject.SetActive(false);

        saveManager.SaveGame(destinationScene, destinationIndex);

        await Task.CompletedTask;

    }

    private async void PlayerRidingTrain(){
        Player.GetChild(0).transform.GetComponent<MixamoPlayerController>().playerControls.Disable();


        Player.transform.parent = trainController.gameObject.transform;
        Player.transform.localPosition = new Vector3(0f,0.337000012f,-2.97099996f);


        Player.GetChild(0).transform.localPosition = Vector3.zero;
        
        Player.GetChild(0).transform.GetComponent<Rigidbody>().isKinematic = true;
        Player.GetChild(0).transform.GetComponent<BoxCollider>().enabled = false;
        
        await Task.CompletedTask;
    }


    async void CloseStationUI(ClickEvent evt){
        gameObject.SetActive(false);
        await Task.CompletedTask;
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
