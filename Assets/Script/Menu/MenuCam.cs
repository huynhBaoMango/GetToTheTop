using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using TMPro;

public class MenuCam : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public CinemachineVirtualCamera currentCamera;

    public void Start()
    {
        // Lấy username từ PlayerPrefs và hiển thị
        string username = PlayerPrefs.GetString("username", "Guest"); // "Guest" là giá trị mặc định nếu không có username
        usernameText.text = username;
        currentCamera.Priority++;
    }


    public void UpdateCamera(CinemachineVirtualCamera target)
    {
        currentCamera.Priority--;

        currentCamera = target;

        currentCamera.Priority++;
    }
}
