# SpecCereal

## notes
- Siden jeg bruger DTO (data transfer objects) S� skal der aldrig sendes et ID med i en request. ID'et bliver auto generaret af databasen.
- Jeg bruger swagger ui til at "logge in". Ved at jeg trykker p� authorize knappen og inds�tter login oplysninger, s� s�tter den authorization headeren for alle requests.
- Jeg bruger bcrypt til at hashe passwords.