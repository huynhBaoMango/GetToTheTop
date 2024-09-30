using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet;
using FishNet.Managing;
using FishNet.Connection;
using FishNet.Transporting;

public class MainMenuManagerNOSTEAM : MonoBehaviour
{
    private NetworkManager _networkManager;
    [SerializeField] private TMP_InputField lobbyInput;
    [SerializeField] private TextMeshProUGUI lobbyTitle, lobbyIDText;
    [SerializeField] private GameObject pnLobby, pnJoin, btStart;

    private void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        if (_networkManager == null)
        {
            Debug.LogError("NetworkManager not found, HUD will not function.");
            return;
        }

        _networkManager.ClientManager.OnClientConnectionState += OnLobbyEntered;
    }

    private void OnLobbyEntered(ClientConnectionStateArgs obj)
    {
        btStart.SetActive(IsHost);
    }

    private bool IsHost => _networkManager.IsHost;
}
