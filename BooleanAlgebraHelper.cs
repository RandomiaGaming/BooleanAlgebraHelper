public static class BooleanAlgebraHelper
{
    //Returns the shortest byte[] which produces the specified truthTable.
    //Returns null if non could be found within the specified searchDepth number of bytes.
    //Only uses the symboles with in the expression table.
    public static byte[] FindEquation(byte[] truthTable, int searchDepth, byte[] expressionTable)
    {
        return FindEquationInternal(truthTable, searchDepth, expressionTable).output;
    }
    private static FindEquationReturnPacket FindEquationInternal(byte[] truthTable, int searchDepth, byte[] expressionTable)
    {
        //Recursie base case
        if (searchDepth <= 0)
        {
            FindEquationReturnPacket output = new FindEquationReturnPacket();
            output.permutationsTried = new byte[1][] { new byte[0] };
            return output;
        }
        else
        {
            //Call recursive function one layer down
            FindEquationReturnPacket subReturnPacket = FindEquationInternal(truthTable, searchDepth - 1, expressionTable);
            if (subReturnPacket.outputFound)
            {
                return subReturnPacket;
            }

            //Initialize packet
            FindEquationReturnPacket output = new FindEquationReturnPacket();
            output.permutationsTried = new byte[subReturnPacket.permutationsTried.Length * expressionTable.Length][];

            //Create permutations
            int index = 0;
            for (int i = 0; i < expressionTable.Length; i++)
            {
                for (int j = 0; j < subReturnPacket.permutationsTried.Length; j++)
                {
                    output.permutationsTried[index] = new byte[subReturnPacket.permutationsTried[j].Length + 1];
                    System.Array.Copy(subReturnPacket.permutationsTried[j], 0, output.permutationsTried[index], 1, subReturnPacket.permutationsTried[j].Length);
                    output.permutationsTried[index][0] = expressionTable[i];
                    index++;
                }
            }

            //Check permutations for solution
            for (int i = 0; i < output.permutationsTried.Length; i++)
            {
                if (AssertTable(output.permutationsTried[i], truthTable) is 1)
                {
                    output.outputFound = true;
                    output.output = output.permutationsTried[i];
                    return output;
                }
            }

            return output;
        }
    }
    private struct FindEquationReturnPacket
    {
        public byte[] output;
        public bool outputFound;
        public byte[][] permutationsTried;
    }


    public static readonly byte[][] lookupTable = new byte[22][]
    {
            new byte[1] { /*NA*/ 0 }, //0 F false
            new byte[1] { /*NA*/ 1 }, //1 T true

            new byte[2] { /*F*/ 0, /*T*/ 0 }, //2 FF Contradiction
            new byte[2] { /*F*/ 0, /*T*/ 1 }, //3 FT Return P
            new byte[2] { /*F*/ 1, /*T*/ 0 }, //4 TF NOT
            new byte[2] { /*F*/ 1, /*T*/ 1 }, //5 TT Tautology
            
            new byte[4] { /*FF*/ 0, /*FT*/ 0, /*TF*/ 0, /*TT*/ 0 }, //6  FFFF Contradiction
            new byte[4] { /*FF*/ 0, /*FT*/ 0, /*TF*/ 0, /*TT*/ 1 }, //7  FFFT And
            new byte[4] { /*FF*/ 0, /*FT*/ 0, /*TF*/ 1, /*TT*/ 0 }, //8  FFTF
            new byte[4] { /*FF*/ 0, /*FT*/ 0, /*TF*/ 1, /*TT*/ 1 }, //9  FFTT Return P
            new byte[4] { /*FF*/ 0, /*FT*/ 1, /*TF*/ 0, /*TT*/ 0 }, //10 FTFF
            new byte[4] { /*FF*/ 0, /*FT*/ 1, /*TF*/ 0, /*TT*/ 1 }, //11 FTFT Return Q
            new byte[4] { /*FF*/ 0, /*FT*/ 1, /*TF*/ 1, /*TT*/ 0 }, //12 FTTF XOR
            new byte[4] { /*FF*/ 0, /*FT*/ 1, /*TF*/ 1, /*TT*/ 1 }, //13 FTTT OR
            new byte[4] { /*FF*/ 1, /*FT*/ 0, /*TF*/ 0, /*TT*/ 0 }, //14 TFFF NOR
            new byte[4] { /*FF*/ 1, /*FT*/ 0, /*TF*/ 0, /*TT*/ 1 }, //15 TFFT Equals
            new byte[4] { /*FF*/ 1, /*FT*/ 0, /*TF*/ 1, /*TT*/ 0 }, //16 TFTF
            new byte[4] { /*FF*/ 1, /*FT*/ 0, /*TF*/ 1, /*TT*/ 1 }, //17 TFTT
            new byte[4] { /*FF*/ 1, /*FT*/ 1, /*TF*/ 0, /*TT*/ 0 }, //18 TTFF
            new byte[4] { /*FF*/ 1, /*FT*/ 1, /*TF*/ 0, /*TT*/ 1 }, //19 TTFT Implies
            new byte[4] { /*FF*/ 1, /*FT*/ 1, /*TF*/ 1, /*TT*/ 0 }, //20 TTTF NAND
            new byte[4] { /*FF*/ 1, /*FT*/ 1, /*TF*/ 1, /*TT*/ 1 }, //21 TTTT Tautology

        //22 Variable 0
        //23 Variable 1
        //24 Variable 2
        //25 Variable 3
        //26 Variable 4
        //27 Variable 5
        //28 Variable 6
        //29 Variable 7
    };
    public static byte Evaluate(byte[] equation)
    {
        if (equation is null)
        {
            throw new System.Exception("equation may not be null.");
        }
        EvaluationContext context = new EvaluationContext();
        context.equation = equation;
        return EvaluateInternal(ref context);
    }
    public static byte Evaluate(byte[] equation, byte varStates)
    {
        EvaluationContext context = new EvaluationContext();
        context.equation = equation;
        context.var0 = (byte)(varStates & 1);
        context.var1 = (byte)((varStates & 2) >> 1);
        context.var2 = (byte)((varStates & 4) >> 2);
        context.var3 = (byte)((varStates & 8) >> 3);
        context.var4 = (byte)((varStates & 16) >> 4);
        context.var5 = (byte)((varStates & 32) >> 5);
        context.var6 = (byte)((varStates & 64) >> 6);
        context.var7 = (byte)((varStates & 128) >> 7);
        return EvaluateInternal(ref context);
    }
    private struct EvaluationContext
    {
        public byte[] equation;
        public long currentIndex;
        public byte var0;
        public byte var1;
        public byte var2;
        public byte var3;
        public byte var4;
        public byte var5;
        public byte var6;
        public byte var7;
    }
    private static byte EvaluateInternal(ref EvaluationContext context)
    {
        if (context.currentIndex >= context.equation.Length)
        {
            return 255;
        }
        byte command = context.equation[context.currentIndex];
        if (command < 2)
        {
            return lookupTable[command][0];
        }
        else if (command < 6)
        {
            context.currentIndex++;
            byte arg = EvaluateInternal(ref context);
            if (arg is 255)
            {
                return 255;
            }
            return lookupTable[command][arg];
        }
        else if (command < 22)
        {
            context.currentIndex++;
            byte arg = EvaluateInternal(ref context);
            if (arg is 255)
            {
                return 255;
            }
            context.currentIndex++;
            byte arg2 = EvaluateInternal(ref context);
            if (arg2 is 255)
            {
                return 255;
            }
            arg = (byte)(arg | (arg2 << 1));
            return lookupTable[command][arg];
        }
        else if (command is 22)
        {
            return context.var0;
        }
        else if (command is 23)
        {
            return context.var1;
        }
        else if (command is 24)
        {
            return context.var2;
        }
        else if (command is 25)
        {
            return context.var3;
        }
        else if (command is 26)
        {
            return context.var4;
        }
        else if (command is 27)
        {
            return context.var5;
        }
        else if (command is 28)
        {
            return context.var6;
        }
        else if (command is 29)
        {
            return context.var7;
        }
        else
        {
            return 255;
        }
    }


    public static byte AssertTable(byte[] equation, byte[] truthTable)
    {
        byte varState = 0;
        for (int i = 0; i < truthTable.Length; i++)
        {
            if (Evaluate(equation, varState) != truthTable[i])
            {
                return 0;
            }
            varState++;
        }
        return 1;
    }


    public static byte[] ASMCompile(string asm)
    {
        if (asm is null)
        {
            throw new System.Exception("source may not be null.");
        }
        string[] statements = asm.Split(' ');
        byte[] output = new byte[statements.Length];
        for (int i = 0; i < statements.Length; i++)
        {
            string statement = statements[i];
            bool decypheredStatement = false;
            for (int j = 0; j < compilerRules.Length; j++)
            {
                CompilerRule currentRule = compilerRules[j];
                bool currentRuleApplies = false;
                for (int k = 0; k < compilerRules[j].Inputs.Length; k++)
                {
                    if (statement == currentRule.Inputs[k])
                    {
                        currentRuleApplies = true;
                        break;
                    }
                }
                if (currentRuleApplies)
                {
                    output[i] = currentRule.Output;
                    decypheredStatement = true;
                    break;
                }
            }
            if (!decypheredStatement)
            {
                throw new System.Exception($"Unable to decypher meaning of boolean ASM statement {statement}.");
            }
        }
        return output;
    }
    private struct CompilerRule
    {
        public byte Output;
        public string[] Inputs;
        public CompilerRule(byte output, params string[] inputs)
        {
            Output = output;
            Inputs = inputs;
        }
    }
    private static readonly CompilerRule[] compilerRules = new CompilerRule[]
    {
            new CompilerRule(0, "0", "F", "f", "false", "FALSE", "False"),
            new CompilerRule(1, "1", "T", "t", "true", "TRUE", "True"),
            new CompilerRule(2, "2", "FF", "ff"),
            new CompilerRule(3, "3", "FT", "ft"),
            new CompilerRule(4, "4", "TF", "tf", "NOT", "not", "~", "!"),
            new CompilerRule(5, "5", "TT", "tt"),
            new CompilerRule(6, "6", "FFFF", "ffff"),
            new CompilerRule(7, "7", "FFFT", "ffft", "AND", "and", "&&", "&", "^"),
            new CompilerRule(8, "8", "FFTF", "fftf"),
            new CompilerRule(9, "9", "FFTT", "fftt"),
            new CompilerRule(10, "10", "FTFF", "ftff"),
            new CompilerRule(11, "11", "FTFT", "ftft"),
            new CompilerRule(12, "12", "FTTF", "fttf", "XOR", "xor", "_", "x"),
            new CompilerRule(13, "13", "FTTT", "fttt", "OR", "or", "v", "||", "|"),
            new CompilerRule(14, "14", "TFFF", "tfff", "NOR", "nor"),
            new CompilerRule(15, "15", "TFFT", "tfft", "EQUALS", "equals", "Equals", "="),
            new CompilerRule(16, "16", "TFTF", "tftf"),
            new CompilerRule(17, "17", "TFTT", "tftt"),
            new CompilerRule(18, "18", "TTFF", "ttff"),
            new CompilerRule(19, "19", "TTFT", "ttft", "IMPLIES", "implies", "Implies", "->", ">"),
            new CompilerRule(20, "20", "TTTF", "tttf", "NAND", "nand"),
            new CompilerRule(21, "21", "TTTT", "tttt"),
            new CompilerRule(22, "22", "Var0", "var0", "v0", "V0"),
            new CompilerRule(23, "23", "Var1", "var1", "v1", "V1"),
            new CompilerRule(24, "24", "Var2", "var2", "v2", "V2"),
            new CompilerRule(25, "25", "Var3", "var3", "v3", "V3"),
            new CompilerRule(26, "26", "Var4", "var4", "v4", "V4"),
            new CompilerRule(27, "27", "Var5", "var5", "v5", "V5"),
            new CompilerRule(28, "28", "Var6", "var6", "v6", "V6"),
            new CompilerRule(29, "29", "Var7", "var7", "v7", "V7"),
    };


    private static readonly string[] visualizationTable = new string[30]
    {
            "F", //0 F false
            "T", //1 T true

            "2", //2 FF Contradiction
            "3", //3 FT Return P
            "!", //4 TF NOT
            "5", //5 TT Tautology
            
            "6", //6  FFFF Contradiction
            "&&", //7  FFFT And
            "8", //8  FFTF
            "9", //9  FFTT Return P
            "10", //10 FTFF
            "11", //11 FTFT Return Q
            "XOR", //12 FTTF XOR
            "||", //13 FTTT OR
            "NOR", //14 TFFF NOR
            "==", //15 TFFT Equals
            "16", //16 TFTF
            "17", //17 TFTT
            "18", //18 TTFF
            "->", //19 TTFT Implies
            "NAND", //20 TTTF NAND
            "21", //21 TTTT Tautology

            "Var0", //22 Variable 0
            "Var1", //23 Variable 1
            "Var2", //24 Variable 2
            "Var3", //25 Variable 3
            "Var4", //26 Variable 4
            "Var5", //27 Variable 5
            "Var6", //28 Variable 6
            "Var7", //29 Variable 7
    };
    private static readonly string[] textbookVisualizationTable = new string[30]
    {
            "F", //0 F false
            "T", //1 T true

            "2", //2 FF Contradiction
            "3", //3 FT Return P
            "~", //4 TF NOT
            "5", //5 TT Tautology
            
            "6", //6  FFFF Contradiction
            "^", //7  FFFT And
            "8", //8  FFTF
            "9", //9  FFTT Return P
            "10", //10 FTFF
            "11", //11 FTFT Return Q
            "XOR", //12 FTTF XOR
            "v", //13 FTTT OR
            "NOR", //14 TFFF NOR
            "=", //15 TFFT Equals
            "16", //16 TFTF
            "17", //17 TFTT
            "18", //18 TTFF
            "->", //19 TTFT Implies
            "NAND", //20 TTTF NAND
            "21", //21 TTTT Tautology

            "P", //22 Variable 0
            "Q", //23 Variable 1
            "Var2", //24 Variable 2
            "Var3", //25 Variable 3
            "Var4", //26 Variable 4
            "Var5", //27 Variable 5
            "Var6", //28 Variable 6
            "Var7", //29 Variable 7
    };
    public static string Visualize(byte[] equation, bool textbookMode = false)
    {
        if (equation is null)
        {
            throw new System.Exception("equation may not be null.");
        }
        VisualizationContext context = new VisualizationContext();
        context.equation = equation;
        if (textbookMode)
        {
            return VisualizeInternalTextbookMode(ref context);
        }
        else
        {
            return VisualizeInternal(ref context);
        }
    }
    private struct VisualizationContext
    {
        public byte[] equation;
        public long currentIndex;
    }
    private static string VisualizeInternal(ref VisualizationContext context)
    {
        if (context.currentIndex >= context.equation.Length)
        {
            throw new System.Exception("End of equation reached but arguments were still needed to saticify all opperators.");
        }
        byte command = context.equation[context.currentIndex];
        if (command < 2)
        {
            return visualizationTable[command];
        }
        else if (command < 6)
        {
            context.currentIndex++;
            string arg = VisualizeInternal(ref context);
            return visualizationTable[command] + arg;
        }
        else if (command < 22)
        {
            context.currentIndex++;
            string arg = VisualizeInternal(ref context);
            context.currentIndex++;
            string arg2 = VisualizeInternal(ref context);
            return "(" + arg + " " + visualizationTable[command] + " " + arg2 + ")";
        }
        else if (command < 30)
        {
            return visualizationTable[command];
        }
        else
        {
            throw new System.Exception($"Invalid control byte found in expression {command}.");
        }
    }
    private static string VisualizeInternalTextbookMode(ref VisualizationContext context)
    {
        if (context.currentIndex >= context.equation.Length)
        {
            throw new System.Exception("End of equation reached but arguments were still needed to saticify all opperators.");
        }
        byte command = context.equation[context.currentIndex];
        if (command < 2)
        {
            return textbookVisualizationTable[command];
        }
        else if (command < 6)
        {
            context.currentIndex++;
            string arg = VisualizeInternalTextbookMode(ref context);
            return textbookVisualizationTable[command] + arg;
        }
        else if (command < 22)
        {
            context.currentIndex++;
            string arg = VisualizeInternalTextbookMode(ref context);
            context.currentIndex++;
            string arg2 = VisualizeInternalTextbookMode(ref context);
            return "(" + arg + " " + textbookVisualizationTable[command] + " " + arg2 + ")";
        }
        else if (command < 30)
        {
            return textbookVisualizationTable[command];
        }
        else
        {
            throw new System.Exception($"Invalid control byte found in expression {command}.");
        }
    }
}