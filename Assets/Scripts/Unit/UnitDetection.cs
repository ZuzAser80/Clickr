namespace Assets.Scripts.Unit {
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(Collider))]
    public class UnitDetection : MonoBehaviour {
        [SerializeField] private PathwalkingUnit unit;

        private void Awake() {
            GetComponent<SphereCollider>().radius = unit.GetProperties().SpotRadius;
        }

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.layer != LayerMask.NameToLayer("Unit") || other.gameObject == transform.parent) { return; }
            // todo: refactor
            //unit.Detect(other.gameObject.GetComponent<PathwalkingUnit>());
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}