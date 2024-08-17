using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomname;
    [SerializeField] TMP_Text errortext;
    [SerializeField] TMP_Text roomtext;
    [SerializeField] Transform roomlistcontent;
    [SerializeField] GameObject roomlistiteamprefab;
    [SerializeField] Transform playerlistcontent;
    [SerializeField] GameObject playerlistiteamprefab;
    [SerializeField] GameObject startbutton;
    [SerializeField] TMP_Dropdown teamDropdown; // Dropdown for selecting team

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to master...");
        PhotonNetwork.ConnectUsingSettings();
        teamDropdown.onValueChanged.AddListener(delegate { OnTeamDropdownValueChanged(); });
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master.");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("We are in the lobby!");
        MenuManger.Instance.OpenMenu("title");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    public void Createroom()
    {
        if (string.IsNullOrEmpty(roomname.text))
        {
            return;
        }
        MenuManger.Instance.OpenMenu("loading");
        PhotonNetwork.CreateRoom(roomname.text);
        string selectedTeam = "A";
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", selectedTeam } });
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("You are the Master Client.");
        }
        else
        {
            Debug.Log("You are a regular client.");
        }
        MenuManger.Instance.OpenMenu("room");
        roomtext.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
        startbutton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startbutton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errortext.text = "Room creation failed: " + message;
        MenuManger.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManger.Instance.OpenMenu("loading");
    }

    public void Joinroom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManger.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManger.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomlistcontent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomlistiteamprefab, roomlistcontent).GetComponent<Roomlist>().Setup(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerlistiteamprefab, playerlistcontent).GetComponent<Playerlistiteam>().setup(newPlayer);
    }

    private void OnTeamDropdownValueChanged()
    {
       
        if (teamDropdown.options.Count > 0)
        {
            string selectedTeam = teamDropdown.options[teamDropdown.value].text;

            
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", selectedTeam } });
            Debug.Log($"Player assigned to Team {selectedTeam}. Team A: {GetTeamCount("A")}, Team B: {GetTeamCount("B")}");
        }
        else
        {
            Debug.LogWarning("No options available in the dropdown.");
        }
    }

    private void UpdatePlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform child in playerlistcontent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerlistiteamprefab, playerlistcontent).GetComponent<Playerlistiteam>().setup(players[i]);
        }
    }

    private int GetTeamCount(string teamName)
    {
        int count = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team") && player.CustomProperties["Team"].ToString() == teamName)
            {
                count++;
            }
        }
        return count;
    }
}
