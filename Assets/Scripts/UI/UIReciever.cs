using System;
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

        // [SerializeField] private Image baseHpOwn;
        // [SerializeField] private Image baseHpEnemy;

        // [SerializeField] private TextMeshProUGUI baseHpText;
        // [SerializeField] private TextMeshProUGUI enemyHpText;

        [Client]
        public void UpdateUIRpc(float fillOwn, float filleEnemy, int ownCount) {
            _fillOwn.fillAmount = fillOwn;
            _fillEnemy.fillAmount = filleEnemy;
            // baseHpOwn.fillAmount = bHO / bMax;
            // baseHpEnemy.fillAmount = bHE / bMax;
            // baseHpText.text = bHO.ToString();
            // enemyHpText.text = bHE.ToString();
            _countText.text = ownCount.ToString();

        }   

    }
}