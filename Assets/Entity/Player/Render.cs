using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Render : MonoBehaviour
{
    [Header("Components")]
    public GameObject model;

    [Header("Values")]
    public Statistics statistics;

    void Start()
    {
        
    }

    void Update()
    {
        this.model.transform.localScale = Vector3.one * (statistics.Speed().value / statistics.baseSpeed) * 7F;
    }
}
