
using IO.Swagger.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Common : MonoBehaviour
{

    private static Common instance;
    private void Awake() => instance = this;

    private List<Skin> skins = new List<Skin>();

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

        //Cap nhat du lieu nguoi dung vao Database sau khi mua skin
        Debug.Log($"Skin {skin.skinName} purchased successfully!");
        return true;
    }



    //Ham chon skin
    public static bool SelectSkin(User user, Skin skin)
    {
        if (!user.ownedSkins.Contains(skin.skinId))
        {
            Debug.Log("User doesn't own this skin");
            return false;
        }

        //Gan skin da chon cho user
        user.selectedSkin = skin.skinId;
        Debug.Log($"{skin.skinName} selected successfully");
        return true;
    }


    //Ham load danh sach skin tu file hoac truc tiep tu database




    // Hàm cập nhật thông tin người dùng trong Firebase
    //private void UpdateUserInDatabase(User user)
    //{
    //    string databaseUrl = "https://projectm-91ec6-default-rtdb.firebaseio.com/User/" + userId + ".json";
    //    string json = JsonUtility.ToJson(user);

    //    StartCoroutine(UpdateUserCoroutine(databaseUrl, json));
    //}

    //private IEnumerator UpdateUserCoroutine(string url, string json)
    //{
    //    UnityWebRequest request = new UnityWebRequest(url, "PUT");
    //    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
    //    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //    request.downloadHandler = new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/json");

    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log("User updated in database successfully.");
    //    }
    //    else
    //    {
    //        Debug.LogError("Failed to update user in database: " + request.error);
    //    }
    //}



    // Hàm lấy danh sách skin đã mua
    //public List<Skin> GetPurchasedSkins(User user)
    //{
    //    List<Skin> purchasedSkins = new List<Skin>();
    //    foreach (Skin skin in skins)
    //    {
    //        if (skin.isPurchased)
    //        {
    //            purchasedSkins.Add(skin);
    //        }
    //    }
    //    return purchasedSkins;
    //}
}
