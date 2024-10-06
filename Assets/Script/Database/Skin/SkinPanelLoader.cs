using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinPanelLoader : MonoBehaviour
{
    public GameObject skinPrefab;
    public Transform skinPanel;
    public List<Skin> skins;

    public void LoadSkinToPanel()
    {
        foreach (var skin in skins)
        {
            GameObject skinItem = Instantiate(skinPrefab, skinPanel);

            //Set tên skin
            skinItem.transform.Find("skinNameText").GetComponent<TextMeshProUGUI>().text = skin.skinName;

            //Set hình ảnh skin
            Image skinImage = skinItem.transform.Find("imgSkin").GetComponent<Image>();
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
        Debug.Log("Buy skin: " + skin.skinName);

        User currentUser = GetCurrentUser(); // Phương thức này sẽ lấy thông tin người dùng hiện tại
        if (currentUser != null)
        {
            // Gọi hàm PurchaseSkin từ class Common
            if (Common.instance.PurchaseSkin(currentUser, skin))
            {
                Debug.Log($"Skin {skin.skinName} purchased successfully!");
            }
            else
            {
                Debug.Log("Failed to purchase skin.");
            }
        }
        else
        {
            Debug.Log("Current user not found.");
        }
    }

    private User GetCurrentUser()
    {
        // Lấy dữ liệu từ PlayerPrefs
        string localId = PlayerPrefs.GetString("localId");
        string idToken = PlayerPrefs.GetString("idToken");
        string username = PlayerPrefs.GetString("username");
        string email = PlayerPrefs.GetString("email");

        // Kiểm tra xem người dùng có tồn tại không
        if (string.IsNullOrEmpty(localId) || string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(username))
        {
            Debug.Log("No current user found.");
            return null;
        }

        // Trả về đối tượng User với thông tin đã lấy
        User currentUser = new User(email, username, "password", 1000) // Điền thông tin email và password tạm thời
        {
            selectedSkin = PlayerPrefs.GetString("selectedSkin"), // Nếu bạn lưu selectedSkin
            ownedSkins = JsonUtility.FromJson<List<string>>(PlayerPrefs.GetString("ownedSkins")) // Nếu bạn lưu danh sách skin
        };

        return currentUser;
    }
}
