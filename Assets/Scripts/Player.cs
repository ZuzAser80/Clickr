using System;
using System.Collections;
using System.Linq;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour {
    public GameObject rofl;
    [SyncVar] public Color color;

    [SyncVar] public int count;
    [SyncVar] public float timer;
    [SyncVar] public float enemyTimer;

    [SerializeField] private GameObject _config;
    [SerializeField] private GameObject _ui;
    private UIReciever _reciever;
    private Player _enemy;

    #region Timer + count
    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; count++; StopAllCoroutines(); PutOnCooldown(); }, 
            delegate { timer += MathF.Round(Time.deltaTime / 3.5f, 3); },
            3.5f
        ));
    }

    public IEnumerator wait(Action action, Action update, float seconds) {
        for (float i = 0; i < seconds;) {
            update?.Invoke();
            yield return new WaitForEndOfFrame();
            i += Time.deltaTime;
        }
        action?.Invoke();
    }
    #endregion

    public void SetEnemy(Player _) { _enemy = _; }

    public override void OnStartAuthority()
    {
        _ui.SetActive(true);
        _reciever = _ui.GetComponentInChildren<UIReciever>();
    }

    public override void OnStartClient() {
        PutOnCooldown();
    }

    [TargetRpc]
    public void HandleEventRpc(GameObject unit) {
        CmdSpawnUnit(unit);
    }

    #region Commands
    [Command]
    public void CmdSpawnUnit(GameObject unit) {
        
    }

    [Command]
    public void CmdUpdateUI() {
        if(_enemy != null) {  _enemy.enemyTimer = timer; }   
    }

    [Command]
    public void CmdClick() {
        FindObjectOfType<Cannon>().Shoot(_config, ref count, this);
        if(_enemy != null) {  _enemy.enemyTimer = timer; }   
    }
    #endregion

    private void Update() {
        if(!isLocalPlayer) { return; }
        CmdUpdateUI();
        _reciever.UpdateUIRpc(timer, enemyTimer, count);
        if(Input.GetKeyDown(KeyCode.Space)) {
            CmdClick();
        }
    }
}