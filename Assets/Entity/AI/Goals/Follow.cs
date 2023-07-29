using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : Goal
{
    [Header("Values")]
    public float followDistance;
    public float lookDistance;

    void Start() {
        this.title = "Follow";
    }

    public override bool ShouldEngage(AI ai) {
        GameObject entityToFollow = Closest(ai, ai.Players());
        float dx = ai.transform.position.x - entityToFollow.transform.position.x;
        float dy = ai.transform.position.y - entityToFollow.transform.position.y;
        float dz = ai.transform.position.z - entityToFollow.transform.position.z;
        float d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        if (d > followDistance && d <= lookDistance) {
            return true;
        }
        return false;
    }

    public override void Engage(AI ai) {
        base.Engage(ai);
        GameObject entityToFollow = Closest(ai, ai.Players());
        float dx = ai.transform.position.x - entityToFollow.transform.position.x;
        float dy = ai.transform.position.y - entityToFollow.transform.position.y;
        float dz = ai.transform.position.z - entityToFollow.transform.position.z;
        float d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
        if (d <= followDistance || d > lookDistance) {
            ai.Break();
            return;
        }
        Vector3 direction = (entityToFollow.transform.position - ai.transform.position).normalized;
        ai.Rigidbody().AddForce(direction * ai.Spawnable().Speed(), ForceMode.Force);
    }

    public GameObject Closest(AI ai, List<GameObject> gameObjects) {
        float distance = float.MaxValue;
        GameObject gameObject = gameObjects[0];
        foreach (GameObject go in gameObjects) {
            float dx = ai.transform.position.x - go.transform.position.x;
            float dy = ai.transform.position.y - go.transform.position.y;
            float dz = ai.transform.position.z - go.transform.position.z;
            float d = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
            if (d < distance) {
                distance = d;
                gameObject = go;
            }
        }
        return gameObject;
    }
}
