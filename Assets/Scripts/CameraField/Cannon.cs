using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class Cannon : MonoBehaviour {
    [SerializeField] private Vector3 upperAngle;
    [SerializeField] private Vector3 lowerAngle;
    [SerializeField] private float RotationSpeed = 1f;

    private Vector3 _current;

    private void Update() {
        RotateTo();
    }

    private void RotateTo() {
        float angle = Mathf.Atan2(_current.y, _current.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
        if(Quaternion.Angle(transform.rotation, rotation) < 1f) { SwitchCurrent(); }
    }

    public void Shoot(GameObject _config, int count, DiContainer container, Color color) {
        for (int i = 0; i < count; i++) {
            var g = container.InstantiatePrefab(_config, transform.position, Quaternion.identity, null);
            var _p = g.GetComponent<ProjectileConfig>();
            _p.StartM(color, transform.right);
        }
    }

    public IEnumerator wait(Action action, Action update, float seconds) {
        for(int i = 0; i < seconds * 2; i++) {
            update?.Invoke();
            yield return new WaitForSeconds(0.5f);
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