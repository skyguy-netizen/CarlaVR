using UnityEngine;
// using UnityEngine.XRNode;
using UnityEngine.UI;
using NativeWebSocket;
using System;
using System.Collections;

public class CarlaVRController : MonoBehaviour
{
    private WebSocket websocket;
    public RawImage vrDisplay; // Assign via Inspector
    private Texture2D texture;

    private byte[] latestImageData;
    private bool hasNewImage = false;
    // InputDevice headDevice;
    public GameObject mainCamera;
    


    void Start()
    {
        // headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        texture = new Texture2D(2, 2);
        StartCoroutine(InitializeWebSocket());
    }

    IEnumerator InitializeWebSocket()
    {
        websocket = new WebSocket("ws://169.235.18.112:8765");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connected to CARLA WebSocket");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket Closed: " + e);
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            var data = JsonUtility.FromJson<CarlaFrameData>(message);
            if (data.type == "frame")
            {
                latestImageData = Convert.FromBase64String(data.data); //render frame
                hasNewImage = true;
            }
        };

        yield return websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif

        if (hasNewImage && latestImageData != null)
        {
            texture.LoadImage(latestImageData);
            vrDisplay.texture = texture;
            hasNewImage = false;
        }

        // if (headDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))
        // {
        Vector3 euler = mainCamera.transform.rotation.eulerAngles;
        Debug.Log("[VR] Rotation: " + euler);


        //apply corrections
        float pitch = -euler.x;
        float yaw = euler.y;
        float roll = -euler.z;

        var payload = JsonUtility.ToJson(new CarlaRotation(pitch, yaw, roll));

        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            websocket.SendText(payload);
        }
        // }
    }

    private void OnDestroy()
    {
        if (websocket != null)
            websocket.Close();
    }

    [System.Serializable]
    private class CarlaFrameData
    {
        public string type;
        public string data;
    }


    [System.Serializable]
    public class CarlaRotation
    {
        public float pitch;
        public float yaw;
        public float roll;

        public CarlaRotation(float pitch, float yaw, float roll)
        {
            this.pitch = pitch;
            this.yaw = yaw;
            this.roll = roll;
        }
    }
}
