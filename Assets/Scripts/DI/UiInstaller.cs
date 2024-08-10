using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Assets.Scripts.Sides;
using System.Collections.Generic;

namespace Assets.Scripts.DI {
    public class UiInstaller : MonoInstaller {

        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Image slider;
        [SerializeField] private Button launch;

        [SerializeField] private List<EffectZone> effectZones;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SideSpawnManager>().AsSingle()
                .WithArguments(effectZones);
            Container.BindInterfacesAndSelfTo<UiHandler>().AsSingle()
                .WithArguments(countText, slider, launch);
        }
    }
}