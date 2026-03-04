using System;

namespace ASD
{
    class CrossoutChecker
    {
        /// <summary>
        /// Sprawdza, czy podana lista wzorców zawiera wzorzec x
        /// </summary>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="x">Jedyny znak szukanego wzorca</param>
        /// <returns></returns>
        bool comparePattern(char[][] patterns, char x)
        {
            foreach (char[] pat in patterns)
            {
                if (pat.Length == 1 && pat[0] == x)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sprawdza, czy podana lista wzorców zawiera wzorzec xy
        /// </summary>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="x">Pierwszy znak szukanego wzorca</param>
        /// <param name="y">Drugi znak szukanego wzorca</param>
        /// <returns></returns>
        bool comparePattern(char[][] patterns, char x, char y)
        {
            foreach (char[] pat in patterns)
            {
                if (pat.GetLength(0) == 2 && pat[0] == x && pat[1] == y)
                    return true;
            }
            return false;
        }

        private int[,] GetErasuresDP(char[] sequence, char[][] patterns)
        {
            int n = sequence.Length;
            int[,] dp = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dp[i, j] = int.MaxValue;
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (comparePattern(patterns, sequence[i]))
                {
                    dp[i, i] = 1;
                }
            }

            for (int len = 2; len <= n; len++)
            {
                for (int i = 0; i <= n - len; i++)
                {
                    int j =  i + len -1;
                    if (comparePattern(patterns, sequence[i], sequence[j]))
                    {
                        if (len == 2)
                        {
                            dp[i, j] = Math.Min(dp[i, j], 1);
                        }
                        else if (dp[i + 1, j - 1] != int.MaxValue)
                        {
                            dp[i, j] = Math.Min(dp[i, j], dp[i + 1, j - 1] + 1);
                        }
                    }

                    for (int k = i; k < j; k++)
                    {
                        if (dp[i, k] != int.MaxValue && dp[k + 1, j] != int.MaxValue)
                        {
                            dp[i, j] = Math.Min(dp[i, j], dp[i, k] + dp[k + 1, j]);
                        }
                    }
                }
            }
            return dp;
        }

        /// <summary>
        /// Metoda sprawdza, czy podany ciąg znaków można sprowadzić do ciągu pustego przez skreślanie zadanych wzorców.
        /// Zakładamy, że każdy wzorzec składa się z jednego lub dwóch znaków!
        /// </summary>
        /// <param name="sequence">Ciąg znaków</param>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="crossoutsNumber">Minimalna liczba skreśleń gwarantująca sukces lub int.MaxValue, jeżeli się nie da</param>
        /// <returns></returns>
        public bool Erasable(char[] sequence, char[][] patterns, out int crossoutsNumber)
        {
            if (sequence.Length == 0)
            {
                crossoutsNumber = 0;
                return false;
            }
            int[,] dp = GetErasuresDP(sequence, patterns);
            int n = sequence.Length;
            int result = dp[0, n - 1];
            if (result == int.MaxValue)
            {
                crossoutsNumber = int.MaxValue;
                return false;
            }
            else
            {
                crossoutsNumber = result;
                return true;
            }
        }

        /// <summary>
        /// Metoda sprawdza, jaka jest minimalna długość ciągu, który można uzyskać z podanego poprzez skreślanie zadanych wzorców.
        /// Zakładamy, że każdy wzorzec składa się z jednego lub dwóch znaków!
        /// </summary>
        /// <param name="sequence">Ciąg znaków</param>
        /// <param name="patterns">Lista wzorców</param>
        /// <returns></returns>
        public int MinimumRemainder(char[] sequence, char[][] patterns)
        {
            if (sequence.Length == 0) return 0;
            int n = sequence.Length;
            int[,] dp = GetErasuresDP(sequence, patterns);
            
            int[] remainder = new int[n+1];
            remainder[0] = 0;

            for (int i = 1; i <= n; i++)
            {
                remainder[i] = remainder[i - 1] + 1;
                for (int j = 0; j < i; j++)
                {
                    if (dp[j, i - 1] != int.MaxValue)
                    {
                        remainder[i] =  Math.Min(remainder[i], remainder[j]);
                    }
                }
            }
            return remainder[n];
        }

        // można dopisać metody pomocnicze

    }
}
