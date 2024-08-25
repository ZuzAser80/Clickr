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

    [SerializeField] private GameObject config;
    [SerializeField] private List<PathwalkingUnit> spawnables = new List<PathwalkingUnit>();
    [SerializeField] private PathwalkingUnit baseUnitPrefab;

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

    public void UpdateEnemyTimer() {
        if(_enemy != null) { enemyTimer = _enemy.timer;  _enemy.enemyBaseHp = baseHp; }
    }

    [Command]
    public void CmdSpawnUnit(int index) {
        FindObjectOfType<BattleFieldSpawn>().Spawn(this, spawnables[index]);
    }

    [Command]
    public void SpawnBase() {
        FindObjectOfType<BattleFieldSpawn>().SpawnBase(this, baseUnitPrefab, out baseUnit);
    }

    [Command]
    public void CmdClick() {
        FindObjectOfType<Cannon>().Shoot(config, ref count, this);
    }

    #region RPCs

    [TargetRpc]
    public void SpawnUnit(int index) {
        CmdSpawnUnit(index);
    } 

    [TargetRpc]
    public void AddOne() {
        Debug.Log("ADDED ONE");
        count++;
    }

    #endregion
}