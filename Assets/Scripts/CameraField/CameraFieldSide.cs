using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Mirror;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Sides {
    [Serializable]
    public class CameraFieldSide : NetworkBehaviour
    {
        //ебаный кусок говна
        // todo: delete this shit nigga
        [SerializeField] private Cannon _cannon;
        [SerializeField] private GameObject _config;
        [SerializeField] private UIReciever _reciver;

        [Client]
        public void HandleClick(NetworkIdentity player) {
            _cannon.Shoot(_config, ref player.GetComponent<Player>().count, player.GetComponent<Player>().color);
        }
    }
}