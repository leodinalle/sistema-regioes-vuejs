# 🌎 Sistema de Regiões - CRUD Vue.js

> Sistema completo para gerenciamento de regiões por estado, desenvolvido com Vue.js 3 e .NET Core.

![Vue.js](https://img.shields.io/badge/Vue.js-3.0-4FC08D?style=flat-square&logo=vue.js&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=.net&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)

## 📋 Sobre o Projeto

Sistema desenvolvido para avaliação técnica, implementando um CRUD completo para gerenciamento de regiões brasileiras organizadas por UF (Unidade Federativa). A aplicação oferece uma interface moderna e responsiva para cadastro, edição, listagem e inativação de regiões.

### ✨ Funcionalidades

- ✅ **CRUD Completo**: Create, Read, Update e Delete de regiões
- ✅ **Interface Responsiva**: Adaptável a diferentes tamanhos de tela
- ✅ **Validações em Tempo Real**: Feedback imediato ao usuário
- ✅ **Modal de Confirmação**: Segurança nas operações de inativação
- ✅ **Ordenação Automática**: Regiões organizadas por UF e nome
- ✅ **Tratamento de Erros**: Mensagens claras e informativas
- ✅ **Status Visual**: Indicadores de região ativa/inativa

## 🚀 Tecnologias Utilizadas

### Frontend
- **Vue.js 3** - Framework JavaScript progressivo
- **Composition API** - API moderna do Vue.js
- **Lucide Vue Next** - Ícones modernos
- **CSS Customizado** - Estilização sem frameworks externos
- **Vite** - Build tool rápida

### Backend
- **.NET 8** - Framework web moderno
- **Entity Framework Core** - ORM para .NET
- **ASP.NET Core Web API** - API RESTful
- **Swagger** - Documentação da API

### Banco de Dados
- **PostgreSQL** - Banco de dados relacional

## 🛠️ Como Executar

### Pré-requisitos
- Node.js 18+ 
- .NET 8 SDK
- PostgreSQL 12+
- Git

### 1️⃣ Clonar o Repositório
\`\`\`bash
git clone https://github.com/leodinalle/sistema-regioes-vuejs.git
cd sistema-regioes-vuejs
\`\`\`

### 2️⃣ Configurar o Backend

\`\`\`bash
# Navegar para a API
cd RegionAPI

# Restaurar dependências
dotnet restore

# Configurar string de conexão no appsettings.json
# "DefaultConnection": "Host=localhost;Database=RegionDB;Username=seu_usuario;Password=sua_senha"

# Executar migrations
dotnet ef database update

# Executar a API
dotnet run
\`\`\`

A API estará disponível em: `https://localhost:5035`

### 3️⃣ Configurar o Frontend

\`\`\`bash
# Em outro terminal, navegar para o frontend
cd frontend

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm run dev
\`\`\`

O frontend estará disponível em: `http://localhost:5173`

## 🔌 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/region` | Listar todas as regiões |
| `POST` | `/api/region` | Criar nova região |
| `PUT` | `/api/region` | Atualizar região existente |
| `PATCH` | `/api/region/{id}/active` | Ativar região |
| `PATCH` | `/api/region/{id}/inactive` | Inativar região |

### Exemplo de Payload

\`\`\`json
{
  "uf": "SP",
  "nome": "Grande São Paulo",
  "ativo": true
}
\`\`\`

## 🎨 Interface

### Tela Principal
- Formulário de cadastro/edição no topo
- Tabela responsiva com listagem das regiões
- Ações de editar, ativar/inativar para cada região
- Modal de confirmação para operações críticas

### Recursos Visuais
- Design moderno com gradientes
- Ícones intuitivos (Lucide)
- Feedback visual para ações do usuário
- Estados de loading durante requisições
- Mensagens de sucesso/erro

## 🧪 Funcionalidades Técnicas

### Validações
- Campos obrigatórios (UF e Nome)
- Seleção de UF através de dropdown
- Validação em tempo real no frontend

### Tratamento de Erros
- Captura de erros de API
- Mensagens amigáveis ao usuário
- Fallbacks para cenários de falha

### Performance
- Requisições otimizadas
- Componentes reativos
- Ordenação client-side

## 🔧 Scripts Disponíveis

### Frontend
\`\`\`bash
npm run dev          # Servidor de desenvolvimento
npm run build        # Build para produção
npm run preview      # Preview do build
\`\`\`

### Backend
\`\`\`bash
dotnet run           # Executar API
dotnet build         # Compilar projeto
dotnet test          # Executar testes
\`\`\`


## 👨‍💻 Desenvolvedor

**Leonardo Dinale**
- 📧 Email: leodinalle@gmail.com
- 💼 LinkedIn: https://www.linkedin.com/in/leonardo-alves-melo-dinale-254a23281/
- 🐱 GitHub: https://github.com/leodinalle

## 📄 Licença

Este projeto foi desenvolvido para fins de avaliação técnica.

---
