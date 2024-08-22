using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.UI;
using Assets.Scripts.Unit;
using Assets.Scripts.Unit.Units;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour {

    [SyncVar] public Color color;
    [SyncVar] public int count;
    [SyncVar] public float timer;
    [SyncVar] public float enemyTimer;

    [SerializeField] private GameObject config;
    [SerializeField] private GameObject ui;
    [SerializeField] private Transform _camera;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float cameraSpeed = 1f;
    public PathwalkingUnit def;

    public Material material;

    private UIReciever _reciever;
    private Player _enemy;
    private float rotationX;

    #region Timer + count
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
    #endregion

    public void SetEnemy(Player _) { _enemy = _; }

    public override void OnStartAuthority()
    {
        ui.SetActive(true);
        _reciever = ui.GetComponentInChildren<UIReciever>();
        _camera.gameObject.SetActive(true);
    }

    public override void OnStartClient() { 
        PutOnCooldown(); 
    } 

    #region Commands
    [Command]
    public void CmdSpawnUnit() {
        FindObjectOfType<BattleFieldSpawn>().Spawn(this, def);
    }

    [Command]
    public void CmdUpdateUI() {
        if(_enemy != null) {  _enemy.enemyTimer = timer; }   
    }

    [Command]
    public void CmdClick() {
        FindObjectOfType<Cannon>().Shoot(config, ref count, this);
        if(_enemy != null) {  _enemy.enemyTimer = timer; }   
    }
    #endregion

    #region RPCs

    [TargetRpc]
    public void SpawnUnit() {
        if(!isLocalPlayer) { return; }
        Debug.Log("SpawnUnit : " + gameObject);
        CmdSpawnUnit();
    } 

    #endregion

    private void Update() {
        if(!isLocalPlayer) { return; }
        CmdUpdateUI();
        _reciever.UpdateUIRpc(timer, enemyTimer, count);
        if(Input.GetKeyDown(KeyCode.Space)) {
            CmdClick();
        }
        // Camera rotation
        transform.position = new Vector3(_camera.position.x + Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime, _camera.position.y, _camera.position.z);
        
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -45, 45);
        _camera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); 
    }
}