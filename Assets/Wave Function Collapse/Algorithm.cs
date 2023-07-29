using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Algorithm {

    public static bool VERBOSE = false; 

    public static int FORWARD = 1;
    public static int BACKWARD = 2;
    public static int LEFT = 3;
    public static int RIGHT = 4;

    private Dictionary<Vector2Int, Tile> sample;
    private List<Tile> tiles;
    private System.Random rand;

    public Algorithm(int seed, List<Tile> tiles) : this(seed, tiles, null) {}

    public Algorithm(int seed, List<Tile> tiles, Tile initial) {
        sample = new Dictionary<Vector2Int, Tile>();
        this.tiles = tiles;
        this.rand = new System.Random(seed);
        if (initial == null) {
            Collapse(0,0);
        } else {
            lowerBound = new Vector2Int(0,0);
            upperBound = new Vector2Int(0,0);
            sample[lowerBound] = initial;
        }
    }

    private Vector2Int lowerBound = new Vector2Int(int.MaxValue, int.MaxValue);
    private Vector2Int upperBound = new Vector2Int(int.MinValue, int.MinValue);

    public bool Contains(int x, int y) {
        return sample.ContainsKey(new Vector2Int(x,y));
    }

    public Tile Collapse(int x, int y) {
        if (!sample.ContainsKey(new Vector2Int(x,y))) {
            if (x < lowerBound.x || y  < lowerBound.y) {
                lowerBound = new Vector2Int(x < lowerBound.x ? x : lowerBound.x, y < lowerBound.y ? y : lowerBound.y);
            }
            if (x > upperBound.x || y > upperBound.y) {
                upperBound = new Vector2Int(x > upperBound.x ? x : upperBound.x, y > upperBound.y ? y : upperBound.y);
            }
            int dx = upperBound.x - lowerBound.x;
            dx++;
            int dy = upperBound.y - lowerBound.y;
            dy++;
            int area = dx * dy;
            Iterate(x, y, area);
        }
        return sample[new Vector2Int(x,y)];
    }

    void Iterate(int x, int y, int area) {
        for (int itr = 0; itr <= area; itr++) {
            bool flag = false;
            string trace = "";
            float lowestEntropy = float.MaxValue;
            Vector2Int respectiveCoord = new Vector2Int(x,y);
            Dictionary<Tile, int> respectiveTiles = new Dictionary<Tile, int>();
            foreach (Tile t in tiles) {
                respectiveTiles.Add(t, 1);
            }
            for (int i = lowerBound.x; i <= upperBound.x; i++) {
                for (int j = lowerBound.y; j <= upperBound.y; j++) {
                    Vector2Int tile = new Vector2Int(i,j);
                    if (!sample.ContainsKey(tile)) {
                        flag = true;
                        string temp = "";
                        Dictionary<Tile, int> possibilites = new Dictionary<Tile, int>();
                        AddPossibleTiles(possibilites, new Vector2Int(i + 1, j), LEFT);
                        AddPossibleTiles(possibilites, new Vector2Int(i - 1, j), RIGHT);
                        AddPossibleTiles(possibilites, new Vector2Int(i, j + 1), BACKWARD);
                        AddPossibleTiles(possibilites, new Vector2Int(i, j - 1), FORWARD);
                        temp += " [beg-pos: " + possibilites.Count + "] ";
                        RemoveImpossibleTiles(possibilites, new Vector2Int(i + 1, j), LEFT);
                        RemoveImpossibleTiles(possibilites, new Vector2Int(i - 1, j), RIGHT);
                        RemoveImpossibleTiles(possibilites, new Vector2Int(i, j + 1), BACKWARD);
                        RemoveImpossibleTiles(possibilites, new Vector2Int(i, j - 1), FORWARD);
                        temp += " [end-pos: " + possibilites.Count + "] ";
                        if (sample.ContainsKey(new Vector2Int(i + 1, j))) {
                            temp += " { [" + (i + 1) + ", " + (j) + "]: " + sample[new Vector2Int(i + 1, j)].title + " } ";
                        }
                        if (sample.ContainsKey(new Vector2Int(i - 1, j))) {
                            temp += " { [" + (i - 1) + ", " + (j) + "]: " + sample[new Vector2Int(i - 1, j)].title + " } ";
                        }
                        if (sample.ContainsKey(new Vector2Int(i, j + 1))) {
                            temp += " { [" + (i) + ", " + (j + 1) + "]: " + sample[new Vector2Int(i, j + 1)].title + " } ";
                        }
                        if (sample.ContainsKey(new Vector2Int(i, j - 1))) {
                            temp += " { [" + (i) + ", " + (j - 1) + "]: " + sample[new Vector2Int(i, j - 1)].title + " } ";
                        }
                        if (possibilites.Count > 0) {
                            float weight = 0;
                            foreach (Tile t in possibilites.Keys) {
                                weight += possibilites[t];
                            }
                            float entropy = 1F / (weight / (float) possibilites.Count);
                            temp += " [entropy: " + entropy + "]";
                            if (entropy < lowestEntropy) {
                                lowestEntropy = entropy;
                                respectiveCoord = tile;
                                respectiveTiles = possibilites;
                                trace = temp;
                            }
                        } 
                    }
                }
            }
            if (!sample.ContainsKey(respectiveCoord)) {
                int maxWeight = 0;
                for (int i = 0; i < respectiveTiles.Count; i++) {
                    maxWeight += respectiveTiles.ElementAt(i).Value;
                }
                int targetWeight = rand.Next(0, maxWeight);
                int iteration = 0;
                for (int i = 0; i < respectiveTiles.Count; i++) {
                    Tile tile = respectiveTiles.ElementAt(i).Key;
                    int weight = respectiveTiles.ElementAt(i).Value;
                    iteration += weight;
                    if (targetWeight < iteration) {
                        sample[respectiveCoord] = tile;
                        trace = " [" + respectiveCoord.x + ", " + respectiveCoord.y + "] " + trace;
                        trace += " [Bounds: " + lowerBound.ToString() + " | " + upperBound.ToString() + "]";
                        if (VERBOSE) {
                            Debug.Log(tile.title + " - " + respectiveTiles.Count + " - " + trace);
                            if (respectiveTiles.Count == tiles.Count) {
                                Debug.Log("Randomizing " + tile.title + " at [" + respectiveCoord.x + ", " + respectiveCoord.y + "].");
                            }
                        }
                        break;
                    }
                }
            }
            if (!flag) {
                return;
            }
        }
    }

    Dictionary<Tile, int> DirectionalTiles(Vector2Int location, int dir) {
        if (sample.ContainsKey(location)) {
            return sample[location].Possibilities(dir);
        }
        return null;
    }

    void AddPossibleTiles(Dictionary<Tile, int> possibilities, Vector2Int location, int dir) {
        if (sample.ContainsKey(location)) {
            Dictionary<Tile, int>  ti = DirectionalTiles(location, dir);
            foreach (Tile t in ti.Keys) {
                if (!possibilities.ContainsKey(t) && ti[t] > 0) {
                    possibilities.Add(t, ti[t]);
                }
            }
        }
    }

    void RemoveImpossibleTiles(Dictionary<Tile, int> possibilities, Vector2Int location, int dir) {
        if (sample.ContainsKey(location)) {
            List<Tile> remove = new List<Tile>();
            Dictionary<Tile, int> ti = DirectionalTiles(location, dir);
            foreach (Tile tile in possibilities.Keys) {
                if (!ti.ContainsKey(tile)) {
                    remove.Add(tile);
                }
            }
            foreach (Tile r in remove) {
                possibilities.Remove(r);
            }
        }
    }
}
