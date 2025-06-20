using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

[RequireComponent(typeof(Camera))]
public class CameraCapture : MonoBehaviour {
    [SerializeField, Required] Camera _targetCamera;
    [SerializeField] int _width = 1920;
    [SerializeField] int _height = 1080;
    [SerializeField] string _saveFolder = "Screenshots"; // Relative to project root or persistentDataPath
    [SerializeField] string _fileName = "";

    private void Reset() {
        _targetCamera = GetComponent<Camera>();
    }

    [Button]
    public void CaptureCameraView() {
        Assert.IsNotNull(_targetCamera);
        // Combine with persistentDataPath for a safe location
        //this is for game-data-path
        //string folderPath = Path.Combine(Application.persistentDataPath, _saveFolder);
        string folderPath = Path.Combine(Application.dataPath, _saveFolder);

        // Create the folder if it doesn't exist
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"{_fileName}.png");

        //RenderTexture rt = new RenderTexture(_width, _height, 24);
        RenderTexture rt = new RenderTexture(_width, _height, 24, RenderTextureFormat.ARGB32);
        _targetCamera.targetTexture = rt;
        //Texture2D screenShot = new Texture2D(_width, _height, TextureFormat.RGB24, false);
        Texture2D screenShot = new Texture2D(_width, _height, TextureFormat.RGBA32, false);

        _targetCamera.clearFlags = CameraClearFlags.SolidColor;
        _targetCamera.backgroundColor = Color.clear;

        _targetCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
        screenShot.Apply();

        _targetCamera.targetTexture = null;
        RenderTexture.active = null;
        rt.Release();
        if (Application.isPlaying) {
            Destroy(rt);
        } else {
            DestroyImmediate(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.LogWarning($"Camera view saved to: {filePath}");
    }
}