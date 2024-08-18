using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EffectZone : NetworkBehaviour {
    public Action ApplyEffect;

    [ServerCallback]
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("Ball")) { return; }
        other.GetComponent<ProjectileConfig>().Die();
        ApplyEffect?.Invoke();
    }
}