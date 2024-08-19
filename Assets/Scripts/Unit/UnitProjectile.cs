using System.Linq;
using Mirror;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Unit {
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class UnitProjectile : NetworkBehaviour {
        public float StartSpeed = 1f;
        public float Lifetime = 5f;
        public bool Explode = false;
        public float ExplosionRadius;
        public float Damage;

        private Collider[] res;
        private PathwalkingUnit _unit;

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.layer != LayerMask.NameToLayer("Unit") || other.GetComponent<PathwalkingUnit>() == _unit) { return; }
            other.gameObject.GetComponent<IDamagable>().Damage(Damage);
            Destroy(gameObject);
        }

        private void OnDestroy() {
            if(Explode) { 
                Physics.OverlapSphereNonAlloc(transform.position, ExplosionRadius, res);
                res.ToList().ForEach(x => { 
                    if(x.gameObject.layer == LayerMask.NameToLayer("Unit")) {  
                        x.gameObject.GetComponent<IDamagable>().Damage(Damage);
                    }
                });
            }
        }

        [Server]
        public void Init(Vector3 startDirection, PathwalkingUnit owner) {
            _unit = owner;
            GetComponent<Rigidbody>().velocity = startDirection * StartSpeed;
            Destroy(gameObject, Lifetime);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}