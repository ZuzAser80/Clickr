using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReciever : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Image _fillOwn;
    [SerializeField] private Image _fillEnemy;
    [SerializeField] private Button _button;

    [Client]
    public void UpdateUIRpc(float fillOwn, int ownCount) {
        _fillOwn.fillAmount = fillOwn;
        _countText.text = ownCount.ToString();
    }   
}