using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;

[Serializable]
public class Side : IInitializable
{
    private Color _color;
    private Cannon _cannon;
    private GameObject _config;

    private float _timer;
    private int _count;

    public Action Click;
    private DiContainer _container;
    public int Count;
    
    public Side(Color color, Cannon cannon, GameObject projectileConfig, DiContainer container) {
        _color = color;
        _cannon = cannon;
        _config = projectileConfig;
        _container = container;
        _count = 1;
    }

    private void HandleClick() {
        _cannon.Shoot(_config, _count, _container, _color);
        _count = 0;
        PutOnCooldown();
    }

    public float GetCount() { return _count; }

    public float GetTimer() { return _timer; }

    private void PutOnCooldown() {
        _cannon.StartCoroutine(_cannon.wait(delegate 
            { Debug.Log("DONE COOLDOWN"); _timer = 1; _count++; PutOnCooldown(); }, 
            5
        ));
    }

    public void Initialize()
    {
        Click += delegate { HandleClick(); };
    }
}
