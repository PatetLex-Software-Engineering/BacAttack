using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [Header("Components")]
    public Material material;
    public PhysicMaterial physicsMaterial;
    public LayerMask layer;

    [Header("Render")]
    public List<ChunkLoader> chunkLoaders;

    private Dictionary<Vector2Int, Chunk> chunks;

    [Header("Environment")]
    public List<Tile> waves;
    public Voxel air;
    public Voxel skin;
    public int chunkSize = 16;
    public float zoom = 0.03F;
    public int octaves = 2;
    public float persistance = 0.5F;
    public float lacunarity = 2F;
    public int seed;

    private Algorithm waveFunction;

    private Texture2DArray texture2DArray;

    void Awake() {
        seed = Random.Range(0, 99999);
        Random.InitState(seed);
    }

    void Start()
    {
        chunks = new Dictionary<Vector2Int, Chunk>();

        if (waves != null && waves.Count > 0) {
            // Algorithm.VERBOSE = true;
            waveFunction = new Algorithm(seed, waves, waves[0]);
        }
    }

    void Update()
    {
        // Chunk Loading
        UpdateChunks();
    }

    void UpdateChunks() {
        List<Chunk> remove = new List<Chunk>();
        foreach (Chunk chunk in chunks.Values) {
            chunk.Update();
            remove.Add(chunk);
        }
        foreach (ChunkLoader chunkLoader in chunkLoaders) {
            Vector2 chunkCoords = ChunkCoords(chunkLoader.transform.position.x, chunkLoader.transform.position.z);
            for (int x = (int) chunkCoords.x - chunkLoader.loadingDistance; x <= (int) chunkCoords.x + chunkLoader.loadingDistance; x++) {
                for (int y = (int) chunkCoords.y - chunkLoader.loadingDistance; y <= (int) chunkCoords.y + chunkLoader.loadingDistance; y++) {
                    bool present = false;
                    foreach (Chunk chunk in chunks.Values) {
                        if (chunk.x == x && chunk.y == y) {
                            remove.Remove(chunk);
                            if (chunk.gameObject != null) {
                                chunk.gameObject.SetActive(true);
                            }
                            present = true;
                        }
                    }
                    if (!present) {
                        Chunk chunk = new Chunk(this, x, y);
                        chunk.Generate(chunkLoader);
                        chunks.Add(new Vector2Int(x, y), chunk);
                    }
                }
            }
        }
        foreach (Chunk chunk in remove) {
            if (chunk.gameObject != null) {
                chunk.gameObject.SetActive(false);
            }
        }
    }

    public Texture2DArray TextureArray() {
        return texture2DArray;
    }

    public ICollection<Chunk> LoadedChunks() {
        return chunks.Values;
    }

    public ICollection<Chunk> LoadedChunks(ChunkLoader chunkLoader) {
        List<Chunk> c = new List<Chunk>();
        Vector2 chunkCoords = ChunkCoords(chunkLoader.transform.position.x, chunkLoader.transform.position.z);
        for (int x = (int) chunkCoords.x - chunkLoader.loadingDistance; x <= (int) chunkCoords.x + chunkLoader.loadingDistance; x++) {
            for (int y = (int) chunkCoords.y - chunkLoader.loadingDistance; y <= (int) chunkCoords.y + chunkLoader.loadingDistance; y++) {
                if (chunks.ContainsKey(new Vector2Int(x,y))) {
                    Chunk chunk = chunks[new Vector2Int(x,y)];
                    if (chunk.gameObject != null) {
                        if (chunk.gameObject.activeSelf) {
                            c.Add(chunk);
                        }
                    }
                }
            }
        }
        return c;
    }

    public Vector2Int ChunkCoords(Vector3 pos) {
        return ChunkCoords(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    public Vector2Int ChunkCoords(float x, float y) {
        int chunkX = Mathf.FloorToInt(x / (float) chunkSize);
        int chunkY = Mathf.FloorToInt(y / (float) chunkSize);
        return new Vector2Int(chunkX, chunkY);
    }

    public Chunk ChunkAt(Vector2Int xy) {
        if (!chunks.ContainsKey(xy)) {
            return null;
        }
        return chunks[xy];
    }
    public Chunk ChunkAt(int x, int y) {
        return ChunkAt(new Vector2Int(x,y));
    }

    private bool TextureEquals(Texture2D a, Texture2D b) {
        Color[] firstPix = a.GetPixels();
        Color[] secondPix = b.GetPixels();
        if (firstPix.Length != secondPix.Length) {
            return false;
        }
        for (int i= 0; i < firstPix.Length; i++) {
            if (firstPix[i] != secondPix[i]) {
                return false;
            }
        }
        return true;
    }

    public class Noise {
        public static float TerrainNoise(Terrain terrain, float x, float y) {
            System.Random prng = new System.Random(terrain.seed);
            Vector2[] octaveOffsets = new Vector2[terrain.octaves];
            for (int i = 0; i < terrain.octaves; i++) {
                float offsetX = prng.Next(-100000, 100000);
                float offsetY = prng.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
            float amplitude = 0;
            float octavianAmplitude = 1;
            float frequency = 1;
            float value = 0;
            for (int i = 0; i < terrain.octaves; i++) {
                float sampleX = x * frequency;
                float sampleY = y * frequency;
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                value += perlinValue * octavianAmplitude;
                amplitude += octavianAmplitude;
                octavianAmplitude *= terrain.persistance;
                frequency *= terrain.lacunarity;
            }
            return Mathf.InverseLerp(-amplitude, amplitude, value);
        }


    }

    public class WaveFunction {
        public static Tile Collapse(Terrain terrain, int x, int y) {
            return terrain.waveFunction.Collapse(x, y);
        }

        public static bool Contains(Terrain terrain, int x, int y) {
            return terrain.waveFunction.Contains(x, y);
        }
    }
}
