using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EffectZone : MonoBehaviour {
    public Action ApplyEffect;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("Ball")) { return; }
        ApplyEffect.Invoke();
    }
}