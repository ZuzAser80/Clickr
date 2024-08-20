using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EffectZone : NetworkBehaviour {
    public Action ApplyEffect;
    private ProjectileConfig _;

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("Ball")) { return; }
        _ = other.GetComponent<ProjectileConfig>();
        _.Die();
        //_.owner.HandleEventRpc();
        //Debug.Log(":::::: " + other.GetComponent<ProjectileConfig>().isLocalPlayer);
        ApplyEffect?.Invoke();
    }
}