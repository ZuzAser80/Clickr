using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Sides {
    public class SideSpawnManager : IInitializable {
        private List<EffectZone> _effectZones = new List<EffectZone>();
        private CameraFieldSide _side;

        public SideSpawnManager(List<EffectZone> effectZone, CameraFieldSide side) {
            _effectZones = effectZone;
            _side = side;
        }

        public void Initialize()
        {
            _effectZones[0].ApplyEffect += delegate { Spawn("Soldier"); };
            //_effectZones[1].ApplyEffect += delegate { _side.AddCount(1); };
            _effectZones[2].ApplyEffect += delegate { Spawn("Gunner"); };
            //_effectZones[3].ApplyEffect += delegate { _side.AddCount(1); };
            _effectZones[4].ApplyEffect += delegate { Spawn("Cannon"); };
            //_effectZones[5].ApplyEffect += delegate { _side.AddCount(1); };
            _effectZones[6].ApplyEffect += delegate { Spawn("Tank"); };
        }

        public void Spawn(string name) {
            Debug.Log("Spawned unit: " + name);
        }
    }
}