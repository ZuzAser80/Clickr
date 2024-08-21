using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EffectZone : NetworkBehaviour {
    private ProjectileConfig _;
    [SerializeField] private int number;

    [Server]
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("Ball")) { return; }
        _ = other.GetComponent<ProjectileConfig>();
        _.Die();
        // switch (number) {
            

        // }
        _.owner.SpawnUnit();
    }
}