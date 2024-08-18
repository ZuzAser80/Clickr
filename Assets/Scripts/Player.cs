using System;
using System.Collections;
using Assets.Scripts.Sides;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour {
    public GameObject rofl;
    public Color color;
    public int count;
    public float timer;

    [SerializeField] private GameObject uiHolder;
    [SerializeField] private UIReciever _reciever;

    public override void OnStartAuthority()
    {
        Debug.Log("OnStartAuthority");
        uiHolder.SetActive(true);
        color = UnityEngine.Random.ColorHSV();
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

    // todo: ui work

    [Command]
    public void CmdClick() {
        Debug.Log("NIGGER#2: " + this + " : " + color);
        PutOnCooldown();
        FindObjectOfType<CameraFieldSide>().HandleClick(NetworkClient.localPlayer);
    }

    private void Update() {
        _reciever.UpdateUIRpc(timer, 0, count);
        if(Input.GetKeyDown(KeyCode.Space) && isLocalPlayer && authority) {
            CmdClick();
        }
    }
}