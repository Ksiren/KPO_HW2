using KPO_app2;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Security.Principal;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<FinancialAccountingApp>();
        app.Run();
    }


    static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MemoryStorage<BankAccount>>();
        services.AddSingleton<MemoryStorage<Category>>();
        services.AddSingleton<MemoryStorage<Operation>>();

        services.AddSingleton<IStorage<BankAccount>>(provider =>
            new StorageProxy<BankAccount>(provider.GetRequiredService<MemoryStorage<BankAccount>>()));
        services.AddSingleton<IStorage<Category>>(provider =>
            new StorageProxy<Category>(provider.GetRequiredService<MemoryStorage<Category>>()));
        services.AddSingleton<IStorage<Operation>>(provider =>
            new StorageProxy<Operation>(provider.GetRequiredService<MemoryStorage<Operation>>()));

        services.AddSingleton<IBankFactory, BankFactory>();
        services.AddSingleton<IBankAccountFacade, BankAccountFacade>();
        services.AddSingleton<IOperationFacade, OperationFacade>();


        services.AddSingleton<BalanceAnalytics>();
        services.AddSingleton<CategoryAnalytics>();

        services.AddSingleton<DataImporterTemplate, JsonImporter>();
        services.AddSingleton<FinancialAccountingApp>();
    }
}

public class FinancialAccountingApp
{
    private readonly IBankAccountFacade accountFacade;
    private readonly IOperationFacade operationFacade;
    private readonly BalanceAnalytics balanceAnalytics;
    private readonly CategoryAnalytics categoryAnalytics;
    private readonly DataImporterTemplate importerTemplate;
    private readonly IBankFactory factory;
    private readonly IStorage<BankAccount> bankAccountStorage;
    private readonly IStorage<Category> categoryStorage;
    private readonly IStorage<Operation> operationStorage;

    public FinancialAccountingApp(IBankAccountFacade _accountFacade, IOperationFacade _operationFacade,
        BalanceAnalytics _balanceAnalytics, CategoryAnalytics _categoryAnalytics ,DataImporterTemplate _importerTemplate, IBankFactory _factory,
        IStorage<BankAccount> _bankAccountStorage, IStorage<Category> _categoryStorage, IStorage<Operation> _operationStorage)
    {
        accountFacade = _accountFacade;
        operationFacade = _operationFacade;
        balanceAnalytics = _balanceAnalytics;
        categoryAnalytics = _categoryAnalytics;
        importerTemplate = _importerTemplate;
        factory = _factory;
        bankAccountStorage = _bankAccountStorage;
        categoryStorage = _categoryStorage;
        operationStorage = _operationStorage;
    }

    public void Run()
    {
        Console.WriteLine("=== Банк: Учет финансов ===");

        Program();

        ShowStatistics();
    }

    private void Program()
    {
        try
        {

            string menu = "Меню:\n1.Создать счет \n2.Редактnировать счет \n3. Создать категорию \n4. Создать Операцию \n5. Редактировать операцию \n6. Анализ \n7. Экспорт\n8.Импорт \n0.Exit";
            int userAnswer = 100;
            while (userAnswer != 0)
            {
                switch (userAnswer)
                {
                    case 1: AddingAccount(); break;
                    case 2: RedactAccount(); break;
                    case 3: AddingCategory(); break;
                    case 4: AddingOperation(); break;
                    case 5: RedactOperation(); break;
                    case 6: ShowStatistics(); break;
                    case 7: ExportingFiles(); break;
                    case 8: ImportingFiles(); break;
                    default: Console.WriteLine("(You have to choose number 1-8 or 0 if you want to finish)\n"); break;
                }
                Console.WriteLine(menu);
                bool is_int = int.TryParse(Console.ReadLine(), out userAnswer);
                if (!is_int) userAnswer = -1;
            }

            Console.WriteLine("\nGoodbye :)");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private void ShowStatistics()
    {
        FinancialData finData = new FinancialData(bankAccountStorage.GetAll(), categoryStorage.GetAll(), operationStorage.GetAll());
        balanceAnalytics.Visit(finData);
        categoryAnalytics.Visit(finData);
        Console.WriteLine($"Счет изменился на: {balanceAnalytics.balanceDifference}\n");
        Console.WriteLine($"Доходы: {balanceAnalytics.totalIncome}\n");
        Console.WriteLine($"Расходы: {balanceAnalytics.totalExpenses}\n");
        Console.WriteLine("Доходы:");
        foreach (var elem in categoryAnalytics.incomeByCategory) { Console.WriteLine( $"{elem.Key}  {elem.Value}\n"); }
        Console.WriteLine("Расходы:");
        foreach (var elem in categoryAnalytics.expensesByCategory) { Console.WriteLine($"{elem.Key} {elem.Value}\n"); }
    }

    private void AddingAccount()
    {
        Console.WriteLine("Write name:\n");
        string name = Console.ReadLine();
        Console.WriteLine("Write balance:\n");
        bool is_int = int.TryParse(Console.ReadLine(), out int balance);
        var account = factory.MakeBankAccount(name, balance);
        if (account == null) { Console.WriteLine("can't be negative balance or null name");  return; }
        Console.WriteLine($"Создан счет с ID: {account.id}");
        bankAccountStorage.Add(account);
    }
    private void RedactAccount()
    {
        Console.WriteLine("1.Delete Account\n2.Change Account Name\n3. Get all accounts\n4. Change balance\n");
        bool is_int = int.TryParse(Console.ReadLine(), out int userAnswer);
        if (!is_int) userAnswer = -1;
        switch (userAnswer)
        {
            case 1:
                foreach (var acc in bankAccountStorage.GetAll())
                {
                    Console.WriteLine($"{acc.name} - id {acc.id}\n");
                }
                Console.WriteLine("Write id of account from upper list\n");
                bool is_guid = Guid.TryParse(Console.ReadLine(), out Guid id);
                if (!is_guid) Console.WriteLine("Not Id\n"); return;
                accountFacade.DeleteAccount(id); break;
            case 2:
                foreach (var acc in bankAccountStorage.GetAll())
                {
                    Console.WriteLine($"{acc.name} - id {acc.id}\n");
                }
                Console.WriteLine("Write id of account from upper list\n");
                is_guid = Guid.TryParse(Console.ReadLine(), out id);
                if (!is_guid) { Console.WriteLine("Not Id\n"); return; }
                Console.WriteLine("write new name:\n");
                string new_name = Console.ReadLine();
                accountFacade.ChangeAccountName(id, new_name); break;
            case 3:
                foreach (var acc in bankAccountStorage.GetAll())
                {
                    Console.WriteLine($"{acc.name} - id {acc.id}  --{acc.balance}--\n");
                } break;
            case 4:
                foreach (var acc in bankAccountStorage.GetAll())
                {
                    Console.WriteLine($"{acc.name} - id {acc.id} (balance {acc.balance})\n");
                }
                Console.WriteLine("Write id of account from upper list\n");
                is_guid = Guid.TryParse(Console.ReadLine(), out id);
                if (!is_guid) {Console.WriteLine("Not Id\n"); return; }
                Console.WriteLine("write new balance:\n");
                bool is_int2 = int.TryParse(Console.ReadLine(), out int new_balance);
                accountFacade.ChangeBalance(id, new_balance); break;

            default: Console.WriteLine("(You have to choose number 1-3)\n"); break;
        }
    }

    private void AddingCategory()
    {
        Console.WriteLine("Write name:\n");
        string name = Console.ReadLine();
        Console.WriteLine("Choose type:\n0. Расход \n1.Доход\n");
        bool is_int = int.TryParse(Console.ReadLine(), out int type_num);
        bool type;
        if (is_int)
        {
            switch (type_num)
            {
                case 0: type = false; break;
                case 1: type = true; break;
                default: Console.WriteLine("You have to choose number 1 or 0"); return;
            }
        }
        else { Console.WriteLine("You have to choose number 1 or 0"); return; }

        var category = factory.MakeCategory(type, name);
        if (category == null) { Console.WriteLine("can't be null name"); return; }
        Console.WriteLine($"Создана категория с ID: {category.id}");
        categoryStorage.Add(category);

    }

    private void AddingOperation()
    {
        Console.WriteLine("Write description (if you need):\n");
        string description = Console.ReadLine();

        Console.WriteLine("Choose account:\n");
        foreach (var acc in bankAccountStorage.GetAll())
        {
            Console.WriteLine($"{acc.name} - id {acc.id}\n");
        }
        Console.WriteLine("Write id of account from upper list\n");
        bool is_guid = Guid.TryParse(Console.ReadLine(), out Guid bank_account_id);
        if (!is_guid) {Console.WriteLine("Not Id\n"); return; }

        Console.WriteLine("Write amount of operation:");
        bool is_catagory = decimal.TryParse(Console.ReadLine(), out decimal amount);
        if (!is_catagory) {Console.WriteLine("wrongg\n"); return; }
        if (amount < 0) { amount *= -1; }

        Console.WriteLine("Choose category:\n");
        foreach (var cat in categoryStorage.GetAll())
        {
            Console.WriteLine($"{cat.name} - id {cat.id}  ({(cat.type ? "Доход": "Расход")})\n");
        }
        Console.WriteLine("Write id of category from upper list\n");
        is_guid = Guid.TryParse(Console.ReadLine(), out Guid category_id);
        if (!is_guid) { Console.WriteLine("Not Id\n"); return; }

        DateTime date = DateTime.Now;
        var operation = factory.MakeOperation(categoryStorage.GetById(category_id).type, bank_account_id, amount, date, category_id, description, bankAccountStorage);
        if (operation == null) { Console.WriteLine("wrong date"); return; }
        Console.WriteLine($"Создана операция с ID: {operation.id}");
        operationStorage.Add(operation);

    }
    void RedactOperation()
    {
        Console.WriteLine("1.Delete Operation\n2.Change Operation Description\n3. Get Account Operations\n 4. Get Category Operations\n");
        bool is_int = int.TryParse(Console.ReadLine(), out int userAnswer);
        if (!is_int) userAnswer = -1;
        switch (userAnswer)
        {
            case 1:
                foreach (var oper in operationStorage.GetAll())
                {
                    Console.WriteLine($"{oper.date} - id {oper.id}  (description - '{oper.description}')\n");
                }
                Console.WriteLine("Write id of operation from upper list\n");
                bool is_guid = Guid.TryParse(Console.ReadLine(), out Guid id);
                if (!is_guid) { Console.WriteLine("Not Id\n"); return; }
                operationFacade.DeleteOperation(id); break;
            case 2:
                foreach (var oper in operationStorage.GetAll())
                {
                    Console.WriteLine($"{oper.date} - id {oper.id}  (description - '{oper.description}')\n");
                }
                Console.WriteLine("Write id of operation from upper list\n");
                is_guid = Guid.TryParse(Console.ReadLine(), out id);
                if (!is_guid) { Console.WriteLine("Not Id\n"); return; }
                Console.WriteLine("write new description:\n");
                string new_description = Console.ReadLine();
                operationFacade.ChangeOperDescription(id, new_description); break;
            case 3:
                foreach (var acc in bankAccountStorage.GetAll())
                {
                    Console.WriteLine($"{acc.name} - id {acc.id}\n");
                }
                Console.WriteLine("Write id of account from upper list\n");
                is_guid = Guid.TryParse(Console.ReadLine(), out id);
                if (!is_guid) { Console.WriteLine("Not Id\n"); return; }
                operationFacade.GetAccountOperations(id); break;
            case 4:
                foreach (var cat in categoryStorage.GetAll())
                {
                    Console.WriteLine($"{cat.name} - id {cat.id}\n");
                }
                Console.WriteLine("Write id of category from upper list\n");
                is_guid = Guid.TryParse(Console.ReadLine(), out id);
                if (!is_guid) { Console.WriteLine("Not Id\n"); return; }
                operationFacade.GetCategoryOperations(id); break;
            default: Console.WriteLine("(You have to choose number 1-4)\n"); break;
        }

    }

    void ImportingFiles()
    {
        Console.WriteLine("1.JSON\n");
        bool is_int = int.TryParse(Console.ReadLine(), out int userAnswer);
        if (!is_int) userAnswer = -1;
        switch (userAnswer)
        {
            case 1:
                Console.WriteLine("Write path to file:");
                string path = Console.ReadLine();
                break;
            default: Console.WriteLine("out of range\n"); break;
        }
    }

    void ExportingFiles()
    {
        Console.WriteLine("1.JSON\n2.CSV\n");
        bool is_int = int.TryParse(Console.ReadLine(), out int userAnswer);
        if (!is_int) userAnswer = -1;
        Console.WriteLine("That you want to parse?\n1.Category\n2.Account\n3.Operation\n");
        bool is_int2 = int.TryParse(Console.ReadLine(), out int userAnswer2);
        if (!is_int2) userAnswer = -1;
        Category importingThing;
        BankAccount importingThing1;
        Operation importingThing2;
        switch (userAnswer2)
        {
            case 1:
                Console.WriteLine("write id of category");
                bool is_guid = Guid.TryParse(Console.ReadLine(), out Guid id);
                if (!is_guid) Console.WriteLine("Not Id\n"); return;
                importingThing = categoryStorage.GetById(id);
                break;
            case 2:
                Console.WriteLine("write id of category");
                 is_guid = Guid.TryParse(Console.ReadLine(), out id);
                if (!is_guid) Console.WriteLine("Not Id\n"); return;
                importingThing1 = bankAccountStorage.GetById(id);
                break;
            case 3:
                Console.WriteLine("write id of category");
                is_guid = Guid.TryParse(Console.ReadLine(), out id);
                if (!is_guid) Console.WriteLine("Not Id\n"); return;
                importingThing2 = operationStorage.GetById(id);
                break;
            default: Console.WriteLine("operstion out of range\n"); return;
        }
        switch (userAnswer)
        {
            case 1:
                JsonExportVisitor visitor = new JsonExportVisitor();
                switch (userAnswer2) {
                    case 1:  visitor.Visit(importingThing); break;
                    case 2: visitor.Visit(importingThing1); break;
                    case 3: visitor.Visit(importingThing2); break;
                }
                break;
            case 2:
                CsvExportVisitor visitor1 = new CsvExportVisitor();
                switch (userAnswer2)
                {
                    case 1: visitor1.Visit(importingThing); break;
                    case 2: visitor1.Visit(importingThing1); break;
                    case 3: visitor1.Visit(importingThing2); break;
                }
                break;
            default: Console.WriteLine("operstion out of range\n"); break;
        }
    }
}