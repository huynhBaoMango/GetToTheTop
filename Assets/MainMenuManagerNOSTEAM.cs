using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet;
using FishNet.Managing.Scened;

public class MainMenuManagerNOSTEAM : MonoBehaviour
{
    private NetworkManager _networkManager;
    [SerializeField] private TMP_InputField lobbyInput;
    [SerializeField] private TextMeshProUGUI lobbyTitle, lobbyIDText;
    [SerializeField] private GameObject pnLobby, pnJoin, btStart;

    private LocalConnectionState _clientState = LocalConnectionState.Stopped;
    private LocalConnectionState _serverState = LocalConnectionState.Stopped;

    private void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        if (_networkManager == null)
        {
            Debug.LogError("NetworkManager not found, HUD will not function.");
            return;
        }
        else
        {
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

    }

    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        _clientState = obj.ConnectionState;
        CloseAllPanels();
        pnLobby.SetActive(true);
        btStart.SetActive(InstanceFinder.IsServer);
        Debug.Log("CLIENT");
    }


    private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        _serverState = obj.ConnectionState;
        CloseAllPanels();
        pnLobby.SetActive(true);
        Debug.Log("SERVER");
    }


    public void OnClick_Server()
    {
        if (_networkManager == null)
            return;

        if (_serverState != LocalConnectionState.Stopped)
            _networkManager.ServerManager.StopConnection(true);
        else
        {
            _networkManager.ServerManager.StartConnection();
        }
        OnClick_Client();

    }

    public void OnClick_Client()
    {
        if (_networkManager == null)
            return;

        if (_clientState != LocalConnectionState.Stopped)
            _networkManager.ClientManager.StopConnection();
        else
            _networkManager.ClientManager.StartConnection();

    }

    private void CloseAllPanels()
    {
        pnLobby.SetActive(false);
        pnJoin.SetActive(false);
    }

    public void OnClickStartGame()
    {
        LoadScene("SampleScene");
        UnloadScene("Menu");
    }

    private void LoadScene(string sceneName)
    {
        if(!InstanceFinder.IsServer) return;

        SceneLoadData sld = new SceneLoadData(sceneName);
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    private void UnloadScene(string sceneName)
    {
        if (!InstanceFinder.IsServer) return;

        SceneUnloadData sld = new SceneUnloadData(sceneName);
        InstanceFinder.SceneManager.UnloadGlobalScenes(sld);
    }

    private bool IsHost => _networkManager.IsHost;
}
