using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Unit {
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class UnitProjectile : MonoBehaviour {
        public float StartSpeed = 1f;
        public float Lifetime = 5f;
        public bool Explode = false;
        public float ExplosionRadius;
        public float Damage;

        private Collider[] res;
        private Rigidbody _rb;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.layer != LayerMask.NameToLayer("Unit")) { return; }
            //damage unit
            Destroy(gameObject);
        }

        private void OnDestroy() {
            if(Explode) { 
                Physics.OverlapSphereNonAlloc(transform.position, ExplosionRadius, res);
                res.ToList().ForEach(x => { 
                    if(x.gameObject.layer == LayerMask.NameToLayer("Unit")) {  
                        //damage unit
                    }
                });
            }
        }

        public void Init(Vector3 startDirection) {
            _rb.velocity = startDirection * StartSpeed;
            Destroy(gameObject, Lifetime);
        }
    }
}