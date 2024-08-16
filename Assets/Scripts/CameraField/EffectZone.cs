using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EffectZone : MonoBehaviour {
    public Action ApplyEffect;

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("Ball")) { return; }
        other.GetComponent<ProjectileConfig>().Die();
        ApplyEffect?.Invoke();
    }
}