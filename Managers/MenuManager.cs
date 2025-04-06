using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuUI;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private MixamoPlayerController playerController;

    [SerializeField] private UiInteraction uiInteraction;

    public InputAction PlayPause;

    public SaveManager saveManager;

    public bool isPaused;


    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start(){

        
        saveManager=FindObjectOfType<SaveManager>();
        PlayPause.Enable();
        PlayPause.performed += context => TogglePause();
        
    }

    

    void OnDisable(){

        PlayPause.Disable();
    }


    void TogglePause(){
        if(PauseMenuUI.activeInHierarchy){

            ResumeGame();

        }else{

            PauseGame();

        }

    }



    public void PauseGame(){

        isPaused = true;
        
        //UnityEngine.Cursor.lockState = CursorLockMode.None;
        //UnityEngine.Cursor.visible = true;
        ShowCursor();

        playerController.playerControls.Disable();

        uiInteraction.enabled = true;

        Time.timeScale = 0f;

        PauseMenuUI.SetActive(true);
        
        pauseManager.ShowPauseMenu();

    }

    public void ResumeGame(){

        isPaused = false;

        //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        //UnityEngine.Cursor.visible = false;
        HideCursor();

        playerController.playerControls.Enable();

        //uiInteraction.enabled = false;

        PauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
    }

    public void QuitGame(){
        saveManager.SaveGame();
        Application.Quit();
    }


    public void ShowCursor()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void ConfineCursor()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
    }


}
