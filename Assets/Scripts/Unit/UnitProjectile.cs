using System.Linq;
using Mirror;
using UnityEngine;

namespace Assets.Scripts.Unit {
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class UnitProjectile : NetworkBehaviour {
        public float StartSpeed = 1f;
        public float Lifetime = 5f;
        public bool Explode = false;
        public float ExplosionRadius;
        public float Damage;
        public GameObject explosion;

        private Collider[] res;
        private PathwalkingUnit _unit;

        [ServerCallback]
        private void OnTriggerEnter(Collider other) {
            if(!other.TryGetComponent(out PathwalkingUnit unit) || unit.color == _unit.color) { return; }
            unit.Damage(Damage);
            Destroy(gameObject);
        }

        [ServerCallback]
        private void OnDestroy() {
            if(Explode) { 
                var r = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(r, 0.5f);
                NetworkServer.Spawn(r);
                Physics.OverlapSphereNonAlloc(transform.position, ExplosionRadius, res);
                res.ToList().ForEach(x => { 
                    if(x.TryGetComponent(out IDamagable dmg)) {  
                        dmg.Damage(Damage);
                    }
                });
            }
        }

        [ServerCallback]
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