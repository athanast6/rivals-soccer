using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class SoccerPlayer : MonoBehaviour
{

    //References
    [SerializeField] private RecruitUI recruitUI;
    [SerializeField] private GameObject NPC_Canvas;
    private Rigidbody rb;
    private GameObject GreetingText;

    private Animator animator;
    private NavMeshAgent agent;
    private Vector3 startPosition;

    public PlayerAttributes playerAttributes;


   


    void Start(){
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;

        transform.localScale = new Vector3(playerAttributes.sizeScale,playerAttributes.sizeScale,playerAttributes.sizeScale);
        NPC_Canvas.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = playerAttributes.playerName;

        GreetingText = NPC_Canvas.transform.GetChild(1).gameObject;
        GreetingText.transform.GetComponent<TextMeshProUGUI>().text = playerAttributes.greetingMessage;

        
    }

    void OnEnable(){
        InvokeRepeating("SetRandomNPCLocation",0.0f,60.0f);
    }

    void OnDisable(){
        CancelInvoke("SetRandomNPCLocation");
    }

    void Update(){
        if(agent.remainingDistance < 0.1f){
            animator.SetTrigger("Idle");
        }
    }

    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Home Player")){

            StopNPC(other.transform);
            RecruitToSquad();
            DisplayGreeting();

           
        }
    }

    private void StopNPC(Transform Player){
        agent.ResetPath();

        transform.LookAt(Player);

        animator.SetTrigger("Idle");
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }



    private void RecruitToSquad(){

        if(playerAttributes.isOnTeam){return;}
        recruitUI.recruit = this;

        //Show the ui panel for options to recruit him to squad.
        recruitUI.gameObject.SetActive(true);
        

    }


    

    async void DisplayGreeting(){
        GreetingText.SetActive(true);

        await Task.Delay(8000);
        GreetingText.SetActive(false);

        await Task.CompletedTask;
        
    }
    

   

    

    void SetRandomNPCLocation(){
        animator.SetTrigger("Walk");
        var newLocation = startPosition + new Vector3(Random.Range(-40.0f,40.0f),0f,Random.Range(-40.0f,40.0f));
        agent.SetDestination(newLocation);

        
    }
}