using Assets.Scripts.Sides;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Battle {
    public class BattleSide : IInitializable
    {
        private CameraFieldSide _side;
        private SideSpawnManager _manager;

        public BattleSide (CameraFieldSide side) {
            _side = side;
        }

        public void Initialize()
        {
            Debug.Log("Side : " + _side);
        }
    }
}