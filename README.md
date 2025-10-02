# SpecCereal

Et RESTful web API skrevet i C# .NET, der håndterer og parser ernæringsdata om morgenmadsprodukter (cereals). API’et understøtter CRUD-operationer, og alle ikke-GET-kald er beskyttet bag "Authorization"-headeren, hvor gyldige loginoplysninger kræves. Der er også understøttelse for hentning af produktbilleder baseret på produktets ID.

## Funktioner

- **Ernærings-API**: CRUD-operationer for morgenmadsprodukter med ernæringsoplysninger.
- **CSV-import**: Upload og parsing af CSV-filer for bulk import af data.
- **Billedhåndtering**: Tilknytning og udlevering af produktbilleder via API’et.
- **Brugerautentificering**: Simpel autentificering med BCrypt-hash af adgangskoder.
- **Rolleadministration**: Brugere har roller (standard: `basic`). Pt. uden funktionalitet.
- **Swagger UI**: Interaktiv API-dokumentation og test.

## Projektstruktur

- `Cereal/` - ASP.NET Core Web API-projekt (backend)
- `CerealLib/` - Delt bibliotek med modeller og DTO’er
- `ConsoleApp1/` - Konsolapplikation til diverse hurtige/midlertidige tests

## Kom godt i gang

### Forudsætninger

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL-database (se connectionString i `Cereal/appsettings.json`)

### Opsætning

1. **Klon projektet**
2. **Konfigurer databasen**  
   Opdater connectionStringen i `Cereal/appsettings.json`.
3. **Anvend migrationer**

   **Via terminal:**
   ```sh
   dotnet ef migrations add [navn på migration, eg. init]
   ```
   Hvorefter du skal opdatere databasen med den migration:
   ```sh
   dotnet ef database update
   ```

   **Via EF Core Package Manager Console (PMC) i Visual Studio:**
   ```powershell
   Add-Migration [navn på migration, eg. init]
   ```
   Hvorefter du skal opdatere databasen med den migration:
   ```powershell
   Update-Database
   ```

4. **Start API’et**
   ```sh
   dotnet run --project Cereal
   ```
   API’et vil være tilgængeligt på [http://localhost:5201/swagger](http://localhost:5201/swagger).

### Brug af API’et

- **Swagger UI**:  
  Gå til `/swagger` og brug "Authorize"-knappen for at logge ind.
- **Autentificering**:  
  Alle endpoints under `/api/v1/NutritionParser` kræver login.
- **CSV-upload**:  
  Brug `/api/v1/NutritionParser/UploadCSV` til at importere produkter fra en CSV-fil.
- **Oprettelse af enkelt produkt**:  
  Brug `/api/v1/NutritionParser/UploadProduct` til at oprette et enkelt produkt.
- **Billedopsætning**:  
  Kald `/api/Nutrition/setup` én gang for at tilknytte billeder til produkter.

### Noter

- DTO’er bruges til input og visse output; ID’er genereres automatisk af databasen.
- Adgangskoder hashes sikkert med BCrypt.
- Produktbilleder placeres i `Cereal/wwwroot/Images/Products/` og navngives efter produktets navn (uden filendelse).
