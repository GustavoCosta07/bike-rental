# Bike Rental System - Setup e Execução

## Sobre o Projeto
Este é um sistema de gerenciamento de aluguel de motos, composto por um **backend em .NET 8.0** com uma **API RESTful**, um **banco de dados PostgreSQL** e integração com serviços de mensageria (**AWS SQS/SNS**).  
O ambiente é totalmente containerizado utilizando **Docker**.  

O sistema permite o gerenciamento de:
- Motos
- Entregadores
- Aluguéis

Também há validação de dados e armazenamento de arquivos, como imagens de licenças de condução (CNH).

## Pré-requisitos
Antes de iniciar, certifique-se de ter instalado:

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- Ferramentas para acesso ao banco de dados, como [pgAdmin](https://www.pgadmin.org/) ou [DBeaver](https://dbeaver.io/) (opcional)

## Como Rodar o Projeto

Na raiz do repositório (onde está o arquivo `docker-compose.yml`), execute:

```sh
docker-compose up --build
```

Esse comando irá construir as imagens, baixar as dependências e iniciar os containers.  
A primeira execução pode levar alguns minutos, dependendo do hardware e da sua conexão com a internet.

> **Dica:** Para rodar os containers em segundo plano, utilize:
```sh
docker-compose up -d --build
```

Para acompanhar os logs dos containers via terminal:

```sh
docker-compose logs -f
```

Ou utilize o **Docker Desktop** para inspecionar os containers e ver os logs. É possível verificar, por exemplo, se os testes foram executados com sucesso.

## 2. Acessar o Projeto

- **Swagger (Documentação da API):** [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)
- **Banco de Dados (PostgreSQL):** Roda na porta `5432`

Para acessar o banco de dados diretamente, use as credenciais definidas no `docker-compose.yml` ou `appsettings.json`. Por padrão:

- **Host:** `localhost`
- **Porta:** `5432`
- **Usuário:** `bikeuser`
- **Senha:** `bikepassword`
- **Database:** `bikerental`

## Testando as Rotas da API

### 1. **Motorcycles**

- **GET /api/motorcycles**  
  Lista todas as motos cadastradas no banco.

- **POST /api/motorcycles**  
  Cria uma nova moto. Se o ano informado for `2024`, um evento será salvo na fila (via AWS SQS/SNS).  
  Após essa chamada, será retornado o `id` da nova moto.

- **GET /api/motorcycles/{id}**  
  Busca uma moto específica pelo `id` retornado anteriormente.

- **PUT /api/motorcycles/{id}**  
  Atualiza a placa da moto pelo `id`.

- **DELETE /api/motorcycles/{id}**  
  Deleta uma moto.  
  > **Recomendação:** Não delete a moto criada por você, pois a moto criada por padrão já está vinculada a uma locação no fluxo.

### 2. **DeliveryPersons**

- **POST /api/delivery-persons**  
  Cria um novo entregador. O objeto enviado já está pré-definido corretamente.

- **GET /api/delivery-persons/{id}**  
  Busca um entregador específico pelo `id` retornado na chamada de criação.

- **POST /api/delivery-persons/{id}/driver-license-image**  
  Adiciona a imagem da CNH para o entregador pelo `id`.

### 3. **Rentals**

- **POST /api/rentals**  
  Cria uma nova locação.  
  > **Atenção:**  
  - Use o `id` da moto que você criou (a default já está alugada)  
  - Use o `id` do entregador criado por você

- **GET /api/rentals/{id}**  
  Busca uma locação pelo `id`.

- **PUT /api/rentals/{id}**  
  Atualiza a data de devolução da locação e calcula encargos com base na data fornecida.

## Considerações Finais

Se algum container demorar a subir, aguarde um pouco. O Docker pode levar alguns instantes para inicializar todos os serviços corretamente.

Caso queira reiniciar do zero (resetar os dados do banco):

```sh
docker-compose down -v
docker-compose up --build
```

Isso irá remover os volumes do banco de dados e restaurar os dados iniciais.

---
