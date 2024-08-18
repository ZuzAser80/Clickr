using Assets.Scripts.Sides;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Unit.Units {
    public class BaseUnit : MonoBehaviour, IDamagable
    {
        private float _maxHealth;
        private float _currentHealth;
        private CameraFieldSide _side;
        //private UiHandler _handler;

        private void Awake() {
            Debug.Log("Awake");
        }

        [Inject]
        public void Construct(CameraFieldSide side)
        {
            _maxHealth = 32;
            _side = side;
            //_handler = handler;
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