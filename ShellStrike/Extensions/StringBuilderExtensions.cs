using ShellStrike.Card;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class StringBuilderExtensions
{
    public static bool EndsWith(this StringBuilder stringBuilder, string MatchCase, out string FoundCase)
    {
        if (MatchCase.Length > stringBuilder.Length) { FoundCase = string.Empty; return false; }
        string textAtEnd = stringBuilder.ToString(stringBuilder.Length - MatchCase.Length, MatchCase.Length);
        if (textAtEnd.Equals(MatchCase))
        {
            FoundCase = MatchCase;
            return true;
        }
        else { FoundCase = string.Empty; return false; }
    }

    public static bool EndsWith(this StringBuilder stringBuilder, string MatchCase)
    {
        if (MatchCase.Length > stringBuilder.Length) { return false; }
        string textAtEnd = stringBuilder.ToString(stringBuilder.Length - MatchCase.Length, MatchCase.Length);
        return textAtEnd.Equals(MatchCase);
    }

    public static bool EndsWith(this StringBuilder stringBuilder, string[] MatchCases)
    {
        foreach (string MatchCase in MatchCases)
        {
            if (MatchCase.Length > stringBuilder.Length) { return false; }
            string textAtEnd = stringBuilder.ToString(stringBuilder.Length - MatchCase.Length, MatchCase.Length);
            if (textAtEnd.Equals(MatchCase))
                return true;
        }
        return false;
    }

    [DebuggerStepThrough]
    public static bool EndsWith(this StringBuilder stringBuilder, List<BreakCase> BreakCases, out BreakCase BreakCaseOut)
    {

        if (stringBuilder.Length % 150 == 0) { Thread.Sleep(5); }
        BreakCase BreakCase;
        string bcText;
        string[] bSplittedCases;
        for (int b = 0; b < BreakCases.Count; b++)
        {
            BreakCase = BreakCases[b];
            bcText = BreakCase.Text + "|";
            bSplittedCases = bcText.Split('|');
            for (int s = 0; s < bSplittedCases.Length; s++)
            {
                string bcSplittedCase = bSplittedCases[s];
                if (string.IsNullOrEmpty(bcSplittedCase) || bcSplittedCase.Length > stringBuilder.Length) { continue; }
                string textAtEnd = stringBuilder.ToString(stringBuilder.Length - bcSplittedCase.Length, bcSplittedCase.Length);
                if (textAtEnd == bcSplittedCase)
                {
                    BreakCaseOut = BreakCase;
                    return true;
                }
            }
        }
        BreakCaseOut = null;
        return false;
    }
}
