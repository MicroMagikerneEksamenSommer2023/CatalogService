# API-dokumentation for Catalog Service

Nedenstående dokument beskriver de tilgængelige endpoints og deres funktionalitet i Catalog Service API.

## Endpoints

### GET /catalogservice/v1/getall

Beskrivelse: Henter alle katalog items.

Handling: Denne endpoint henter alle katalog items fra databasen.

Svar:
- 200 OK: Returnerer en liste af katalog items som JSON.
- 404 Not Found: Hvis der ikke findes nogen katalogposter.
- 500 Internal Server Error: Hvis der opstår en uventet fejl.

---

### GET /catalogservice/v1/getbyid/{id}

Beskrivelse: Henter en katalog item baseret på ID.

Parametre:
- `id` (streng): Katalog item unikke ID.

Handling: Denne endpoint henter en specifik katalog item baseret på det angivne ID.

Svar:
- 200 OK: Returnerer katalog item som JSON.
- 404 Not Found: Hvis katalog item ikke findes.
- 500 Internal Server Error: Hvis der opstår en uventet fejl.

---

### GET /catalogservice/v1/getbycategory/{category}

Beskrivelse: Henter katalog items baseret på kategori.

Parametre:
- `category` (streng): Kategorienavn.

Handling: Denne endpoint henter alle katalog items, der tilhører den angivne kategori.

Svar:
- 200 OK: Returnerer en liste af katalog items som JSON.
- 404 Not Found: Hvis der ikke findes nogen katalog items i den angivne kategori.
- 500 Internal Server Error: Hvis der opstår en uventet fejl.

---

### DELETE /catalogservice/v1/deletebyid/{id}

Beskrivelse: Sletter et katalog item baseret på ID.

Parametre:
- `id` (streng): Katalog item unikke ID.

Handling: Denne endpoint sletter et specifik katalog item baseret på det angivne ID.

Svar:
- 200 OK: Returnerer en bekræftelsesmeddelelse som JSON.
- 404 Not Found: Hvis katalog item ikke findes.
- 500 Internal Server Error: Hvis der opstår en uventet fejl.

---

### PUT /catalogservice/v1/updateitem

Beskrivelse: Opdaterer et katalog item.

Parametre:
- JSON-data (katalog item): Katalogiyemdata, der skal opdateres.
- Billeder (valgfrit): En eller flere billeder tilknyttet katalog item.

Handling: Denne endpoint opdaterer et eksisterende katalog item med den angivne data. Hvis der er billeder angivet, tilføjes eller opdateres de tilknyttede billeder.

Svar:
- 200 OK: Returnerer den opdaterede katalog item som JSON.
- 404 Not Found: Hvis katalog item ikke findes.
- 500 Internal Server Error: Hvis der opstår en uventet fejl.

---

### POST /catalogservice/v1/createitem

Beskrivelse: Opretter et ny katalog item.

# Dokumentation for Catalog Service

Nedenstående dokument beskriver klasserne og deres funktionalitet i Catalog Service.

## Klasser

### CatalogItem

Beskrivelse: Representerer en katalog item og indeholder information om et item, der kan vises i et katalog.

Attributter:
- `Id` (streng, valgfri): Den unikke ID for katalog item.
- `SellerId` (streng): ID'en for sælgeren af katalog item.
- `ItemName` (streng): Navnet på katalog item.
- `Description` (streng): Beskrivelse af katalog item.
- `Category` (streng): Kategorien, som katalog item tilhører.
- `Valuation` (decimaltal): Vurderingsværdien for katalog item.
- `StartingBid` (decimaltal): Startbudet for katalog item.
- `BuyoutPrice` (decimaltal): Købsprisen for katalog item.
- `StartTime` (DateTime): Starttidspunktet for katalog item.
- `EndTime` (DateTime): Sluttidspunktet for katalog item.

Formål: CatalogItem-klassen bruges til at repræsentere og håndtere information om katalog item i systemet.

### CatalogItemDB

Beskrivelse: Representerer et katalog item i databasen og indeholder information om et item, der er gemt i databasen.

Attributter:
- `Id` (streng, valgfri): Den unikke ID for katalog item.
- `SellerId` (streng): ID'en for sælgeren af katalog item.
- `ItemName` (streng): Navnet på katalog item.
- `Description` (streng): Beskrivelse af katalog item.
- `Category` (streng): Kategorien, som katalog item tilhører.
- `Valuation` (decimaltal): Vurderingsværdien for katalog item.
- `StartingBid` (decimaltal): Startbudet for katalog item.
- `BuyoutPrice` (decimaltal): Købsprisen for katalog item.
- `ImagePaths` (liste af streng, valgfri): Stier til billeder tilknyttet katalog item.
- `StartTime` (DateTime): Starttidspunktet for katalog item.
- `EndTime` (DateTime): Sluttidspunktet for katalog item.

Formål: CatalogItemDB-klassen bruges til at repræsentere og håndtere information om katalog item i databasen.

### Wrapper

Beskrivelse: Denne klasse indeholder oplysninger om starttid, sluttid, startpris og købspris for et item. Den bruges som en wrapper-klasse til at overføre disse oplysninger til endpointet.

Egenskaber:
- `StartTime` (DateTime): Starttidspunktet for et item.
- `EndTime` (DateTime): Sluttidspunktet for et item.
- `StartingPrice` (double): Startprisen for et item.
- `BuyoutPrice` (double): Købsprisen for et item.

Constructor:
- `Wrapper(DateTime startTime, DateTime endTime, double startingPrice, double buyoutPrice)`: Opretter en ny instans af Wrapper-klassen med de angivne starttid, sluttid, startpris og købspris.

Denne klasse bruges af Catalog Service til at overføre starttid, sluttid, startpris og købspris for et item til endpointet.

### ItemsNotFoundException

Beskrivelse: Denne klasse repræsenterer en undtagelse, der kastes, når der ikke findes nogen items i Catalog Service.

Constructor:
- `ItemsNotFoundException()`: Opretter en ny instans af ItemsNotFoundException-klassen uden nogen meddelelse.
- `ItemsNotFoundException(string message)`: Opretter en ny instans af ItemsNotFoundException-klassen med den angivne meddelelse.
- `ItemsNotFoundException(string message, Exception inner)`: Opretter en ny instans af ItemsNotFoundException-klassen med den angivne meddelelse og en indre undtagelse.

Denne klasse bruges af Catalog Service til at signalere, at der ikke findes nogen items i systemet.

### ImageResponse

Beskrivelse: Denne klasse repræsenterer svaret fra en billedanmodning. Den indeholder en liste af byte-arrays, der repræsenterer billedfiler, og yderligere data om et katalog item.

Egenskaber:
- `FileBytes` (List<byte[]>): En liste af byte-arrays, der repræsenterer billedfiler.
- `AdditionalData` (CatalogItem): Yderligere data om et katalog item.

Constructor:
- `ImageResponse(List<byte[]> fileBytes, CatalogItem additionalData)`: Opretter en ny instans af ImageResponse-klassen med den angivne liste af byte-arrays og yderligere data om en katalogvare.

Denne klasse bruges af Catalog Service til at sende svar på en billedanmodning sammen med yderligere data om et katalog item.


### JsonModelBinder

Beskrivelse: Denne klasse implementerer IModelBinder-interface og bruges til at binde JSON-data til modeller i ASP.NET Core.

Metode:
- `BindModelAsync(ModelBindingContext bindingContext)`: Implementerer bindeoperationen for JSON-data. Den forsøger at konvertere den modtagne JSON-data til den angivne modeltype og returnerer resultatet som en ModelBindingResult.

Denne klasse bruges af Catalog Service til at binde JSON-data til modeller under håndtering af anmodninger.

### TimeDTO

Beskrivelse: Denne klasse repræsenterer tidspunktet for et katalog item og dens ID. Den bruges til at overføre disse oplysninger til endpointet.

Egenskaber:
- `CatalogId` (string): ID'et for katalog item.
- `EndTime` (DateTime): Sluttidspunktet for katalog item.

Constructor:
- `TimeDTO(string catalogId, DateTime endTime)`: Opretter en ny instans af TimeDTO-klassen med den angivne katalog item-ID og sluttidspunkt.

Denne klasse bruges af Catalog Service til at overføre tidspunktet og ID'et for et katalog item til endpointet.



