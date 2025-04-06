//using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuUI: MonoBehaviour
{
    // Reference to the pause menu script
    public MenuManager menuManager;
    public UiInteraction uiInteraction;
    private GameObject pauseMenu;


    void OnEnable()
    {
        /*
        pauseMenu = GetComponent<UIDocument>();
        
        uiInteraction.currentUIDocument = pauseMenu;

        // Find and link the resume button
        var resumeButton = pauseMenu.rootVisualElement.Q<Button>("resumeButton");

        if (resumeButton != null)
        {
            resumeButton.RegisterCallback<ClickEvent>(ev => ResumeGame());
        }

        // Find and link the pause button
        var quitButton = pauseMenu.rootVisualElement.Q<Button>("quitButton");

        if (quitButton != null)
        {
            quitButton.RegisterCallback<ClickEvent>(ev => QuitGame());
        }
        */
    }

    // Function to resume the game
    void ResumeGame()
    {
        if (menuManager != null)
        {
            menuManager.ResumeGame();
        }
    }

    void QuitGame(){
        menuManager.QuitGame();
    }

    
}
