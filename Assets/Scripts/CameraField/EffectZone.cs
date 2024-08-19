using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EffectZone : NetworkBehaviour {
    public Action ApplyEffect;

    [Server]
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("Ball")) { return; }
        other.GetComponent<ProjectileConfig>().Die();
        Debug.Log(":::::: " + other.GetComponent<ProjectileConfig>().isLocalPlayer);
        ApplyEffect?.Invoke();
    }
}