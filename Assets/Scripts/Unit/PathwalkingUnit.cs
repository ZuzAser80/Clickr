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
        private Vector3 obj;

        private Collider[] res = new Collider[]{};

        private void Awake() {
            source = GetComponent<AudioSource>();
            
            _navMeshAgent = GetComponent<NavMeshAgent>();
            if(transform.childCount > 0) { _lookDir = transform.GetChild(0); }
            _currentHealth = _properties.MaxHealth;
            oldMat = GetComponent<Renderer>().material;
            if(_navMeshAgent != null) {
                _navMeshAgent.speed = _properties.MaxSpeed;
            }
            _renderer = GetComponent<Renderer>();
        }

        void OnColorChanged(Color _Old, Color _New)
        {
            if(_New != Color.white) { old = _New; _renderer.material = oldMat; } else { 
                GetComponent<Renderer>().material = impactMat; 
                for(int i = 0; i < transform.childCount; i++) {
                    if(transform.GetChild(i).TryGetComponent(out Renderer renderer)) {
                        renderer.material = impactMat;
                    }
                }
                return; 
            }
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

        //[ServerCallback]
        public void Detect(PathwalkingUnit unit) { 
            _currentEnemy = unit;
            StopRpc();
            _currentEnemy.onDeath += delegate { 
                if(this == null || gameObject == null) { return; }
                ContinueRpc();
                _currentEnemy = null;
            };
            if(canShoot) {
                StartCoroutine(reload());
            }
        }

        /// <summary>
        /// Отправляет юнита к точке
        /// </summary>
        /// <param name="objective">точка / позиция к которой отправляют юнита</param>
        [ClientRpc]
        public virtual void StartPathfindRpc(Vector3 objective) { if(_navMeshAgent == null) { return; } obj = objective; _navMeshAgent.SetDestination(objective); }

        /// <summary>
        /// Возвращает данные о юните
        /// </summary>
        /// <returns></returns>
        public UnitProperties GetProperties() => _properties;

        /// <summary>
        /// Чтоб юнит дальше шел
        /// </summary>
        [ClientRpc]
        public void ContinueRpc() { if(_navMeshAgent != null && this != null) { _navMeshAgent.ResetPath(); _navMeshAgent.isStopped = false; _navMeshAgent.SetDestination(obj); }  }

        /// <summary>
        /// Остановка юнита
        /// </summary>
        [ClientRpc]
        public void StopRpc() { if(_navMeshAgent != null && this != null) { _navMeshAgent.isStopped = true; } else if(this == null) {DestroyImmediate(gameObject);} }

        public float GetHealth() { return _currentHealth; }

        /// <summary>
        /// Корутина для перезарядки
        /// </summary>
        /// <returns></returns>
        private IEnumerator reload() {
            canShoot = false;
            ContinueRpc();
            if (_currentEnemy == null) {
                ContinueRpc();
            }
            for(int i = 0; i < _properties.ProjectileCount; i++) {
                ShootRpc(_currentEnemy);
                yield return new WaitForSeconds(_properties.RPM);
            }
            yield return new WaitForSeconds(_properties.Reload + Random.Range(0, _properties.Reload *0.1f));
            if(_navMeshAgent != null && _navMeshAgent.isStopped) {
                StopAllCoroutines();
                StartCoroutine(reload());
            }
            canShoot = true;
        }

        /// <summary>
        /// Урон по юниту
        /// </summary>
        /// <param name="amount">кол-во урона</param>
        [ServerCallback]
        public virtual void Damage(float amount) { 
            StartCoroutine(impact());
            if(_currentHealth > amount) { 
                _currentHealth -= amount; 
            } else {
                Die();
            } 
        }
        

        /// <summary>
        /// Импакт(белый на пару секунд)
        /// </summary>
        /// <returns></returns>
        private IEnumerator impact() {
            color = Color.white;
            yield return new WaitForSeconds(0.3f);
            color = old;
        }

        /// <summary>
        /// Импакт в момент смерти (там  от типа темный а потом белый)
        /// </summary>
        /// <returns></returns>
        private IEnumerator deathImpact() {
            color = Color.white;
            yield return new WaitForSeconds(0.3f);
            color = old / 2;
            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject, 1);
            NetworkServer.Destroy(gameObject);
        }

        /// <summary>
        /// Смерть
        /// </summary>
        [ServerCallback]
        public virtual void Die() {
            if (onDeathClip != null && source != null) { source.PlayOneShot(onDeathClip); }
            //StopRpc();
            onDeath?.Invoke();
            StartCoroutine(deathImpact());
        }

        /// <summary>
        /// Юнит стреляет
        /// </summary>
        /// <param name="target">Цель</param>
        [ServerCallback]
        public void ShootRpc(PathwalkingUnit target)
        {
            if(target == null) { return; }
            Vector3 direction = target.transform.position - _lookDir.position;
            direction.y += Random.Range(0, _properties.ArcAngle) / 100;
            direction.x += Random.Range(-_properties.MaxSpread, _properties.MaxSpread) / 10;
            _lookDir.rotation = Quaternion.LookRotation(direction);

            if(fireClip != null && source != null) { source.PlayOneShot(fireClip); }
            
            var p = Instantiate(_properties.UnitProjectile, transform.position, _lookDir.rotation);
            p.Init(_lookDir.forward, this);
            NetworkServer.Spawn(p.gameObject);
        }
    }
}