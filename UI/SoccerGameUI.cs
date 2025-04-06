/*


SoccerGameUI.cs


Handles UI Related Tasks for the soccer game's UI
Updating scores
Updating recent goal scored
Updating clock
End of game ui animations


*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SoccerGameUI : MonoBehaviour
{

    //Script References
    private SoccerGameManager soccerGameManager;

    //UI References
    [SerializeField] private TextMeshProUGUI homeScoreText;
    [SerializeField] private TextMeshProUGUI awayScoreText;


    
    void Start()
    {
        soccerGameManager = FindObjectOfType<SoccerGameManager>();

        soccerGameManager.OnGoalScored += UpdateUI;
    }

    void UpdateUI(bool homeTeam)
    {
        homeScoreText.text = "Home: " + soccerGameManager.homeScore;
        awayScoreText.text = "Away: " + soccerGameManager.awayScore;
        //goalScoredText.text = goalInfo;
    }
}
