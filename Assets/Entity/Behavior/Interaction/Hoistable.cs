using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoistable : Interactable {
    
    [Header("Values")]
    public float speedRatio;

    public override void Interact(Interaction player) {
        player.SetHoist(this);
    }

    public override bool ShouldInteract(Interaction player) {
        Statistics statistics = player.gameObject.GetComponent<Statistics>();
        Spawnable spawnable = gameObject.GetComponent<Spawnable>();
        return player.Hoist() == null && statistics.ATP().value > (Mathf.Abs(statistics.ATP().regeneration.value - spawnable.ATP()) * 250);
    }

    public override KeyCode Key(Interaction player) {
        return KeyCode.E;
    }

    public override string Action(Interaction player) {
        return "Hoist";
    }
}