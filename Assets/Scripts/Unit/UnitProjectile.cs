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
        [SerializeField] private AudioClip explos;

        private AudioSource source;
        private float c = 0;
        private Collider[] res = new Collider[30];
        private PathwalkingUnit _unit;

        [ServerCallback]
        private void OnTriggerEnter(Collider other) {
            if(!other.TryGetComponent(out PathwalkingUnit unit) && other.gameObject.layer==LayerMask.NameToLayer("Ground")) { Destroy(gameObject); return; }
            if(unit == null || unit.color == _unit.color) { return; }
            unit.Damage(Damage);
            Destroy(gameObject);
        }

        [ServerCallback]
        private void OnDestroy() {
            if(Explode && c > 1) { 
                if(explosion != null) {
                    explosion.GetComponent<UnitExplosion>().rad = (int)ExplosionRadius;
                    var r = Instantiate(explosion, transform.position, Quaternion.identity);
                    source.PlayOneShot(explos);
                    Destroy(r, 0.5f);
                    NetworkServer.Spawn(r);
                }
                Physics.OverlapSphereNonAlloc(transform.position, ExplosionRadius, res);
                res.ToList().ForEach(x => { 
                    if(x != null && x.TryGetComponent(out IDamagable dmg) && (Object)dmg != _unit) {  
                        dmg.Damage(Damage);
                    }
                });
            }
        }

        private void Update() {
            c += Time.deltaTime;
        }

        [ServerCallback]
        public void Init(Vector3 startDirection, PathwalkingUnit owner) {
            _unit = owner;
            GetComponent<Rigidbody>().velocity = startDirection * StartSpeed;
            source = GetComponent<AudioSource>();
            Destroy(gameObject, Lifetime);
            c = 0;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}