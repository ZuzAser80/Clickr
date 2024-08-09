using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Assets.Scripts.Sides;
using System.Collections.Generic;

namespace Assets.Scripts.DI {
    public class SceneInstaller : MonoInstaller {
        [SerializeField] private Color color;
        [SerializeField] private Cannon cannon;
        [SerializeField] private GameObject config;

        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Slider slider;
        [SerializeField] private Button launch;

        [SerializeField] private List<EffectZone> effectZones;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Side>().AsSingle()
                .WithArguments(color, cannon, config);
            Container.BindInterfacesAndSelfTo<SideSpawnManager>().AsSingle()
                .WithArguments(effectZones);
            Container.BindInterfacesAndSelfTo<UIHandler>().AsSingle()
                .WithArguments(countText, slider, launch);
        }
    }
}