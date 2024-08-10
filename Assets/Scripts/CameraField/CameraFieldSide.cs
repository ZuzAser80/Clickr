using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Sides {
    [Serializable]
    public class CameraFieldSide : MonoBehaviour
    {
        [SerializeField] private Color _color;
        [SerializeField] private Cannon _cannon;
        [SerializeField] private GameObject _config;

        private float _timer;
        private int _count;

        public Action Click;
        private DiContainer _container;

        private void HandleClick() {
            _cannon.Shoot(_config, ref _count, _container, _color);
            PutOnCooldown();
        }

        public float GetCount() { return _count; }

        public float GetTimer() { return _timer; }

        private void PutOnCooldown() {
            _cannon.StartCoroutine(_cannon.wait(
                delegate { _timer = 0; _count++; _cannon.StopAllCoroutines(); PutOnCooldown(); }, 
                delegate { _timer += MathF.Round(Time.deltaTime / 3.5f, 3); },
                3.5f
            ));
        }

        public void AddCount(int amount) { _count += amount; }

        public void Awake()
        {
            _count = 1;
            Click += delegate { HandleClick(); };
            PutOnCooldown();
            _container = GetComponent<ZenjectBinding>().Context.Container;
        }
    }
}