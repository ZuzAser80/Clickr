using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ProjectileConfig : MonoBehaviour
{
    public Vector3 StartVelocity;
    public float StartCollisionCooldown;

    private Collider _collider;
    private Rigidbody _rb;
    
    private void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Ball");
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        _collider.enabled = false;
    }

    private IEnumerator startMove() {
        yield return new WaitForSeconds(StartCollisionCooldown);
        _collider.enabled = true;
        _rb.velocity += StartVelocity;
    }

    private void StartM() {
        StartCoroutine(startMove());
    }

}
