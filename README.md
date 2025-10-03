# SpecCereal

Et RESTful Web API skrevet i C# .NET 8, der håndterer og parser ernæringsdata for morgenmadsprodukter (cereals). Projektet understøtter CRUD-operationer, simpel filtering og sortering, alle ikke-GET-kald er beskyttet bag "Authorization" headeren, CSV-import, billed hentning og tilbyder en interaktiv Swagger UI til test og dokumentation.

## Funktionalitet

- **CRUD for cereals**: Create, Read, Update og Delete produkter med ernæringsoplysninger.
- **CSV-import**: Upload og parse CSV-filer for bulk-import af produkter.
- **Billedhåndtering**: Automatisk tilknytning og udlevering af produktbilleder via produktets ID.
- **Brugerautentificering**: Basic authentication via header med BCrypt-hash af adgangskoder.
- **Rolleadministration**: Brugere har roller (standard: `basic`).
- **Swagger UI**: Interaktiv API-dokumentation og testmiljø.
- **Filtrering og sortering**: Mulighed for at filtrere og sortere produkter via query parameters.

## Projektstruktur

- `Cereal/` - ASP.NET Core Web API (backend)
- `CerealLib/` - Delt bibliotek med modeller og DTOs
- `ConsoleApp1/` - Konsolapplikation til test og hjælpefunktioner

## Kom godt i gang

### Forudsætninger

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL-database (se forbindelsesstreng i `Cereal/appsettings.json`)

### Opsætning

1. **Klon repository**
2. **Konfigurer database**
   - Opdater forbindelsesstrengen i `Cereal/appsettings.json` hvis nødvendigt.
3. **Kør migrationer**

   **Via terminal:**
   ```sh
   cd Cereal
   dotnet ef migrations add [navn på migration, fx init]
   dotnet ef database update
   ```

   **Via EF Core Package Manager Console (PMC) i Visual Studio:**
   ```powershell
   Add-Migration [navn på migration, fx init]
   Update-Database
   ```

4. **Start API**
   ```sh
   dotnet run
   ```

### Brug af API’et

- **Swagger UI**  
  Tilgå `/swagger` og brug "Authorize"-knappen for at logge ind.
- **Authentication**  
  Alle endpoints under `/api/v1/NutritionParser` kræver login (Basic Auth).
- **CSV-upload**  
  POST til `/api/v1/NutritionParser/UploadCSV` med filsti som body for at importere produkter fra CSV.
- **Opret enkelt produkt**  
  POST til `/api/v1/NutritionParser/UploadProduct` med produktdata i JSON.
- **Sletning**  
  DELETE på `/api/v1/NutritionParser/{id}` for at slette et produkt, eller `/api/v1/NutritionParser/DeleteAll` for at slette alle. Her bliver der kørt "Truncate"
- **Opdatering**  
  PUT på `/api/v1/NutritionParser/{id}` med opdaterede data.
- **Filtrering og sortering**  
  GET på `/api/Nutrition/products?category=Type&value=C&sorting=Calories_desc` for filtreret/sorteret resultat. Her er query parameterne optional.
- **Billeder**  
  GET på `/api/Nutrition/{id}/image` returnerer produktets billede.
- **Billedopsætning**  
  Kald `/api/Nutrition/setup` én gang for at matche billeder til produkter. Hvis der er sket en fejl. Hvor Nutrition.ImagePath ikke har noget.

### Responsekoder

Alle endpoints returnerer relevante HTTP-statuskoder, fx:
- `200 OK` – Succesfuld forespørgsel
- `201 Created` – Ressource oprettet
- `204 No Content` – Ingen indhold/slettet
- `400 Bad Request` – Forkert input
- `401 Unauthorized` – Manglende/ugyldig login (authorization header)
- `404 Not Found` – Ressource ikke fundet
- `409 Conflict` – Konflikt, fx duplikat
- `500 Internal Server Error` – Serverfejl

### Noter

- DTOs bruges til input og nogle output.
- IDs genereres automatisk i databasen.
- Adgangskoder hashes sikkert med BCrypt.
- Produktbilleder placeres i `wwwroot/Images/Products/` og navngives efter produktets navn.
- Ville gerne have lavet seeding, så databasen altid har noget, når api'et starter.

## Eksempel på JSON til oprettelse af produkt

```json
{
  "name": "Nyt Cereal",
  "mfr": "K",
  "type": "C",
  "calories": 100,
  "protein": 3,
  "fat": 1,
  "sodium": 150,
  "fiber": 2.5,
  "carbo": 15.0,
  "sugars": 5,
  "potass": 100,
  "vitamins": 25,
  "shelf": 2,
  "weight": 1.0,
  "cups": 0.75,
  "rating": 50.0
}
```
