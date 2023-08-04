using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : Goal
{
    [Header("Values")]
    public Direction direction;
    public float radiusRatio;

    private Vector3Int movePos;

    void Start() {
        this.title = "Transport";
        this.movePos = Vector3Int.zero;
    }

    void Update() {

    }

    public override bool ShouldEngage(AI ai) {
        return true;
    }

    public override void Engage(AI ai) {
        base.Engage(ai);
        if (direction == Direction.RANDOM) {
            direction = Random.Range(0, 2) == 0 ? Direction.NORTH_TO_SOUTH : Direction.SOUTH_TO_NORTH;
        }
        if (movePos != Vector3Int.zero) {
            if (!ai.hitBox.bounds.Contains(movePos)) {
                Vector3 direction = (movePos - ai.transform.position).normalized;
                ai.Rigidbody().AddForce(direction * ai.Spawnable().Speed(), ForceMode.Force);
                return;
            } 
        }

        Chunk chunk = ai.terrain.ChunkAt(ai.terrain.ChunkCoords(ai.transform.position));
        if (chunk == null) {
            ai.Break();
            return;
        }

        int t = direction == Direction.NORTH_TO_SOUTH ? -1 : 1;
        Tile tile = Sample(ai.terrain, chunk.x, chunk.y + t);
        Chunk moveChunk = null;
        if (tile != null && !tile.title.Contains("Space") && !(ai.terrain.ChunkAt(chunk.x, chunk.y + t) == null
                                                             || ai.terrain.ChunkAt(chunk.x, chunk.y + t).gameObject == null
                                                              || !ai.terrain.ChunkAt(chunk.x, chunk.y + t).gameObject.activeSelf)) {
            moveChunk = ai.terrain.ChunkAt(chunk.x, chunk.y + t);
        } else {
            int tx = direction == Direction.NORTH_TO_SOUTH ? 1 : -1;
            Tile tileX = Sample(ai.terrain, chunk.x + tx, chunk.y);
            if (tileX != null && !tileX.title.Contains("Space") && !(ai.terrain.ChunkAt(chunk.x + tx, chunk.y) == null
                                                             || ai.terrain.ChunkAt(chunk.x + tx, chunk.y).gameObject == null
                                                              || !ai.terrain.ChunkAt(chunk.x + tx, chunk.y).gameObject.activeSelf)) {
                moveChunk = ai.terrain.ChunkAt(chunk.x + tx, chunk.y);
            } else if (ai.gameObject.GetComponent<ChunkDespawn>() != null) {
                Destroy(ai.gameObject);
            }
        }
        if (moveChunk != null) {
            movePos = Pull(ai.terrain, moveChunk);
        } else {
            ai.Break();
        }
    } 


    public Vector3Int Pull(Terrain terrain, Chunk chunk) {
        int radius = Mathf.FloorToInt((float) terrain.chunkSize * (0.4F * radiusRatio));;
		int center = terrain.chunkSize / 2;
        Vector3Int r = new Vector3Int((chunk.x * terrain.chunkSize) + center, center, (chunk.y * terrain.chunkSize) + center);
        Vector3 rand = new Vector3(Random.Range(-1F, 1F), Random.Range(-1F, 1F), Random.Range(-1F, 1F)).normalized;
        Vector3 part = rand * Random.Range(0, radius);
        Vector3Int a = new Vector3Int(Mathf.CeilToInt(part.x), Mathf.CeilToInt(part.y), Mathf.CeilToInt(part.z));
        return r + a;
    }

    public Tile Sample(Terrain terrain, int x, int y) {
        if (Terrain.WaveFunction.Contains(terrain, x, y)) {
            return Terrain.WaveFunction.Collapse(terrain, x, y);
        }
        return null;
    }


    public enum Direction {
        RANDOM, NORTH_TO_SOUTH, SOUTH_TO_NORTH
    }
}
