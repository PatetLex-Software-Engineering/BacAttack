using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [System.Serializable]
    public class WeightEntry {
        public Tile tile;
        public int weight;
    }

    [Header("Components")]
    public string title;
    public Material material;

    [Header("Rule Set")]
    public List<WeightEntry> weights;
    public List<Tile> forwardsTiles;
    public List<Tile> backwardsTiles;
    public List<Tile> leftTiles;
    public List<Tile> rightTiles;

    private Dictionary<Tile, int> forward;
    private Dictionary<Tile, int> backward;
    private Dictionary<Tile, int> left;
    private Dictionary<Tile, int> right;

    public Dictionary<Tile, int> Possibilities(int dir) {
        if (dir == Algorithm.FORWARD) {
            return forward;
        } else if (dir == Algorithm.BACKWARD) {
            return backward;
        } else if (dir == Algorithm.LEFT) {
            return left;
        } else {
            return right;
        }
    }

    void Build() {
        forward = new Dictionary<Tile, int>();
        backward = new Dictionary<Tile, int>();
        left = new Dictionary<Tile, int>();
        right = new Dictionary<Tile, int>();
        foreach (Tile tile in forwardsTiles) {
            foreach (WeightEntry entry in weights) {
                if (tile == entry.tile) {
                    forward.Add(tile, entry.weight);
                    break;
                }
            }
        }
        foreach (Tile tile in backwardsTiles) {
            foreach (WeightEntry entry in weights) {
                if (tile == entry.tile) {
                    backward.Add(tile, entry.weight);
                    break;
                }
            }
        }
        foreach (Tile tile in leftTiles) {
            foreach (WeightEntry entry in weights) {
                if (tile == entry.tile) {
                    left.Add(tile, entry.weight);
                    break;
                }
            }
        }
        foreach (Tile tile in rightTiles) {
            foreach (WeightEntry entry in weights) {
                if (tile == entry.tile) {
                    right.Add(tile, entry.weight);
                    break;
                }
            }
        }
    }

    void Start()
    {
        Build();
    }

    void Update()
    {

    }


}
