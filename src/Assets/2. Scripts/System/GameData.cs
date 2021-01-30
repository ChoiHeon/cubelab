using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [System.Serializable]
    public class Stage
    {
        public Time clearTime;
        public int score;
        public int starts;
    }

    [System.Serializable]
    public class Chapter
    {
        public int stageCount;
        public List<Stage> stageInfo;

        public Chapter()
        {
            stageInfo = new List<Stage>();
        }
    }
}
