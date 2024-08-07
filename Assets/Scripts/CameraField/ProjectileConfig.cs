using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ProjectileConfig : MonoBehaviour
{
    private Vector2 _startVelocity;
    public float StartVelocityMultiplier = 1;

    [SerializeField] private float StartCollisionCooldown;

    private Collider2D _collider;
    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;
    
    private void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Ball");
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();

        _collider.enabled = false;
    }

    private IEnumerator startMove() {
        yield return new WaitForSeconds(StartCollisionCooldown);
        _collider.enabled = true;
        _rb.velocity = _startVelocity * StartVelocityMultiplier;
    }

    public void StartM(Color color, Vector3 startVelocity) {
        _startVelocity = startVelocity;
        _renderer.color = color;
        StartCoroutine(startMove());
    }

    public void Die() { Destroy(gameObject); }

}
