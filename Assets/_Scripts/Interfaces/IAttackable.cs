namespace _Scripts.Interfaces
{
    public interface IAttackable
    {
        public void ReceiveAttack(int damage);
        public bool IsDead();
        
        public void IsVulnerable();
        public void SetIsVulnerable(bool isVulnerable);
    }
}