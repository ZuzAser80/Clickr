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

    [SerializeField] private TextMeshProUGUI alarm;

    [SerializeField] private List<Color> redColors = new List<Color>();
    [SerializeField] private List<Color> blueColors = new List<Color>();

    [SerializeField] private GameObject onWinPanel;
    [SerializeField] private GameObject onLosePanel;
    [SerializeField] private RectTransform hpParent;
    [SerializeField] private List<TextMeshProUGUI> tmp = new List<TextMeshProUGUI>();

    [SerializeField] private Color c;

    [ClientRpc]
    public void UpdateUI(float hp, float maxHp, bool left) {
        Debug.Log(":::::: + " + fillRed.fillAmount);
        if(left) {
            fillRed.fillAmount = MathF.Round(hp/maxHp, 2);
            textRed.text = hp.ToString();
            Debug.Log(":::::: + " + fillRed.fillAmount);
            
        } else {
            fillBlue.fillAmount = MathF.Round(hp/maxHp, 2);
            if (fillBlue.fillAmount <= 0.2f) {
                textBlue.color = blueColors[0];
                alarm.gameObject.SetActive(true);
            } 
            if (fillBlue.fillAmount <= 0.5f) {
                Debug.Log("=====");
                textBlue.color = blueColors[1];
            } 
            textBlue.text = hp.ToString();
        }
    }

    private void Update() {
        if (!alarm.gameObject.activeSelf) {return;}
        c.a = (Mathf.Sin(Time.time * 4) + 1.0f) / 2.0f;
        alarm.color = c; 
    }

    public void UpdateUISingle(float hp, float enemyHp, float maxHp, List<int> curReq, List<int> maxReq) {
        fillRed.fillAmount = MathF.Round(hp/maxHp, 2);
        textRed.text = hp.ToString();
        fillBlue.fillAmount = MathF.Round(enemyHp/maxHp, 2);
        textBlue.text = enemyHp.ToString();
        if (fillRed.fillAmount <= .2f) {
            textRed.color = redColors[0];
            alarm.gameObject.SetActive(true);
        } 
        if (fillRed.fillAmount <= .5f) {    
            textRed.color = redColors[1];
        } 
        if (fillBlue.fillAmount <= .2f) {
            textBlue.color = blueColors[0];
        } 
        if (fillBlue.fillAmount <= .5f) {    
            textBlue.color = blueColors[1];
        }
        tmp.ForEach(x => {
            if(x != null && maxReq.Count >= tmp.IndexOf(x) && curReq.Count >= tmp.IndexOf(x)) { 
                x.text = (maxReq[tmp.IndexOf(x)] - curReq[tmp.IndexOf(x)]).ToString();
            }
        });
    }

    public void UpdateHp() => hpParent.anchoredPosition = new Vector2(hpParent.anchoredPosition.x == 352 ? 0 : 352, hpParent.anchoredPosition.y);

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