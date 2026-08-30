using UnityEngine;

public class TeamA_Unit_FighterFSM_Death : Unit_Abstract<TeamA_Unit_FighterManager>
{
    public override void EnterState(TeamA_Unit_FighterManager manager)
    {
        // Debug.Log("Fighter morreu!");
        manager.gameObject.SetActive(false);
    }

    public override void UpdateState(TeamA_Unit_FighterManager manager)
    {
        // Não faz nada enquanto está morto
    }

    public override void ExitState(TeamA_Unit_FighterManager manager)
    {
        // Debug.Log("Fighter ressuscitou (não deveria acontecer)");
    }
}
