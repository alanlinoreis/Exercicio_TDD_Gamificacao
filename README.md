# Sistema de Gerenciamento de Eventos

## 📘 Introdução  
Este projeto é um **Sistema de Gerenciamento de Eventos**, desenvolvido no âmbito da disciplina de Programação Orientada a Objetos da Universidade Tecnológica Federal do Paraná – Câmpus Medianeira (UTFPR) — Câmpus Medianeira. Ele visa implementar funcionalidades básicas para cadastro, consulta e gerenciamento de eventos, exercitando conceitos de orientação a objetos, arquitetura em camadas e testes de unidade.

## 🧩 Tecnologias utilizadas  
- Linguagem: C#  
- Plataforma/.NET: 9.0
- Estrutura de solução: projeto de domínio, aplicação, testes (ex: `EventManagement.Domain`, `EventManagement.Application`, `EventManagement.Tests`)  
- Ferramentas de teste: xUnit

## 📂 Estrutura da solução  
A solução está organizada da seguinte forma:  
- `src/` — código‑fonte da aplicação  
  - `EventManagement.Domain` — entidades de negócio, interfaces, regras do domínio  
  - `EventManagement.Application` — serviços de aplicação, casos de uso, orquestração  
- `tests/` — projetos de teste  
  - `EventManagement.Domain.Tests` — testes de unidade para a camada de domínio  
- `EventManagement.sln` — arquivo de solução principal  

## ⚙️ Como executar o projeto  
Siga os passos abaixo para executar o sistema localmente:

1. Clone o repositório  
   ```bash
   git clone https://github.com/alanlinoreis/Sistema_de_Gerenciamento_de_Eventos.git
   cd Sistema_de_Gerenciamento_de_Eventos
   ```

2. Abra a solução `EventManagement.sln` em sua IDE favorita.

3. Restaure os pacotes NuGet  
   ```bash
   dotnet restore
   ```

4. Build da solução  
   ```bash
   dotnet build
   ```

5. Executar os testes de unidade  
   ```bash
   dotnet test
   ```

## 🧪 Exemplos de uso  
- Após executar a aplicação, você poderá cadastrar um novo evento por meio do serviço disponibilizado.  
- Em seguida, poderá listar todos os eventos ou consultar por critérios específicos (data, local, etc.).  
- Os testes de unidade garantem que as regras do domínio (por exemplo: validação de dados, invariantes) estejam funcionando corretamente.

## 📄 Licença  
Este projeto está licenciado sob a licença **MIT**. Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👤 Autor  
Alan Lino dos Reis  
Curso: Ciência da Computação – UTFPR Medianeira  
Disciplina: Programação Orientada a Objetos  
Professor: Everton Coimbra  
