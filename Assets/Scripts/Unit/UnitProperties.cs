using System;
using Mirror;
using UnityEngine;

namespace Assets.Scripts.Unit {
    [Serializable]
    public struct UnitProperties {
        public int MaxHealth;
        public float MaxSpeed;
        public float SpotRadius;
        public int ProjectileCount;
        public float RPM;
        [Tooltip("Поправка настильности")] public float ArcAngle;
        public float MaxSpread;
        public UnitProjectile UnitProjectile;
        public float Reload;
        [SyncVar] public Color side;
    }
}