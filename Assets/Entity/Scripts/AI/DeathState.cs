namespace Entity.Scripts.AI
{
    public class DeathState : AIState
    {
        public override void Enter()
        {
            //Alert near enemies of the death
            entityWeapons.StopShooting();
        }
    }
}