using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera firstPersonCamera;
    public Camera canvasCamera;


    private void Awake()
    {
        Instance = this;
    }

    // Call this function to disable FPS camera,
    // and enable overhead camera.
        public void ShowCanvasView()
    {
        firstPersonCamera.enabled = false;
        canvasCamera.enabled = true;
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void ShowFirstPersonView()
    {
        firstPersonCamera.enabled = true;
        canvasCamera.enabled = false;
    }
}
