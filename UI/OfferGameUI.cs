using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class OfferGameUI : MonoBehaviour
{
    private UIDocument offerGameUI;
    [SerializeField] SoccerGameManager soccerGameManager;

    public InputAction startGame;


    private UiInteraction uiInteraction;
    
    void Awake(){
        uiInteraction = transform.parent.GetComponent<UiInteraction>();
    }
    async void OnEnable(){

        
        offerGameUI = GetComponent<UIDocument>();
        uiInteraction.currentUIDocument = offerGameUI;

        offerGameUI.rootVisualElement.Q<Button>("payReferee").pickingMode = PickingMode.Position;
        
        offerGameUI.rootVisualElement.Q<Button>("payReferee").RegisterCallback<ClickEvent>(PayReferee);

        

        //payRefButton.CapturePointer(0);


        //startGame.Enable();
        //startGame.performed += context => PayReferee();
        uiInteraction.enabled = true;
        
        
        

        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;


        await Task.Delay(5000);

        uiInteraction.enabled = false;


        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void PayReferee(ClickEvent evt){

        Debug.Log("Paid ref.");

        soccerGameManager.StartGame();

        gameObject.SetActive(false);

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

    }
}
