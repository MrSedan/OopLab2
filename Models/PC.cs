namespace LabBack.Models;

public class PC : Computer
{
    public PC()
        : base()
    {
        UserShell = "XFCE";
        Os = "Linux";
    }

    public PC(int processorFrequency, int ramAmount)
        : base(processorFrequency, ramAmount)
    {
        UserShell = "XFCE";
        Os = "Linux";
    }

    public PC(int processorFrequency, int ramAmount, string userShell)
        : base(processorFrequency, ramAmount)
    {
        UserShell = userShell;
        Os = "Linux";
    }

    public PC(int processorFrequency, int ramAmount, string userShell, string os)
        : base(processorFrequency, ramAmount)
    {
        UserShell = userShell;
        Os = os;
    }

    public string UserShell { get; set; }

    public string Os { get; set; }

    public void IncreaseRamAmount(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Количество памяти не может быть отрицательным.");
        }

        RamAmount += amount;
    }

    public override string DisplayInfo()
    {
        return $"ПК: ОЗУ {RamAmount} МБ, частота процессора {ProcessorFrequency} МГц, ОС {Os}, пользовательская оболочка {UserShell}";
    }

    public override string ExecuteTask()
    {
        return "ПК обрабатывает пользовательские приложения и офисные задачи";
    }

    public string OpenApplication(string appName)
    {
        return $"ПК открывает приложение: {appName}";
    }

    public string OpenWebsite(string url)
    {
        return $"ПК открывает сайт: {url}";
    }
}
