using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class GroundWeapon : NetworkBehaviour
{
    public int weaponIndex  = -1;
    public int PickupWeapon()
    {
        DespawnWeapon();
        return weaponIndex;
    }
    [ServerRpc(RequireOwnership = false)]
    private void DespawnWeapon()
    {
        ServerManager.Despawn(gameObject);
    }
}
