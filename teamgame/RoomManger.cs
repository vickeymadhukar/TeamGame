using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
public class RoomManger : MonoBehaviourPunCallbacks
{
    public static RoomManger Instance;

    private void Awake()
    {
        if (Instance)//check onther room is exsit
        {
            Destroy(gameObject); 
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneloaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }


    void OnSceneloaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManger"), Vector3.zero, Quaternion.identity);
        }
    }
}
