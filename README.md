# 🎮 Plataforma de Jogos Indie - Arquitetura de Microsserviços

## 📋 Índice
- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Microsserviços](#microsserviços)
  - [Usuarios (Porta 5000)](#1-microsserviço-usuarios-porta-5000)
  - [Jogos (Porta 5001)](#2-microsserviço-jogos-porta-5001)
  - [Compras (Porta 5002)](#3-microsserviço-compras-porta-5002)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Como Executar](#como-executar)
- [Testando a API](#testando-a-api)
- [Integrações entre Microsserviços](#integrações-entre-microsserviços)
- [Estrutura de Pastas](#estrutura-de-pastas)

---

## 🎯 Visão Geral

Este projeto implementa uma **Plataforma de Jogos Indie** utilizando arquitetura de microsserviços em **C# .NET 8**. A plataforma permite que usuários naveguem por um catálogo de jogos indie, realizem compras e gerenciem suas bibliotecas pessoais de jogos.

### Por que Microsserviços?

A arquitetura de microsserviços foi escolhida para:
- **Escalabilidade independente**: Cada serviço pode ser escalado conforme a demanda
- **Manutenção facilitada**: Alterações em um serviço não afetam os outros
- **Tecnologias diversas**: Cada serviço pode usar tecnologias diferentes se necessário
- **Deploy independente**: Serviços podem ser atualizados sem afetar o sistema todo
- **Resiliência**: Falha em um serviço não derruba toda a aplicação

---

## 🏗️ Arquitetura

O sistema é composto por **3 microsserviços independentes**, cada um com:
- Sua própria **base de dados SQLite**
- Seu próprio **servidor web** rodando em porta específica
- Sua própria **API REST** com documentação Swagger
- Comunicação via **HTTP/REST** entre serviços

```
┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐
│   USUARIOS      │      │     JOGOS       │      │    COMPRAS      │
│   (Port 5000)   │◄────►│   (Port 5001)   │◄────►│   (Port 5002)   │
│                 │      │                 │      │                 │
│  usuarios.db    │      │   jogos.db      │      │  compras.db     │
└─────────────────┘      └─────────────────┘      └─────────────────┘
```

### Fluxo de Comunicação

1. **Cliente** → Faz requisição HTTP para qualquer microsserviço
2. **Microsserviço** → Processa a requisição
3. **Microsserviço** → Se necessário, faz chamadas HTTP para outros microsserviços
4. **Microsserviço** → Retorna resposta ao cliente

---

## 🔧 Microsserviços

### 1. Microsserviço **Usuarios** (Porta 5000)

**Responsabilidade**: Gerenciar contas de usuários, perfis e bibliotecas de jogos.

#### 📦 Modelo de Dados (Usuario)
```csharp
- Id: int                    // Identificador único do usuário
- Nome: string               // Nome completo do usuário
- Email: string              // Email (único no sistema)
- Senha: string              // Senha (deve ser hasheada em produção)
- Pontos: int                // Sistema de pontos/recompensas
- Ativo: bool                // Status da conta (ativo/inativo)
- DataCriacao: DateTime      // Data de registro
- BibliotecaJogos: List<int> // IDs dos jogos que o usuário possui
```

#### 🔌 Endpoints Disponíveis

| Método | Endpoint | Descrição | Exemplo de Uso |
|--------|----------|-----------|----------------|
| `POST` | `/api/usuarios/register` | Registra novo usuário | Criar conta |
| `GET` | `/api/usuarios` | Lista todos os usuários | Admin visualizar usuários |
| `GET` | `/api/usuarios/{id}` | Busca usuário por ID | Ver perfil |
| `PUT` | `/api/usuarios/{id}` | Atualiza dados do usuário | Editar perfil |
| `DELETE` | `/api/usuarios/{id}` | Desativa conta do usuário | Excluir conta |
| `POST` | `/api/usuarios/{id}/biblioteca/{jogoId}` | Adiciona jogo à biblioteca | Adicionar jogo manualmente |
| `GET` | `/api/usuarios/{id}/biblioteca` | Lista jogos do usuário | Ver biblioteca |
| `PUT` | `/api/usuarios/{id}/biblioteca` | Atualiza biblioteca completa | Sincronizar biblioteca |

#### 💾 Banco de Dados
- **Arquivo**: `usuarios.db` (SQLite)
- **Tabela**: `Usuarios`
- **Índice único**: Email (não permite emails duplicados)
- **Campo especial**: `BibliotecaJogos` armazenado como JSON

---

### 2. Microsserviço **Jogos** (Porta 5001)

**Responsabilidade**: Gerenciar catálogo de jogos indie disponíveis na plataforma.

#### 📦 Modelo de Dados (Jogo)
```csharp
- Id: int                // Identificador único do jogo
- Titulo: string         // Nome do jogo
- Descricao: string      // Descrição detalhada
- Preco: decimal         // Preço em reais (formato: 19.99)
- Desenvolvedor: string  // Nome do desenvolvedor/estúdio
- DataLancamento: DateTime // Data de lançamento original
- Disponivel: bool       // Se está disponível para compra
- DataCriacao: DateTime  // Quando foi cadastrado na plataforma
```

#### 🔌 Endpoints Disponíveis

| Método | Endpoint | Descrição | Exemplo de Uso |
|--------|----------|-----------|----------------|
| `GET` | `/api/jogos` | Lista jogos disponíveis | Catálogo da loja |
| `GET` | `/api/jogos/{id}` | Busca jogo por ID | Página de detalhes |
| `POST` | `/api/jogos` | Cadastra novo jogo | Admin adicionar jogo |
| `PUT` | `/api/jogos/{id}` | Atualiza dados do jogo | Editar preço/descrição |
| `DELETE` | `/api/jogos/{id}` | Remove jogo (soft delete) | Remover da loja |

#### 💾 Banco de Dados
- **Arquivo**: `jogos.db` (SQLite)
- **Tabela**: `Jogos`
- **Tipo especial**: `Preco` armazenado como `decimal(18,2)` para precisão monetária
- **Soft Delete**: DELETE apenas marca `Disponivel = false`, não remove do banco

---

### 3. Microsserviço **Compras** (Porta 5002)

**Responsabilidade**: Processar transações de compra e manter histórico.

#### 📦 Modelo de Dados (Compra)
```csharp
- Id: int            // Identificador único da compra
- UsuarioId: int     // ID do usuário que comprou
- JogoId: int        // ID do jogo comprado
- ValorPago: decimal // Valor pago na transação
- DataCompra: DateTime // Data/hora da compra
- Status: string     // Status da compra (Concluída, Pendente, etc)
```

#### 🔌 Endpoints Disponíveis

| Método | Endpoint | Descrição | Exemplo de Uso |
|--------|----------|-----------|----------------|
| `GET` | `/api/compras` | Lista todas as compras | Admin ver vendas |
| `GET` | `/api/compras/{id}` | Busca compra por ID | Ver detalhes da compra |
| `GET` | `/api/compras/usuario/{usuarioId}` | Lista compras de um usuário | Histórico pessoal |
| `POST` | `/api/compras` | Registra nova compra | Finalizar compra |

#### 💾 Banco de Dados
- **Arquivo**: `compras.db` (SQLite)
- **Tabela**: `Compras`
- **Tipo especial**: `ValorPago` armazenado como `decimal(18,2)`

#### 🔗 Integrações Especiais

Quando uma compra é registrada (`POST /api/compras`), o serviço:

1. **Valida o Usuário** (chama `GET http://localhost:5000/api/usuarios/{id}`)
   - Verifica se o usuário existe
   - Retorna erro se não encontrado

2. **Valida o Jogo** (chama `GET http://localhost:5001/api/jogos/{id}`)
   - Verifica se o jogo existe
   - Verifica se está disponível para compra
   - Obtém o preço atual do jogo
   - Retorna erro se não disponível

3. **Verifica Duplicação**
   - Consulta banco local para ver se usuário já possui o jogo
   - Retorna erro se já comprado

4. **Registra a Compra**
   - Salva transação no banco `compras.db`
   - Usa o preço obtido do serviço de Jogos

5. **Atualiza Biblioteca do Usuário** (chama `PUT http://localhost:5000/api/usuarios/{id}/biblioteca`)
   - Adiciona o jogo à biblioteca do usuário
   - Atualiza o campo `BibliotecaJogos`

---

## 🛠️ Tecnologias Utilizadas

### Backend
- **C# .NET 8**: Framework principal
- **ASP.NET Core**: Para criar APIs REST
- **Entity Framework Core**: ORM para acesso ao banco de dados
- **SQLite**: Banco de dados leve e portátil

### Documentação
- **Swagger/OpenAPI**: Documentação interativa das APIs
- **Swashbuckle**: Geração automática de documentação Swagger

### Comunicação
- **HttpClient**: Para comunicação HTTP entre microsserviços
- **IHttpClientFactory**: Gerenciamento eficiente de conexões HTTP
- **System.Text.Json**: Serialização/deserialização JSON

### Arquitetura
- **DTOs (Data Transfer Objects)**: Separação entre modelos de domínio e API
- **Repository Pattern**: Através do DbContext do EF Core
- **Dependency Injection**: Injeção de dependências nativa do .NET

---

## 🚀 Como Executar

### Pré-requisitos
- **.NET 8 SDK** instalado ([Download](https://dotnet.microsoft.com/download))
- **Visual Studio 2022** ou **VS Code** (opcional)
- **Git** para clonar o repositório

### Passo 1: Clonar o Repositório
```bash
git clone <url-do-repositorio>
cd Arquitetura_de_software_projeto_2025
```

### Passo 2: Restaurar Dependências
```bash
dotnet restore
```

### Passo 3: Executar os Microsserviços

**Opção A: Executar todos em terminais separados**

Terminal 1 - Usuarios:
```bash
dotnet run --project usuarios/usuarios.csproj
```

Terminal 2 - Jogos:
```bash
dotnet run --project jogos/Jogos.csproj
```

Terminal 3 - Compras:
```bash
dotnet run --project compras/Compras.csproj
```

**Opção B: Executar em background (Windows PowerShell)**
```powershell
Start-Process dotnet -ArgumentList "run --project usuarios/usuarios.csproj" -WindowStyle Hidden
Start-Process dotnet -ArgumentList "run --project jogos/Jogos.csproj" -WindowStyle Hidden
Start-Process dotnet -ArgumentList "run --project compras/Compras.csproj" -WindowStyle Hidden
```

### Passo 4: Verificar se os Serviços Estão Rodando

Acesse no navegador:
- **Usuarios**: http://localhost:5000/swagger
- **Jogos**: http://localhost:5001/swagger
- **Compras**: http://localhost:5002/swagger

---

## 🧪 Testando a API

### Usando Swagger (Recomendado para Iniciantes)

1. Acesse http://localhost:5000/swagger
2. Clique em um endpoint (ex: `POST /api/usuarios/register`)
3. Clique em "Try it out"
4. Preencha o JSON de exemplo
5. Clique em "Execute"
6. Veja a resposta abaixo

### Usando Arquivos .http (VS Code)

Cada microsserviço tem um arquivo `.http` com exemplos prontos:
- `usuarios/Usuarios.http`
- `jogos/Jogos.http`
- `compras/Compras.http`

**Como usar:**
1. Instale a extensão "REST Client" no VS Code
2. Abra o arquivo `.http`
3. Clique em "Send Request" acima de cada requisição

### Usando PowerShell

**Criar Usuário:**
```powershell
$body = @{
    Nome = "João Silva"
    Email = "joao@teste.com"
    Senha = "senha123"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/usuarios/register" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body
```

**Criar Jogo:**
```powershell
$body = @{
    Titulo = "Celeste"
    Descricao = "Um jogo de plataforma desafiador"
    Preco = 19.99
    Desenvolvedor = "Maddy Makes Games"
    DataLancamento = "2018-01-25"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5001/api/jogos" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body
```

**Realizar Compra:**
```powershell
$body = @{
    UsuarioId = 1
    JogoId = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5002/api/compras" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body
```

### Usando cURL (Linux/Mac)

**Criar Usuário:**
```bash
curl -X POST http://localhost:5000/api/usuarios/register \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "email": "joao@teste.com",
    "senha": "senha123"
  }'
```

---

## 🔄 Integrações entre Microsserviços

### Fluxo Completo de uma Compra

```
1. Cliente envia POST /api/compras
   {
     "usuarioId": 1,
     "jogoId": 1
   }

2. Microsserviço COMPRAS valida usuário
   → GET http://localhost:5000/api/usuarios/1
   ← Retorna dados do usuário

3. Microsserviço COMPRAS valida jogo
   → GET http://localhost:5001/api/jogos/1
   ← Retorna dados do jogo (incluindo preço)

4. Microsserviço COMPRAS verifica duplicação
   → Consulta banco local compras.db
   ← Verifica se usuário já comprou este jogo

5. Microsserviço COMPRAS registra transação
   → INSERT na tabela Compras
   ← Compra registrada com sucesso

6. Microsserviço COMPRAS atualiza biblioteca
   → PUT http://localhost:5000/api/usuarios/1/biblioteca
   ← Biblioteca atualizada com novo jogo

7. Cliente recebe resposta
   ← Dados da compra concluída
```

### Diagrama de Sequência

```
Cliente          Compras         Usuarios        Jogos
  |                |                |              |
  |---POST-------->|                |              |
  |                |---GET--------->|              |
  |                |<--Usuario------|              |
  |                |                               |
  |                |---GET------------------------>|
  |                |<--Jogo------------------------|
  |                |                               |
  |                |--Salva Compra                 |
  |                |                               |
  |                |---PUT--------->|              |
  |                |<--OK-----------|              |
  |                |                               |
  |<--Compra-------|                               |
```

---

## 📁 Estrutura de Pastas

```
Arquitetura_de_software_projeto_2025/
│
├── usuarios/                          # Microsserviço de Usuários
│   ├── Controllers/
│   │   └── UsuariosController.cs     # Endpoints da API
│   ├── Data/
│   │   └── AppDbContext.cs           # Contexto do EF Core
│   ├── Dtos/
│   │   ├── UsuarioCreateDto.cs       # DTO para criação
│   │   ├── UsuarioDto.cs             # DTO para resposta
│   │   ├── UsuarioUpdateDto.cs       # DTO para atualização
│   │   └── BibliotecaUpdateDto.cs    # DTO para biblioteca
│   ├── Models/
│   │   └── Usuario.cs                # Modelo de domínio
│   ├── Properties/
│   │   └── launchSettings.json       # Configuração de porta
│   ├── Program.cs                    # Configuração da aplicação
│   ├── appsettings.json              # Configurações gerais
│   ├── usuarios.csproj               # Arquivo do projeto
│   ├── Usuarios.http                 # Testes HTTP
│   └── usuarios.db                   # Banco de dados (gerado)
│
├── jogos/                             # Microsserviço de Jogos
│   ├── Controllers/
│   │   └── JogosController.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Dtos/
│   │   ├── JogoCreateDto.cs
│   │   ├── JogoDto.cs
│   │   └── JogoUpdateDto.cs
│   ├── Models/
│   │   └── Jogo.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Jogos.csproj
│   ├── Jogos.http
│   └── jogos.db                      # Banco de dados (gerado)
│
├── compras/                           # Microsserviço de Compras
│   ├── Controllers/
│   │   └── ComprasController.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Dtos/
│   │   ├── CompraCreateDto.cs
│   │   ├── CompraDto.cs
│   │   ├── JogoInfoDto.cs           # DTO para integração
│   │   └── UsuarioInfoDto.cs        # DTO para integração
│   ├── Models/
│   │   └── Compra.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Compras.csproj
│   ├── Compras.http
│   └── compras.db                    # Banco de dados (gerado)
│
├── Arquitetura_de_software_projeto_2025.sln  # Solution do Visual Studio
└── README.md                          # Este arquivo
```

### Explicação dos Componentes

#### Controllers
Contêm os **endpoints da API**. Recebem requisições HTTP, processam e retornam respostas.

#### Data
Contêm o **DbContext** do Entity Framework, responsável por:
- Configurar conexão com banco de dados
- Mapear modelos para tabelas
- Executar migrations

#### Dtos (Data Transfer Objects)
Objetos usados para **transferir dados** entre cliente e servidor:
- **CreateDto**: Dados necessários para criar um registro
- **Dto**: Dados retornados ao cliente
- **UpdateDto**: Dados para atualizar um registro

#### Models
Representam as **entidades de domínio** (tabelas do banco):
- Definem estrutura dos dados
- Contêm regras de negócio
- Mapeados pelo EF Core

#### Program.cs
Arquivo principal que:
- Configura serviços (DI, DbContext, Swagger)
- Configura middleware (CORS, HTTPS)
- Inicia a aplicação

#### appsettings.json
Arquivo de configuração com:
- String de conexão do banco
- Configurações de logging
- Variáveis de ambiente

---

## 📚 Conceitos Importantes

### DTOs vs Models

**Models (Entidades de Domínio)**
- Representam tabelas do banco
- Contêm lógica de negócio
- Podem ter campos sensíveis (senha)

**DTOs (Data Transfer Objects)**
- Usados na comunicação API
- Não expõem campos sensíveis
- Podem combinar dados de várias entidades

**Exemplo:**
```csharp
// Model - Tem senha
public class Usuario {
    public string Senha { get; set; }
}

// DTO - Não expõe senha
public class UsuarioDto {
    // Sem campo Senha!
}
```

### Soft Delete

Em vez de deletar registros do banco, apenas marcamos como inativo:

```csharp
// Não faz isso:
_context.Jogos.Remove(jogo);

// Faz isso:
jogo.Disponivel = false;
_context.SaveChanges();
```

**Vantagens:**
- Mantém histórico
- Permite recuperação
- Preserva integridade referencial

### HttpClient e Integrações

Para comunicação entre microsserviços:

```csharp
// Injeção do HttpClient
public ComprasController(IHttpClientFactory httpClientFactory) {
    _httpClient = httpClientFactory.CreateClient();
}

// Chamada para outro serviço
var response = await _httpClient.GetAsync(
    "http://localhost:5000/api/usuarios/1"
);
```

---

## 🔒 Melhorias Futuras (Produção)

Este é um projeto educacional. Para produção, considere:

### Segurança
- [ ] Implementar autenticação JWT
- [ ] Hash de senhas com BCrypt
- [ ] HTTPS obrigatório
- [ ] Rate limiting
- [ ] Validação de entrada robusta

### Banco de Dados
- [ ] Migrar para PostgreSQL/SQL Server
- [ ] Implementar migrations adequadas
- [ ] Backup automático
- [ ] Connection pooling

### Arquitetura
- [ ] API Gateway (Ocelot)
- [ ] Service Discovery (Consul)
- [ ] Message Broker (RabbitMQ)
- [ ] Circuit Breaker (Polly)
- [ ] Distributed Tracing (Jaeger)

### DevOps
- [ ] Containerização (Docker)
- [ ] Orquestração (Kubernetes)
- [ ] CI/CD (GitHub Actions)
- [ ] Monitoramento (Prometheus/Grafana)
- [ ] Logging centralizado (ELK Stack)

---

## 📖 Recursos de Aprendizado

### Documentação Oficial
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Microsserviços .NET](https://docs.microsoft.com/dotnet/architecture/microservices)

### Tutoriais Recomendados
- [REST API com ASP.NET Core](https://docs.microsoft.com/aspnet/core/tutorials/first-web-api)
- [Microsserviços com .NET](https://docs.microsoft.com/dotnet/architecture/microservices/multi-container-microservice-net-applications)

---

## 👥 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto é educacional e está disponível sob a licença MIT.

---

## 📞 Contato

Para dúvidas ou sugestões, abra uma issue no repositório.

---

**Desenvolvido com 💙 para aprendizado de Arquitetura de Software**
