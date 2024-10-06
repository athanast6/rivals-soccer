using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEditor.Animations;

public class FightPlayerController : MonoBehaviour
{



    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    private float playerSpeed;
    private Vector2 moveDirection;







    private CinemachineFreeLook lookCamera;
    private Animator playerAnimator;
    [SerializeField] private AnimatorController fightAnim;
    private Rigidbody rb;




    public InputActionMap playerControls;
    private InputAction movement;


    void OnEnable(){
        playerAnimator = GetComponent<Animator>();
        playerAnimator.runtimeAnimatorController = fightAnim;
    }


    void Awake(){
        
        movement = playerControls.FindAction("Movement");

        lookCamera = transform.GetChild(0).gameObject.GetComponent<CinemachineFreeLook>();

        


        rb = GetComponent<Rigidbody>();

        InitializeInputs();

        playerControls.Enable();
    }

    void Update(){
        moveDirection = movement.ReadValue<Vector2>();

        

        if(moveDirection.x == 0 && moveDirection.y == 0)
        {
            playerAnimator.SetTrigger("Idle");
            playerAnimator.ResetTrigger("Walk");
            playerAnimator.ResetTrigger("Run");

        }else
        {
            if(playerControls.FindAction("Run").ReadValue<float>() !=0f){

                playerAnimator.SetTrigger("Run");
                playerAnimator.ResetTrigger("Idle");
                playerAnimator.ResetTrigger("Walk");

                playerSpeed = runSpeed;

            }else{
                
                playerSpeed = walkSpeed;
                playerAnimator.SetTrigger("Walk");
                playerAnimator.ResetTrigger("Idle");
                playerAnimator.ResetTrigger("Run");

            }
        

            //playerAnimator.SetFloat("LeftRight",moveDirection.x);
            //playerAnimator.SetFloat("ForwardBackward",moveDirection.y);
        }

    }


    void FixedUpdate(){
        transform.Translate(Vector3.forward * playerSpeed * moveDirection.y * Time.deltaTime);

        transform.localEulerAngles = new Vector3(0f,lookCamera.m_XAxis.Value,0f);
        transform.Translate(Vector3.right * walkSpeed * moveDirection.x * Time.deltaTime);
    }

    void InitializeInputs(){
        playerControls.FindAction("Punch").performed +=
            context => {OnPunch();};

        playerControls.FindAction("Fight").performed +=
            context => {ExitFightMode();};
    }




    void OnPunch(){
        playerAnimator.Play("Punching");

        rb.AddRelativeForce(transform.forward*50.0f);
    }

    void ExitFightMode(){
        
        GetComponent<MixamoPlayerController>().enabled = true;
        GetComponent<FightPlayerController>().enabled = false;
    }
}
