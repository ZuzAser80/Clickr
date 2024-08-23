using Assets.Scripts.Unit;
using Assets.Scripts.Unit.Units;
using Mirror;
using UnityEngine;

public class BattleFieldSpawn : NetworkBehaviour {
    
    [Server]
    public void Spawn(Player owner, PathwalkingUnit unit) {
        var r = Instantiate(unit, owner.spawnPoint.position, Quaternion.identity);
        r.color = owner.color;
        NetworkServer.Spawn(r.gameObject, owner.gameObject);
        r.StartPathfindRpc(Vector3.zero);
    }

    [ServerCallback]
    public void SpawnBase(Player owner, BaseUnit baseUnit) {
        var r = Instantiate(baseUnit, owner.spawnPoint.position, Quaternion.identity);
        r.color = owner.color;
        NetworkServer.Spawn(r.gameObject, owner.gameObject);
    }

    [Server]
    public void Spawn(ISP owner, PathwalkingUnit unit) {
        var r = Instantiate(unit, owner.spawnPoint.position, Quaternion.identity);
        r.color = owner.color;
        NetworkServer.Spawn(r.gameObject, owner.gameObject);
        r.StartPathfindRpc(Vector3.zero);
    }
}
