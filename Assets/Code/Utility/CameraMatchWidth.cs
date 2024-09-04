using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMatchWidth : MonoBehaviour
{

    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth = 10;

    Camera _camera;
    void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = 5.6f;
        // if(NearlyEqual(Screen.width / (float)Screen.height,0.5625f))
        // {
        //     _camera.orthographicSize = 5.6f;
        // }
        // else
        // {
        //     SetSize();
        // }
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void Update()
    {
//#if UNITY_EDITOR
//        SetSize();
//#endif
    }

    void SetSize()
    {
        float unitsPerPixel = sceneWidth / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        _camera.orthographicSize = desiredHalfHeight;
    }

    private bool NearlyEqual(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.01f;
    }
}