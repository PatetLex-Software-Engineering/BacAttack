using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{

    [Header("Components")]
    public Terrain terrain;
    public Collider hitBox;
    public LayerMask layer;

    [System.Serializable]
    public class PriorityGoal {
        public Goal goal;
        public int priority;
    }

    [Header("Goals")]
    public List<PriorityGoal> priorityGoals;

    private Dictionary<int, Goal> goals;
    private int activePriority;
    private float time;

    private Rigidbody rb;
    private Spawnable spawnable;

    private List<GameObject> players;

    void Start() {
        goals = new Dictionary<int, Goal>();
        foreach (PriorityGoal g in priorityGoals) {
            if (!goals.ContainsKey(g.priority)) {
                goals.Add(g.priority, g.goal);
            } else {
                UnityEngine.Debug.Log("Duplicate goal [" + g.goal.Title() + ", " + goals[g.priority].Title() + "].");
            }
        }

        rb = GetComponent<Rigidbody>();
        spawnable = GetComponent<Spawnable>();

        players = new List<GameObject>();
        GameObject[] arr = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> entities = new List<GameObject>();
        for (int i = 0; i < arr.Length; i++) {
            if (arr[i].tag == "Player") {
                players.Add(arr[i]);
            }
        }
    }

    void FixedUpdate() {
        time += 0.02F;

        if (terrain.LoadedChunks().Count == 0) {
            return;
        }
        foreach (Chunk chunk in terrain.LoadedChunks()) {
            if (chunk.gameObject == null) {
                return;
            }
        }

        for (int i = 0; i <= 10; i++) {
            if ((activePriority == -1 || i <= activePriority) && goals.ContainsKey(i)) {
                Goal goal = goals[i];
                if (goal.ShouldEngage(this)) {
                    activePriority = i;
                    break;
                }
            }
        }
        if (goals.ContainsKey(activePriority)) {
            goals[activePriority].Engage(this);
        }
    }

    public Rigidbody Rigidbody() {
        return rb;
    }

    public Spawnable Spawnable() {
        return spawnable;
    }

    public float Time() {
        return time;
    }
    
    public void Break() {
        activePriority = -1;
    }

    public List<GameObject> Players() {
        return players;
    }
}
