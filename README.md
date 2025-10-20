# Gamification.Domain (dotnet 9) - Exercise Project

Projeto exemplo baseado no enunciado: **Concessão de Badges por Missão** (TDD).

## Como rodar

Requer .NET 9 SDK instalado.

No terminal:
```bash
cd /path/to/gamification_dotnet9_project
dotnet restore
dotnet test
```

O projeto contém:
- `src/Gamification.Domain/` — domínio com services, policies, ports (interfaces) e modelos.
- `tests/Gamification.Domain.Tests/` — testes xUnit cobrindo regras principais:
  - unicidade/idempotência
  - elegibilidade
  - janelas de bônus (integral, reduzido, sem bônus)
  - atomicidade e auditoria

Decisões de design e limitações estão comentadas nos arquivos de código.

