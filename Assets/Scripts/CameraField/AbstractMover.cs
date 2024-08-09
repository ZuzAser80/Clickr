using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractMover : MonoBehaviour {

    [SerializeField] private List<MoverNode> nodes = new List<MoverNode>();
    [SerializeField] private bool startAwake = true;

    public Action StartMoving;
    private Vector3 _i;

    private void Awake() {
        StartMoving += delegate { moveList(nodes); };
    }

    private void Start() {
        if(startAwake) { StartCoroutine(moveList(nodes)); }
    }

    private IEnumerator moveList(List<MoverNode> nodes) {
        foreach(var node in nodes) {
            //Debug.Log("started moving: " + node.Pos);
            yield return move(node);
        }
    }

    private IEnumerator move(MoverNode target) {
        _i = (Vector2)transform.position + target.Pos;
        while(Vector3.Distance(_i, transform.position) > 0.1f) {
            transform.position = Vector3.Lerp(transform.position, _i, target.MoveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(target.CooldownOnReached);
    }
    
    private void OnDrawGizmos() {
        foreach (var node in nodes) {
            Gizmos.color = Color.red;
            Gizmos.DrawCube((Vector2)transform.position + node.Pos, new Vector3(1, 1, 1));
        }
    }
}

[Serializable]
public class MoverNode {
    public Vector2 Pos;
    public float CooldownOnReached;
    [Header("Скорость")]
    public float MoveSpeed = 1;
}