using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;

public class PlayerWeapone : NetworkBehaviour
{
    [SerializeField] private List<APlayerWeapon> weapons = new List<APlayerWeapon>();
    [SerializeField] private APlayerWeapon currentWeapon; // Không cần SyncVar nữa
    [SerializeField] private Animator animator; // Thêm Animator

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
        if (Input.GetKey(KeyCode.Mouse0))
            FireWeapon();
    }

    private void FireWeapon()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Fire();

            // Kích hoạt animation bắn
            if (animator != null)
            {
                animator.SetTrigger("shot"); // Kích hoạt trigger cho animation bắn
                Debug.Log("Animation shot được kích hoạt");
            }
        }
        else
        {
            Debug.LogError("Không có vũ khí nào được trang bị!");
        }
    }
}
