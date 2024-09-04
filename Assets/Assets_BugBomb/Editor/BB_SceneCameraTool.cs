using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
enum ViewDirection { Top, Front, Right, Other }

[InitializeOnLoad]
public static class BB_SceneCameraTool
{
    static bool showDebug = true;

    static Transform selectedObject;
    static ViewDirection lastView;
    static Vector3 cameraPosition;
    static bool orthographic = false;
    static float zoomSpeed = 2;

    static BB_SceneCameraTool()
    {
        SceneView.duringSceneGui += view =>
        {
            var e = Event.current;

            if (e.type == EventType.KeyUp && e != null && e.keyCode != KeyCode.None)
            {
                //Debug.Log("ShortcutLisener  Key pressed in editor: " + e.keyCode);

                switch (e.keyCode)
                {
                    case KeyCode.Keypad1:
                        MoveSceneViewCamera_FRONT();
                        break;

                    case KeyCode.Keypad3:
                        MoveSceneViewCamera_RIGHT();
                        break;

                    case KeyCode.Keypad5:
                        ChangeProjection();
                        break;

                    case KeyCode.Keypad7:
                        MoveSceneViewCamera_TOP();
                        break;

                    case KeyCode.Keypad9:
                        GetCameraDistance(Selection.activeGameObject.transform);
                        lastView = ViewDirection.Other;
                        break;

                    case KeyCode.KeypadPlus:
                        Zoom(1f);
                        break;

                    case KeyCode.KeypadMinus:
                        Zoom(-1f);
                        break;
                }
            }
        };
    }

    public static float GetCameraDistance(Transform selected)
    {
        SceneView.lastActiveSceneView.FrameSelected();
        SceneView.lastActiveSceneView.Repaint();
        return 5;// SceneView.lastActiveSceneView.cameraDistance;
    }

    public static void MoveSceneViewCamera_TOP()
    {
        if (showDebug)
            Debug.Log("SceneCamera: TOP");

        lastView = ViewDirection.Top;
        selectedObject = Selection.activeGameObject.transform;


        // position
        cameraPosition = SceneView.lastActiveSceneView.pivot;
        cameraPosition.y = 20;
        cameraPosition.x = 0;
        cameraPosition.z = 0;

        // rotation
        SceneView.lastActiveSceneView.rotation = Quaternion.Euler(90, 0, 0);

        if (selectedObject != null)
        {
            cameraPosition.x = selectedObject.position.x;
            cameraPosition.y = GetCameraDistance(selectedObject);
            cameraPosition.z = selectedObject.position.z;
            //policzyć właściwą wysokość

            //SceneView.lastActiveSceneView.AlignViewToObject(Selection.activeGameObject.transform);// align view to object - works amloust like 'F'
            //SceneView.lastActiveSceneView.AlignWithView(); // Przesuwa i obraca obiekt tak żeby był w obecnej kamerze 
            //SceneView.lastActiveSceneView.LookAt(selectedObject.position);
        }

        SceneView.lastActiveSceneView.pivot = cameraPosition; // ustawia pivot kamery!


        // camera settings
        //SceneView.CameraSettings camSettings = new SceneView.CameraSettings();
        //camSettings...
        //SceneView.lastActiveSceneView.cameraSettings = camSettings;

        // position of the window in sceenSpace - moving scene window
        // SceneView.lastActiveSceneView.position = new Rect(0,0,1,1);

        SceneView.lastActiveSceneView.Repaint();
        orthographic = SceneView.lastActiveSceneView.orthographic;
        DebugSceneCamera();
    }

    static public void MoveSceneViewCamera_FRONT()
    {
        if (showDebug)
            Debug.Log("SceneCamera: FRONT");

        lastView = ViewDirection.Front;

        selectedObject = Selection.activeGameObject.transform;
        
        // position
        cameraPosition = SceneView.lastActiveSceneView.pivot;
        cameraPosition.y = 0;
        cameraPosition.x = 0;
        cameraPosition.z = 20;

        // rotation
        SceneView.lastActiveSceneView.rotation = Quaternion.Euler(0, 0, 0);

        if (selectedObject != null)
        {
            cameraPosition.x = selectedObject.position.x;
            cameraPosition.y = selectedObject.position.y;
            cameraPosition.z = GetCameraDistance(selectedObject);
        }

        SceneView.lastActiveSceneView.pivot = cameraPosition;// ustawia pivot kamery!
        SceneView.lastActiveSceneView.Repaint();
        orthographic = SceneView.lastActiveSceneView.orthographic;
        DebugSceneCamera();
    }

    static public void MoveSceneViewCamera_RIGHT()
    {
        if (showDebug)
            Debug.Log("SceneCamera: RIGHT");

        lastView = ViewDirection.Right;
        selectedObject = Selection.activeGameObject.transform;

        // position
        cameraPosition = SceneView.lastActiveSceneView.pivot;
        cameraPosition.x = 20;
        cameraPosition.y = 0;
        cameraPosition.z = 0;

        // rotation
        SceneView.lastActiveSceneView.rotation = Quaternion.Euler(0, -90, 0);

        if (selectedObject != null)
        {
            cameraPosition.x = GetCameraDistance(selectedObject);
            cameraPosition.y = selectedObject.position.y;
            cameraPosition.z = selectedObject.position.z;
        }

        SceneView.lastActiveSceneView.pivot = cameraPosition;
        SceneView.lastActiveSceneView.Repaint();
        orthographic = SceneView.lastActiveSceneView.orthographic;
        DebugSceneCamera();
    }



    public static void Zoom(float v)
    {
        Vector3 forward = SceneView.lastActiveSceneView.rotation * Vector3.forward;

        if (v > 0) // plus
        {
            if (!orthographic)
                SceneView.lastActiveSceneView.pivot = cameraPosition + (zoomSpeed * forward);
            else
                SceneView.lastActiveSceneView.size -= zoomSpeed;
        }
        else //minus
        {
            if (!orthographic)
                SceneView.lastActiveSceneView.pivot = cameraPosition - (zoomSpeed * forward);
            else
                SceneView.lastActiveSceneView.size += zoomSpeed;
        }

        SceneView.lastActiveSceneView.Repaint();

        cameraPosition = SceneView.lastActiveSceneView.pivot;
        DebugSceneCamera();
    }

    public static void ChangeProjection()
    {
        if (showDebug)
            Debug.Log("SceneCamera: ChangeProjection");

        SceneView.lastActiveSceneView.orthographic = !SceneView.lastActiveSceneView.orthographic;
        orthographic = SceneView.lastActiveSceneView.orthographic;
    }

    public static void DebugSceneCamera()
    {
        Debug.Log($"Camera pivot: {SceneView.lastActiveSceneView.pivot}   rotation: {SceneView.lastActiveSceneView.rotation}  camDist: {SceneView.lastActiveSceneView.cameraDistance}");
    }
}
#endif