using System;
using System.Collections;
using Assets.Scripts.Sides;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour {
    public GameObject rofl;
    [SyncVar(hook = nameof(PlayerColorChanged))]
    public Color color;

    public int count;
    public float timer;

    [SerializeField] private GameObject _config;
    private UIReciever reciever;

    public override void OnStartAuthority()
    {
        //color = UnityEngine.Random.ColorHSV();
    }

    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; count++; StopAllCoroutines(); PutOnCooldown(); }, 
            delegate { timer += MathF.Round(Time.deltaTime / 3.5f, 3);  },
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

    public override void OnStartLocalPlayer()
    {
        reciever = FindObjectOfType<UIReciever>();
        reciever.CurrentPlayer = this;
    }

    private void PlayerColorChanged(Color o, Color n)
    {
        color = n;
    }

    // todo: ui work

    [Command]
    public void CmdClick() {
        PutOnCooldown();
        FindObjectOfType<Cannon>().Shoot(_config, ref count, color);
    }

    private void Update() {
        //_reciever.UpdateUIRpc(timer, 0, count);
        if(Input.GetKeyDown(KeyCode.Space) && isLocalPlayer) {
            CmdClick();
        }
    }
}