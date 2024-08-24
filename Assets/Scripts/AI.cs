using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class AI : ISP {

    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; count++; StopAllCoroutines(); PutOnCooldown(); }, 
            delegate { timer += MathF.Round(Time.deltaTime, 3); },
            2.5f
        ));
        StartCoroutine(tryShoot(0.5f));
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    public override void OnStartClient() { 
        PutOnCooldown(); 
    } 

    public IEnumerator tryShoot(float seconds) {
        yield return new WaitForSeconds(seconds);
        CmdClick();
        StartCoroutine(tryShoot(seconds));
    }

    public IEnumerator wait(Action action, Action update, float seconds) {
        for (float i = 0; i < seconds;) {
            update?.Invoke();
            yield return new WaitForEndOfFrame();
            i += Time.deltaTime;
        }
        action?.Invoke();
    }
}