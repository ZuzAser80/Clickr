using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class Cannon : MonoBehaviour {
    [SerializeField] private Vector3 upperAngle;
    [SerializeField] private Vector3 lowerAngle;
    [SerializeField] private float RotationSpeed = 1f;

    private Vector3 _current;

    private GameObject _g;
    private ProjectileConfig _p;
    private Vector3 _dir;
    private float _;

    private void Awake() {
        _current = upperAngle;
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

    public void Shoot(GameObject config, ref int count, DiContainer container, Color color) {
        if (count % 2 != 0) { ShootInDir(transform.right, 0, config, container, color); }
        _ = count > 1 ? (count <= 8 ? Mathf.Floor(count / 2) : 4) : 0;
        for (int i = 1;  i <= _; i++) {
            ShootInDir(transform.right, (count % 2 == 0 ? 0 : 90/((count+1)*2)) + 90/(count+1) * i, config, container, color);
        }
        for (int i = 1; i <= _; i++) {
            ShootInDir(transform.right, (count % 2 == 0 ? 0 : -90/((count+1)*2)) - 90/(count+1) * i, config, container, color);
        }
        count -= count <= 8 ? count : 8;
    }

    private void ShootInDir(Vector2 fwd, float angle, GameObject config, DiContainer container, Color color) {
        _g = container.InstantiatePrefab(config, transform.position, Quaternion.identity, null);
        _p = _g.GetComponent<ProjectileConfig>();
        _dir = Quaternion.AngleAxis(angle, Vector3.forward) * fwd;
        _p.StartM(color, _dir);
    }
    
    public IEnumerator wait(Action action, Action update, float seconds) {
        for (float i = 0; i < seconds;) {
            update?.Invoke();
            yield return new WaitForEndOfFrame();
            i += Time.deltaTime;
        }
        action?.Invoke();
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