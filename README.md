# FIAP Cloud Games — Documentação Técnica

## Visão Geral

O sistema é composto por três microsserviços principais, cada um com responsabilidades bem definidas e bancos de dados independentes, todos hospedados em serviços de banco de dados do Azure.

## Arquitetura dos Microsserviços

### 1. IAM (Identity and Access Management)
**Responsável pelo gerenciamento de usuários e autenticação**

**Principais responsabilidades:**
- Cadastro, login e CRUD completo de usuários
- Geração de tokens JWT com dados do usuário (como e-mail)
- Gerenciamento da biblioteca de jogos do usuário
- Envio de e-mails de confirmação de compra ou adição à biblioteca

**Integrações:**
- Ao registrar um usuário, é gerado um **ExternalId**, utilizado como identificador universal entre microsserviços
- Ao adicionar um jogo à biblioteca, o IAM envia um e-mail ao usuário utilizando o serviço Serverless de envio de e-mails

### 2. Microsserviço de Jogos
**Gerencia o catálogo e as compras de jogos**

**Principais responsabilidades:**
- Listagem de jogos disponíveis
- Gerenciamento de promoções
- Processamento de compras (integração com o microserviço Financeiro)

**Fluxo de compra:**
- O usuário autentica-se pelo IAM e obtém um token JWT
- O microserviço de Jogos utiliza esse token para extrair o ExternalId do usuário
- A compra é enviada ao microserviço Financeiro, contendo os ExternalIds do usuário e do jogo

### 3. Microsserviço Financeiro
**Gerencia o ciclo de vida das transações financeiras**

**Principais responsabilidades:**
- Recebimento de solicitações de compra enviadas pelo microserviço de Jogos
- Registro e persistência de pagamentos
- Atualização do status das transações

**Fluxo de pagamento:**
- O microserviço de Jogos envia a solicitação de compra
- O Financeiro registra a compra e aguarda o pagamento
- Quando o pagamento é realizado, ele é persistido e aguarda o processamento pelo serviço Serverless `processa_pagamento`
- A função verifica, a cada minuto, pagamentos pendentes e altera seu estado para processado
- Em seguida, envia uma requisição para a rota de verificação de compra, que valida se o valor total foi pago
- Caso o pagamento esteja completo, o Financeiro envia uma solicitação ao IAM para adicionar o jogo à biblioteca do usuário e marca a compra como finalizada
- O IAM, ao receber a confirmação, adiciona o jogo e envia o e-mail de notificação ao cliente

## Estratégia de Integração — ExternalId

Cada microsserviço mantém seu próprio banco de dados, isolado dos demais. Para permitir a comunicação entre sistemas sem dependência direta de dados, utiliza-se a estratégia de **ExternalId**, um identificador universal de referência para objetos de outros contextos.

**Essa abordagem é essencial para garantir:**
- Baixa acoplagem entre microsserviços
- Resiliência transacional
- Independência de dados entre contextos

## Serverless no Azure

O projeto utiliza duas funções Azure Functions escritas em .NET:

### `processa_pagamento`
- **Execução:** A cada minuto
- **Funcionalidades:**
  - Consulta o banco de dados por pagamentos não processados
  - Atualiza o status e envia a notificação ao endpoint de verificação de compras
  - Permite o parcelamento de uma compra em vários pagamentos

### `envia_email`
- **Acionamento:** Via requisição HTTP
- **Responsabilidade:** Envio de notificações por e-mail via SMTP (usando conta Gmail)
- **Contexto:** Acionada sempre que o IAM precisa notificar o usuário sobre nova compra ou atualização de biblioteca

## API Gateway

Todas as requisições externas passam pelo **Azure API Gateway**, que centraliza e distribui o tráfego para os microsserviços IAM, Jogos e Financeiro.

**Funcionalidades do Gateway:**
- Validar o JWT em todas as requisições autenticadas
- Encaminhar o token entre os serviços, mantendo a integridade da sessão do usuário
- Centralização do roteamento e gestão de tráfego

## Event Sourcing e Observabilidade

### Entidade DomainEvent
- Armazena informações sobre o evento ocorrido (entidade afetada, ação, dados e timestamp)
- Cada evento é identificado por um **GUID de rastreamento (TraceId)**
- Os eventos são persistidos no MongoDB, permitindo reconstruir o estado do sistema a partir de um único TraceId

### Tracing e Logging
- Dois middlewares controlam o cabeçalho **CorrelationId** e propagam o **TraceId** entre os microsserviços
- Cada requisição é registrada no MongoDB com seu corpo e identificador de rastreamento
- Permite a reprodução completa de fluxos e análise de causa raiz em falhas ou inconsistências

### Monitoramento e Observabilidade com New Relic
Todo o ecossistema do Fiap Cloud Games — incluindo os microsserviços, funções serverless e gateway — é monitorado em tempo real pelo **New Relic**.

## Elasticsearch

O Elasticsearch é integrado ao sistema para otimizar consultas e permitir recomendações baseadas em dados.

**Implementações:**
- **Índice `games`:** Jogos são indexados
- **Índice `purchases`:** Compras são indexadas
- Permite correlacionar usuários e gêneros de jogos, facilitando futuras recomendações
- Suporte a consultas avançadas e agregações (ex.: Jogos recomendados a usuários por gênero)

## Tecnologias e Serviços Utilizados

| Categoria | Tecnologia |
|-----------|------------|
| Linguagem principal | .NET 8 (C#) |
| Banco de dados | SQL Server (Azure), MongoDB |
| Busca e recomendação | Elasticsearch |
| Serverless | Azure Functions |
| Mensageria e integração | HTTP / API Gateway |
| Autenticação | JWT |
| Observabilidade | Event Sourcing + Tracing (MongoDB) + New Relic |
| Deploy e Cloud | Azure |

## Fluxo Resumido do Sistema

1. **Autenticação:** O usuário realiza login via IAM, que gera o JWT
2. **Seleção:** O usuário escolhe um jogo e realiza a compra via Jogos
3. **Processamento:** O Jogos envia a solicitação ao Financeiro, com os ExternalIds do usuário e do jogo
4. **Pagamento:** O Financeiro registra e aguarda pagamentos
5. **Processamento Serverless:** O usuário faz pagamentos e aguarda o processamento da função serverless
6. **Validação:** Função serverless processa e avisa Financeiro sobre o pagamento
7. **Finalização:** Após confirmação do pagamento e validação de valores, o Financeiro notifica o IAM, que adiciona o jogo à biblioteca do usuário e envia o e-mail de confirmação
8. **Rastreamento:** Todos os eventos são rastreados e indexados no Elasticsearch e MongoDB
