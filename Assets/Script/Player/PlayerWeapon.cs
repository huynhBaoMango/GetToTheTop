using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;

public class PlayerWeapone : NetworkBehaviour
{
    [SerializeField] private List<APlayerWeapon> weapons = new List<APlayerWeapon>();
    [SerializeField] private APlayerWeapon currentWeapon; // Không cần SyncVar nữa

    private void Awake()
    {
        // Khởi tạo currentWeapon với vũ khí đầu tiên trong danh sách
        if (weapons.Count > 0)
        {
            currentWeapon = weapons[0];
            currentWeapon.gameObject.SetActive(true); // Kích hoạt vũ khí đầu tiên
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
            FireWeapon();
        
    }

    // Loại bỏ các hàm InitializeWeapons, InitializeWeapon, SetWeaponIndex, OncurrentWeaponIndexChanged

    private void FireWeapon()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Fire();
        }
        else
        {
            Debug.LogError("Không có vũ khí nào được trang bị!");
        }
    }
}