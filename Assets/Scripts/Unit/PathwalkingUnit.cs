using System;
using System.Collections;
using System.Linq;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Unit {
    [RequireComponent(typeof(NavMeshAgent))]
    public class PathwalkingUnit : NetworkBehaviour, IDamagable, IShooter
    {
        [SerializeField] protected UnitProperties _properties;

        public Action onDeath;
        [SyncVar(hook = nameof(OnColorChanged))] public Color color;

        [SerializeField] private Collider detector;

        private NavMeshAgent _navMeshAgent;

        private float _currentHealth;
        private PathwalkingUnit _currentEnemy;
        private bool canShoot = true;
        private Transform _lookDir;

        private Collider[] res = new Collider[]{};

        private void Awake() {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _lookDir = transform.GetChild(0);
        }

        void OnColorChanged(Color _Old, Color _New)
        {
        
            var playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }

        [ServerCallback]
        public void Detect(PathwalkingUnit unit) { 
            //if(!isLocalPlayer) { return; }
            Debug.Log("Detect: " + unit);
            _currentEnemy = unit;
            Stop();
            _currentEnemy.onDeath += delegate { 
                Debug.Log("Enemy killed"); 
                _navMeshAgent.isStopped = false; 
                _currentEnemy = null;
            };
            if(canShoot) {
                StartCoroutine(reload());
            }
        }

        [ClientRpc]
        public virtual void StartPathfindRpc(Vector3 objective) => _navMeshAgent.SetDestination(objective);

        public UnitProperties GetProperties() => _properties;

        [ClientRpc]
        public void Stop() => _navMeshAgent.isStopped = true;

        private IEnumerator reload() {
            canShoot = false;
            ShootRpc(_currentEnemy);
            yield return new WaitForSeconds(_properties.Reload);
            if(_navMeshAgent.isStopped) {
                StopAllCoroutines();
                StartCoroutine(reload());
            }
            canShoot = true;
        }

        [ClientRpc]
        public virtual void Damage(float amount) { 
            if(_currentHealth > amount) { 
                _currentHealth -= amount; 
            } else {
                Die();
            } 
        }

        [ClientRpc]
        public virtual void Die() {
            Debug.Log("Die");
            onDeath?.Invoke();
            Destroy(gameObject);
        }

        [ServerCallback]
        public void ShootRpc(PathwalkingUnit target)
        {
            Vector3 direction = target.transform.position - _lookDir.position;
            direction.y += Random.Range(0, _properties.ArcAngle) / 100;
            direction.x += Random.Range(-_properties.MaxSpread, _properties.MaxSpread) / 10;
            _lookDir.rotation = Quaternion.LookRotation(direction);

            var p = Instantiate(_properties.UnitProjectile, transform.position, _lookDir.rotation);
            p.Init(_lookDir.forward, this);
            NetworkServer.Spawn(p.gameObject);
        }
    }
}