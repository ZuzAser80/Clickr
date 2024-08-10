using Assets.Scripts.Battle;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.DI {
    public class BattleInstaller : MonoInstaller {
        [SerializeField] private Transform baseSpawn;
        [SerializeField] private GameObject baseGO;
        [Space]
        [SerializeField] private Transform unitySpawnArea;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BattleSide>().AsSingle().NonLazy();
            //Creating base unit
            Container.InstantiatePrefab(baseGO, baseSpawn.position, baseSpawn.rotation, baseSpawn);
        }
    }
}