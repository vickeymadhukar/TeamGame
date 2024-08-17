using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManger : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        // The GameManger will handle the player spawn
        gamemanger gameManger = FindObjectOfType<gamemanger>();
        if (gameManger != null)
        {
            gameManger.SpawnPlayer();
        }
        else
        {
            Debug.LogError("GameManger not found in the scene.");
        }
    }
}
