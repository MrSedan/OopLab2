namespace LabBack.Models;

public class Server : Computer
{
    public Server()
        : base()
    {
        MaxConnections = 10;
        CurrentConnections = 0;
    }

    public Server(int processorFrequency, int ramAmount, int maxConnections)
        : base(processorFrequency, ramAmount)
    {
        MaxConnections = maxConnections;
        CurrentConnections = 0;
    }

    public Server(int processorFrequency, int ramAmount, int maxConnections, int currentConnections)
        : base(processorFrequency, ramAmount)
    {
        MaxConnections = maxConnections;
        CurrentConnections = currentConnections;
    }

    public int MaxConnections { get; set; }

    public int CurrentConnections { get; private set; }

    public void IncrementConnection()
    {
        CurrentConnections++;
    }

    public override string DisplayInfo()
    {
        return $"Сервер: ОЗУ {RamAmount} МБ, частота процессора {ProcessorFrequency} МГц, активных подключений {CurrentConnections} из максимально возможных {MaxConnections}";
    }

    public override string ExecuteTask()
    {
        return "Сервер обрабатывает сетевые запросы клиентов";
    }

    public bool AcceptConnection(out string message)
    {
        if (CurrentConnections < MaxConnections)
        {
            IncrementConnection();
            message = $"Сервер принял новое подключение. Текущее количество подключений: {CurrentConnections}";
            return true;
        }

        message = "Сервер перегружен. Невозможно принять новое подключение.";
        return false;
    }

    public string ProcessRequest()
    {
        return CurrentConnections > 0
            ? "Сервер обрабатывает сетевой запрос клиента"
            : "Сервер не имеет активных подключений для обработки запросов";
    }
}
