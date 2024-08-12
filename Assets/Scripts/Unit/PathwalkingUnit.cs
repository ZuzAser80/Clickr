using System;
using System.Collections;
using Assets.Scripts.Sides;
using Assets.Scripts.Unit.Units;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Assets.Scripts.Unit {
    [RequireComponent(typeof(NavMeshAgent))]
    public class PathwalkingUnit : MonoBehaviour, IDamagable, IShooter
    {
        [SerializeField] protected UnitProperties _properties;

        private CameraFieldSide _side;
        private NavMeshAgent _navMeshAgent;

        private float _currentHealth;
        private PathwalkingUnit _currentEnemy;
        private bool canShoot = true;
        private Transform _lookDir;

        private void Awake() {
            _lookDir = transform.GetChild(0);
            StartPathfind(new Vector3(0, 0, 0));
        }

        [Inject]
        public void Construct(CameraFieldSide side) {
            _side = side;
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _currentHealth = _properties.MaxHealth;
            // todo: set color
        }

        public virtual void Detect(PathwalkingUnit unit) { 
            Stop();
            _currentEnemy = unit;
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
            Destroy(gameObject);
        }

        public void Shoot(PathwalkingUnit target)
        {
            Vector3 direction = target.transform.position - _lookDir.position;
            direction.y = Mathf.Deg2Rad * UnityEngine.Random.Range(0, _properties.ArcAngle);
            //direction.z = Mathf.Deg2Rad * UnityEngine.Random.Range(1f, 73f) * 5;
            _lookDir.rotation = Quaternion.LookRotation(direction);
            // add more random plane rotation
            // then spawn at .forward vector uniproj from _properties

        }
    }
}