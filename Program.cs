using System;
public static class Program
{
    public static void Main(string[] args)
    {
        for (int i = 6; i <= 21; i++)
        {
            byte[] result = BooleanAlgebraHelper.FindEquation(BooleanAlgebraHelper.lookupTable[i], 15, BooleanAlgebraHelper.ASMCompile("Var0 Var1 NOR"));

            Console.WriteLine(CreateHeader(BooleanAlgebraHelper.lookupTable[i]) + BooleanAlgebraHelper.Visualize(result, true));
        }
        Console.WriteLine("Calculated functions. Press any key to exit...");
        Console.ReadKey(true);
    }
    public static string CreateHeader(byte[] truthTable)
    {
        string output = "";
        foreach (byte b in truthTable)
        {
            if (b is 1)
            {
                output += "T";
            }
            else
            {
                output += "F";
            }
        }
        return output + ": ";
    }
}