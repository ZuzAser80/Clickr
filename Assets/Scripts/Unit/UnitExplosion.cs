using System.Collections;
using UnityEngine;

public class UnitExplosion : MonoBehaviour {
    
    public int rad;
    private float a = 1;
    
    private void Start() {
        GetComponent<Renderer>().material.color = Color.white;
        transform.localScale *= rad*2;
        StartCoroutine(clr());
    }

    private IEnumerator clr() {
        if(a < 0) { Destroy(this); yield return null; }
        GetComponent<Renderer>().material.color = new Color(255, 255, 255, a);
        a -= 0.01f;
        yield return new WaitForSeconds(0.004f);
        StartCoroutine(clr());
    }
}