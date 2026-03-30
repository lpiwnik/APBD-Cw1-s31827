using Computer_Rental_APP.core;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Devices;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Loans;
using Computer_Rental_APP.Models.Users;
using Computer_Rental_APP.Services;

namespace Computer_Rental_APP.core;

public class AppCommands(
    UserService userService,
    DeviceService deviceService,
    LoanService loanService,
    UserRoleService roleService)
{
    // ─────────────────────────────────────────────
    //  HELP
    // ─────────────────────────────────────────────

    public void Help()
    {
        Section("AVAILABLE COMMANDS");
        Console.WriteLine($"  {"demo",-20} Run full demonstration scenario");
        Console.WriteLine($"  {"devices",-20} List all devices with status");
        Console.WriteLine($"  {"available",-20} List only available devices");
        Console.WriteLine($"  {"users",-20} List all users");
        Console.WriteLine($"  {"loans",-20} List all active loans");
        Console.WriteLine($"  {"overdue",-20} List overdue loans");
        Console.WriteLine($"  {"report",-20} Print system summary report");
        Console.WriteLine($"  {"help",-20} Show this help");
        Console.WriteLine($"  {"exit",-20} Exit application");
    }

    // ─────────────────────────────────────────────
    //  LIST METHODS  (używają ToShortRow z modeli)
    // ─────────────────────────────────────────────

    public void ListAllRoles()
    {
        Section("ROLES CONFIGURATION");
        Console.WriteLine($"  {"ID",-5} | {"Role Name",-15} | {"Limit",-7} | {"Penalty/Day",12}");
        Console.WriteLine("  " + new string('-', 48));
        foreach (var r in roleService.GetRolesList())
            Console.WriteLine("  " + r.ToShortRow());
    }

    public void ListAllUsers()
    {
        Section("ALL REGISTERED USERS");
        Console.WriteLine($"  {"ID",-5} | {"Full Name",-25} | {"Role",-15}");
        Console.WriteLine("  " + new string('-', 52));
        foreach (var u in userService.GetUsersList())
            Console.WriteLine("  " + u.ToShortRow());
    }

    public void ListAllDevices()
    {
        Section("HARDWARE INVENTORY");
        Console.WriteLine($"  {"ID",-5} | {"Device Name",-25} | {"State",-15} | {"Rate/Day",-5}");
        Console.WriteLine("  " + new string('-', 62));
        foreach (var d in deviceService.GetDevicesList())
            Console.WriteLine("  " + d.ToShortRow());
    }

    public void ListAvailableDevices()
    {
        Section("AVAILABLE DEVICES");
        var list = deviceService.GetDevicesList(DeviceState.Available);
        if (!list.Any()) { Console.WriteLine("  No devices available."); return; }
        Console.WriteLine($"  {"ID",-5} | {"Device Name",-25} | {"State",-15} | {"Rate/Day",-5}");
        Console.WriteLine("  " + new string('-', 62));
        foreach (var d in list)
            Console.WriteLine("  " + d.ToShortRow());
    }

    public void ListActiveLoans()
    {
        Section("ACTIVE LOANS");
        var list = loanService.GetLoansList()
            .Where(l => l.LoanStatus is LoanStatus.Active or LoanStatus.Overdue)
            .ToList();
        if (!list.Any()) { Console.WriteLine("  No active loans."); return; }
        PrintLoanHeader();
        foreach (var l in list)
            Console.WriteLine("  " + l.ToShortRow());
    }

    public void ListOverdueLoans()
    {
        Section("OVERDUE LOANS");
        var list = loanService.GetOverdueLoans();
        if (!list.Any()) { Console.WriteLine("  No overdue loans."); return; }
        PrintLoanHeader();
        foreach (var l in list)
            Console.WriteLine("  " + l.ToShortRow());
    }

    public void PrintReport()
    {
        Section("SYSTEM REPORT");
        var devices = deviceService.GetDevicesList();
        var loans   = loanService.GetLoansList();

        Console.WriteLine($"  Devices total        : {devices.Count}");
        Console.WriteLine($"  Available            : {devices.Count(d => d.State == DeviceState.Available)}");
        Console.WriteLine($"  Rented               : {devices.Count(d => d.State == DeviceState.Rented)}");
        Console.WriteLine($"  Broken / unavailable : {devices.Count(d => d.State is DeviceState.Broken or DeviceState.InService)}");
        Console.WriteLine($"  Active loans         : {loans.Count(l => l.LoanStatus == LoanStatus.Active)}");
        Console.WriteLine($"  Overdue              : {loanService.GetOverdueLoans().Count}");
        Console.WriteLine($"  Returned late        : {loans.Count(l => l.LoanStatus == LoanStatus.ReturnedLate)}");
        Console.WriteLine($"  Total fees collected : {loans.Sum(l => l.TotalFee):C}");
    }

    // ─────────────────────────────────────────────
    //  DEMO SCENARIO
    // ─────────────────────────────────────────────

    public void RunDemo()
    {
        Console.Clear();
        Banner("COMPUTER RENTAL SYSTEM — DEMO");
        Pause();

        // ════════════════════════════════════════════
        //  BLOK A: ROLE
        // ════════════════════════════════════════════
        SubBanner("BLOCK A — ROLES");

        // ── KROK 1: Dodanie ról ──────────────────────────────────────
        Step(1, "Adding roles: Student (limit 2, penalty 10 PLN/day) & Employee (limit 5, penalty 5 PLN/day)");

        PrintResult("Add role Student",  roleService.AddRole(new UserRole("Student",  2, 10.0m)));
        PrintResult("Add role Employee", roleService.AddRole(new UserRole("Employee", 5,  5.0m)));
        ListAllRoles();
        Pause();

        // ── KROK 2: Duplikat roli ────────────────────────────────────
        Step(2, "VALIDATION — Adding role 'Student' again — should FAIL (AlreadyExists)");

        PrintResult("Add role Student (duplicate)",
            roleService.AddRole(new UserRole("Student", 2, 10.0m)));
        Pause();

        // ── KROK 3: Nieprawidłowy limit (ujemny) ────────────────────
        Step(3, "VALIDATION — Setting loan limit to -5 — should FAIL (ValidationError)");

        var studentRole = roleService.GetRolesList().FirstOrDefault(r => r.Name == "Student");
        if (studentRole != null)
            PrintResult("UpdateLoanLimit(-5)", roleService.UpdateLoanLimit(studentRole.Id, -5));
        Pause();

        // ════════════════════════════════════════════
        //  BLOK B: UŻYTKOWNICY
        // ════════════════════════════════════════════
        SubBanner("BLOCK B — USERS");

        var sRole = roleService.GetRolesList().FirstOrDefault(r => r.Name == "Student");
        var eRole = roleService.GetRolesList().FirstOrDefault(r => r.Name == "Employee");

        if (sRole == null || eRole == null)
        {
            Console.WriteLine("  [ERROR] Roles not found — aborting demo.");
            return;
        }

        // ── KROK 4: Dodanie użytkowników ────────────────────────────
        Step(4, "Registering users: Alice & Bob (Students), John (Employee)");

        PrintResult("Add Alice (Student)",
            userService.AddUser(new Student("Alice", "Yellow", "alice@edu.pl", sRole.Id,
                "s1001", 3, StudyLevel.Engineer)));

        PrintResult("Add Bob (Student)",
            userService.AddUser(new Student("Bob", "Blue", "bob@edu.pl", sRole.Id,
                "s1002", 1, StudyLevel.Magister)));

        PrintResult("Add John (Employee)",
            userService.AddUser(new Employee("John", "White", "john@uni.pl", eRole.Id,
                "jwhite", EmploymentType.FullTime, new DateTime(2020, 9, 1))));

        ListAllUsers();
        Pause();

        // ── KROK 5: Duplikat numeru studenta ────────────────────────
        Step(5, "VALIDATION — Adding student with already existing number 's1001' — should FAIL (AlreadyExists)");

        PrintResult("Add Eve with s1001 (duplicate student number)",
            userService.AddUser(new Student("Eve", "Green", "eve@edu.pl", sRole.Id,
                "s1001", 2, StudyLevel.Engineer)));
        Pause();

        // ── KROK 6: Duplikat aliasu pracownika ──────────────────────
        Step(6, "VALIDATION — Adding employee with already existing alias 'jwhite' — should FAIL (AlreadyExists)");

        PrintResult("Add Mark with alias jwhite (duplicate alias)",
            userService.AddUser(new Employee("Mark", "Brown", "mark@uni.pl", eRole.Id,
                "jwhite", EmploymentType.PartTime, new DateTime(2022, 1, 15))));
        Pause();

        // ════════════════════════════════════════════
        //  BLOK C: SPRZĘT
        // ════════════════════════════════════════════
        SubBanner("BLOCK C — DEVICES");

        // ── KROK 7: Dodanie sprzętu ─────────────────────────────────
        Step(7, "Stocking inventory: 3 laptops, 1 camera, 1 projector");

        PrintResult("Add Dell XPS (Laptop)",    deviceService.AddDevice(new Laptop("Dell XPS",    50m, 16,   "Intel i7")));
        PrintResult("Add MacBook Pro (Laptop)", deviceService.AddDevice(new Laptop("MacBook Pro", 60m, 32,   "Apple M2")));
        PrintResult("Add Asus ROG (Laptop)",    deviceService.AddDevice(new Laptop("Asus ROG",    55m, 32,   "AMD Ryzen 9")));
        PrintResult("Add Sony A7 (Camera)",     deviceService.AddDevice(new Camera("Sony A7",     30m, 2000, "FE 24-70mm")));
        PrintResult("Add Epson 4K (Projector)", deviceService.AddDevice(new Projector("Epson 4K", 20m, "4K", 450)));

        ListAllDevices();
        Pause();

        // ════════════════════════════════════════════
        //  BLOK D: WYPOŻYCZENIA — HAPPY PATH
        // ════════════════════════════════════════════
        SubBanner("BLOCK D — LOANS: HAPPY PATH");

        var alice   = userService.GetUsersList().FirstOrDefault(u => u.FirstName == "Alice");
        var bob     = userService.GetUsersList().FirstOrDefault(u => u.FirstName == "Bob");
        var john    = userService.GetUsersList().FirstOrDefault(u => u.FirstName == "John");
        var dell    = deviceService.GetDevicesList().FirstOrDefault(d => d.Name.Contains("Dell"));
        var macbook = deviceService.GetDevicesList().FirstOrDefault(d => d.Name.Contains("MacBook"));
        var asus    = deviceService.GetDevicesList().FirstOrDefault(d => d.Name.Contains("Asus"));
        var sony    = deviceService.GetDevicesList().FirstOrDefault(d => d.Name.Contains("Sony"));
        var epson   = deviceService.GetDevicesList().FirstOrDefault(d => d.Name.Contains("Epson"));

        // ── KROK 8: Alice bierze Dell ────────────────────────────────
        Step(8, "Alice borrows Dell XPS for 7 days — should succeed");

        if (alice != null && dell != null)
            PrintResult("Alice borrows Dell XPS",
                loanService.CreateLoan(new Loan(alice.Id, dell.Id, DateTime.Now.AddDays(7))));

        // ── KROK 9: Bob bierze MacBook ───────────────────────────────
        Step(9, "Bob borrows MacBook Pro for 3 days — should succeed");

        if (bob != null && macbook != null)
            PrintResult("Bob borrows MacBook Pro",
                loanService.CreateLoan(new Loan(bob.Id, macbook.Id, DateTime.Now.AddDays(3))));

        ListActiveLoans();
        Pause();

        // ════════════════════════════════════════════
        //  BLOK E: WALIDACJE WYPOŻYCZEŃ
        // ════════════════════════════════════════════
        SubBanner("BLOCK E — LOANS: VALIDATION");

        // ── KROK 10: Zajęty sprzęt ──────────────────────────────────
        Step(10, "VALIDATION — Alice tries Dell XPS again — status Rented, should FAIL (Busy)");

        if (alice != null && dell != null)
        {
            var result = loanService.CreateLoan(new Loan(alice.Id, dell.Id, DateTime.Now.AddDays(3)));
            PrintResult("Alice borrows Dell XPS (already rented)", result);
            Expect("Busy", result.Status == OperationStatus.Busy);
        }
        Pause();

        // ── KROK 11: Alice bierze Asus (ma 1, limit 2 → ok) ─────────
        Step(11, "Alice borrows Asus ROG — 1 active loan, limit is 2 — should succeed");

        if (alice != null && asus != null)
            PrintResult("Alice borrows Asus ROG",
                loanService.CreateLoan(new Loan(alice.Id, asus.Id, DateTime.Now.AddDays(5))));

        // ── KROK 12: Alice przekracza limit (ma 2, limit 2) ──────────
        Step(12, "VALIDATION — Alice tries Sony A7 — 2 active loans, limit is 2, should FAIL (LimitExceeded)");

        if (alice != null && sony != null)
        {
            var result = loanService.CreateLoan(new Loan(alice.Id, sony.Id, DateTime.Now.AddDays(2)));
            PrintResult("Alice borrows Sony A7 (over limit)", result);
            Expect("LimitExceeded", result.Status == OperationStatus.LimitExceeded);
        }
        Pause();

        // ── KROK 13: Zmiana limitu → nowa reguła działa natychmiast ──
        Step(13, "BUSINESS RULE CHANGE — Student loan limit: 2 → 1");

        if (studentRole != null)
        {
            PrintResult("UpdateLoanLimit(1)", roleService.UpdateLoanLimit(studentRole.Id, 1));
            ListAllRoles();
        }
        Pause();

        Step(13, "VALIDATION — Bob tries Epson 4K — new limit is 1, Bob already has 1 loan, should FAIL (LimitExceeded)");

        if (bob != null && epson != null)
        {
            var result = loanService.CreateLoan(new Loan(bob.Id, epson.Id, DateTime.Now.AddDays(2)));
            PrintResult("Bob borrows Epson 4K (limit now 1, Bob has 1 loan)", result);
            Expect("LimitExceeded", result.Status == OperationStatus.LimitExceeded);
        }

        // ── Przywrócenie limitu przed dalszymi testami ───────────────
        Step(13, "Restoring Student loan limit back to 2");
        if (studentRole != null)
            PrintResult("UpdateLoanLimit(2)", roleService.UpdateLoanLimit(studentRole.Id, 2));
        Pause();

        // ════════════════════════════════════════════
        //  BLOK F: WALIDACJE STANU URZĄDZENIA
        // ════════════════════════════════════════════
        SubBanner("BLOCK F — DEVICE STATE VALIDATION");

        // ── KROK 14: Oznaczenie wypożyczonego jako Broken ────────────
        Step(14, "OBSERVATION — MarkAsBroken on Rented Dell XPS (no guard in DeviceService)");

        if (dell != null)
        {
            var stateBefore = deviceService.GetDeviceById(dell.Id).ObjectData?.State;
            Console.WriteLine($"  Dell XPS state before: {stateBefore}");

            var result = deviceService.MarkAsBroken(dell.Id);
            PrintResult("MarkAsBroken(Dell XPS — currently Rented)", result);

            var stateAfter = deviceService.GetDeviceById(dell.Id).ObjectData?.State;
            Console.WriteLine($"  Dell XPS state after : {stateAfter}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  [NOTE] DeviceService.MarkAsBroken has no Rented-state guard.");
            Console.WriteLine("         Adding: if (device.State == Rented) return Failure(Busy)");
            Console.WriteLine("         would prevent this. Documented as known design gap.");
            Console.ResetColor();
        }
        Pause();

        // ── KROK 15: Próba wypożyczenia zepsutego urządzenia ─────────
        Step(15, "VALIDATION — Bob tries to borrow broken Dell XPS — should FAIL (Busy)");

        if (bob != null && dell != null)
        {
            var result = loanService.CreateLoan(new Loan(bob.Id, dell.Id, DateTime.Now.AddDays(1)));
            PrintResult("Bob borrows Dell XPS (Broken)", result);
            Expect("Busy", result.Status == OperationStatus.Busy);
        }
        Pause();

        // ── KROK 16: Oznaczenie wolnego Sony A7 jako Broken ──────────
        Step(16, "Marking free Sony A7 as BROKEN — should succeed");

        if (sony != null)
            PrintResult("Sony A7 → Broken", deviceService.MarkAsBroken(sony.Id));

        Step(16, "VALIDATION — John tries to borrow broken Sony A7 — should FAIL (Busy)");

        if (john != null && sony != null)
        {
            var result = loanService.CreateLoan(new Loan(john.Id, sony.Id, DateTime.Now.AddDays(3)));
            PrintResult("John borrows Sony A7 (Broken)", result);
            Expect("Busy", result.Status == OperationStatus.Busy);
        }

        ListAllDevices();
        Pause();

        // ════════════════════════════════════════════
        //  BLOK G: ZWROTY
        // ════════════════════════════════════════════
        SubBanner("BLOCK G — RETURNS");

        // ── KROK 17: Zwrot w terminie — Bob / MacBook ────────────────
        Step(17, "Bob returns MacBook Pro on time — no penalty expected");

        var bobLoan = loanService.GetActiveUserLoans(bob?.Id ?? -1).FirstOrDefault();
        if (bobLoan != null)
        {
            var result = loanService.ReturnLoan(bobLoan.Id);
            PrintResult("Bob returns MacBook Pro", result);

            var detail = loanService.GetLoanById(bobLoan.Id);
            if (detail.ObjectData != null)
                Console.WriteLine(detail.ObjectData.ToTemplateRow());
        }
        Pause();

        // ── KROK 18: Zwrot opóźniony — Alice / Asus ──────────────────
        Step(18, "Simulating Alice's Asus ROG as 3 days overdue — penalty = 3 × 10 PLN = 30 PLN");

        var aliceAsusLoan = loanService.GetActiveUserLoans(alice?.Id ?? -1)
            .FirstOrDefault(l => l.DeviceId == asus?.Id);

        if (aliceAsusLoan != null)
        {
            loanService.SimulateDueDate(aliceAsusLoan.Id, DateTime.Now.AddDays(-3));
            var result = loanService.ReturnLoan(aliceAsusLoan.Id);
            PrintResult("Alice returns Asus ROG (3 days late)", result);

            var detail = loanService.GetLoanById(aliceAsusLoan.Id);
            if (detail.ObjectData != null)
                Console.WriteLine(detail.ObjectData.ToTemplateRow());
        }
        Pause();

        // ── KROK 19: Aktywne wypożyczenia Alice ──────────────────────
        Step(19, "Active loans for Alice after all returns");

        var aliceActive = loanService.GetActiveUserLoans(alice?.Id ?? -1);
        if (!aliceActive.Any())
            Console.WriteLine("  Alice has no active loans.");
        else
        {
            PrintLoanHeader();
            foreach (var l in aliceActive)
                Console.WriteLine("  " + l.ToShortRow());
        }
        Pause();

        // ── KROK 20: Lista przeterminowanych ─────────────────────────
        Step(20, "Listing overdue loans");
        ListOverdueLoans();
        Pause();

        // ════════════════════════════════════════════
        //  BLOK H: RAPORT KOŃCOWY
        // ════════════════════════════════════════════
        SubBanner("BLOCK H — FINAL REPORT");
        Step(21, "Final system report");
        PrintReport();

        Banner("DEMO FINISHED");
    }

    // ─────────────────────────────────────────────
    //  UI HELPERS
    // ─────────────────────────────────────────────

    private void Step(int n, string description)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"\n[STEP {n}] " + new string('─', 50));
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  {description}");
        Console.ResetColor();
    }

    private void Section(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n>>> {title}");
        Console.ResetColor();
    }

    private void SubBanner(string title)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"\n  ┌{new string('─', 54)}┐");
        Console.WriteLine($"  │  {title,-52}│");
        Console.WriteLine($"  └{new string('─', 54)}┘");
        Console.ResetColor();
    }

    private void Banner(string title)
    {
        var line = new string('═', 60);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n{line}\n  {title}\n{line}");
        Console.ResetColor();
    }

    private void PrintResult(string label, OperationResult result)
    {
        Console.ForegroundColor = result.Success ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"  {(result.Success ? "✓" : "✗")} {label}: {result.Message}");
        Console.ResetColor();
    }

    private void Expect(string expectedStatus, bool matches)
    {
        Console.ForegroundColor = matches ? ConsoleColor.DarkGreen : ConsoleColor.Red;
        Console.WriteLine($"  {(matches ? "✓" : "✗")} Expected: {expectedStatus} — {(matches ? "CORRECT" : "UNEXPECTED RESULT")}");
        Console.ResetColor();
    }

    private static void PrintLoanHeader() =>
        Console.WriteLine($"  {"ID",-5} | {"Dev",3} : {"Device",-14} | {"Usr",3} : {"User",-19} | {"Status",-12}");

    private void Pause()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("\n  [Press Enter to continue...]");
        Console.ResetColor();
        Console.ReadLine();
    }
}