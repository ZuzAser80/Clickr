using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI {
    public class UIReciever : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Image _fillOwn;
        [SerializeField] private Image _fillEnemy;
        [SerializeField] private Button _button;

        [Client]
        public void UpdateUIRpc(float fillOwn, float filleEnemy) {
            _fillOwn.fillAmount = fillOwn;
            _fillEnemy.fillAmount = filleEnemy;
        }   

    }
}