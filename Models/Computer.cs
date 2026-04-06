namespace LabBack.Models;

public abstract class Computer
{
    public int Id { get; set; }

    protected Computer()
    {
    }

    protected Computer(int processorFrequency)
    {
        ProcessorFrequency = processorFrequency;
        RamAmount = 1024;
    }

    protected Computer(int processorFrequency, int ramAmount)
    {
        ProcessorFrequency = processorFrequency;
        RamAmount = ramAmount;
    }

    public int ProcessorFrequency { get; set; }

    public int RamAmount { get; set; }

    public abstract string DisplayInfo();

    public abstract string ExecuteTask();
}
