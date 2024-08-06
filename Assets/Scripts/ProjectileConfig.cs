using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ProjectileConfig : MonoBehaviour
{
    public Vector2 StartVelocity;
    public float StartVelocityMultiplier = 1;

    [SerializeField] private float StartCollisionCooldown;
    
    private Collider2D _collider;
    private Rigidbody2D _rb;
    
    private void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Ball");
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        _collider.enabled = false;
    }

    private IEnumerator startMove() {
        yield return new WaitForSeconds(StartCollisionCooldown);
        _collider.enabled = true;
        _rb.velocity = StartVelocity * StartVelocityMultiplier;
    }

    public void StartM() {
        StartCoroutine(startMove());
    }

}
