using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.DI;
using Assets.Scripts.Unit.Units;
using Mirror;
using ModestTree;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;


namespace Assets.Scripts.Unit {
    [RequireComponent(typeof(NavMeshAgent))]
    public class PathwalkingUnit : NetworkBehaviour, IDamagable, IShooter
    {
        [SerializeField] protected UnitProperties _properties;

        public Action onDeath;
        [SyncVar(hook = nameof(OnColorChanged))] public Color color;

        [SyncVar][SerializeField] private GameObject owner;
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

        public GameObject GetSide() {
            return owner;
        }

        [Server]
        public void SetSide(GameObject player) {
            owner = player;
            color = player.GetComponent<Player>().color;
        }

        public virtual void Detect() { 
            if(_currentEnemy != null) { return; }
            Physics.OverlapSphereNonAlloc(transform.position, _properties.SpotRadius, res);
            if(res.IsEmpty()) {return;}
            foreach(var x in res.ToList()) {
                if(x.TryGetComponent(out PathwalkingUnit u) && u.owner != owner) {  
                    x.gameObject.GetComponent<IDamagable>().Damage(_properties.UnitProjectile.Damage);
                    _currentEnemy = x.GetComponent<PathwalkingUnit>();
                    break;
                }
            }
            if(_currentEnemy == null) { return; }
            _navMeshAgent.isStopped = true;
            _currentEnemy.onDeath += delegate { 
                Debug.Log("Enemy killed"); 
                _navMeshAgent.isStopped = false; 
                _currentEnemy = null;
            };
            if(canShoot) {
                StartCoroutine(reload());
            }
        }

        private void Update() {
            Detect();
        }

        [Server]
        public virtual void StartPathfind(Vector3 objective) => _navMeshAgent.SetDestination(objective);

        public UnitProperties GetProperties() => _properties;

        public void Stop() => _navMeshAgent.isStopped = true;

        private IEnumerator reload() {
            canShoot = false;
            Shoot(_currentEnemy);
            yield return new WaitForSeconds(_properties.Reload);
            if(_navMeshAgent.isStopped) {
                StopAllCoroutines();
                StartCoroutine(reload());
            }
            canShoot = true;
        }

        public virtual void Damage(float amount) { 
            if(_currentHealth > amount) { 
                _currentHealth -= amount; 
            } else {
                Die();
            } 
        }

        public virtual void Die() { 
            onDeath?.Invoke();
            Destroy(gameObject);
        }

        //[Server]
        public void Shoot(PathwalkingUnit target)
        {
            Debug.Log("Shoot: " + target);
            Vector3 direction = target.transform.position - _lookDir.position;
            //direction.y = 90;
            _lookDir.rotation = Quaternion.LookRotation(direction);

            var p = Instantiate(_properties.UnitProjectile.gameObject, transform.position, _lookDir.rotation);
            NetworkServer.Spawn(p);
        }
    }
}