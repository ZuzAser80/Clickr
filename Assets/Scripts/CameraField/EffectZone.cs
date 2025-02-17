using System;
using Mirror;
using TMPro;
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
        if(_.owner != null) {
            if(number % 2 == 0) {
                _.owner.SpawnUnit(number / 2);
            } else {
                _.owner.AddOne();
            }
        } else {
            _.singleOwner.AddC();
            if(number == 6 && _.singleOwner.GetType() == typeof(SinglePlayer)) {
                ((SinglePlayer)_.singleOwner).eff_t();
            }
            if(number == 18) {
                _.singleOwner.SpawnUnit(0); 
                _.singleOwner.SpawnUnit(0); 
                _.singleOwner.SpawnUnit(0); 
                _.singleOwner.SpawnUnit(0); 
                return;
            }
            if(number == 8 || number == 10) {
                _.singleOwner.CmdSpawnUnitWIndex(4, number); 
                return;
            }
            if(number % 2 == 0) {
                _.singleOwner.SpawnUnit(number / 2); 
            }
        }
    }
}