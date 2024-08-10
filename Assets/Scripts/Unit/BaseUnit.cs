using Assets.Scripts.Sides;
using UnityEngine;

namespace Assets.Scripts.Unit {
    public class BaseUnit : MonoBehaviour, IDamagable
    {
        private float _maxHealth;
        private float _currentHealth;
        private CameraFieldSide _side;

        public void Init(float maxHealth, CameraFieldSide side) {
            _maxHealth = maxHealth;
            _side = side;
        }

        public void Damage(float amount) {
            if(_currentHealth > amount) {
                _currentHealth -= amount;
            } else {
                Die();
            }
        }

        public void Die()
        {
            
        }
    }
}