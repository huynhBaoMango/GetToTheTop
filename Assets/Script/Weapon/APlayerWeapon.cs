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
    public float firerate = 0.5f;
    public LayerMask WeaponHitLayers;
    private Transform _cameraTransform;
    [SerializeField] private ParticleSystem muzzleFlash;
    private Animator animator;
    private float _lastfiretime;
    private NetworkAnimator _networldAnimator;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform; // L?u tr? Camera transform
        if(TryGetComponent(out NetworkAnimator Netanim))
            _networldAnimator = Netanim;
    }

    public void Fire()
    {
        if(Time.time > _lastfiretime+firerate)
            return;
        _lastfiretime = Time.time;
        AnimateWeapon();

        // Ki?m tra va ch?m raycast
        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, MaxRange, WeaponHitLayers))
            return;

        if (hit.transform.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamege(damage);
          
        }
    }
    

    [ServerRpc]
    public virtual void AnimateWeapon()
    {
        PlayAnimationObserver();
    }
    [ObserversRpc]
    public void PlayAnimationObserver()
    {
        Debug.Log("Chơi Animation bắn");
        if (_networldAnimator != null)
        {
            _networldAnimator.SetTrigger("fire");
        }
        else
        {
            Debug.LogError("NetworkAnimator chưa được gán!");
        }
    }


}
