using Computer_Rental_APP.core;
using Computer_Rental_APP.Services;

var userRoleService = new UserRoleService("Data/usersRole.json");
var userService     = new UserService("Data/users.json", userRoleService);
var deviceService   = new DeviceService("Data/devices.json");
var loanService     = new LoanService("Data/loans.json", userService, deviceService);

var app = new AppCommands(userService, deviceService, loanService, userRoleService);

Console.Clear();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("==================================================");
Console.WriteLine("       COMPUTER RENTAL SYSTEM — PJATK APBD        ");
Console.WriteLine("==================================================");
Console.ResetColor();
app.Help();

var menu = new Dictionary<string, Action>
{
    { "demo",      app.RunDemo           },
    { "devices",   app.ListAllDevices    },
    { "available", app.ListAvailableDevices },
    { "users",     app.ListAllUsers      },
    { "loans",     app.ListActiveLoans   },
    { "overdue",   app.ListOverdueLoans  },
    { "report",    app.PrintReport       },
    { "help",      app.Help              },
    { "exit",      () => Environment.Exit(0) }
};

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("\nCMD > ");
    Console.ResetColor();

    var input = Console.ReadLine()?.ToLower().Trim() ?? "";
    if (string.IsNullOrEmpty(input)) continue;

    if (menu.TryGetValue(input, out var action))
    {
        action();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("  Unknown command. Type 'help' for available options.");
        Console.ResetColor();
    }
}