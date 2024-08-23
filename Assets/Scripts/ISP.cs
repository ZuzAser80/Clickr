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

    [SerializeField] private GameObject config;
    [SerializeField] private List<PathwalkingUnit> spawnables = new List<PathwalkingUnit>();

    public Transform spawnPoint;
    protected ISP  _enemy;
    protected BaseUnit _baseUnit;

    public void SetEnemy(ISP _) { _enemy = _; }

    public override void OnStartAuthority()
    {
        _baseUnit = GetComponentInChildren<BaseUnit>();
    }

    public void UpdateEnemyTimer() {
        if(_enemy != null) { enemyTimer = _enemy.timer;  _enemy.enemyBaseHp = baseHp; }
    }

    [Command]
    public void CmdSpawnUnit(int index) {
        FindObjectOfType<BattleFieldSpawn>().Spawn(this, spawnables[index]);
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