//using FishNet.Object;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerPickup : NetworkBehaviour
//{
//    [SerializeField] private float pickupRanger = 4f;
//    [SerializeField] private KeyCode pickupkey = KeyCode.E;
//    [SerializeField] private LayerMask pickupLayers;
//    private Transform _cameraTranform;

//    private PlayerWeapone _playerWeapone;
//    public override void OnStartClient()
//    {
//        base.OnStartClient();

//        if (!IsOwner)
//        {
//            enabled = false;
//            return;
//        }
//        _cameraTranform = Camera.main.transform;
//        if(TryGetComponent(out PlayerWeapone pWeapon))
//            _playerWeapone = pWeapon;
//        else
//            Debug.LogError("khong tim thay playerwearpon",gameObject);
//    }
//    private void Update()
//    {
//        if (Input.GetKeyUp(pickupkey))
//        {
//            Pickup();
//        }
//    }
//    //private void Pickup()
//    //{
//    //    if (!Physics.Raycast(_cameraTranform.position, _cameraTranform.forward, out RaycastHit hit, pickupRanger, pickupLayers))
//    //        return;
//    //    if(hit.transform.TryGetComponent(out GroundWeapon groundWeapon))
//    //    {
//    //        _playerWeapone.InitializeWeapon(groundWeapon.PickupWeapon());
//    //    }
//    //}
//}
