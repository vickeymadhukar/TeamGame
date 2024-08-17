using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
public class Roomlist : MonoBehaviour
{
    [SerializeField] TMP_Text text;
   public  RoomInfo info;

    public void Setup(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name;
    }

    // Update is called once per frame
   public void Onclick()
    {
        Launcher.Instance.Joinroom(info);
    }
}
