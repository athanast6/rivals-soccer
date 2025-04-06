using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainStation : MonoBehaviour
{
    [SerializeField] GameObject TrainStationUI;
    private Transform Player;

    void Start(){
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.transform.parent.CompareTag("Player")){


            //Train Station Entered
            Debug.Log("Train station entered");
            
            TrainStationUI.SetActive(true);

            
            FindObjectOfType<MenuManager>().PauseGame();

            Player.GetComponentInChildren<MixamoPlayerController>().playerControls.Disable();
            

        }
    }

    /*
    public void SelectTicket(){
        if(playerInventory.money >= trainCost){
            ConfirmUI.SetActive(true);
        }
    }


    public async void PurchaseTicket(){
        trainController.enabled = true;

        ExitTrainUI();

        playerInventory.money -= trainCost;

        //Save Game

        //Set player on train.

        PlayerRidingTrain();
        

        await Task.Delay(12500);

        await FadeAsync(0,1,2.5f,fadeCanvas);


        //Go to destination
        SceneManager.LoadSceneAsync(destinationScene);
        SceneManager.UnloadSceneAsync(currentScene);

        await Task.CompletedTask;

    }

    public void CancelPurchase(){
        ConfirmUI.SetActive(false);
    }

    public void ExitTrainUI(){
        TrainStationUI.SetActive(false);
        lookCamera.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Player.GetComponentInChildren<MixamoPlayerController>().playerControls.Enable();
    }


    private async void PlayerRidingTrain(){
        Player.transform.parent = trainController.gameObject.transform;
        Player.transform.localPosition = new Vector3(0f,0.337000012f,-2.97099996f);
        Player.GetChild(0).transform.localPosition = Vector3.zero;
        Player.GetChild(0).transform.GetComponent<MixamoPlayerController>().playerControls.Disable();
        Player.GetChild(0).transform.GetComponent<Rigidbody>().isKinematic = true;
        Player.GetChild(0).transform.GetComponent<BoxCollider>().enabled = false;
        
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
    */
}
