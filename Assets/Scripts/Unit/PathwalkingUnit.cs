using System;
using System.Collections;
using Assets.Scripts.DI;
using Assets.Scripts.Sides;
using Assets.Scripts.Unit.Units;
using Mirror;
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

        [SerializeField] private Player owner;
        private NavMeshAgent _navMeshAgent;

        private float _currentHealth;
        private PathwalkingUnit _currentEnemy;
        private bool canShoot = true;
        private Transform _lookDir;

        private void Awake() {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _lookDir = transform.GetChild(0);
            StartPathfind(new Vector3(0, 0, 0));
        }

        public Player GetSide() {
            return owner;
        }

        public virtual void Detect(PathwalkingUnit unit) { 
            //if(unit.GetSide() == _side) { Debug.Log("SAME SIDE : " + _side); return; }
            _navMeshAgent.isStopped = true;
            _currentEnemy = unit;
            _currentEnemy.onDeath += delegate { 
                Debug.Log("Enemy killed"); 
                _navMeshAgent.isStopped = false; 
            };
            if(canShoot) {
                StartCoroutine(reload());
            }
        }

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

        [Server]
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