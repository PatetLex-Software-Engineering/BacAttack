using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ChunkLoader : MonoBehaviour
{
    [Header("Component")]
    public Transform position;
    public Terrain terrain;

    [Header("Values")]
    public int loadingDistance;

	private List<Action> queue;
	private Task current;

    public void Queue(Action action) {
        if (queue == null) {
            queue = new List<Action>();
        }
        queue.Add(action);
    }

    public bool HasQueue() {
        return this.queue == null || this.queue.Count > 0;
    }


    void Start()
    {

    }

    void Update() {
        if (queue != null && queue.Count > 0) {
			Action up = queue[0];
			if (current == null || current.IsCompleted) {
				current = Task.Run(up);
				queue.RemoveAt(0);
			}
		}
    }

    public List<Chunk> Vicinity() {
        List<Chunk> c = new List<Chunk>();
        Vector2 chunkCoords = terrain.ChunkCoords(this.transform.position.x, this.transform.position.z);
        for (int x = (int) chunkCoords.x - this.loadingDistance; x <= (int) chunkCoords.x + this.loadingDistance; x++) {
            for (int y = (int) chunkCoords.y - this.loadingDistance; y <= (int) chunkCoords.y + this.loadingDistance; y++) {
                Chunk chunk = terrain.ChunkAt(new Vector2Int(x,y));
                if (chunk != null) {
                    c.Add(chunk);
                }
            }
        }
        return c;
    }
}
