using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SkinPanelLoader : MonoBehaviour
{
    public GameObject skinPrefab;
    public Transform skinPanel;
    public List<Skin> skins;

    private void Start()
    {
        Debug.Log("Skin Prefab: " + skinPrefab);
        Debug.Log("Skin Panel: " + skinPanel);
        Debug.Log("Skins Count: " + (skins != null ? skins.Count.ToString() : "null"));
        if (skins == null || skins.Count == 0)
        {
            Debug.LogError("Skins list is null or empty!");
            return; // Không làm gì nếu danh sách skins rỗng
        }

        //Common.instance.LoadSkinsFromDatabase(OnSkinsLoaded);
        LoadSkinToPanel();
    }

    //Hàm gọi khi skins đã được tải thành công
    //private void OnSkinsLoaded(List<Skin> loadedSkins)
    //{
    //    skins = loadedSkins; // Lưu danh sách skins đã tải vào biến skins
    //    Debug.Log("Skins loaded: " + skins.Count);
    //    LoadSkinToPanel();   // Hiển thị skins trên panel
    //}


    public void LoadSkinToPanel()
    {
        Debug.Log("Loading skins to panel...");
        if (skins == null || skins.Count == 0)
        {
            Debug.LogError("Skins list is null or empty!");
            return; // Không làm gì nếu danh sách skins rỗng
        }

        foreach (var skin in skins)
        {
            GameObject skinItem = Instantiate(skinPrefab, skinPanel);
            Debug.Log("skinItem created: " + skinItem.name);
            if (skinItem == null)
            {
                Debug.LogError("Failed to instantiate skinItem!");
                continue;
            }

            //Set tên skin
            //skinItem.transform.Find("skinNameText").GetComponent<TextMeshPro>().text = skin.skinName;
            TextMeshPro skinNameText = skinItem.GetComponentInChildren<TextMeshPro>();
            Debug.Log("skinNameText found: " + skinNameText);

            if (skinNameText == null)
            {
                Debug.LogError("skinNameText not found in skinItem!");
            }
            else
            {
                skinNameText.text = skin.skinName;
            }

            //Set hình ảnh skin
            Image skinImage = skinItem.GetComponentInChildren<Image>();
            Debug.Log("skinImage found: " + skinImage);
            Sprite skinSprite = Resources.Load<Sprite>(skin.assetPath); //Load từ assetPath

            if (skinSprite != null)
            {
                skinImage.sprite = skinSprite;
            }
            else
            {
                Debug.Log("Skin image not found at: " + skin.assetPath);
            }

            //Nút mua skin
            Button btnBuy = skinItem.GetComponent<Button>();
            btnBuy.onClick.AddListener(() => OnBuySkinButton(skin));
        }
    }

    //Hàm xử lí khi click Buy
    public void OnBuySkinButton(Skin skin)
    {
        StartCoroutine(GetCurrentUserAndPurchase(skin));
    }

    // Coroutine để lấy thông tin người dùng từ Firebase
    private IEnumerator GetCurrentUser()
    {
        string userId = PlayerPrefs.GetString("userId"); // Lấy tên người dùng từ PlayerPrefs
        string databaseUrl = "https://projectm-91ec6-default-rtdb.firebaseio.com/User/" + userId + ".json";

        UnityWebRequest request = UnityWebRequest.Get(databaseUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Chuyển đổi JSON trả về thành đối tượng User
            User currentUser = JsonUtility.FromJson<User>(request.downloadHandler.text);
            yield return currentUser; // Trả về người dùng
        }
        else
        {
            Debug.LogError("Failed to retrieve user data: " + request.error);
            yield return null; // Nếu có lỗi, trả về null
        }
    }

    // Coroutine để mua skin
    private IEnumerator PurchaseSkinCoroutine(User user, Skin skin)
    {
        if (Common.instance.PurchaseSkin(user, skin))
        {
            // Cập nhật thông tin người dùng vào Firebase sau khi mua skin thành công
            Common.instance.UpdateUserInDatabase(user);

            Debug.Log($"{skin.skinName} purchased successfully!");
        }
        else
        {
            // Nếu không mua được skin, có thể hiển thị thông báo cho người dùng
            Debug.Log($"Failed to purchase {skin.skinName}. Check the console for details.");
        }

        yield return null; // Kết thúc coroutine
    }

    // Coroutine để lấy thông tin người dùng và thực hiện mua skin
    private IEnumerator GetCurrentUserAndPurchase(Skin skin)
    {
        User currentUser = null;

        // Gọi coroutine để lấy thông tin người dùng
        yield return StartCoroutine(GetCurrentUser());

        // Kiểm tra xem thông tin người dùng có hợp lệ không
        if (currentUser != null)
        {
            // Gọi coroutine để mua skin
            yield return StartCoroutine(PurchaseSkinCoroutine(currentUser, skin));
        }
        else
        {
            Debug.Log("User data is invalid. Cannot purchase skin.");
        }
    }


}