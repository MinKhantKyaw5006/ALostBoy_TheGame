using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] _allvirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    [SerializeField] private bool isPanning = false;
    private Coroutine _zoomCoroutine; // Coroutine variable for managing zoom
    private bool isZooming = false; // Flag to check if zooming is in progress
    [SerializeField] private float _flipScreenTime = 0.5f; // Duration to complete the transition
    private PlayerController _player;

    [SerializeField] private float screenXForRightFacing = 0.55f;
    [SerializeField] private float screenXForLeftFacing = 0.45f;



    // Add a public variable for zoom adjustment

    [SerializeField]private float originalFOV;
    public float OriginalFOV { get => originalFOV; }




    public float defaultZoomAmount = 5f; // This can be adjusted in the Inspector
    public float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;

    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;

    private float _normYPanAmount;
    private Vector2 _startingTrackedObjectOffset;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        for(int i = 0; i < _allvirtualCameras.Length; i++)
        {
            if (_allvirtualCameras[i].enabled)
            {
                //set the current active camera
                _currentCamera = _allvirtualCameras[i];

                //set the framing transposer
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                _normYPanAmount = _framingTransposer.m_YDamping;
                
                break;
            }
        }

        //set the YDamping amount so it's based on the inspector value
        //_normYPanAmount = _framingTransposer.m_YDamping;

        //_startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;

        // Assume you initialize _currentCamera somewhere here
        // Subscribe to the player's flip event



    }



    #region Lerp the Y Damping
    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
        //Debug.Log("Player falling");
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        //grab the starting damping amount
        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;
        
        //determind the end damping amount
        if(isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }

        else
        {
            endDampAmount = _normYPanAmount;
        }

        //lerp the pan amount
        float elapsedTime = 0f;
        while(elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }
    #endregion

    #region Pan Camera

    /*
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStringPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStringPos));
    }
    */

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStringPos)
    {
        if (_panCameraCoroutine != null)
        {
            StopCoroutine(_panCameraCoroutine); // Ensure any existing panning coroutine is stopped before starting a new one
        }
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStringPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStringPos)
    {
        isPanning = true; // Indicate that panning is in progress
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = _framingTransposer.m_TrackedObjectOffset; // Use the current offset as the starting position

        // Calculate the end position based on the pan direction and distance
        switch (panDirection)
        {
            case PanDirection.Up:
                endPos = Vector2.up * panDistance;
                break;
            case PanDirection.Down:
                endPos = Vector2.down * panDistance;
                break;
            case PanDirection.Left:
                endPos = Vector2.left * panDistance;
                break;
            case PanDirection.Right:
                endPos = Vector2.right * panDistance;
                break;
        }

        if (!panToStringPos)
        {
            endPos += startingPos; // Only modify endPos if not panning to string position
        }
        else
        {
            endPos = _startingTrackedObjectOffset; // If panning to string position, use the original stored offset
        }

        // Perform the actual panning over time
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, elapsedTime / panTime);
            _framingTransposer.m_TrackedObjectOffset = panLerp; // Apply the interpolated offset to the camera

            yield return null;
        }

        if (panToStringPos)
        {
            // If returning to starting position, explicitly set the offset to ensure it's accurate
            _framingTransposer.m_TrackedObjectOffset = _startingTrackedObjectOffset;
        }
        isPanning = false; // Panning is complete
    }


    /*
    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStringPos)
    {
        //isPanning = true;
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        //handle pan from trigger
        if (!panToStringPos)
        {
            //set the direction and distance
            switch (panDirection) 
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;

                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;

                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;

                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;


            }

            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }
        else
        {
            //handle the pan back to starting position
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        //handle the actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            _framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    

        

    }
    */

    #endregion

    #region
    /*
    public void ZoomCamera(float amount, float duration, bool zoomIn)
    {
        float targetFOV = zoomIn ? originalFOV - amount : originalFOV + amount;
        StartCoroutine(AdjustFOV(targetFOV, duration));
    }


    public void ResetZoom(float duration)
    {
        StartCoroutine(AdjustFOV(originalFOV, duration));
    }

    private IEnumerator AdjustFOV(float targetFOV, float duration)
    {
        float startFOV = _currentCamera.m_Lens.FieldOfView;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            _currentCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            yield return null;
        }
    }
    */
    public void ZoomCamera(float amount, float duration, bool zoomIn)
    {
        if (_zoomCoroutine != null)
        {
            StopCoroutine(_zoomCoroutine); // Stop any ongoing zoom coroutine
        }
        float targetFOV = zoomIn ? originalFOV - amount : originalFOV + amount;
        _zoomCoroutine = StartCoroutine(AdjustFOV(targetFOV, duration));
    }

    public void ResetZoom(float duration)
    {
        if (_zoomCoroutine != null)
        {
            StopCoroutine(_zoomCoroutine); // Stop any ongoing zoom coroutine
        }
        _zoomCoroutine = StartCoroutine(AdjustFOV(originalFOV, duration));
    }

    private IEnumerator AdjustFOV(float targetFOV, float duration)
    {
        isZooming = true; // Indicate that zooming is in progress
        float startFOV = _currentCamera.m_Lens.FieldOfView;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            _currentCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            yield return null;
        }

        isZooming = false; // Zooming is complete
    }

    public void StartPanAndZoom(float panDistance, float panTime, PanDirection panDirection, bool panToStringPos, float zoomAmount, float zoomDuration, bool zoomIn)
    {
        // Stop previous coroutines if they are running
        if (_panCameraCoroutine != null) StopCoroutine(_panCameraCoroutine);
        if (_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);

        // Start panning and zooming simultaneously
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStringPos));
        _zoomCoroutine = StartCoroutine(AdjustFOV(zoomIn ? originalFOV - zoomAmount : originalFOV + zoomAmount, zoomDuration));
    }




    #endregion


    #region Swap Cameras
    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        //if the current camera is the camera on the left and our trigger exit direction was on the right
        if(_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            //activate the new camera
            cameraFromRight.enabled = true;

            //deactivate the old camera
            cameraFromLeft.enabled = false;

            //set the new camera as the current camera
            _currentCamera = cameraFromRight;

            //update our composer variable
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        }

        //if the current camera is the camera on the right and our trigger hit direction was on the left
        else if(_currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            //activate the new camera
            cameraFromLeft.enabled = true;

            //deactivate the old camera
            cameraFromRight.enabled = false;

            //set the new camera as the current camera
            _currentCamera = cameraFromLeft;

            //update our composer variable
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }
    #endregion


}
