using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
//using UnityEngine.WSA;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class UiInteraction : MonoBehaviour
{
    public GameObject ui_canvas;
    GraphicRaycaster ui_raycaster;

    public UIDocument currentUIDocument;
    public float moveSpeed;
    public InputAction click;
    private PointerEventData click_data = new PointerEventData(EventSystem.current);
    private List<RaycastResult> click_results = new List<RaycastResult>();


    void OnEnable(){
        
    
        click.performed += context => GetUiElementsClicked();
        click.Enable();
       
        
    }

    void OnDisable(){
        click.Disable();
        
    }
 
    void Update()
    {
        // use isPressed if you wish to ray cast every frame:
        //if(Mouse.current.leftButton.isPressed)
        
        // use wasReleasedThisFrame if you wish to ray cast just once per click:
        //if(Mouse.current.leftButton.wasPressedThisFrame)
        //{
        //    GetUiElementsClicked();
        //}

        var gamepad = Gamepad.current;
        if(gamepad != null){
            var moveDirection = gamepad.leftStick.ReadValue();
            
            MoveCursor(moveDirection * moveSpeed);
        }

    }

    public void GetUiElementsClicked()
    {

        click_data.position = Mouse.current.position.ReadValue();// + new Vector2(0f,1080.0f);
        Debug.Log(click_data.position);

        // Get the root VisualElement of the UI Toolkit system
        

        // Convert mouse position to local UI Toolkit space
        //Debug.Log(Input.mousePosition);
        //var localMousePosition = new Vector2(Input.mousePosition.x,Screen.height - Input.mousePosition.y);
        

        // Perform hit testing to find the VisualElement at the mouse position
        //if(currentUIDocument==null || currentUIDocument.rootVisualElement == null){return;}
        //var visualElementUnderMouse = currentUIDocument.rootVisualElement.panel.Pick(localMousePosition);
        
        //if(visualElementUnderMouse == null){return;}

        //Debug.Log(visualElementUnderMouse.name);

        // var button = visualElementUnderMouse.Q<Button>();
        //if (button != null)
        //{
        //    using(var clickEvent = ClickEvent.GetPooled()){
        //        clickEvent.target = button;
        //        button.panel.visualTree.SendEvent(clickEvent);
        //    }
        
        //}

        
        //click_results.Clear();
 
        // Retrieve mouse position
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Debug.Log($"Mouse Position: {mousePosition}");

        // Optionally, if you are using UI, convert mouse position to Raycast data
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };

        // Handle button click (example with a raycaster)
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        // Process the results
        foreach (RaycastResult result in results)
        {
            Debug.Log($"Clicked UI Element: {result.gameObject.name}");
            // Call the button click handler if it's a button
            UnityEngine.UI.Button button;
            if (result.gameObject.TryGetComponent<UnityEngine.UI.Button>(out button))
            {
                button.onClick.Invoke();
            }
        }

        
    }

    private void MoveCursor(Vector2 moveDirection){

        if(moveDirection.magnitude > 0.1f){

            
            var curPosition = Mouse.current.position.ReadValue();
            var newPosition = new Vector2(curPosition.x + moveDirection.x, curPosition.y + moveDirection.y);
            Mouse.current.WarpCursorPosition(newPosition);
        }

       
    }
}