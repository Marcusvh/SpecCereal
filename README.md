# SpecCereal

## notes
- Siden jeg bruger DTO (data transfer objects) Så skal der aldrig sendes et ID med i en request. ID'et bliver auto generaret af databasen.
- Jeg bruger swagger ui til at "logge in". Ved at jeg trykker på authorize knappen og indsætter login oplysninger, så sætter den authorization headeren for alle requests.
- Jeg bruger bcrypt til at hashe passwords.