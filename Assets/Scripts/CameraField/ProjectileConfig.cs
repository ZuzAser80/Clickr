using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ProjectileConfig : NetworkBehaviour
{
    private Vector2 _startVelocity;
    public float StartVelocityMultiplier = 1;

    [SerializeField] private float StartCollisionCooldown;

    private Collider2D _collider;
    private Rigidbody2D _rb;
    public Player owner;
    [SyncVar(hook = nameof(HandleColor))]
    public Color color;
    private SpriteRenderer _renderer;
    
    
    private void Awake() {
        gameObject.layer = LayerMask.NameToLayer("Ball");
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();

        _collider.enabled = false;
    }

    [Server]
    private IEnumerator startMove() {
        _rb.velocity = _startVelocity * StartVelocityMultiplier;
        yield return new WaitForSeconds(StartCollisionCooldown);
        _collider.enabled = true;
    }

    public void HandleColor(Color o, Color n) {
        color = n;
        _renderer.color = color;
    }

    // fumble
    [Server]
    public void StartM(Player player, Vector3 startVelocity) {
        _startVelocity = startVelocity;
        owner = player;
        StartCoroutine(startMove());
    }

    public void Die() { Destroy(gameObject); }

}
