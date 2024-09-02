using System;
using System.Collections;
using System.Collections.Generic;
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
    [SyncVar] public bool isLeft;

    [SerializeField] private GameObject config;
    [SerializeField] private GameObject ui;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private List<PathwalkingUnit> spawnables = new List<PathwalkingUnit>();
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float cameraSpeed = 1f;
    [SerializeField] private PathwalkingUnit baseUnitPrefab;

    public Transform spawnPoint;

    public Material material;

    private UIReciever _reciever;
    private Player _enemy;
    private float rotationX;
    private PathwalkingUnit baseUnit;

    #region Timer + count
    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; count++; StopAllCoroutines(); if(count < 1) { PutOnCooldown(); } }, 
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

    public virtual void SetEnemy(Player _) { _enemy = _; }

    public override void OnStartAuthority()
    {
        ui.SetActive(true);
        
        _reciever = ui.GetComponentInChildren<UIReciever>();
        cameraHolder.gameObject.SetActive(true);
        SpawnBase();
    }

    public override void OnStartClient() { 
        PutOnCooldown(); 
        
    } 

    #region Commands
    [Command]
    public void CmdSpawnUnit(int index) {
        FindObjectOfType<BattleFieldSpawn>().Spawn(this, spawnables[index]);
    }

    [Command]
    public void SpawnBase() {
        FindObjectOfType<BattleFieldSpawn>().SpawnBase(this, baseUnitPrefab, out baseUnit);
    }

    [Command]
    public virtual void CmdUpdateUI() {
        if(_enemy != null) {  _enemy.enemyTimer = timer; 
            //baseHp = baseUnit.GetHealth(); _enemy.enemyBaseHp = baseUnit.GetHealth(); 
            FindObjectOfType<UIBaseHpManager>().UpdateUI(baseUnit.GetHealth(), baseUnit.GetProperties().MaxHealth, isLeft);
        }   
    }

    [Command]
    public virtual void CmdClick() {
        FindObjectOfType<Cannon>().Shoot(config, ref count, this);
        if(count < 1) {
            PutOnCooldown();
        }
    }
    
    [Command]
    public void WinCmd() {
        Debug.Log("Player: " + this + " Won");
    }
    [Command]
    public virtual void LoseCmd() {
        _enemy.WinCmd();
        Debug.Log("Player: " + this + " Won");
    }
    #endregion

    #region RPCs

    [TargetRpc]
    public void SpawnUnit(int index) {
        if(!isLocalPlayer) { return; }
        CmdSpawnUnit(index);
    } 

    public void AddOne() {
        count++;
    }

    #endregion

    public Vector3 GetEnemyBasePos() {
        if(_enemy == null) { return Vector3.zero; }
        return _enemy.spawnPoint.position;
    }

    private void Update() {
        if(!isLocalPlayer) { return; }
        CmdUpdateUI();
        _reciever.UpdateUIRpc(timer, enemyTimer, count);
        if(Input.GetKeyDown(KeyCode.Space)) {
            CmdClick();
        }
        // Camera rotation
        cameraHolder.position = new Vector3(cameraHolder.position.x + Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime, cameraHolder.position.y, cameraHolder.position.z);
        
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -45, 45);
        _camera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        cameraHolder.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); 
    }
}