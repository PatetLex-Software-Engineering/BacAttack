using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{

    public string title;
    public Texture2D texture;
    public Vector2 spawnRange;
    public int weight;

    private int textureID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int TextureID() {
        return this.textureID;
    }

    public void setTextureID(int id) {
        this.textureID = id;
    }

    public class VoxelInstance {
        public Voxel voxel;
        public float pointValue;
    }
}
