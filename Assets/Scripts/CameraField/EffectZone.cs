using System;
using Mirror;
using Unity.VisualScripting;
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
        if(number % 2 == 0) {
            _.owner.SpawnUnit(number / 2);
        } else {
            _.owner.AddOne();
        }
    }
}