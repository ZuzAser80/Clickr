using System;
using Mirror;
using UnityEngine;

namespace Assets.Scripts.Unit.Units {
    public class BaseUnit : NetworkBehaviour, IDamagable
    {
        [SerializeField] public float MaxHealth;

        [SyncVar(hook = nameof(OnColorChanged))] public Color color;

        private float _maxHealth => MaxHealth;
        [SyncVar] public float _currentHealth;

        public Action onPlayerLose;

        void OnColorChanged(Color _Old, Color _New)
        {
            var playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }

        private void Awake() {
            _currentHealth = _maxHealth;
        }

        public void Damage(float amount) {
            if(_currentHealth > amount) {
                _currentHealth -= amount;
            } else {
                _currentHealth = 0;
                Die();
            }

        }

        public void Die()
        {
            onPlayerLose?.Invoke();
            Destroy(this);
        }
    }
}