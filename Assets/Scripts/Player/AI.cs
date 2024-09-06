using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class AI : ISP {

    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; if(count < 1) { count += localCount + 1; localCount = 0; } StopAllCoroutines(); PutOnCooldown(); }, 
            delegate { timer += MathF.Round(Time.deltaTime / 2.5f, 3); },
            2.5f
        ));
        StartCoroutine(tryShoot(2.5f));
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    public override void OnStartClient() { 
        PutOnCooldown(); 
    } 

    public IEnumerator tryShoot(float seconds) {
        //if(isOnCooldown) {seconds += 5;}
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