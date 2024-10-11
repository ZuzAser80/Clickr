using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Cannon : NetworkBehaviour {

    [SerializeField] private Vector3 upperAngle;
    [SerializeField] private Vector3 lowerAngle;
    [SerializeField] private float RotationSpeed = 1f;
    [SerializeField] private List<GameObject> mags = new List<GameObject>();   

    private Vector3 _current;

    private GameObject _g;
    private ProjectileConfig _p;
    private Vector3 _dir;
    private float _;
    [SerializeField] private Animator animator;

    private void Awake() {
        _current = upperAngle;
        //animator = GetComponent<Animator>();
    }

    #region Rotation
    private void Update() {
        RotateTo();
    }

    private void RotateTo() {
        float angle = Mathf.Atan2(_current.y, _current.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
        if(Quaternion.Angle(transform.rotation, rotation) < 5f) { SwitchCurrent(); }
    }
    #endregion

    public void Shoot(GameObject config, ref int count, Player player) {
        if (count % 2 != 0) { ShootInDir(transform.right, 0, config, player); }
        _ = count > 1 ? (count <= 8 ? Mathf.Floor(count / 2) : 4) : 0;
        for (int i = 1;  i <= _; i++) {
            ShootInDir(transform.right, (count % 2 == 0 ? 0 : 90/((count+1)*2)) + 90/(count+1) * i, config, player);
        }
        for (int i = 1; i <= _; i++) {
            ShootInDir(transform.right, (count % 2 == 0 ? 0 : -90/((count+1)*2)) - 90/(count+1) * i, config, player);
        }
        count -= count <= 8 ? count : 8;
    }

    private void ShootInDir(Vector2 fwd, float angle, GameObject config, Player player) {
        _g = Instantiate(config, transform.position + transform.forward, Quaternion.identity);
        _p = _g.GetComponent<ProjectileConfig>();
        _dir = Quaternion.AngleAxis(angle, Vector3.forward) * fwd;
        _p.StartM(player, _dir);
        _p.color = player.color;
        NetworkServer.Spawn(_g);
    }

    public void Shoot(GameObject config, ISP player, float speed) {
        if (player.GetType() != typeof(AI)) { animator.SetTrigger("Shot"); }
        
        StartCoroutine(c(0.25f, config, player, speed));
    }


    private IEnumerator c(float wait, GameObject config, ISP player, float speed) { yield return new WaitForSeconds(wait); ShootInDir(transform.right, 0, config, player, speed); }

    private void ShootInDir(Vector2 fwd, float angle, GameObject config, ISP player, float speed) {
        _g = Instantiate(config, transform.position + transform.forward, Quaternion.identity);
        _p = _g.GetComponent<ProjectileConfig>();
        _dir = Quaternion.AngleAxis(angle, Vector3.forward) * fwd;
        _p.StartM(player, _dir, speed);
        _p.color = player.color;
        NetworkServer.Spawn(_g);
    }

    public void SetCountInMag(int c) {
        mags.ForEach(x => x.SetActive(false));
        for (int i = 0; i < c; i++) {
            mags[i].SetActive(true);
        }
    }

    private void SwitchCurrent() { _current = _current == upperAngle ? lowerAngle : upperAngle; }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + upperAngle);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + lowerAngle);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.right);
    }

}