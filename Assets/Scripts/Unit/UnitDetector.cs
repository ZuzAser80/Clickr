using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Unit {
    [RequireComponent(typeof(Collider))]
    public class UnitDetector : NetworkBehaviour {

        public UnityEvent onDetect;
        [SerializeField] private PathwalkingUnit unit;

        private void Awake() {
            unit = GetComponentInParent<PathwalkingUnit>();
            GetComponent<SphereCollider>().radius = unit.GetProperties().SpotRadius;
        }

        private void OnTriggerEnter(Collider other) {
            if(!other.TryGetComponent(out PathwalkingUnit u) || u.color == unit.color) { return; }
            unit.Detect(u);
        }
    }
}