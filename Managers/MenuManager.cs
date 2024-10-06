using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject PauseMenuUI;
    private UIDocument pauseMenu;
    [SerializeField] private MixamoPlayerController playerController;

    [SerializeField] private UiInteraction uiInteraction;

    public InputAction PlayPause;

    public SaveManager saveManager;


    

    void Start(){

        pauseMenu = PauseMenuUI.transform.GetComponent<UIDocument>();
        saveManager=FindObjectOfType<SaveManager>();
        PlayPause.Enable();
        PlayPause.performed += context => TogglePause();
        
    }


    void TogglePause(){
        if(PauseMenuUI.activeInHierarchy){

            ResumeGame();

        }else{

            PauseGame();

        }

    }



    public void PauseGame(){
        
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        playerController.playerControls.Disable();

        uiInteraction.enabled = true;

        Time.timeScale = 0f;

        PauseMenuUI.SetActive(true);
    }

    public void ResumeGame(){

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        playerController.playerControls.Enable();

        uiInteraction.enabled = false;

        PauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
    }

    public void QuitGame(){
        saveManager.SaveGame();
        Application.Quit();
    }


}
