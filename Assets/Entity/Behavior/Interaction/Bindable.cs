using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bindable : Interactable {

    [Header("Components")]
    public ParticleSystem death;
    public Material material;

    public override void Interact(Interaction player) {
        Spawnable spawnable = gameObject.GetComponent<Spawnable>();
        Statistics statistics = player.gameObject.GetComponent<Statistics>();
        player.transform.position = gameObject.transform.position;
        float val = statistics.ATP().value + spawnable.ATP();
        if (val > statistics.ATP().maxValue) {
            statistics.ATP().maxValue = val;
        }
        statistics.ATP().value = val;

        GameObject obj = Instantiate<GameObject>(death.gameObject);
        ParticleSystem system = obj.GetComponent<ParticleSystem>();
        system.transform.position = gameObject.transform.position;
        ParticleSystem.MainModule module = system.main;
        module.startColor = material.color;
        system.gameObject.SetActive(true);
        Destroy(obj, module.startLifetime.constant);

        Destroy(gameObject);
    }

    public override bool ShouldInteract(Interaction player) {
        return player.Hoist() == null;
    }

    public override KeyCode Key(Interaction player) {
        return KeyCode.E;
    }

    public override string Action(Interaction player) {
        return "Bind";
    }
}
