using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using System.Globalization;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealt = 100;
    private int _currentHealth;
    public void Awake()
    {
        _currentHealth = maxHealt;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!IsOwner)
        {
            enabled = false;
            return;
        }
        
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamege(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"New player health: {_currentHealth}");

        if (_currentHealth <= 0)
            Die();
    }
    private void Die()
    {
        Debug.Log("palyer dead");
    }
}
