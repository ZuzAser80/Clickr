using System;
using System.Collections;
using System.Linq;
using Mirror;
using Unity.Mathematics;
using Unity.VisualScripting;
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

        [SerializeField] private AudioClip fireClip;
        [SerializeField] private AudioClip onDeathClip;


        private NavMeshAgent _navMeshAgent;

        [SerializeField] private float _currentHealth;
        private PathwalkingUnit _currentEnemy;
        private bool canShoot = true;
        private Transform _lookDir;
        [SyncVar] public Color old;
        public Material impactMat;
        private Material oldMat;
        private Renderer _renderer;
        private AudioSource source;
        

        private Collider[] res = new Collider[]{};

        private void Awake() {
            source = GetComponent<AudioSource>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if(transform.childCount > 0) { _lookDir = transform.GetChild(0); }
            _currentHealth = _properties.MaxHealth;
            oldMat = GetComponent<Renderer>().material;
            _navMeshAgent.speed = _properties.MaxSpeed;
            _renderer = GetComponent<Renderer>();
        }

        void OnColorChanged(Color _Old, Color _New)
        {
            if(_New != Color.white) { old = _New; _renderer.material = oldMat; } else { GetComponent<Renderer>().material = impactMat; return; }
            var playerMaterialClone = new Material(_renderer.material);
            playerMaterialClone.color = _New;
            playerMaterialClone.EnableKeyword("_EMISSION");
            playerMaterialClone.SetColor("_EmissionColor", _New);
            _renderer.material = playerMaterialClone;
            for(int i = 0; i < transform.childCount; i++) {
                if(transform.GetChild(i).TryGetComponent(out Renderer renderer)) {
                    renderer.material = playerMaterialClone;
                }
            }
        }

        [ServerCallback]
        public void Detect(PathwalkingUnit unit) { 
            _currentEnemy = unit;
            MeshFlipRpc();
            _currentEnemy.onDeath += delegate { 
                if(this == null) { return; }
                MeshFlipRpc();
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
        public void MeshFlipRpc() => _navMeshAgent.isStopped = !_navMeshAgent.isStopped;

        public float GetHealth() { return _currentHealth; }

        private IEnumerator reload() {
            canShoot = false;
            for(int i = 0; i < _properties.ProjectileCount; i++) {
                ShootRpc(_currentEnemy);
                yield return new WaitForSeconds(_properties.RPM);
            }
            yield return new WaitForSeconds(_properties.Reload);
            if(_navMeshAgent.isStopped) {
                StopAllCoroutines();
                StartCoroutine(reload());
            }
            canShoot = true;
        }

        [ServerCallback]
        public virtual void Damage(float amount) { 
            StartCoroutine(impact());
            if(_currentHealth > amount) { 
                _currentHealth -= amount; 
            } else {
                Die();
            } 
        }

        private IEnumerator impact() {
            color = Color.white;
            yield return new WaitForSeconds(0.3f);
            color = old;
        }

        [ServerCallback]
        public virtual void Die() {
            if (onDeathClip != null) { source.PlayOneShot(onDeathClip); }
            onDeath?.Invoke();
            Destroy(gameObject);
        }

        [ServerCallback]
        public void ShootRpc(PathwalkingUnit target)
        {
            if(target == null) { return; }
            Vector3 direction = target.transform.position - _lookDir.position;
            direction.y += Random.Range(0, _properties.ArcAngle) / 100;
            direction.x += Random.Range(-_properties.MaxSpread, _properties.MaxSpread) / 10;
            _lookDir.rotation = Quaternion.LookRotation(direction);

            source.PlayOneShot(fireClip);

            var p = Instantiate(_properties.UnitProjectile, transform.position, _lookDir.rotation);
            p.Init(_lookDir.forward, this);
            NetworkServer.Spawn(p.gameObject);
        }
    }
}