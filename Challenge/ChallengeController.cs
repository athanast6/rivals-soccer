using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ChallengeController : MonoBehaviour
{
    [SerializeField] List<Bottle> bottles;
    [SerializeField] GameObject Ball;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject ChallengeUI;
    private GameObject StartChallengeObj;




    private TextMeshProUGUI bottlesHitText;


   


    public float rewardCoins;
    private int bottlesHit;

    void Start(){
        bottlesHitText = ChallengeUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }



    public void StartChallenge(GameObject challengeObj){

        bottlesHit = 0;
        bottlesHitText.text = "Bottles Hit: 0/5";

        StartChallengeObj = challengeObj;

        for(int i=0;i<bottles.Count;i++){

            bottles[i].transform.localPosition = bottles[i].startPosition;
            bottles[i].gameObject.SetActive(true);
        }

        Ball.SetActive(true);

        Player.transform.position = Ball.transform.position + (-2.0f*Ball.transform.forward);

        ChallengeUI.SetActive(true);
    }


    public void BottleHit(){
        bottlesHit +=1;
        bottlesHitText.text = "Bottles Hit: "+ bottlesHit + "/5";
        CheckBottles();
    }

    void CheckBottles(){
        Debug.Log("Bottle hit");

        foreach(var bottle in bottles){
            if(!bottle.hitBall){return;}
        }

        ChallengeComplete();
        
    }
    private async void ChallengeComplete(){

        Ball.SetActive(false);
        await Task.Delay(2500);


        //Challenge complete when all bottles on ground.
        Player.GetComponent<PlayerInventory>().money += rewardCoins;

        bottlesHitText.text = "Challenge Completed! Won " + rewardCoins + " coins!";

        await Task.Delay(5000);

        ChallengeUI.SetActive(false);

        for(int i=0;i<bottles.Count;i++){
            bottles[i].gameObject.SetActive(false);
        }

        await Task.Delay(15000);
        ResetChallenge();


        await Task.CompletedTask;
    }
    
    private void ResetChallenge(){
        StartChallengeObj.SetActive(true);
    }
}
