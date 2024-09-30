using System.Collections;
using UnityEngine;

public class UnitExplosion : MonoBehaviour {
    
    public int rad;
    private float a = 1;
    private ParticleSystem sys;
    
    private void Start() {
        GetComponent<Renderer>().material.color = Color.white;
        sys = GetComponent<ParticleSystem>();
        transform.localScale *= rad*2;
        StartCoroutine(clr());
    }

    private IEnumerator clr() {
        if(a < -50) { sys.Play(); if(sys.isPlaying) { Destroy(this); yield return null; }}
        GetComponent<Renderer>().material.color = new Color(255, 255, 255, a);
        a -= 25f;
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(clr());
    }
}