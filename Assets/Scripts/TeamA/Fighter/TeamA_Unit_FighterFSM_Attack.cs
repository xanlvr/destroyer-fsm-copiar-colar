using UnityEngine;

public class TeamA_Unit_FighterFSM_Attack : Unit_Abstract<TeamA_Unit_FighterManager>
{
    private float timeSinceLastAttack = 0f;

    public override void EnterState(TeamA_Unit_FighterManager manager)
    {
        // Debug.Log("Fighter: Atacando!");
        timeSinceLastAttack = 0f;
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

        // Se saiu do range, volta para MOVE
        if (distanceToTarget > Team_Base.fighterAttackRange)
        {
            manager.SetState(manager.MoveState);
            return;
        }

        // Incrementa tempo desde último ataque
        timeSinceLastAttack += Time.deltaTime;

        // Se passou o intervalo, ataca
        if (timeSinceLastAttack >= Team_Base.fighterAttackInterval)
        {
            // Tenta atacar unidade inimiga Fighter
            if (manager.currentTarget.TryGetComponent<TeamA_Unit_FighterManager>(out TeamA_Unit_FighterManager enemyFighter))
            {
                enemyFighter.UnitTakeDamage(Team_Base.fighterAttackDamage);
                // Debug.Log($"Fighter atacou outro Fighter! Dano: {Team_Base.fighterAttackDamage}");
            }
            // Tenta atacar Destroyer inimigo
            else if (manager.currentTarget.TryGetComponent<TeamA_Unit_DestroyerManager>(out TeamA_Unit_DestroyerManager enemyDestroyer))
            {
                enemyDestroyer.UnitTakeDamage(Team_Base.fighterAttackDamage);
                // Debug.Log($"Fighter atacou um Destroyer! Dano: {Team_Base.fighterAttackDamage}");
            }
            // Tenta atacar torre
            else if (manager.currentTarget.TryGetComponent<TeamA_Health>(out TeamA_Health towerHealth))
            {
                towerHealth.TakeDamage(Team_Base.fighterAttackDamage);
                // Debug.Log($"Fighter atacou a torre! Dano: {Team_Base.fighterAttackDamage}");
            }

            timeSinceLastAttack = 0f;
        }
    }

    public override void ExitState(TeamA_Unit_FighterManager manager)
    {
        // Debug.Log("Fighter: Saindo de ATTACK");
        timeSinceLastAttack = 0f;
    }
}
