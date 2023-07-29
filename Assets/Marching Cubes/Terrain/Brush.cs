using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush
{
    private Vector3 point;
    private Voxel voxel;

    private List<Vector3Int> propagation;

    public Brush(Vector3 reference, Voxel voxel) {
        this.point = reference;
        this.voxel = voxel;
        this.propagation = new List<Vector3Int>();
    }

    public void Sphere(int radius) {
        Vector3Int center = new Vector3Int(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), Mathf.RoundToInt(point.z));
        for (int x = center.x - radius; x <= center.x + radius; x++) {
            for (int y = center.y - radius; y <= center.y + radius; y++) {
                for (int z = center.z - radius; z <= center.z + radius; z++) {
                    Vector3Int p = new Vector3Int(x, y, z);
                    float distance = Vector3.Distance(center, p);
                    if (distance <= radius) {
                        propagation.Add(p);
                    }
                }
            }
        }
    }

    public void Point(Vector3Int pos) {
        propagation.Add(pos);
    }

    public void Apply(Terrain terrain) {
        List<Chunk> march = new List<Chunk>();
        foreach (Vector3Int prop in propagation) {
            Chunk sc = terrain.ChunkAt(terrain.ChunkCoords(prop.x, prop.z));
            for (int x = sc.x - 1; x <= sc.x + 1; x++) {
                for (int y = sc.y - 1; y <= sc.y + 1; y++) {
                    Chunk chunk = terrain.ChunkAt(x, y);
                    int absX = prop.x - (chunk.x * terrain.chunkSize);
                    int absY = prop.z - (chunk.y * terrain.chunkSize);
                    if (absX >= 0 && absX <= terrain.chunkSize && absY >= 0 && absY <= terrain.chunkSize) {
                        chunk.SetVoxel(prop - new Vector3Int(chunk.x * terrain.chunkSize, 0, chunk.y * terrain.chunkSize), voxel);
                        if (!march.Contains(chunk)) {
                            march.Add(chunk);
                        }
                    }
                }
            }
        }
        foreach (Chunk chunk in march) {
            chunk.March();
        }
    }
}
