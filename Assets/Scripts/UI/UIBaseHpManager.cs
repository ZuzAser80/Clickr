using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBaseHpManager : NetworkBehaviour {
    [SerializeField] private Image fillRed;
    [SerializeField] private Image fillBlue;

    [SerializeField] private TextMeshProUGUI textRed;
    [SerializeField] private TextMeshProUGUI textBlue;

    [SerializeField] private GameObject onWinPanel;
    [SerializeField] private GameObject onLosePanel;
    [SerializeField] private List<TextMeshProUGUI> tmp = new List<TextMeshProUGUI>();

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

    public void UpdateUISingle(float hp, float enemyHp, float maxHp, List<int> curReq, List<int> maxReq) {
        fillRed.fillAmount = MathF.Round(hp/maxHp, 2);
        textRed.text = hp.ToString();
        fillBlue.fillAmount = MathF.Round(enemyHp/maxHp, 2);
        textBlue.text = enemyHp.ToString();
        tmp.ForEach(x => x.text = (maxReq[tmp.IndexOf(x)] - curReq[tmp.IndexOf(x)]).ToString());
    }

    public void Win() {
        FindObjectOfType<SinglePlayer>().WinCmd();
        onWinPanel.SetActive(true);
    }

    public void Lost() {
        FindObjectOfType<SinglePlayer>().LoseCmd();
        onLosePanel.SetActive(true);
    }

    public void Switch(int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }

    public void FlipGOState(GameObject go) {
        go.SetActive(!go.activeSelf);
    }  

    public void Click() {
        FindObjectOfType<SteamFace>().PlayClick();
    }

    public void Restart() { FindObjectOfType<SinglePlayer>().Pause(); FindObjectOfType<SinglePlayer>().Restart(); } 
}