using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Component.Animating;

public abstract class APlayerWeapon : NetworkBehaviour
{
    public int damage;
    public float MaxRange = 20f;
    public LayerMask WeaponHitLayers;
    private Transform _cameraTransform;
    private Animator animator;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform; // L?u tr? Camera transform
    }

    public void Fire()
    {
        AnimateWeapon();

        // Ki?m tra va ch?m raycast
        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, MaxRange, WeaponHitLayers))
            return;

        if (hit.transform.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamege(damage);
          
        }
    }
      


    public abstract void AnimateWeapon();
}
