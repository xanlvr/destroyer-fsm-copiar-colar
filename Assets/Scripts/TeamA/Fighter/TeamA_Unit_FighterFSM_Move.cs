using UnityEngine;

public class TeamA_Unit_FighterFSM_Move : Unit_Abstract<TeamA_Unit_FighterManager>
{
    public override void EnterState(TeamA_Unit_FighterManager manager)
    {
        // Debug.Log("Fighter: Movendo em direção ao inimigo!");
    }

    public override void UpdateState(TeamA_Unit_FighterManager manager)
    {
        // Se perdeu o alvo, volta para SEARCH
        if (manager.currentTarget == null)
        {
            manager.SetState(manager.SearchState);
            return;
        }

        float distanceToTarget = Vector3.Distance(
            manager.transform.position,
            manager.currentTarget.position
        );

        // Se chegou perto, muda para ATTACK
        if (distanceToTarget <= Team_Base.fighterAttackRange)
        {
            manager.SetState(manager.AttackState);
            return;
        }

        // Move em direção ao alvo
        Vector3 direction = (manager.currentTarget.position - manager.transform.position).normalized;
        manager.transform.position += direction * Team_Base.fighterMoveSpeed;
    }

    public override void ExitState(TeamA_Unit_FighterManager manager)
    {
        // Debug.Log("Fighter: Saindo de MOVE");
    }
}
