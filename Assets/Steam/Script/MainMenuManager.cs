using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Steamworks;
using System;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;
    private void Awake() => instance = this;

    [SerializeField] private TMP_InputField lobbyInput;
    [SerializeField] private TextMeshProUGUI lobbyTitle, lobbyIDText;
    [SerializeField] private GameObject pnLobby, pnJoin;

    public static void LobbyEntered(string lobbyName, bool isHost)
    {
        instance.lobbyTitle.text = lobbyName;
        instance.lobbyIDText.text = BoostrapManager.CurrentLobbyID.ToString();
        instance.NewGameButton();
    }

    public void NewGameButton()
    {
        CloseAllPanels();
        pnLobby.SetActive(true);
    }


    public void CreateLobby()
    {
        
        BoostrapManager.CreateLobby();
    }

    public void JoinLobbyButton()
    {
        CloseAllPanels();
        pnJoin.SetActive(true);
    }

    public void JoinLobby()
    {
        CSteamID steamID = new CSteamID(Convert.ToUInt64(lobbyInput.text));
        BoostrapManager.JoinByID(steamID);
    }

    public void LeaveLobby()
    {
        BoostrapManager.LeaveLobby();
    }

    public void CloseAllPanels()
    {
        pnJoin.SetActive(false);
        pnLobby.SetActive(false);
    }
    
}
