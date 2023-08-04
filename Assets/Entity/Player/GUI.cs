using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject current;
    public GameObject loading;
    public GameObject hud;
    public GameObject death;
    public GameObject menu;

    [Header("UI")]
    public Image health;
    public Image atp;

    private Statistics statistics;

    void Start()
    {
        statistics = gameObject.GetComponent<Statistics>();
    }

    void Update()
    {
        Cursor.visible = current != null;
        Cursor.lockState = current == null ? CursorLockMode.Locked : CursorLockMode.None;

        hud.SetActive(current == null);
        UpdateImage(health, statistics.Health());
        UpdateImage(atp, statistics.ATP());

        if (current == loading) {
            ChunkLoader loader = gameObject.GetComponent<ChunkLoader>();
            List<Vector2Int> coords = loader.Coordinates();
            bool flag = true;
            foreach (Vector2Int coord in coords) {
                Chunk chunk = loader.terrain.ChunkAt(coord);
                if (chunk == null || chunk.gameObject == null) {
                    flag = false;
                }
            }
            if (flag) {
                Close();
            }
        }
    }

    void FixedUpdate() {
        if (current != null) {
            statistics.Speed().AddModifier(new Statistics.Modifier(0, Statistics.Modifier.Operator.SET));
        }
    }

    void UpdateImage(Image image, Statistics.Regenerable statistic) {
        image.fillAmount = statistic.value / statistic.CalculatedValue();
    } 

    public void SetMenu(GameObject menu) {
        if (current == null) {
            current = menu;
            current.SetActive(true);
        }
    }

    public void Close() {
        if (this.current != null) {
            this.current.SetActive(false);
            current = null;
        }
    }
}
