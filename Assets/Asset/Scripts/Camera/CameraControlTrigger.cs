using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects CustomInspectorObjects;

    private Collider2D _coll;

    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

            if(CustomInspectorObjects.swapCameras && CustomInspectorObjects.cameraOnLeft != null && CustomInspectorObjects.cameraOnRight != null)
            {
                //swap cameras
                CameraManager.instance.SwapCamera(CustomInspectorObjects.cameraOnLeft, CustomInspectorObjects.cameraOnRight, exitDirection);
            }

            if (CustomInspectorObjects.panCameraOnContact)
            {
                //pan the camera
                CameraManager.instance.PanCameraOnContact(CustomInspectorObjects.panDistance, CustomInspectorObjects.panTime, CustomInspectorObjects.panDirection,false);
            }

    
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CustomInspectorObjects.panCameraOnContact)
            {
                //pan the camera
                CameraManager.instance.PanCameraOnContact(CustomInspectorObjects.panDistance, CustomInspectorObjects.panTime, CustomInspectorObjects.panDirection, true);


           
            }
        }
    }
}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;
  

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;


    
 


}
public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}



#if UNITY_EDITOR

[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraControlTrigger.CustomInspectorObjects.swapCameras)
        {
            cameraControlTrigger.CustomInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraControlTrigger.CustomInspectorObjects.cameraOnLeft,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControlTrigger.CustomInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControlTrigger.CustomInspectorObjects.cameraOnRight,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

        }

        if (cameraControlTrigger.CustomInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.CustomInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                cameraControlTrigger.CustomInspectorObjects.panDirection);

            cameraControlTrigger.CustomInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.CustomInspectorObjects.panDistance);
            cameraControlTrigger.CustomInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.CustomInspectorObjects.panTime);

        }




        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}
#endif