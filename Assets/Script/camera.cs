using UnityEngine;

public class camera : MonoBehaviour
{
    // Lưu trữ thông tin cần thiết của camera
    public Vector3 position;
    public Quaternion rotation;
    public float fieldOfView;

    // Hàm lưu trữ thông tin camera
    public void SaveCameraData(Camera camera)
    {
        position = camera.transform.position;
        rotation = camera.transform.rotation;
        fieldOfView = camera.fieldOfView;
    }

    // Hàm nạp lại thông tin camera
    public void LoadCameraData(Camera camera)
    {
        camera.transform.position = position;
        camera.transform.rotation = rotation;
        camera.fieldOfView = fieldOfView;
    }
}