using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class PlayData : MonoBehaviour
{
    public static string fileName = "PlayInfo.dat";
    public List<Chapter> chapterInfo;

    public PlayData()
    {
        chapterInfo = new List<Chapter>();
    }
}