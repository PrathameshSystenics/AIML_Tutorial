using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore_Tutorial;


interface IService
{
    int Amount { get; set; }
    string getName();
    void Process(int amount);
}

class BankService : IService
{
    private int amount = 100;

    public int Amount
    {
        get { return this.amount; }
        set { }
    }

    public string getName()
    {
        return "IDFC First Bank";
    }

    public void Process(int amount)
    {
        Console.WriteLine("Processing the Account Transactions");
        this.amount += amount;
    }
}


class Account
{
    private readonly IService service;

    // Iservice is getting injecting into these class. 
    public Account(IService service)
    {
        this.service = service;
    }

    public int GetAmount()
    {
        return this.service.Amount;
    }

    public void ProcessAmount(int amount)
    {
        this.service.Process(amount);
    }

    public string GetBankName()
    {
        return this.service.getName() + " " + "Prathamesh Account";
    }


}

public class DependencyInj
{

    public static void Main()
    {
        // using the dependency injection

        ServiceCollection sevices = new ServiceCollection();

        // injecting the service
        sevices.AddScoped<IService, BankService>();
        sevices.AddScoped<Account>();

        // buidling the service
        var services = sevices.BuildServiceProvider();


        // getting the service
        Account service = services.GetRequiredService<Account>();

        // Accessing the methods of these class
        Console.WriteLine("======================== Account Class ========================");
        Console.WriteLine(service.GetAmount());
        service.ProcessAmount(200);
        Console.WriteLine(service.GetAmount());
        Console.WriteLine(service.GetBankName());

        IService service2 = services.GetRequiredService<IService>();

        // Accessing the methods and properties of these class
        Console.WriteLine("======================== Bank Service Class ==========================");
        Console.WriteLine(service2.Amount);
        service2.Process(400);
        Console.WriteLine(service2.Amount);
        Console.WriteLine(service2.getName());
    }
}

