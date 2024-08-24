using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBaseHpManager : NetworkBehaviour {
    [SerializeField] private Image fillRed;
    [SerializeField] private Image fillBlue;

    [SerializeField] private TextMeshProUGUI textRed;
    [SerializeField] private TextMeshProUGUI textBlue;

    [ClientRpc]
    public void UpdateUI(float hp, float maxHp, bool left) {
        if(left) {
            fillRed.fillAmount = MathF.Round(hp/maxHp, 2);
            textRed.text = hp.ToString();
        } else {
            fillBlue.fillAmount = MathF.Round(hp/maxHp, 2);
            textBlue.text = hp.ToString();
        }
    }
}