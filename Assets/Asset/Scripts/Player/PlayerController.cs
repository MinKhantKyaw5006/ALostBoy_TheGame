/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour ,IDataPersistence
{
    //camera follow settting---------------------------
    [Header("Camera Setting")]
    [SerializeField] private GameObject _cameraFollowGo;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangethresold;
    //camera follow settting---------------------------



    [Header("Horizontal Movement Setting")]
    [SerializeField] private float walkspeed = 1;

    [Header("Dash Variable Setting")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;
    [SerializeField] private GameObject dashEffect;
    [SerializeField] private float PlayerScale;

    [Header("Ground Check Setting")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask WhatIsGround;







    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;



    private PlayerControls playerControls;
    private Rigidbody2D rb;
    public PlayerStateList pState;
    public static PlayerController Instance;
    private SpriteRenderer sr;
    private Animator anim; // Reference to the Animator component
    private float xAxis, yAxis;
    private bool canDash = true;
    private bool Dashed;
    private float gravity;
    private bool attack = false;
    private float timeBetweenAttack, timeSinceAttack;
    private int stepXRecoiled, stepYRecoiled;
    private Vector2 currentMovementInput;



    private float movementInput;
    public bool IsFacingRight = true;

    private void Awake()
    {
        // Initialize the input controls
        playerControls = new PlayerControls();

        // Subscribe to the performed and canceled events of the Move action
        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        // Initialize healing action callbacks
    }

    void Update()
    {
        GetInput();


        // Listen for the dash input
        if (Input.GetButtonDown("Dash") && canDash)
        {
            StartCoroutine(Dash());
        }

        Move(); // Consider moving the velocity update here if not already
        Flip();
        StartDash();





        //if we are falling past a certain speed thresold
        if (rb.velocity.y < _fallSpeedYDampingChangethresold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        //if we are standing still or moving up
        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            //reset so it can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Get the Animator component

        gravity = rb.gravityScale;
        
                                       
        

        _cameraFollowObject = _cameraFollowGo.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangethresold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
       
    }


    void FixedUpdate()
    {
        
        // Check and perform turning if needed
        //TurnCheck();

     
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");


    }



    //------------------------Flipping---------------------------------------------------------
    //checking to fliping player object
    //checking to flipping player object
    private void TurnCheck()
    {
        if (xAxis > 0 && !IsFacingRight)
        {
            Turn();
        }
        else if (xAxis < 0 && IsFacingRight)
        {
            Turn();
        }
    }

    //player flip method

    private void Turn()
    {
        // Flip the character by rotating around the Y-axis
        Vector3 rotation = transform.eulerAngles;
        rotation.y = IsFacingRight ? 180f : 0f;
        transform.eulerAngles = rotation;

        IsFacingRight = !IsFacingRight;

        //turn the camera follow object
        _cameraFollowObject.CallTurn();
    }


    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-PlayerScale, transform.localScale.y);
            pState.lookingRight = false;

        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(PlayerScale, transform.localScale.y);
            pState.lookingRight = true;
        }
    }


    //------------------------Flipping---------------------------------------------------------

    #region OnClickedPerform

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        // Set the current movement input to the value of the input
        currentMovementInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Reset the current movement input when the input is canceled
        currentMovementInput = Vector2.zero;
    }

    #endregion


    //------------------------Movement---------------------------------------------------------
    //player movement
    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);

        // Update the animator with the walking state
        anim.SetBool("Walking", Mathf.Abs(moveInput) > 0);

    }



    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, WhatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, WhatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, WhatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //------------------------Movement---------------------------------------------------------


    //------------------------Jump---------------------------------------------------------




    //------------------------Jump---------------------------------------------------------

    //------------------------Dashing---------------------------------------------------------
    void StartDash()
    {
        // call dash 

        // Listen for the dash input
        if (Input.GetButtonDown("Dash") && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");

        // Store original drag
        float originalDrag = rb.drag;

        // Apply dash velocity
        rb.velocity = new Vector2(dashSpeed * (pState.lookingRight ? 1 : -1), 0);

        // Wait a bit before increasing drag to stop
        yield return new WaitForSeconds(dashTime * 0.8f); // Adjust as necessary

        // Increase drag to stop quickly
        rb.drag = 10;

        // Wait for the rest of the dash duration
        yield return new WaitForSeconds(dashTime * 0.2f); // Adjust as necessary

        // Reset drag and dashing state
        rb.drag = originalDrag;
        pState.dashing = false;

        // Wait for cooldown before allowing next dash
        yield return new WaitForSeconds(dashCoolDown - dashTime);
        canDash = true;
    }






    //------------------------Dashing---------------------------------------------------------





    //------------------------Save&Load---------------------------------------------------------
    //loading player data
    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }

    //saving player data
    public void SaveData(ref GameData data)
    {
        data.playerPosition = this.transform.position;
    }
    //------------------------Save&Load---------------------------------------------------------


}
*/