using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    [Header("Canvas")]
    public Canvas current;
    public Canvas loading;
    public Canvas hud;
    public Canvas death;

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

        hud.enabled = current == null;
        UpdateImage(health, statistics.Health());
        UpdateImage(atp, statistics.ATP());

        if (current == loading) {
            ChunkLoader loader = gameObject.GetComponent<ChunkLoader>();
            List<Chunk> chunks = loader.Vicinity();
            if (chunks.Count > 0) {
                bool flag = true;
                foreach (Chunk chunk in chunks) {
                    if (chunk.gameObject == null) {
                        flag = false;
                    }
                }
                if (flag) {
                    Close();
                }
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

    public void SetMenu(Canvas menu) {
        if (current == null) {
            current = menu;
            current.enabled = true;
        }
    }

    public void Close() {
        if (this.current != null) {
            this.current.enabled = false;
            current = null;
        }
    }
}
