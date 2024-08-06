using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour {
    [SerializeField] private Vector3 upperAngle;
    [SerializeField] private Vector3 lowerAngle;
    [SerializeField] private float RotationSpeed = 1f;

    public Side side;

    private Vector3 _current;
    private bool _rotating = false;
    private Quaternion rot;

    private void Start() {
        _rotating = true;
    }

    private void Update() {
        if(!_rotating) {return;}
        RotateTo();
    }

    private void RotateTo() {
        float angle = Mathf.Atan2(_current.y, _current.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
        if(Quaternion.Angle(transform.rotation, rotation) < 1f) { SwitchCurrent(); }
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