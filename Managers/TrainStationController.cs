using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Array to hold the names of the scenes to be loaded
    public string[] sceneNames;

    // Reference to the parent VisualElement for organizing buttons
    public VisualElement buttonParent;

    // Reference to the button template
    public VisualTreeAsset buttonTemplate;

    // Reference to the confirm panel
    public VisualElement confirmPanel;

    void Start()
    {
        // Create buttons for each scene in the array
        foreach (string sceneName in sceneNames)
        {
            CreateButton(sceneName);
        }
    }

    // Function to create a button for a scene
    void CreateButton(string sceneName)
    {
        // Clone the button template
        VisualElement buttonInstance = buttonTemplate.CloneTree();

        // Set the text of the button
        buttonInstance.Q<Label>().text = sceneName;

        // Add a click event to the button
        buttonInstance.Q<Button>().clickable.clicked += () => LoadScene(sceneName);

        // Add the button to the parent
        buttonParent.Add(buttonInstance);
    }

    // Function to load a scene
    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Function to quit the application
    public void ExitTrainStation()
    {
        // Show the confirmation panel
        confirmPanel.style.display = DisplayStyle.Flex;
    }

    // Function to confirm quitting the application
    public void ConfirmQuit()
    {
        Application.Quit();
    }

    // Function to cancel quitting the application
    public void CancelQuit()
    {
        // Hide the confirmation panel
        confirmPanel.style.display = DisplayStyle.None;
    }
}
