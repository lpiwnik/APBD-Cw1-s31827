# Computer Rental APP — PJATK APBD

Aplikacja konsolowa w C# (.NET 10) symulująca uczelnianą wypożyczalnię sprzętu komputerowego.  
Projekt realizowany w ramach przedmiotu APBD — ćwiczenia 1.

---

## Uruchomienie

```bash
cd Computer_Rental_APP
dotnet run
```

Aplikacja tworzy folder `Data/` automatycznie przy pierwszym uruchomieniu i zapisuje tam pliki JSON.  
Aby zresetować dane do stanu pustego, usuń pliki z folderu `Data/`.

## Aleternatywne uruchomienie za pomocą dockera
Aplikacja została skonteneryzowana i udostępniona jako gotowy obraz w rejestrze Docker Hub. Poniższa metoda pozwala na natychmiastowe uruchomienie projektu bez konieczności posiadania zainstalowanego SDK .NET czy kompilacji kodu źródłowego.

### 1. Wymagania
* Zainstalowane środowisko **Docker Desktop** lub **Docker Engine**.
* Terminal obsługujący komendy POSIX (np. **Git Bash**, **PowerShell** lub **Zsh**).

### 2. Komenda uruchamiająca (One-liner)

Aby pobrać obraz i uruchomić aplikację z zachowaniem trwałości danych (persystencja bazy danych), należy wykonać poniższe polecenie w katalogu głównym projektu:

```bash
mkdir -p Data && docker run -it --rm \
  -p 8080:8080 \
  --name s31827 \
  --mount type=bind,source="$(pwd)"/Data,target=/app/Data \
  lpiwnik/computer-rental:v1

```

W celu weryfikacji logiki biznesowej oraz persystencji danych, zachęcam do wykonania poniższych kroków:

Inicjalizacja: Po uruchomieniu kontenera wpisz komendę demo – spowoduje to załadowanie przygotowanego zestawu danych (test case).

Zatwierdzenie i wyjście: Zakończ działanie programu komendą exit. Dzięki zmapowanemu wolumenowi, baza danych zostanie trwale zapisana w lokalnym folderze ./Data.

Weryfikacja persystencji: Przy ponownym uruchomieniu tej samej komendy docker run, aplikacja automatycznie wczyta zapisaną sesję. Pozwoli to na natychmiastowe przetestowanie pozostałych funkcjonalności oraz wygenerowanie raportów na bazie wcześniej wprowadzonych danych.
### Wymagania

- .NET 10 SDK

---

## Dostępne komendy

| Komenda     | Opis                                     |
|-------------|------------------------------------------|
| `demo`      | Uruchamia pełny scenariusz demonstracyjny |
| `devices`   | Wyświetla cały sprzęt z aktualnym statusem |
| `available` | Wyświetla tylko sprzęt dostępny do wypożyczenia |
| `users`     | Wyświetla wszystkich użytkowników |
| `loans`     | Wyświetla aktywne wypożyczenia |
| `overdue`   | Wyświetla przeterminowane wypożyczenia |
| `report`    | Raport podsumowujący stan systemu |
| `help`      | Lista komend |
| `exit`      | Zamknięcie aplikacji |

---

## Struktura projektu

```
Computer_Rental_APP/
├── core/
│   ├── AppCommands.cs          # Warstwa UI / scenariusz demonstracyjny
│   └── OperationResult.cs      # Typ wynikowy operacji (generyczny)
├── Models/
│   ├── Abstractions/
│   │   ├── User.cs             # Abstrakcyjna klasa bazowa użytkownika
│   │   └── Device.cs           # Abstrakcyjna klasa bazowa sprzętu
│   ├── Users/
│   │   ├── Student.cs
│   │   ├── Employee.cs
│   │   └── StaffUser.cs
│   ├── Devices/
│   │   ├── Laptop.cs
│   │   ├── Camera.cs
│   │   └── Projector.cs
│   ├── Loans/
│   │   └── Loan.cs
│   ├── Enums/
│   │   ├── DeviceState.cs
│   │   ├── LoanStatus.cs
│   │   ├── OperationStatus.cs
│   │   ├── StudyLevel.cs
│   │   └── EmploymentType.cs
│   └── Interfaces/
│       ├── IEntity.cs
│       └── IDisplayable.cs
└── Services/
    ├── BaseService.cs          # Generyczny serwis z persystencją JSON
    ├── UserService.cs
    ├── UserRoleService.cs
    ├── DeviceService.cs
    └── LoanService.cs
```

---

## Scenariusz demonstracyjny (`demo`)

Komenda `demo` przechodzi przez 8 bloków testowych pokrywających wszystkie wymagania funkcjonalne:

| Blok | Zakres |
|------|--------|
| A — Role | Dodanie ról, duplikat roli (AlreadyExists), ujemny limit (ValidationError) |
| B — Użytkownicy | Dodanie użytkowników, duplikat numeru studenta, duplikat aliasu pracownika |
| C — Sprzęt | Dodanie 3 laptopów, kamery, projektora |
| D — Wypożyczenia | Poprawne wypożyczenia (Alice, Bob) |
| E — Walidacje wypożyczeń | Zajęty sprzęt (Busy), przekroczenie limitu (LimitExceeded), zmiana limitu w locie |
| F — Stan urządzenia | Oznaczenie jako Broken, próba wypożyczenia zepsutego sprzętu |
| G — Zwroty | Zwrot w terminie (brak kary), zwrot opóźniony z naliczoną karą |
| H — Raport | Końcowy raport stanu systemu |

---

## Decyzje projektowe

### Podział warstw i kohezja

Kod podzielony jest na trzy wyraźne warstwy, z których każda ma jedną odpowiedzialność:

- **Modele domenowe** (`Models/`) — wyłącznie dane i ich reprezentacja tekstowa (`ToShortRow`, `ToTemplateRow`). Nie zawierają logiki biznesowej.
- **Serwisy** (`Services/`) — wyłącznie logika biznesowa i persystencja. Nie wiedzą nic o konsoli ani o wyświetlaniu.
- **Warstwa UI** (`core/AppCommands.cs`) — wyłącznie orkiestracja i wyjście na konsolę. Nie zawiera reguł biznesowych.

`Program.cs` jest świadomie cienki — inicjalizuje serwisy, rejestruje menu i uruchamia pętlę. Żadna logika tam nie trafia.

### BaseService\<T\> — niski coupling, reużywalność

Cała logika persystencji (JSON, generowanie ID, CRUD) znajduje się w jednym miejscu — `BaseService<T>`. Każdy serwis dziedziczy po nim i dostaje `AddItemWithResult`, `UpdateItemProperty`, `DeleteItemWithResult` oraz `GetItemById` zwracające `OperationResult<T>`.

Dzięki temu `LoanService`, `UserService`, `DeviceService` i `UserRoleService` nie powielają kodu zapisu/odczytu pliku. Zmiana strategii persystencji (np. z JSON na bazę danych) wymaga zmiany tylko w jednym pliku.

### OperationResult / OperationResult\<T\>

Zamiast rzucać wyjątkami dla przewidywalnych błędów biznesowych (zajęty sprzęt, przekroczony limit), serwisy zwracają `OperationResult`. Klasa ma pole `Success`, `Message` i `Status` (enum `OperationStatus`). Generyczna wersja `OperationResult<T>` dodatkowo zwraca obiekt danych.

To rozwiązanie pozwala warstwie UI świadomie reagować na konkretny status (Success,
`NotFound`,
`Busy`,
`LimitExceeded`,
`ValidationError`,
`CriticalError`,
`AlreadyExists`) bez łapania wyjątków i bez ukrywania powodów niepowodzenia.

### UserRole jako nośnik reguł biznesowych

Limity wypożyczeń i stawki kar nie są zakodowane na stałe w serwisie ani w klasach `Student`/`Employee`. Są przechowywane w obiekcie `UserRole` (pola `LoanLimit` i `PenaltyRate`), który jest ładowany z pliku JSON i przypisywany do użytkownika.

Zmiana reguły biznesowej (np. podniesienie kary lub obniżenie limitu) wymaga aktualizacji jednego rekordu `UserRole` — efekt jest natychmiastowy dla wszystkich użytkowników z tą rolą, bez dotykania kodu serwisu.

### Snapshot cen w Loan

`Loan` przechowuje `SnapDeviceDailyRate` i `SnapPenaltyRate` — wartości skopiowane z urządzenia i roli w momencie wypożyczenia. Dzięki temu zmiana stawki po fakcie nie wpływa na już trwające lub historyczne wypożyczenia. To celowa decyzja projektowa zapewniająca spójność danych historycznych.

### Dziedziczenie i interfejsy

Dziedziczenie (`Student`, `Employee` → `User`; `Laptop`, `Camera`, `Projector` → `Device`) wynika z modelu domeny — typy mają wspólną część i specyficzne pola, nie jest to sztuczna hierarchia.

Interfejsy `IEntity` (kontrakt dla `Id`) i `IDisplayable` (kontrakt dla `ToShortRow`/`ToTemplateRow`) są używane jako ograniczenia w `BaseService<T where T : IEntity>` i jako konwencja wyświetlania w warstwie UI.

### Znana luka — DeviceService.MarkAsBroken

`MarkAsBroken` nie sprawdza czy urządzenie jest aktualnie wypożyczone (status `Rented`). Metoda pozwala zmienić stan niezależnie od bieżącego statusu. Świadoma obserwacja — dodanie guardu (`if device.State == Rented → return Failure(Busy)`) byłoby prostą poprawką, ale pozostawiono to jako punkt dyskusji projektowej.

---

## Reguły biznesowe

| Reguła | Implementacja |
|--------|--------------|
| Limit aktywnych wypożyczeń (Student / Employee) | `UserRole.LoanLimit`, sprawdzane w `LoanService.CreateLoan` |
| Kara za opóźnienie | `UserRole.PenaltyRate × daysOverdue`, obliczana w `LoanService.ReturnLoan` |
| Blokada wypożyczenia niedostępnego sprzętu | `Device.State != Available → Busy` w `LoanService.CreateLoan` |
| Unikalność numeru studenta | Walidacja w `UserService.AddUser` |
| Unikalność aliasu pracownika | Walidacja w `UserService.AddUser` |
| Unikalność nazwy roli | Walidacja w `UserRoleService.AddRole` |
| Minimum 1 dzień rozliczenia | `Math.Max(1, ...)` w `ReturnLoan` |

---

## Persistencja danych

Dane zapisywane są automatycznie do plików JSON w folderze `Data/`:

- `Data/users.json`
- `Data/devices.json`
- `Data/loans.json`
- `Data/usersRole.json`

Serializacja polimorficzna (`[JsonPolymorphic]`) pozwala na poprawny zapis i odczyt hierarchii klas (`Student`/`Employee`, `Laptop`/`Camera`/`Projector`).

---

## Autor

Łukasz Piwnik 
