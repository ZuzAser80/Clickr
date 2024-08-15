namespace Assets.Scripts.Unit {
    public interface IDamagable {
        public abstract void Damage(float amount);
        public abstract void Die();
    }
}