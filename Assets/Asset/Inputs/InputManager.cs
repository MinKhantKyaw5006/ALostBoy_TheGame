/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem

public class InputManager : MonoBehaviour

{
    public static InputManager instance;
    public static PlayerInput PlayerInput;

    public Vector2 MoveeInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool JumpInput { get; private set; }

    public bool JumpisPressed { get; private set; }
    public bool JumpWasRelreased { get; private set; }

    public bool PauseOpen { get; private set; }

    private PlayerControls _playerInputaction; 
    private InputAction _moveinputaction;///example input
    private InputAction _attackinputaction;///example input
    private InputAction _JumpInputaction;///example input
    private InputAction _pauseopenaction;// this pauseopen is only what he cares

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        PlayerInput = GetComponent<PlayerInput>();

        _moveinputaction = PlayerInput.actions["Move"];
        //and other action
        _pauseopenaction = PlayerInput.actions["PauseGame"]; // he said this is important

    }

    private void Update()
    {
        PauseOpen = _pauseopenaction.WasPerformedThisFrame();
    }


}

*/

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    private PlayerInput playerInput;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // Optional, to persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Attempt to find and assign PlayerInput automatically if not manually assigned.
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("InputManager: No PlayerInput component found in the scene.");
            }
            else
            {
                Debug.Log("InputManager: PlayerInput component found and assigned automatically.");
            }
        }
    }

    public PlayerInput PlayerInput
    {
        get { return playerInput; }
    }

    // Example method to switch action maps
    public void SwitchActionMap(string mapName)
    {
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(mapName);
            Debug.Log($"InputManager: Switched current action map to '{mapName}'.");
        }
        else
        {
            Debug.LogError("InputManager: Attempted to switch action map, but PlayerInput is null.");
        }
    }
}

