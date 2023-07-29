using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Elipse : MonoBehaviour
{
    private TextMeshProUGUI text;

    private float time = 50;
    private float t;

    private string original;
    private string elipse;

    void Start()
    {
        this.text = gameObject.GetComponent<TextMeshProUGUI>();
        this.original = text.text;
        this.elipse = "";
    }

    void FixedUpdate() {
        t++;
        float r = t / time;
        float p = r - Mathf.FloorToInt(r);
        if (p == 0) {
            if (elipse == "") {
                elipse = ".";
            } else if (elipse == ".") {
                elipse = "..";
            } else if (elipse == "..") {
                elipse = "...";
            } else {
                elipse = "";
            }
            text.text = original + elipse;
        }
    }
}
