using UnityEngine;

public class TeamA_Unit_FighterManager : Team_Base
{
    // FSM States
    private Unit_Abstract<TeamA_Unit_FighterManager> currentState;
    public TeamA_Unit_FighterFSM_Search SearchState { get; private set; }
    public TeamA_Unit_FighterFSM_Move MoveState { get; private set; }
    public TeamA_Unit_FighterFSM_Attack AttackState { get; private set; }
    public TeamA_Unit_FighterFSM_Death DeathState { get; private set; }

    // Target
    public Transform currentTarget;

    // Health da unidade
    private int currentHealth;

    private void Awake()
    {
        // Inicializa os estados
        SearchState = new TeamA_Unit_FighterFSM_Search();
        MoveState = new TeamA_Unit_FighterFSM_Move();
        AttackState = new TeamA_Unit_FighterFSM_Attack();
        DeathState = new TeamA_Unit_FighterFSM_Death();

        // Começa no estado de SEARCH
        currentState = SearchState;

        // Inicializa a vida
        currentHealth = Team_Base.fighterMaxHealth;
    }

    private void Start()
    {
        currentState.EnterState(this);
    }

    private void Update()
    {
        // Verifica se está morto
        if (currentHealth <= 0)
        {
            if (currentState != DeathState)
            {
                currentState.ExitState(this);
                currentState = DeathState;
                currentState.EnterState(this);
            }
            return;
        }

        // Atualiza o estado atual
        currentState.UpdateState(this);
    }

    // Método para mudar de estado
    public void SetState(Unit_Abstract<TeamA_Unit_FighterManager> newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    // Método para receber dano (chamado quando a unidade é atacada)
    public void UnitTakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        // Debug.Log($"Fighter recebeu {damageAmount} de dano. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            SetState(DeathState);
        }
    }
}
