using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class AI : ISP {

    private float m = 0;

    public void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; if(count < 1) { count += localCount + 1; localCount = 0; } StopCoroutine("wait"); PutOnCooldown(); }, 
            delegate { timer += MathF.Round(Time.deltaTime / 4f, 3); },
            4f
        ));
        StartCoroutine(tryShoot(1f + m));
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    public override void OnStartClient() { 
        PutOnCooldown(); 
        StartCoroutine(addMore());
    } 

    private IEnumerator addMore() {
        m += 0.5f;
        yield return new WaitForSeconds(120);
        StartCoroutine(addMore());
    }

    public IEnumerator tryShoot(float seconds) {
        //if(isOnCooldown) {seconds += 5;}
        if(c == 0) { StartCoroutine(tryShoot(seconds)); yield return null; }
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