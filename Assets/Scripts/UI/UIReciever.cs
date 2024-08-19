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

    public Player CurrentPlayer;

    public static Action<float, float, int> update;

    private void Awake() {
        update += UpdateUIRpc;
    }

    public void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space)) { _button.onClick.Invoke(); }
    }

    //[ClientRpc]
    public void UpdateUIRpc(float fillOwn, float fillEnemy, int ownCount) {
        _fillEnemy.fillAmount = _fillEnemy.fillAmount != 0 && fillEnemy == 0 ? _fillEnemy.fillAmount : 0;
        _fillOwn.fillAmount = CurrentPlayer.timer;
        _countText.text = CurrentPlayer.count.ToString();
    }   

    [ClientRpc]
    public void UpdateClientUIRpc(int playerId)
    {
        //NetworkServer.connections[playerId].identity.GetComponent<Player>().PlzHandleClickRpc();
    }
}