
using System;

namespace ASD
{

    class ChangeMaking
    {

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// bez ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości amount ( czyli rzędu o(amount) )
        /// </remarks>
        public int? NoLimitsDynamic(int amount, int[] coins, out int[] change)
        {
            int coins_size = coins.Length;
            change = new int[coins_size];
            int[] t =  new int[amount + 1];
            int[] p = new int[amount + 1];
            for (int i = 0; i < amount + 1; i++)
            {
                t[i] = int.MaxValue;
                p[i] = 0;
            }

            t[0] = 0;
            for (int i = 1; i <= amount; i++)
            {
                for (int l = 0; l < coins_size; l++)
                {
                    int c = 0;
                    if (coins[l] <= i)
                    {
                        if (t[i - coins[l]] != int.MaxValue)
                        {
                            c = 1 + t[i- coins[l]];
                            if (c < t[i])
                            {
                                t[i] = c;
                                p[i] = l;
                            }
                        }
                    }
                }
            }

            if (t[amount] == int.MaxValue)
            {
                change = null;  // zmienić
                return null;
            }

            int kk = amount;
            while (kk > 0)
            {
                change[p[kk]]++;
                kk -= coins[p[kk]];
            }

            return t[amount];
            // zmienić
        }

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// z uwzględnieniem ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="limits">Liczba dostępnych monet danego nomimału</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// limits[i] - dostepna liczba monet i-tego rodzaju (nominału)
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości iloczynu amount*(liczba rodzajów monet)
        /// ( czyli rzędu o(amount*(liczba rodzajów monet)) )
        /// </remarks>
        public int? Dynamic(int amount, int[] coins, int[] limits, out int[] change)
        {
            int coins_size = coins.Length;
            change = new int[coins_size];
            int[,] t =  new int[coins_size,amount + 1];
            int[,] p = new int[coins_size,amount + 1];
            for (int i = 0; i < coins_size; i++)
            {
                for (int j = 0; j < amount + 1; j++)
                {
                    t[i, j] = int.MaxValue;
                    p[i, j] = 0;
                }
            }

            for (int k = 0; k <= limits[0] && k*coins[0] < amount +1; k++)
            {
                t[0,k*coins[0]] = k;
                p[0,k*coins[0]] = k;
            }
            for (int i = 1; i < coins_size; i++)
            {
                for (int l = 0; l < amount + 1; l++)
                {
                    for (int k = 0; k <= limits[i] && k * coins[i] <= l; k++)
                    {
                        int c = l - k * coins[i];

                        if (t[i - 1, c] != int.MaxValue)
                        {
                            int curr = t[i - 1, c] + k;
                            if (curr < t[i,l])
                            {
                                t[i,l] = curr;
                                p[i,l] = k;
                            }
                        }
                    }
                }
            }

            if (t[coins_size - 1, amount] == int.MaxValue)
            {
                change = null;  // zmienić
                return null;
            }

            int kk = amount;
            for(int i = coins_size - 1; i >= 0; i--)
            {
                int k = p[i, kk];
                change[i] = k;
                kk -= k* coins[i];
            }

            return t[coins_size -1, amount];
        }

    }

}
