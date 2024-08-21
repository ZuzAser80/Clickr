using System;
using UnityEngine;

namespace Assets.Scripts.Unit.Units {
    public class BaseUnit : MonoBehaviour, IDamagable
    {
        private float _maxHealth;
        private float _currentHealth;

        public Action onPlayerLose;

        public void Damage(float amount) {
            if(_currentHealth > amount) {
                _currentHealth -= amount;
            } else {
                Die();
            }
        }

        public void Die()
        {
            onPlayerLose?.Invoke();
        }
    }
}