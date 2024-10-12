using System.Collections;
using UnityEngine;

public class UnitExplosion : MonoBehaviour {
    
    public int rad;
    private float a = 1;
    private ParticleSystem sys;
    
    private void Start() {
        //GetComponent<Renderer>().material.color = Color.white;
        sys = GetComponent<ParticleSystem>();
        transform.localScale *= rad*2;
        StartCoroutine(clr());
    }

    private IEnumerator clr() {
        if(a < 0) { if(!sys.isPlaying) {sys.Play();} else { Destroy(this, 8); yield return null; }}
        GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, a);
        a -= 25f;
        yield return new WaitForSeconds(1f);
        StartCoroutine(clr());
    }
}