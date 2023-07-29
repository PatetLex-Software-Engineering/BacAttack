using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySpawn : MonoBehaviour
{
    [System.Serializable]
    public class Spawn {
        public Spawnable entity;
        public int weight;
    }
    [Header("Components")]
    public Terrain terrain;
    public List<Spawn> spawns;
    private ChunkLoader chunkLoader;

    [Header("Values")]
    public float minDistance;
    public int interval;

    private int radius;

    private List<GameObject> spawnedObjects;

    void Start()
    {
        this.radius = Mathf.FloorToInt((float) terrain.chunkSize * (0.3F));
        this.spawnedObjects = new List<GameObject>();
        this.chunkLoader = GetComponent<ChunkLoader>();
    }

    void FixedUpdate()
    {
        ICollection<Chunk> chunks = terrain.LoadedChunks(chunkLoader);
        if (spawnedObjects.Count <= chunks.Count * 2) {
            foreach (Chunk chunk in chunks) {
                if (Random.Range(0, interval) > 0) {
                    continue;
                }
                if (!Terrain.WaveFunction.Contains(terrain, chunk.x, chunk.y)) {
                    continue;
                }
                Tile tile = Terrain.WaveFunction.Collapse(terrain, chunk.x, chunk.y);
                if (tile.title.Contains("Space")) {
                    continue;
                }
                int c = terrain.chunkSize / 2;
                Vector3 center = new Vector3((chunk.x * terrain.chunkSize) + c, c, (chunk.y * terrain.chunkSize) + c);
                if (Vector3.Distance(center, this.transform.position) >= minDistance) {
                    int maxWeight = 0;
                    for (int i = 0; i < spawns.Count; i++) {
                        maxWeight += spawns[i].weight;
                    }
                    int targetWeight = Random.Range(0, maxWeight);
                    int iteration = 0;
                    for (int i = 0; i < spawns.Count; i++) {
                        int weight = spawns[i].weight;
                        iteration += weight;
                        if (targetWeight < iteration) {
                            GameObject clone = Instantiate<GameObject>(spawns[i].entity.gameObject);

                            float r = Random.Range(0F, 1F);
                            float scale = Mathf.Lerp(spawns[i].entity.scaleRange.x, spawns[i].entity.scaleRange.y, r);
                            float speed = Mathf.Lerp(spawns[i].entity.speedRange.x, spawns[i].entity.speedRange.y, r);
                            float atp = Mathf.Lerp(spawns[i].entity.atpRange.x, spawns[i].entity.atpRange.y, r);
                            Spawnable spawnable = clone.GetComponent<Spawnable>();
                            clone.transform.localScale = new Vector3(scale, scale, scale);
                            spawnable.Instance(scale, speed, atp);

                            clone.SetActive(true);
                            clone.transform.position = Pull(chunk);
                            spawnedObjects.Add(clone);
                            break;
                        }
                    }
                }
            }
        } else {
            spawnedObjects.RemoveAll(obj => obj == null || !obj.activeSelf);
        }
    }

    public Vector3Int Pull(Chunk chunk) {
		int center = terrain.chunkSize / 2;
        Vector3Int r = new Vector3Int((chunk.x * terrain.chunkSize) + center, center, (chunk.y * terrain.chunkSize) + center);
        Vector3 rand = new Vector3(Random.Range(-1F, 1F), Random.Range(-1F, 1F), Random.Range(-1F, 1F)).normalized;
        Vector3 part = rand * Random.Range(0, radius);
        Vector3Int a = new Vector3Int(Mathf.CeilToInt(part.x), Mathf.CeilToInt(part.y), Mathf.CeilToInt(part.z));
        return r;
    }
}
