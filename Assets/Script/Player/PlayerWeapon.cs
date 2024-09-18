using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEditor;
public class PlayerWeapone : NetworkBehaviour
{
    [SerializeField] private List<APlayerWeapon> weapons = new List<APlayerWeapon>();
    [SerializeField] private APlayerWeapon currentWeapon;

    private readonly SyncVar<int> _currentWeaponIndex = new(-1);
    private void Awake()
    {
        _currentWeaponIndex.OnChange += OncurrentWeaponIndexChanged;
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

    public void InitializeWeapons(Transform parrentOfWeapons)
    {
        for (int i = 0; i < weapons.Count; i++)

            weapons[i].transform.SetParent(parrentOfWeapons);

        InitializeWeapon(0);
    }
    public void InitializeWeapon(int weaponIndex)
    {
        SetWeaponIndex(weaponIndex);
    }
    [ServerRpc] private void SetWeaponIndex(int weaponIndex) => _currentWeaponIndex.Value = weaponIndex;
    private void OncurrentWeaponIndexChanged(int oldIndex, int NewIndex, bool assServer)
    {
        for (int i = 0; i < weapons.Count; i++)
            weapons[i].gameObject.SetActive(false);

        if (weapons.Count > NewIndex)
        {
            currentWeapon = weapons[NewIndex];
            currentWeapon.gameObject.SetActive(true);
        }
    }

    private void FireWeapon()
    {
        currentWeapon.Fire();
    }

}