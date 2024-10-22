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
        if (skinPrefab == null)
        {
            Debug.LogError("skinPrefab is not assigned!");
            return;
        }

        if (skinPanel == null)
        {
            Debug.LogError("skinPanel is not assigned!");
            return;
        }

        //if (skins == null || skins.Count == 0)
        //{
        //    Debug.LogError("Skins list is null or empty!");
        //    return;
        //}
        
        Common.instance.LoadSkinsFromDatabase(OnSkinsLoaded);

        Debug.Log("Skin Prefab: " + skinPrefab);
        Debug.Log("Skin Panel: " + skinPanel);
        Debug.Log("Skins Count: " + (skins != null ? skins.Count.ToString() : "null"));

        
        //LoadSkinToPanel();
    }

    //Hàm gọi khi skins đã được tải thành công
    private void OnSkinsLoaded(List<Skin> loadedSkins)
    {
        skins = loadedSkins; // Lưu danh sách skins đã tải vào biến skins
        Debug.Log("Skins loaded: " + skins.Count);
        LoadSkinToPanel();   // Hiển thị skins trên panel
    }


    public void LoadSkinToPanel()
    {
        foreach (Transform child in skinPanel)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("Loading skins to panel...");

        if (skins == null || skins.Count == 0)
        {
            Debug.LogError("Skins list is null or empty!");
            return; // Không làm gì nếu danh sách skins rỗng
        }

        foreach (var skin in skins)
        {
            Debug.Log($"Skin Name: {skin.skinName}, Asset Path: {skin.assetPath}");

            GameObject SkinItemPrefab = Instantiate(skinPrefab, skinPanel);
            Debug.Log("skinItem created: " + SkinItemPrefab.name);

            if (SkinItemPrefab == null)
            {
                Debug.LogError("Failed to instantiate skinItem!");
                continue;
            }

            //Set tên skin
            Transform nameTextTransform = SkinItemPrefab.transform.Find("skinNameText");
            if (nameTextTransform != null)
            {
                TextMeshProUGUI skinNameText = nameTextTransform.GetComponent<TextMeshProUGUI>();
                skinNameText.text = skin.skinName;
            }
            else
            {
                Debug.LogError("Could not find skinNameText in skinItem.");
            }

            Transform priceTextTransform = SkinItemPrefab.transform.Find("skinPriceText");
            if (priceTextTransform != null)
            {
                TextMeshProUGUI skinPriceText = priceTextTransform.GetComponent<TextMeshProUGUI>();
                skinPriceText.text = "$" + skin.price;
            }

            //Set hình ảnh skin
            Transform modelTransform = SkinItemPrefab.transform.Find("skinModel");
            if (modelTransform != null)
            {
                Debug.Log("modelTransform is not null");
                GameObject skinModel = Resources.Load<GameObject>(skin.assetPath); // Load từ assetPath
                Debug.Log("skinModel: " + skinModel);
                if (skinModel != null)
                {
                    // Tạo model trong panel
                    GameObject instantiatedModel = Instantiate(skinModel, modelTransform);
                    instantiatedModel.transform.localPosition = Vector3.zero; // Đặt vị trí cho model
                }
                else
                {
                    Debug.LogError("3D model not found at: " + skin.assetPath);
                }
            }
            else
            {
                Debug.LogError("Could not find skinModel in skinItem.");
            }

            Transform imageTransform = SkinItemPrefab.transform.Find("skinImage");
            if (imageTransform != null)
            {
                Debug.Log("imageTransform is not null");
                Image skinImage = imageTransform.GetComponent<Image>();
                Debug.Log("skinImage: " + skinImage);
                Sprite skinSprite = Resources.Load<Sprite>(skin.imagePath); // Load từ assetPath
                Debug.Log("skinSprite: " + skinSprite);
                if (skinSprite != null)
                {
                    skinImage.sprite = skinSprite;
                }
                else
                {
                    Debug.LogError("Skin image not found at: " + skin.imagePath);
                }
            }
            else
            {
                Debug.LogError("Could not find imgSkin in skinItem.");
            }

            //Nút mua skin
            Transform btnBuyTransform = SkinItemPrefab.transform.Find("btnBuySkin");
            if (btnBuyTransform != null)
            {
                Button btnBuy = btnBuyTransform.GetComponent<Button>();
                btnBuy.onClick.AddListener(() => OnBuySkinButton(skin));
            }
            else
            {
                Debug.LogError("Could not find BtnBuy in skinItem.");
            }
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