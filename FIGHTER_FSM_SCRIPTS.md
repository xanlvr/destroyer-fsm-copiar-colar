# FIGHTER FSM - SCRIPTS PRONTOS PARA COPIAR 🎯

## 📝 IMPORTANTE

O **Fighter** é diferente do Destroyer:
- ❌ NÃO ignora unidades
- ✅ Ataca TUDO na frente (unidades E torre)
- ✅ Busca por qualquer inimigo próximo

---

## 📁 ESTRUTURA

Crie uma pasta **Fighter** em `Assets/Scripts/TeamA/`

```
Assets/Scripts/TeamA/
└── Fighter/
    ├── TeamA_Unit_FighterFSM_Search.cs
    ├── TeamA_Unit_FighterFSM_Move.cs
    ├── TeamA_Unit_FighterFSM_Attack.cs
    └── TeamA_Unit_FighterFSM_Death.cs
```

E em `Assets/Scripts/TeamA/Managers/`:
```
├── TeamA_Unit_FighterManager.cs
```

---

## ✅ PASSO 1: CRIAR FOLDER FIGHTER

1. Clique direito em `Assets/Scripts/TeamA/`
2. Create → Folder
3. Nomeie: `Fighter`

---

## ✅ PASSO 2: CRIAR OS 4 ESTADOS DO FIGHTER

### **2.1 - TeamA_Unit_FighterFSM_Search.cs**

**Local:** Assets/Scripts/TeamA/Fighter/TeamA_Unit_FighterFSM_Search.cs

```csharp
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
```

---

### **2.2 - TeamA_Unit_FighterFSM_Move.cs**

**Local:** Assets/Scripts/TeamA/Fighter/TeamA_Unit_FighterFSM_Move.cs

```csharp
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
```

---

### **2.3 - TeamA_Unit_FighterFSM_Attack.cs**

**Local:** Assets/Scripts/TeamA/Fighter/TeamA_Unit_FighterFSM_Attack.cs

```csharp
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
            // Tenta atacar unidade inimiga
            if (manager.currentTarget.TryGetComponent<TeamA_Unit_FighterManager>(out TeamA_Unit_FighterManager enemyFighter))
            {
                enemyFighter.UnitTakeDamage(Team_Base.fighterAttackDamage);
                // Debug.Log($"Fighter atacou outro Fighter! Dano: {Team_Base.fighterAttackDamage}");
            }
            // Tenta atacar destroyer inimigo
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
```

---

### **2.4 - TeamA_Unit_FighterFSM_Death.cs**

**Local:** Assets/Scripts/TeamA/Fighter/TeamA_Unit_FighterFSM_Death.cs

```csharp
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
```

---

## ✅ PASSO 3: CRIAR MANAGER DO FIGHTER

**Local:** Assets/Scripts/TeamA/Managers/TeamA_Unit_FighterManager.cs

```csharp
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
```

---

## ✅ PASSO 4: ADICIONAR CONSTANTES NO Team_Base.cs

Abra seu **Team_Base.cs** e adicione estas linhas com as constantes do Fighter:

```csharp
// FIGHTER
public const int fighterMaxHealth = 30;
public const int fighterAttackDamage = 8;
public const float fighterMoveSpeed = 5f;
public const float fighterAttackRange = 2f;
public const float fighterAttackInterval = 1.5f;
public const float fighterSearchRay = 15f;
```

---

## ✅ PASSO 5: CONFIGURAR NO INSPECTOR

1. Crie um novo GameObject: `Fighter_Unit`
2. Adicione o componente: `TeamA_Unit_FighterManager`
3. Atribua a tag: `FighterUnit` (crie se não existir)
4. Posicione em X=2, Y=1, Z=0

---

## ✅ PASSO 6: TESTAR

1. Pressione **Play**
2. O Fighter deve:
   - ✅ Procurar inimigos (unidades E torre)
   - ✅ Mover em direção ao alvo
   - ✅ Atacar qualquer coisa que encontrar
   - ✅ Ser atacado por outras unidades

---

## 🎯 DIFERENÇAS DESTROYER vs FIGHTER

| | Destroyer | Fighter |
|---|---|---|
| **Alvo** | Só torre | Tudo (unidades + torre) |
| **Search** | Raycast por "EnemyTower" | Raycast por "EnemyUnit" OU "EnemyTower" |
| **Attack** | Ataca só torres | Ataca unidades E torres |
| **Vida** | 50 HP | 30 HP |
| **Dano** | 15 | 8 |
| **Velocidade** | 4 | 5 |

---

## ✅ PRONTO!

Você tem 2 tipos de unidades agora! 🎉

Destruidor (foca torre) + Fighter (ataca tudo)
