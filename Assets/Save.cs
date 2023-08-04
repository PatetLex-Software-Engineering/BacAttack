using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class Save {
    public static string save;

    public int seed;
    public Player player;
    public List<Entity> entities;
    public List<Wave> tiles;

    public void Serialize() {
        Directory.CreateDirectory(Application.dataPath + "/saves/");
        File.WriteAllText(Application.dataPath + "/saves/" + save + ".bac", JsonUtility.ToJson(this));
    }

    public static Save Load() {
        if (save == null || save == "") {
            return null;
        }
        if (File.Exists(Application.dataPath + "/saves/" + save + ".bac")) {
            string json = File.ReadAllText(Application.dataPath + "/saves/" + save + ".bac");
            return JsonUtility.FromJson<Save>(json);
        }
        return null;
    }

    [Serializable]
    public class Player {
        public Vector3 position;
        public List<Statistics.Regenerable> regenerables;
        public List<Statistics.Statistic> statistics;
    }

    [Serializable]
    public class Entity {
        public string name;
        public Vector3 position;
        public List<Statistics.Statistic> statistics;
    }

    [Serializable]
    public class Wave {
        public string name;
        public Vector2Int coord;
    }
}
