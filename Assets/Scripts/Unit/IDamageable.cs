using Mirror;

namespace Assets.Scripts.Unit {
    public interface IDamagable {
        [ClientRpc] public void Damage(float amount);
        [ClientRpc] public void Die();
    }
}