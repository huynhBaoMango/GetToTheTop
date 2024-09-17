using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public abstract class APlayerWeapon : NetworkBehaviour 
{
    public abstract void Fire();
}
