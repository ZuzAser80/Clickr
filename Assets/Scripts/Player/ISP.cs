using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Unit;
using Assets.Scripts.Unit.Units;
using Mirror;
using UnityEngine;

public abstract class ISP : NetworkBehaviour {

    [SyncVar] public Color color;
    [SyncVar] public int count;
    [SyncVar] public float timer;
    [SyncVar] public float enemyTimer;
    [SyncVar] public float baseHp;
    [SyncVar] public float enemyBaseHp;
    [SyncVar] public bool isLeft;
    [SyncVar] public bool isOnCooldown = false;

    [SyncVar] protected int localCount;

    [SerializeField] private GameObject config;
    [SerializeField] private List<PathwalkingUnit> spawnables = new List<PathwalkingUnit>();
    [SerializeField] private PathwalkingUnit baseUnitPrefab;
    [SerializeField] private List<int> maxReq = new List<int>();
    [SerializeField] private List<int> curReq = new List<int>();

    public Transform spawnPoint;
    protected ISP  _enemy;
    protected PathwalkingUnit baseUnit;

    public void SetEnemy(ISP _) { _enemy = _; }

    public PathwalkingUnit GetBase() {
        return baseUnit;
    }

    public virtual void WinCmd() {
        Debug.Log("Player: " + this + " Won");
    }

    public virtual void LoseCmd() {
        Debug.Log("Player: " + this + " LoseCmd");
        
    }

    public Vector3 GetEnemyBasePos() {
        if(_enemy == null) { return Vector3.zero; }
        return _enemy.spawnPoint.position;
    }

    public override void OnStartAuthority()
    {
        SpawnBase();
    }

    public virtual void UpdateBases() { }

    public void UpdateEnemyTimer() {
        if(_enemy != null) { enemyTimer = _enemy.timer;  _enemy.enemyBaseHp = baseHp; }
    }

    [Command]
    public void CmdSpawnUnit(int index) {
        if(curReq.Count < index || maxReq.Count < index) { return; }
        if (curReq[index] < maxReq[index]-1) {
            curReq[index]++;
        } else {
            curReq[index] = 0;
            FindObjectOfType<BattleFieldSpawn>().Spawn(this, spawnables[index]);
        }
    }

    [Command]
    public void SpawnBase() {
        FindObjectOfType<BattleFieldSpawn>().SpawnBase(this, baseUnitPrefab, out baseUnit);
        UpdateBases();
    }

    [Command]
    public virtual void CmdClick() {
        FindObjectOfType<Cannon>().Shoot(config, ref count,  this);
    }

    #region RPCs

    [TargetRpc]
    public void SpawnUnit(int index) {

        CmdSpawnUnit(index);
    } 

    [TargetRpc]
    public void AddOne() {
        localCount++;
    }

    #endregion
}