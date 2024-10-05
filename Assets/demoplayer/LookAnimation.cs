using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class LookAnimation : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Rig>();
    }
}
