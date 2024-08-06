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

    public Action Click;
    private DiContainer _container;
    public int Count;

    public Side(Color color, Cannon cannon, GameObject projectileConfig, DiContainer container) {
        _color = color;
        _cannon = cannon;
        _config = projectileConfig;
        _container = container;
    }

    private void HandleClick() {
        var g = _container.InstantiatePrefab(_config, _cannon.transform.position, Quaternion.identity, null);
        g.GetComponent<ProjectileConfig>().StartVelocity = _cannon.transform.right;
        g.GetComponent<ProjectileConfig>().StartM();
    }

    public void Initialize()
    {
        Click += delegate { HandleClick(); };
    }
}
