using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapone : MonoBehaviour
{
    [SerializeField] private List<APlayerWeapon> weapons = new List<APlayerWeapon>();

    public void InitializeWeapons(Transform parrentOfWeapons)
    {
        for (int i = 0; i < weapons.Count; i++)
        
            weapons[i].transform.SetParent(parrentOfWeapons);
        
    }

}
