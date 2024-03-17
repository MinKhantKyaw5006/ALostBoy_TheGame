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


            if (CustomInspectorObjects.zoomCameraOnContact)
            {
                // Determine whether to zoom in based on the ZoomDirection
                bool zoomIn = CustomInspectorObjects.zoomDirection == ZoomDirection.In;
                CameraManager.instance.ZoomCamera(CustomInspectorObjects.zoomAmount, CustomInspectorObjects.zoomTime, zoomIn);
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

            if (CustomInspectorObjects.zoomCameraOnContact)
            {
                CameraManager.instance.ResetZoom(CustomInspectorObjects.zoomTime);
            }
        }
    }
}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;


    // Add Zoom fields
    public bool zoomCameraOnContact = false;



    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;

    [HideInInspector] public ZoomDirection zoomDirection;
     public float zoomAmount = 15f; // Or any default value you like
    [HideInInspector] public float zoomTime = 0.5f;






}
public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}

public enum ZoomDirection
{
    In,
    Out
}





#if UNITY_EDITOR

[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;
    SerializedProperty zoomAmount; // Make sure you have this SerializedProperty for zoomAmount

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
        zoomAmount = serializedObject.FindProperty("CustomInspectorObjects.zoomAmount"); // Correctly reference zoomAmount
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

        // Conditionally show zoom settings
        if (cameraControlTrigger.CustomInspectorObjects.zoomCameraOnContact)
        {
            cameraControlTrigger.CustomInspectorObjects.zoomDirection = (ZoomDirection)EditorGUILayout.EnumPopup("Zoom Direction", 
                cameraControlTrigger.CustomInspectorObjects.zoomDirection);

            //cameraControlTrigger.CustomInspectorObjects.panDistance = EditorGUILayout.FloatField("Zoom Amount", cameraControlTrigger.CustomInspectorObjects.zoomAmount);
            //cameraControlTrigger.CustomInspectorObjects.zoomTime = EditorGUILayout.FloatField("Zoom Time", cameraControlTrigger.CustomInspectorObjects.zoomTime);
            // Fix here: Change to directly edit the zoomAmount instead of panDistance
            EditorGUILayout.PropertyField(zoomAmount, new GUIContent("Zoom Amount"));
            cameraControlTrigger.CustomInspectorObjects.zoomTime = EditorGUILayout.FloatField("Zoom Time", cameraControlTrigger.CustomInspectorObjects.zoomTime);

        }




        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}
#endif

