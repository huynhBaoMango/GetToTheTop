using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private InputM inputmanager;

    private void Update()
    {
        Vector3 mousePosition = inputmanager.GetSelectedMapPosition();
        mouseIndicator.transform.position = mousePosition;
    }
}
