

\# HANDS-ON #10 | Segurança de APIs REST



\*\*Ponto de Conhecimento Tecnológico\*\*  

\*\*Série 3 — Segurança Aplicacional\*\*



\## Sobre o projeto



Este HANDS-ON demonstra a implementação e validação

de mecanismos de segurança em APIs REST com ASP.NET

Core e .NET 10.



O laboratório permite explorar autenticação JWT,

autorização baseada em perfis e vulnerabilidades

BOLA/IDOR através de cenários práticos e testes

automatizados.



\## Objetivos



\- Implementar autenticação com JWT.

\- Proteger endpoints com autorização.

\- Aplicar controlo de acesso ao nível do objeto.

\- Demonstrar uma vulnerabilidade BOLA/IDOR.

\- Comparar endpoints seguros e vulneráveis.

\- Automatizar testes de segurança com xUnit.



\## Tecnologias



\- C# / .NET 10

\- ASP.NET Core Web API

\- JWT Bearer Authentication

\- Entity Framework Core

\- SQLite / SQL Server

\- xUnit

\- Visual Studio 2026



\## Estrutura da solução



```text

HandsOn10.ApiSecurity/

|

|-- HandsOn10.Api/

|   |-- Controllers/

|   |-- Data/

|   |-- Models/

|   |-- Services/

|   |-- Program.cs

|   |-- appsettings.json

|

|-- HandsOn10.Tests/

|   |-- SecurityTests.cs

|

|-- README.md

|-- .gitignore

```



\## Cenários de segurança



| Cenário | HTTP esperado |

|---|---|

| Health check público | 200 |

| Pedido sem token JWT | 401 |

| Credenciais inválidas | 401 |

| Acesso ao próprio pedido | 200 |

| Acesso ao pedido de outro utilizador | 403 |

| Utilizador comum acede à área administrativa | 403 |

| Administrador acede à área administrativa | 200 |

| Endpoint vulnerável BOLA/IDOR (laboratório) | 200 |



\## Demonstração BOLA/IDOR



A API disponibiliza dois endpoints para demonstrar

a importância da autorização ao nível do objeto.



\### Endpoint protegido



GET /api/pedidos/102



Um utilizador autenticado não pode consultar pedidos

pertencentes a outros utilizadores sem autorização.



\*\*Resultado esperado: 403 Forbidden.\*\*



\### Endpoint vulnerável



GET /api/pedidos/lab/vulneravel/102



Quando o laboratório está explicitamente ativado,

este endpoint permite demonstrar uma falha de

autorização ao nível do objeto.



\*\*Resultado esperado: 200 OK.\*\*



Este comportamento é intencional e destina-se

exclusivamente a fins educativos.



\## Executar o projeto



\### 1. Clonar o repositório



```powershell

git clone https://github.com/FgAraujo-web/HandsOn10-ApiSecurity.git

cd HandsOn10-ApiSecurity

```



\### 2. Configurar a chave JWT



Na pasta principal da solução:



```powershell

dotnet user-secrets init --project ./HandsOn10.Api

```



Gerar uma chave de demonstração:



```powershell

$bytes = New-Object byte\[] 32

$rng = \[System.Security.Cryptography.RandomNumberGenerator]::Create()

try { $rng.GetBytes($bytes) } finally { $rng.Dispose() }

$key = \[Convert]::ToBase64String($bytes)

dotnet user-secrets set "Jwt:Key" "$key" --project ./HandsOn10.Api

```



Nunca publicar a chave JWT no GitHub.



\### 3. Executar a API



```powershell

dotnet run --project ./HandsOn10.Api --launch-profile https

```



Consultar o endpoint de verificação:



GET /api/health



A porta HTTPS deve ser confirmada na configuração

local da aplicação.



\### 4. Executar os testes



```powershell

dotnet test

```



O laboratório inclui oito testes automatizados

de autenticação, autorização e BOLA/IDOR.



\## Segurança do laboratório



O endpoint vulnerável encontra-se desativado

por defeito:



```json

{

&#x20; "Lab": {

&#x20;   "EnableVulnerableEndpoint": false

&#x20; }

}

```



A ativação deve ocorrer apenas num ambiente local,

isolado e com dados fictícios.



Nunca disponibilizar o endpoint vulnerável

num ambiente público ou de produção.



As contas e credenciais de demonstração presentes

no código destinam-se exclusivamente ao laboratório

e não devem ser reutilizadas noutros sistemas.



\## Principais aprendizagens



A autenticação confirma a identidade do utilizador,

mas não garante autorização para aceder a todos

os recursos.



A prevenção de BOLA/IDOR exige validações de

autorização ao nível de cada objeto solicitado.



Os testes automatizados ajudam a identificar

regressões nos mecanismos de segurança.



\## Referências



\- OWASP API Security Top 10

\- Microsoft Learn — ASP.NET Core Security

\- Microsoft Learn — JWT Bearer Authentication

\- Microsoft Learn — Authorization

\- xUnit



\---



\*\*Ponto de Conhecimento Tecnológico\*\*



Explorar • Experimentar • Aprender • Partilhar



