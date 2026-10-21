using System;
using System.Collections.Generic;

class MealyMachine
{
    // (state, symbol) -> (nextState, output)
    private readonly Dictionary<(string, char), (string, int)> _delta = new()
    {
        { ("q0", 'a'), ("D",  0) }, { ("q0", '*'), ("D",  0) }, { ("q0", '/'), ("q1", 0) },
        { ("q1", 'a'), ("D",  0) }, { ("q1", '*'), ("q2", 0) }, { ("q1", '/'), ("D",  0) },
        { ("q2", 'a'), ("q2", 0) }, { ("q2", '*'), ("q3", 0) }, { ("q2", '/'), ("q2", 0) },
        { ("q3", 'a'), ("q2", 0) }, { ("q3", '*'), ("q3", 0) }, { ("q3", '/'), ("q4", 1) },
        { ("q4", 'a'), ("D",  0) }, { ("q4", '*'), ("D",  0) }, { ("q4", '/'), ("D",  0) },
        { ("D",  'a'), ("D",  0) }, { ("D",  '*'), ("D",  0) }, { ("D",  '/'), ("D",  0) },
    };

    private const string StartState = "q0";
    private const string EndState = "q4";

    // Any character that is not '*' or '/' acts as 'a'
    private static char Normalize(char c) => (c == '*' || c == '/') ? c : 'a';

    public List<int> Run(string input, bool trace = false)
    {
        var outputs = new List<int>();
        string state = StartState;

        foreach (char ch in input)
        {
            char sym = Normalize(ch);
            var (next, output) = _delta[(state, sym)];
            if (trace)
                Console.WriteLine($"    {state} --{ch} | {output}--> {next}");
            outputs.Add(output);
            state = next;
        }
        return outputs;
    }

    // Accepted if the last output is 1 (this happens exactly when we end in q4)
    public bool Accepts(string input)
    {
        var outputs = Run(input);
        return outputs.Count > 0 && outputs[^1] == 1;
    }
}

class Program
{
    static void Main()
    {
        var m = new MealyMachine();

        string[] accepted = { "/*a*/", "/**/", "/***/", "/*aaa*aaa*/", "/*a/a*/" };
        string[] rejected = { "/**", "/**/a/*aa*/", "aaa/**/aa", "/*/", "/**a/", "//aaaa" };

        Console.WriteLine("Expected ACCEPTED:");
        foreach (var s in accepted)
            Console.WriteLine($"  {s,-14} outputs=[{string.Join(",", m.Run(s))}] -> {m.Accepts(s)}");

        Console.WriteLine("\nExpected REJECTED:");
        foreach (var s in rejected)
            Console.WriteLine($"  {s,-14} outputs=[{string.Join(",", m.Run(s))}] -> {m.Accepts(s)}");

        Console.WriteLine("\nTry your own (blank line to quit):");
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (string.IsNullOrEmpty(line)) break;
            Console.WriteLine(m.Accepts(line) ? "ACCEPTED" : "REJECTED");
            m.Run(line, trace: true);
        }
    }
}

