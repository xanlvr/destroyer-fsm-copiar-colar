# DESTROYER FSM - GUIA COPIE E COLE (VERSÃO CORRIGIDA) 🎮

## 📋 IMPORTANTE - ESTRUTURA DO PROJETO

Seu projeto já tem:
- ✅ Team_Base.cs (CONSTANTES)
- ✅ TeamX_Health (TORRES - método TakeDamage())
- ✅ TeamX_Manager (SPAWN de unidades)

Você vai ADICIONAR:
- 🆕 TeamA_Unit_DestroyerManager (cada unidade tem um)
- 🆕 4 Estados do Destroyer (Search, Move, Attack, Death)

---

## 📁 ESTRUTURA DE PASTAS

Seus scripts já estão em `Assets/Scripts/TeamA/`

Você precisa criar APENAS:
```
Assets/Scripts/TeamA/
└── Destroyer/
    ├── TeamA_Unit_DestroyerFSM_Search.cs
    ├── TeamA_Unit_DestroyerFSM_Move.cs
    ├── TeamA_Unit_DestroyerFSM_Attack.cs
    └── TeamA_Unit_DestroyerFSM_Death.cs
```

E em `Assets/Scripts/TeamA/Managers/`:
```
├── TeamA_Unit_DestroyerManager.cs
```

---

## ✅ PASSO 1: CRIAR PASTA DESTROYER

1. Em Assets/Scripts/TeamA, clique com botão DIREITO
2. Create → Folder
3. Nomeie: `Destroyer`

---

## ✅ PASSO 2: CRIAR 4 ESTADOS DO DESTROYER

### **2.1 - TeamA_Unit_DestroyerFSM_Search.cs**

**Local:** Assets/Scripts/TeamA/Destroyer/TeamA_Unit_DestroyerFSM_Search.cs

1. Clique com botão DIREITO em Destroyer
2. Create → C# Script
3. Nomeie: `TeamA_Unit_DestroyerFSM_Search`
4. Abra e **APAGUE TUDO**
5. **COPIE E COLE:**

```csharp
using UnityEngine;

public class TeamA_Unit_DestroyerFSM_Search : Unit_Abstract<TeamA_Unit_DestroyerManager>
{
    public override void EnterState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer: Procurando torre inimiga...");
    }

    public override void UpdateState(TeamA_Unit_DestroyerManager manager)
    {
        // Procura pela torre inimiga usando Raycast
        RaycastHit[] hits = Physics.SphereCastAll(
            manager.transform.position,
            Team_Base.destroyerSearchRay,
            Vector3.forward,
            0f
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("EnemyTower"))
            {
                manager.currentTarget = hit.collider.transform;
                manager.SetState(manager.MoveState);
                return;
            }
        }
    }

    public override void ExitState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer: Saindo de SEARCH");
    }
}
```

**Salve (Ctrl+S)**

---

### **2.2 - TeamA_Unit_DestroyerFSM_Move.cs**

**Local:** Assets/Scripts/TeamA/Destroyer/TeamA_Unit_DestroyerFSM_Move.cs

1. Clique com botão DIREITO em Destroyer
2. Create → C# Script
3. Nomeie: `TeamA_Unit_DestroyerFSM_Move`
4. Abra e **APAGUE TUDO**
5. **COPIE E COLE:**

```csharp
using UnityEngine;

public class TeamA_Unit_DestroyerFSM_Move : Unit_Abstract<TeamA_Unit_DestroyerManager>
{
    public override void EnterState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer: Movendo em direção à torre!");
    }

    public override void UpdateState(TeamA_Unit_DestroyerManager manager)
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
        if (distanceToTarget <= Team_Base.destroyerAttackRange)
        {
            manager.SetState(manager.AttackState);
            return;
        }

        // Move em direção ao alvo
        Vector3 direction = (manager.currentTarget.position - manager.transform.position).normalized;
        manager.transform.position += direction * Team_Base.destroyerMoveSpeed;
    }

    public override void ExitState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer: Saindo de MOVE");
    }
}
```

**Salve (Ctrl+S)**

---

### **2.3 - TeamA_Unit_DestroyerFSM_Attack.cs**

**Local:** Assets/Scripts/TeamA/Destroyer/TeamA_Unit_DestroyerFSM_Attack.cs

1. Clique com botão DIREITO em Destroyer
2. Create → C# Script
3. Nomeie: `TeamA_Unit_DestroyerFSM_Attack`
4. Abra e **APAGUE TUDO**
5. **COPIE E COLE:**

```csharp
using UnityEngine;

public class TeamA_Unit_DestroyerFSM_Attack : Unit_Abstract<TeamA_Unit_DestroyerManager>
{
    private float timeSinceLastAttack = 0f;

    public override void EnterState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer: Atacando torre!");
        timeSinceLastAttack = 0f;
    }

    public override void UpdateState(TeamA_Unit_DestroyerManager manager)
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
        if (distanceToTarget > Team_Base.destroyerAttackRange)
        {
            manager.SetState(manager.MoveState);
            return;
        }

        // Incrementa tempo desde último ataque
        timeSinceLastAttack += Time.deltaTime;

        // Se passou o intervalo, ataca
        if (timeSinceLastAttack >= Team_Base.destroyerAttackInterval)
        {
            // Tenta obter o componente TeamA_Health da torre
            if (manager.currentTarget.TryGetComponent<TeamA_Health>(out TeamA_Health towerHealth))
            {
                towerHealth.TakeDamage(Team_Base.destroyerAttackDamage);
                // Debug.Log($"Destroyer atacou a torre! Dano: {Team_Base.destroyerAttackDamage}");
            }

            timeSinceLastAttack = 0f;
        }
    }

    public override void ExitState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer: Saindo de ATTACK");
        timeSinceLastAttack = 0f;
    }
}
```

**Salve (Ctrl+S)**

---

### **2.4 - TeamA_Unit_DestroyerFSM_Death.cs**

**Local:** Assets/Scripts/TeamA/Destroyer/TeamA_Unit_DestroyerFSM_Death.cs

1. Clique com botão DIREITO em Destroyer
2. Create → C# Script
3. Nomeie: `TeamA_Unit_DestroyerFSM_Death`
4. Abra e **APAGUE TUDO**
5. **COPIE E COLE:**

```csharp
using UnityEngine;

public class TeamA_Unit_DestroyerFSM_Death : Unit_Abstract<TeamA_Unit_DestroyerManager>
{
    public override void EnterState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer morreu!");
        manager.gameObject.SetActive(false);
    }

    public override void UpdateState(TeamA_Unit_DestroyerManager manager)
    {
        // Não faz nada enquanto está morto
    }

    public override void ExitState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer ressuscitou (não deveria acontecer)");
    }
}
```

**Salve (Ctrl+S)**

---

## ✅ PASSO 3: CRIAR MANAGER DO DESTROYER

**Local:** Assets/Scripts/TeamA/Managers/TeamA_Unit_DestroyerManager.cs

1. Clique com botão DIREITO em Assets/Scripts/TeamA/Managers
2. Create → C# Script
3. Nomeie: `TeamA_Unit_DestroyerManager`
4. Abra e **APAGUE TUDO**
5. **COPIE E COLE:**

```csharp
using UnityEngine;

public class TeamA_Unit_DestroyerManager : Team_Base
{
    // FSM States
    private Unit_Abstract<TeamA_Unit_DestroyerManager> currentState;
    public TeamA_Unit_DestroyerFSM_Search SearchState { get; private set; }
    public TeamA_Unit_DestroyerFSM_Move MoveState { get; private set; }
    public TeamA_Unit_DestroyerFSM_Attack AttackState { get; private set; }
    public TeamA_Unit_DestroyerFSM_Death DeathState { get; private set; }

    // Target
    public Transform currentTarget;

    // Health da unidade
    private int currentHealth;

    private void Awake()
    {
        // Inicializa os estados
        SearchState = new TeamA_Unit_DestroyerFSM_Search();
        MoveState = new TeamA_Unit_DestroyerFSM_Move();
        AttackState = new TeamA_Unit_DestroyerFSM_Attack();
        DeathState = new TeamA_Unit_DestroyerFSM_Death();

        // Começa no estado de SEARCH
        currentState = SearchState;

        // Inicializa a vida
        currentHealth = Team_Base.destroyerMaxHealth;
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
    public void SetState(Unit_Abstract<TeamA_Unit_DestroyerManager> newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    // Método para receber dano (chamado quando a unidade é atacada)
    public void UnitTakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        // Debug.Log($"Destroyer recebeu {damageAmount} de dano. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            SetState(DeathState);
        }
    }
}
```

**Salve (Ctrl+S)**

---

## ✅ PASSO 4: CONFIGURAR NO INSPECTOR

### **4.1 - Criar GameObject do Destroyer**

1. Na Hierarquia, clique com botão DIREITO
2. 3D Object → Cube (ou seu modelo)
3. Renomeie para: `Destroyer_Unit`
4. Com o GameObject selecionado, clique em **Add Component**
5. Procure por: `TeamA_Unit_DestroyerManager`
6. Clique para adicionar

### **4.2 - Configurar Tags**

1. Com `Destroyer_Unit` selecionado, vá em **Tag** (topo do Inspector)
2. Clique em **Add Tag**
3. Crie uma tag: `DestroyerUnit`
4. Atribua ao GameObject

### **4.3 - Posicionar**

1. Selecione `Destroyer_Unit`
2. No Inspector, mude a posição para X=0, Y=1, Z=0

---

## ✅ PASSO 5: TESTAR

1. Pressione **Play** (ou Ctrl+P)
2. O Destroyer deve:
   - ✅ Procurar a torre inimiga
   - ✅ Mover em direção a ela
   - ✅ Atacar quando chegar perto
   - ✅ Aparecer "EnemyTower" com tag correta

---

## ⚠️ SE DER ERRO:

1. **"Não encontrou TeamA_Health"** → Certifique-se que a torre tem o componente `TeamA_Health` com método `TakeDamage()`
2. **"Unit_Abstract não encontrado"** → Verifique se `Unit_Abstract.cs` está em `Assets/Scripts/Base/`
3. **"Team_Base não encontrado"** → Verifique se `Team_Base.cs` está em `Assets/Scripts/Base/`

---

## ✅ PRONTO!

Você implementou com sucesso a FSM do Destroyer! 🎉

**Próximas melhorias:**
- [ ] Adicionar Fighter (com estilo tático)
- [ ] Adicionar anims aos estados
- [ ] Adicionar sons
