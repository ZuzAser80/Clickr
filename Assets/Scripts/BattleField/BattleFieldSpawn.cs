using Assets.Scripts.Unit;
using Assets.Scripts.Unit.Units;
using Mirror;
using UnityEngine;

public class BattleFieldSpawn : NetworkBehaviour {

    private PathwalkingUnit cache;
    private PathwalkingUnit cache1;
    
    [Server]
    public void Spawn(Player owner, PathwalkingUnit unit) {
        cache = Instantiate(unit, owner.spawnPoint.position, Quaternion.identity);
        cache.color = owner.color;
        NetworkServer.Spawn(cache.gameObject, owner.gameObject);
        cache.StartPathfindRpc(owner.GetEnemyBasePos() + Vector3.forward * Random.Range(-5, 15));
    }

    [ServerCallback]
    public void SpawnBase(Player owner, PathwalkingUnit baseUnit, out PathwalkingUnit spawned) {
        cache1 = Instantiate(baseUnit, owner.spawnPoint.position, Quaternion.identity);
        cache1.color = owner.color;
        spawned = cache1;
        NetworkServer.Spawn(cache1.gameObject, owner.gameObject);
    }

    [Server]
    public void Spawn(ISP owner, PathwalkingUnit unit) {
        cache = Instantiate(unit, owner.spawnPoint.position + Vector3.forward * Random.Range(-5, 30), Quaternion.identity);
        cache.color = owner.color;
        NetworkServer.Spawn(cache.gameObject, owner.gameObject);
        cache.StartPathfindRpc(owner.GetEnemyBasePos()+ Vector3.forward * Random.Range(-5, 15));
    }

    [ServerCallback]
    public void SpawnBase(ISP owner, PathwalkingUnit baseUnit, out PathwalkingUnit spawned) {
        cache1 = Instantiate(baseUnit, owner.spawnPoint.position, Quaternion.identity);
        cache1.color = owner.color;
        spawned = cache1;
        NetworkServer.Spawn(cache1.gameObject, owner.gameObject);
    }
}
