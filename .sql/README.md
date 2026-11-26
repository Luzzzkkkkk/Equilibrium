# Banco de dados do Equilibrium

O MySQL não fica dentro do repositório: ele é um serviço instalado e iniciado no computador. A aplicação usa o banco `sistema_saude_mental` no servidor local (`localhost`). O arquivo `01_criar_banco.sql` cria o banco e as tabelas correspondentes aos modelos do Entity Framework.

## Como preparar

1. Instale e inicie o MySQL Server.
2. Abra `.sql/01_criar_banco.sql` no MySQL Workbench e execute, ou rode no terminal:

   ```powershell
   mysql -u root -p < .sql/01_criar_banco.sql
   ```

3. Confira a conexão em `Equilibrium/appsettings.Development.json`. Ajuste usuário, senha e servidor para os dados da sua instalação.
4. Inicie a aplicação:

   ```powershell
   dotnet run --project Equilibrium/Equilibrium.csproj
   ```

O `Program.cs` também chama `EnsureCreatedAsync`, que cria as tabelas quando o banco está vazio. O script é útil para preparar o banco manualmente e consultar sua estrutura. Não insira senhas em texto puro diretamente nas tabelas: o login espera senhas protegidas pelo `PasswordHasher` do ASP.NET.

No ambiente `Development`, a aplicação prepara a conta administrativa configurada em `AdminSeed` na primeira inicialização. A senha é convertida em hash antes de ser gravada no banco e não é exibida nas telas.
