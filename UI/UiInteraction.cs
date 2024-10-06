using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cinemachine;
using System;
using UnityEngine.WSA;
using Unity.VisualScripting;


public class UiInteraction : MonoBehaviour
{
    //public GameObject ui_canvas;
    //GraphicRaycaster ui_raycaster;

    public UIDocument currentUIDocument;
    public float moveSpeed;
    public InputAction click;
    

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
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            GetUiElementsClicked();
        }

        var gamepad = Gamepad.current;
        if(gamepad != null){
            var moveDirection = gamepad.leftStick.ReadValue();
            
            MoveCursor(moveDirection * moveSpeed);
        }

    }
 
    public void GetUiElementsClicked()
    {

        //click_data.position = Mouse.current.position.ReadValue() + new Vector2(0f,1080.0f);
        //Debug.Log(click_data.position);

        // Get the root VisualElement of the UI Toolkit system
        

        // Convert mouse position to local UI Toolkit space
        Debug.Log(Input.mousePosition);
        var localMousePosition = new Vector2(Input.mousePosition.x,Screen.height - Input.mousePosition.y);
        

        // Perform hit testing to find the VisualElement at the mouse position
        if(currentUIDocument==null || currentUIDocument.rootVisualElement == null){return;}
        var visualElementUnderMouse = currentUIDocument.rootVisualElement.panel.Pick(localMousePosition);
        
        if(visualElementUnderMouse == null){return;}

        Debug.Log(visualElementUnderMouse.name);

        var button = visualElementUnderMouse.Q<Button>();
        if (button != null)
        {
            using(var clickEvent = ClickEvent.GetPooled()){
                clickEvent.target = button;
                button.panel.visualTree.SendEvent(clickEvent);
            }
        
        }

        /*
        click_results.Clear();
 
        ui_raycaster.Raycast(click_data, click_results);
 
        foreach(RaycastResult result in click_results)
        {
            var ui_element = result.gameObject;
 
            Debug.Log(ui_element.name);

            Button button;
            ui_element.TryGetComponent<Button>(out button);

            if(button!=null)button.onClick.Invoke();
        }
        */
    }

    private void MoveCursor(Vector2 moveDirection){

        if(moveDirection.magnitude > 0.1f){

            
            var curPosition = Mouse.current.position.ReadValue();
            var newPosition = new Vector2(curPosition.x + moveDirection.x, curPosition.y + moveDirection.y);
            Mouse.current.WarpCursorPosition(newPosition);
        }

       
    }
}