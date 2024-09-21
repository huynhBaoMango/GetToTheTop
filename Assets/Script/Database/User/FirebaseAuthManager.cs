using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class FirebaseAuthManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TextMeshProUGUI feedbackText;

    private string apiKey = "AIzaSyA7kIAGmNwyJIJsS9x8uOjln8wDZ_wyDLo";

    //Hàm Register người dùng
    public void RegisterUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        //Kiểm tra điều kiện đăng kí 
        if (password != confirmPassword) {
            feedbackText.text = "Password do not match!";
            return;
        }
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            feedbackText.text = "Please input both email and password";
            return;
        }
        StartCoroutine(RegisterNewUser(email, password));
    }

    IEnumerator RegisterNewUser(string email, string password)
    {
        string url = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + apiKey;
        string json = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            feedbackText.text = "User registered successfully!";
            var responseData = JsonUtility.FromJson<FirebaseAuthRespone>(request.downloadHandler.text);
            StartCoroutine(AddUserToDatabase(responseData.localId, email));  // Thêm vào Realtime Database
        }
        else
        {
            feedbackText.text = "Registration failed: " + request.error;
        }
    }

    // Thêm người dùng vào Realtime Database
    IEnumerator AddUserToDatabase(string userId, string email)
    {
        string databaseUrl = "https://projectm-91ec6-default-rtdb.firebaseio.com/User/" + userId + ".json";
        User newUser = new User(email, "default_username", "password");

        string json = JsonUtility.ToJson(newUser);
        UnityWebRequest request = new UnityWebRequest(databaseUrl, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("User added to database successfully.");
        }
        else
        {
            Debug.LogError("Failed to add user to database: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }




    // Hàm đăng nhập người dùng
    public void LoginUser()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email))
        {
            feedbackText.text = "Please input both email and password";
            return;
        }
        StartCoroutine(LoginExistingUser(email, password));
    }

    IEnumerator LoginExistingUser(string email, string password)
    {
        string url = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + apiKey;
        string json = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            feedbackText.text = "User logged in successfully!";
            var responseData = JsonUtility.FromJson<FirebaseAuthRespone>(request.downloadHandler.text);
            // Tiếp tục với thông tin người dùng sau khi đăng nhập thành công (vd: lưu token, localId...)
        }
        else
        {
            feedbackText.text = "Login failed: " + request.error;
        }
    }
}
