using Photon.Pun;
using UnityEngine;

public class gamemanger : MonoBehaviourPunCallbacks
{
    public Transform[] teamASpawnPoints; // Array of spawn points for Team A
    public Transform[] teamBSpawnPoints; // Array of spawn points for Team B
    public GameObject playerPrefab;      // Player prefab to instantiate

    private void Start()
    {
       
    }

  public  void SpawnPlayer()
    {

        string team = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();
        Transform spawnPoint;

        Debug.Log("Player Team: " + team);


        if (team == "A")
        {
            spawnPoint = teamASpawnPoints[Random.Range(0, teamASpawnPoints.Length)];
            // spawnPoint = teamASpawnPoints[0];  // Manually select the first spawn point for Team A

            Debug.Log("Spawning at Team A point: " + spawnPoint.name);
        }
        else if(team == "B")
        {
            spawnPoint = teamBSpawnPoints[Random.Range(0, teamBSpawnPoints.Length)];
                   
        }
        else
        {
            spawnPoint = teamASpawnPoints[Random.Range(0, teamASpawnPoints.Length)];
        }


        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
