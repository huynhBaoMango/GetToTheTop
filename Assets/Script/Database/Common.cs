
using IO.Swagger.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Common : MonoBehaviour
{

    public static Common instance;
    public void Awake() => instance = this;

    //private List<Skin> skins = new List<Skin>();

    //Ham mua skin
    public bool PurchaseSkin(User user, Skin skin)
    {
        
        if (user.ownedSkins.Contains(skin.skinId))
        {
            Debug.Log("User already owns this skin!");
            return false;
        }

        if (user.coin < skin.price)
        {
            Debug.Log("Not enough coins to purchase this skin!");
            return false;

        }

        user.coin -= skin.price;  //Giam coin cua User
        user.ownedSkins.Add(skin.skinId);  //Them skin da mua vao nguoi dung
        skin.isPurchased = true;

        // Cập nhật dữ liệu người dùng vào Database sau khi mua skin
        UpdateUserInDatabase(user);

        Debug.Log($"Skin {skin.skinName} purchased successfully!");
        return true;
    }



    //Ham chon skin
    public bool SelectSkin(User user, Skin skin)
    {
        if (!user.ownedSkins.Contains(skin.skinId))
        {
            Debug.Log("User doesn't own this skin");
            return false;
        }

        //Gan skin da chon cho user
        user.selectedSkin = skin.skinId;
        // Cập nhật dữ liệu người dùng
        UpdateUserInDatabase(user);


        Debug.Log($"{skin.skinName} selected successfully");
        return true;
    }






    // Hàm cập nhật thông tin người dùng trong Firebase
    public void UpdateUserInDatabase(User user)
    {
        string userId = PlayerPrefs.GetString("userId");
        string databaseUrl = "https://projectm-91ec6-default-rtdb.firebaseio.com/User/" + userId + ".json";
        string json = JsonUtility.ToJson(user);

        StartCoroutine(UpdateUserCoroutine(databaseUrl, json));
    }

    private IEnumerator UpdateUserCoroutine(string url, string json)
    {
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("User updated in database successfully.");
        }
        else
        {
            Debug.LogError("Failed to update user in database: " + request.error);
        }
    }

    // Hàm tải skins từ Firebase
    public void LoadSkinsFromDatabase(Action<List<Skin>> onSkinsLoaded)
    {
        string databaseUrl = "https://projectm-91ec6-default-rtdb.firebaseio.com/Skin.json"; // URL đến danh sách skins
        StartCoroutine(LoadSkinsCoroutine(databaseUrl, onSkinsLoaded));
    }

    // Coroutine để tải skins từ Firebase
    private IEnumerator LoadSkinsCoroutine(string url, Action<List<Skin>> onSkinsLoaded)
    {
        // Tạo yêu cầu GET đến Firebase
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Chuyển đổi JSON trả về thành danh sách Skin
            string json = request.downloadHandler.text;

            // Sử dụng Newtonsoft.Json để chuyển đổi
            var skinDict = JsonConvert.DeserializeObject<Dictionary<string, Skin>>(json);
            List<Skin> skins = new List<Skin>(skinDict.Values); // Lưu danh sách skin vào biến skins

            onSkinsLoaded?.Invoke(skins); // Gọi callback với danh sách skins đã tải
        }
        else
        {
            Debug.LogError("Failed to load skins: " + request.error);
            onSkinsLoaded?.Invoke(new List<Skin>()); // Gọi callback với danh sách rỗng nếu có lỗi
        }
    }
}
