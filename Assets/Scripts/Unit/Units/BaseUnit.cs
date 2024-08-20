using UnityEngine;
using Zenject;

namespace Assets.Scripts.Unit.Units {
    public class BaseUnit : MonoBehaviour, IDamagable
    {
        private float _maxHealth;
        private float _currentHealth;
        //private UiHandler _handler;

        private void Awake() {
            Debug.Log("Awake");
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