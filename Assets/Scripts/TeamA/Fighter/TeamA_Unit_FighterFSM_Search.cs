using UnityEngine;

public class TeamA_Unit_FighterFSM_Search : Unit_Abstract<TeamA_Unit_FighterManager>
{
    public override void EnterState(TeamA_Unit_FighterManager manager)
    {
        // Debug.Log("Fighter: Procurando inimigos...");
    }

    public override void UpdateState(TeamA_Unit_FighterManager manager)
    {
        // Procura por QUALQUER inimigo (unidades OU torre)
        RaycastHit[] hits = Physics.SphereCastAll(
            manager.transform.position,
            Team_Base.fighterSearchRay,
            Vector3.forward,
            0f
        );

        foreach (RaycastHit hit in hits)
        {
            // Procura por unidades inimigas OU torre
            if (hit.collider.CompareTag("EnemyUnit") || hit.collider.CompareTag("EnemyTower"))
            {
                manager.currentTarget = hit.collider.transform;
                manager.SetState(manager.MoveState);
                return;
            }
        }
    }

    public override void ExitState(TeamA_Unit_FighterManager manager)
    {
        // Debug.Log("Fighter: Saindo de SEARCH");
    }
}
