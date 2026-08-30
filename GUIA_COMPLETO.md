# DESTROYER FSM - GUIA COPIE E COLE 🎮

## 📋 ÍNDICE DE CONTEÚDO

1. **PASSO 1** - Criar Pastas no Unity
2. **PASSO 2** - Copiar Script Base (Team_Base.cs)
3. **PASSO 3** - Copiar Script Abstract (Unit_Abstract.cs)
4. **PASSO 4** - Copiar Script Health (Unit_Health.cs)
5. **PASSO 5** - Copiar 4 Estados do Destroyer
6. **PASSO 6** - Copiar Manager do Destroyer
7. **PASSO 7** - Configurar no Inspector
8. **PASSO 8** - Testar Tudo

---

## ✅ PASSO 1: CRIAR PASTAS NO UNITY

**Abra seu projeto Unity e crie essa estrutura de pastas:**

```
Assets/
└── Scripts/
    ├── Base/
    ├── TeamA/
    │   ├── Managers/
    │   └── Destroyer/
```

**Como fazer:**
1. Em Assets, clique com botão DIREITO
2. Create → Folder
3. Nomeie: "Scripts"
4. Dentro de Scripts, crie: "Base"
5. Dentro de Scripts, crie: "TeamA"
6. Dentro de TeamA, crie: "Managers"
7. Dentro de TeamA, crie: "Destroyer"

---

## 📝 PASSO 2: COPIAR Team_Base.cs

**Local:** `Assets/Scripts/Base/Team_Base.cs`

**Como fazer:**
1. Clique com botão DIREITO em Assets/Scripts/Base
2. Create → C# Script
3. Nomeie: `Team_Base`
4. Abra o arquivo e **APAGUE TUDO**
5. **COPIE** o código abaixo e **COLE** no arquivo:

---

### 🔽 COPIE E COLE ISSO EM Team_Base.cs:

```csharp
using UnityEngine;

public class Team_Base : MonoBehaviour
{
    //Propriedades da Base Principal
    public const float unitSpawnTimer = 5f;
    public const int baseHitPoints = 500; 
    
    
    //Propriedades para o Destroyer
    public const float destroyerMoveSpeed = 0.005f;
    public const float destroyerAttackRange = 1.25f;
    public const float destroyerAttackInterval = 3f;
    public const float destroyerSearchRay = 30f;
    public const int destroyerAttackDamage = 15;
    public const int destroyerMaxHealth = 125;
    
}
```

**Salve o arquivo (Ctrl+S)**

---

## 📝 PASSO 3: COPIAR Unit_Abstract.cs

**Local:** `Assets/Scripts/Base/Unit_Abstract.cs`

**Como fazer:**
1. Clique com botão DIREITO em Assets/Scripts/Base
2. Create → C# Script
3. Nomeie: `Unit_Abstract`
4. Abra o arquivo e **APAGUE TUDO**
5. **COPIE** o código abaixo e **COLE** no arquivo:

---

### 🔽 COPIE E COLE ISSO EM Unit_Abstract.cs:

```csharp
using UnityEngine;

public abstract class Unit_Abstract<ManagerType>
{
    public abstract void EnterState(ManagerType manager);

    public abstract void UpdateState(ManagerType manager);

    public abstract void ExitState(ManagerType manager);
}
```

**Salve o arquivo (Ctrl+S)**

---

## 📝 PASSO 4: COPIAR Unit_Health.cs

**Local:** `Assets/Scripts/Base/Unit_Health.cs`

**Como fazer:**
1. Clique com botão DIREITO em Assets/Scripts/Base
2. Create → C# Script
3. Nomeie: `Unit_Health`
4. Abra o arquivo e **APAGUE TUDO**
5. **COPIE** o código abaixo e **COLE** no arquivo:

---

### 🔽 COPIE E COLE ISSO EM Unit_Health.cs:

```csharp
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Unit_Health : MonoBehaviour
{
    private Action onDestroy;
    public int currentHealth;


    public void Init(int totalHealth, Action onDestroy)
    {
        currentHealth = totalHealth;
        this.onDestroy = onDestroy;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            onDestroy.Invoke();
        }
    }
    
}
```

**Salve o arquivo (Ctrl+S)**

---

## 📝 PASSO 5: COPIAR 4 ESTADOS DO DESTROYER

### **5.1 - TeamA_Unit_DestroyerFSM_Search.cs**

**Local:** `Assets/Scripts/TeamA/Destroyer/TeamA_Unit_DestroyerFSM_Search.cs`

1. Clique com botão DIREITO em Assets/Scripts/TeamA/Destroyer
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
        // Estratégia RUSH: Procura apenas pela torre inimiga
        // Debug.Log("Destroyer entrou em SEARCH - procurando torre inimiga!");
    }

    public override void UpdateState(TeamA_Unit_DestroyerManager manager)
    {
        // Usa Raycast para procurar a torre inimiga dentro do SearchRay
        RaycastHit[] hits = Physics.SphereCastAll(
            manager.transform.position,
            Team_Base.destroyerSearchRay,
            Vector3.forward,
            0f
        );

        // Procura pelo objeto de torre inimiga
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("EnemyTower"))
            {
                manager.currentTarget = hit.collider.transform;
                manager.currentState.ExitState(manager);
                manager.currentState = manager.MoveState;
                manager.currentState.EnterState(manager);
                return;
            }
        }
        
        // Se não encontrou nada, continua procurando
    }

    public override void ExitState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer saiu de SEARCH");
    }
}
```

**Salve o arquivo (Ctrl+S)**

---

### **5.2 - TeamA_Unit_DestroyerFSM_Move.cs**

**Local:** `Assets/Scripts/TeamA/Destroyer/TeamA_Unit_DestroyerFSM_Move.cs`

1. Clique com botão DIREITO em Assets/Scripts/TeamA/Destroyer
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
        // Debug.Log("Destroyer entrou em MOVE - rumo à torre!");
    }

    public override void UpdateState(TeamA_Unit_DestroyerManager manager)
    {
        // Se perdeu o alvo, volta para SEARCH
        if (manager.currentTarget == null)
        {
            manager.currentState.ExitState(manager);
            manager.currentState = manager.SearchState;
            manager.currentState.EnterState(manager);
            return;
        }

        // Calcula distância até o alvo
        float distanceToTarget = Vector3.Distance(manager.transform.position, manager.currentTarget.position);

        // Se chegou perto do alvo (dentro do range de ataque)
        if (distanceToTarget <= Team_Base.destroyerAttackRange)
        {
            manager.currentState.ExitState(manager);
            manager.currentState = manager.AttackState;
            manager.currentState.EnterState(manager);
            return;
        }

        // Move em direção ao alvo usando velocidade do Destroyer
        Vector3 directionToTarget = (manager.currentTarget.position - manager.transform.position).normalized;
        manager.transform.position += directionToTarget * Team_Base.destroyerMoveSpeed;
    }

    public override void ExitState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer saiu de MOVE");
    }
}
```

**Salve o arquivo (Ctrl+S)**

---

### **5.3 - TeamA_Unit_DestroyerFSM_Attack.cs**

**Local:** `Assets/Scripts/TeamA/Destroyer/TeamA_Unit_DestroyerFSM_Attack.cs`

1. Clique com botão DIREITO em Assets/Scripts/TeamA/Destroyer
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
        // Debug.Log("Destroyer entrou em ATTACK - destruindo torre!");
        timeSinceLastAttack = 0f;
    }

    public override void UpdateState(TeamA_Unit_DestroyerManager manager)
    {
        // Se perdeu o alvo, volta para SEARCH
        if (manager.currentTarget == null)
        {
            manager.currentState.ExitState(manager);
            manager.currentState = manager.SearchState;
            manager.currentState.EnterState(manager);
            return;
        }

        // Calcula distância até o alvo
        float distanceToTarget = Vector3.Distance(manager.transform.position, manager.currentTarget.position);

        // Se saiu do range, volta para MOVE
        if (distanceToTarget > Team_Base.destroyerAttackRange)
        {
            manager.currentState.ExitState(manager);
            manager.currentState = manager.MoveState;
            manager.currentState.EnterState(manager);
            return;
        }

        // Incrementa tempo desde último ataque
        timeSinceLastAttack += Time.deltaTime;

        // Se passou o intervalo de ataque, ataca
        if (timeSinceLastAttack >= Team_Base.destroyerAttackInterval)
        {
            // Estratégia RUSH: Ataque direto e constante
            // Procura por um componente de Health na torre ou chama TakeDamage diretamente
            if (manager.currentTarget.TryGetComponent<Unit_Health>(out Unit_Health targetHealth))
            {
                targetHealth.TakeDamage(Team_Base.destroyerAttackDamage);
                // Debug.Log($"Destroyer atacou! Dano: {Team_Base.destroyerAttackDamage}");
            }

            timeSinceLastAttack = 0f;
        }
    }

    public override void ExitState(TeamA_Unit_DestroyerManager manager)
    {
        // Debug.Log("Destroyer saiu de ATTACK");
        timeSinceLastAttack = 0f;
    }
}
```

**Salve o arquivo (Ctrl+S)**

---

### **5.4 - TeamA_Unit_DestroyerFSM_Death.cs**

**Local:** `Assets/Scripts/TeamA/Destroyer/TeamA_Unit_DestroyerFSM_Death.cs`

1. Clique com botão DIREITO em Assets/Scripts/TeamA/Destroyer
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
        // Debug.Log("Destroyer MORREU!");
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

**Salve o arquivo (Ctrl+S)**

---

## 📝 PASSO 6: COPIAR MANAGER DO DESTROYER

**Local:** `Assets/Scripts/TeamA/Managers/TeamA_Unit_DestroyerManager.cs`

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
    public Unit_Abstract<TeamA_Unit_DestroyerManager> currentState;
    public TeamA_Unit_DestroyerFSM_Search SearchState { get; private set; }
    public TeamA_Unit_DestroyerFSM_Move MoveState { get; private set; }
    public TeamA_Unit_DestroyerFSM_Attack AttackState { get; private set; }
    public TeamA_Unit_DestroyerFSM_Death DeathState { get; private set; }

    // Target
    public Transform currentTarget;

    // Health
    private Unit_Health healthComponent;

    private void Awake()
    {
        // Inicializa os estados
        SearchState = new TeamA_Unit_DestroyerFSM_Search();
        MoveState = new TeamA_Unit_DestroyerFSM_Move();
        AttackState = new TeamA_Unit_DestroyerFSM_Attack();
        DeathState = new TeamA_Unit_DestroyerFSM_Death();

        // Começa no estado de SEARCH
        currentState = SearchState;
    }

    private void Start()
    {
        // Inicializa o componente de saúde
        healthComponent = GetComponent<Unit_Health>();
        if (healthComponent != null)
        {
            healthComponent.Init(Team_Base.destroyerMaxHealth, OnDeath);
        }

        // Entra no estado inicial
        currentState.EnterState(this);
    }

    private void Update()
    {
        // Verifica se está morto
        if (healthComponent != null && healthComponent.currentHealth <= 0)
        {
            if (currentState != DeathState)
            {
                currentState.ExitState(this);
                currentState = DeathState;
                currentState.EnterState(this);
            }
        }

        // Atualiza o estado atual
        currentState.UpdateState(this);
    }

    private void OnDeath()
    {
        // Transiciona para Death State
        if (currentState != DeathState)
        {
            currentState.ExitState(this);
            currentState = DeathState;
            currentState.EnterState(this);
        }
    }
}
```

**Salve o arquivo (Ctrl+S)**

---

## 📝 PASSO 7: CONFIGURAR NO INSPECTOR

**Agora é a parte PRÁTICA:**

### **7.1 - Criar GameObject para o Destroyer**

1. Na Hierarquia, clique com botão DIREITO
2. 3D Object → Cube (ou qualquer modelo 3D)
3. Renomeie para: `Destroyer_Unit`
4. Com o GameObject selecionado, clique em **Add Component**
5. Procure por: `TeamA_Unit_DestroyerManager`
6. Clique para adicionar
7. Clique em **Add Component** novamente
8. Procure por: `Unit_Health`
9. Clique para adicionar

### **7.2 - Configurar Tags**

1. Com o Destroyer_Unit selecionado, vá em **Tag** (no topo do Inspector)
2. Clique em **Add Tag**
3. Crie uma tag chamada: `DestroyerUnit`
4. Atribua essa tag ao GameObject

### **7.3 - Criar Torre Inimiga**

1. Na Hierarquia, clique com botão DIREITO
2. 3D Object → Cube
3. Renomeie para: `EnemyTower`
4. Com o GameObject selecionado, vá em **Tag**
5. Clique em **Add Tag**
6. Crie uma tag chamada: `EnemyTower`
7. Atribua essa tag ao GameObject
8. Clique em **Add Component**
9. Procure por: `Unit_Health`
10. Clique para adicionar

### **7.4 - Posicionar os GameObjects**

1. Selecione `Destroyer_Unit` e mova para posição X=0, Y=1, Z=0
2. Selecione `EnemyTower` e mova para posição X=10, Y=1, Z=0

---

## 🧪 PASSO 8: TESTAR TUDO

1. Pressione **Play** (barra de espaço ou botão Play no topo)
2. Observe o Destroyer se movendo em direção à Torre
3. Veja a Torre recebendo dano
4. Quando Torre chegar a 0 HP, desativa

---

## ✅ PRONTO!

Você implementou com sucesso a FSM do Destroyer com:
- ✅ Estratégia de RUSH
- ✅ Procura pela torre inimiga
- ✅ Move em direção ao alvo
- ✅ Ataca continuamente
- ✅ Morre quando HP ≤ 0

**Boa sorte! 🎮**
