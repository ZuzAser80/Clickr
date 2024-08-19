using System;
using System.Collections;
using Assets.Scripts.Sides;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour {
    public GameObject rofl;
    [SyncVar(hook = nameof(PlayerColorChanged))]
    public Color color;

    [SyncVar] public int count;
    public float timer;

    [SerializeField] private GameObject _config;
    [SerializeField] private GameObject _ui;
    private UIReciever reciever;

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

    public override void OnStartAuthority()
    {
        _ui.SetActive(true);
    }

    public override void OnStartLocalPlayer()
    {
        //reciever = FindObjectOfType<UIReciever>();
        Debug.Log("::::: " + color + " -- ");
        //PutOnCooldown();
    }

    public override void OnStartClient()
    {
        PutOnCooldown();
    }

    private void PlayerColorChanged(Color o, Color n)
    {
        color = n;
    }

    // todo: ui work

    [TargetRpc]
    public void HandleEventRpc() {
        CmdSpawnUnit();
    }

    [Command]
    public void CmdSpawnUnit() {
        
    }

    [Command]
    public void CmdClick() {
        Debug.Log(" ::: " + count);
        FindObjectOfType<Cannon>().Shoot(_config, ref count, this);
    }

    private void Update() {
        if(!isLocalPlayer) { return; }
        _ui.GetComponent<UIReciever>().UpdateUIRpc(timer, count);
        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log(0);
            CmdClick();
        }

    }
}