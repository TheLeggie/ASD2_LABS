using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace ASD
{
    public class SheepHeard : MarshalByRefObject
    {
        /// <param name="dogs">Tablica opisującapozycje psow</param>
        /// <param name="shed">Pozycja sciany budynku gospodarczego</param>
        /// <returns>Wierzchołki wielokatku w którym owce są bezpieczne</returns>
        public (double x, double y)[] SafePolygon((double x, double y)[] dogs, (float A, float B, float C) shed)
        {
            if (dogs.Length < 2)
                return null;
            var list = new List<(double, double)>(dogs);
            var op = new List<(double, double)>();
            var lower = new List<(double, double)>();
            var upper = new List<(double, double)>();
            foreach(var i in dogs)
            {
                op.Add(OrtogonalProjection(i, shed));
            }
            list.Add(op.Max());
            list.Add(op.Min());
            list.Sort();
            for(int i = 0; i < list.Count; i++)
            {
                while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], list[i]) <= 0)
                    lower.RemoveAt(lower.Count - 1);
                lower.Add(list[i]);
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], list[i]) <= 0)
                    upper.RemoveAt(upper.Count - 1);
                upper.Add(list[i]);
            }

            upper.RemoveAt(upper.Count - 1);
            lower.RemoveAt(lower.Count - 1);
            lower.AddRange(upper);
            return lower.ToArray();
        }
        public int CheckCoverage((double x, double y)[] sheeps, (double x, double y)[] dogs, (float A, float B, float C) shed)
        {
            var sp = SafePolygon(dogs, shed);
            if (sp == null)
                return 0;
            int curr, flag, count = 0;
            (int, int) possibleRange;
            for (int i = 0; i < sheeps.Length; i++)
            {
                curr = sp.Length / 2;
                possibleRange = (1, sp.Length - 1);
                do
                {
                    if (Cross(sp[0], sheeps[i], sp[(possibleRange.Item1 + possibleRange.Item2) / 2]) > 0)
                    {
                        flag = 1;
                        possibleRange.Item2 = (int)Math.Floor((decimal)((possibleRange.Item1 + possibleRange.Item2) / 2));
                    }
                    else if (Cross(sp[0], sheeps[i], sp[(possibleRange.Item1 + possibleRange.Item2) / 2]) < 0)
                    {
                        flag = 1;
                        possibleRange.Item1 = (int)Math.Floor((decimal)((possibleRange.Item1 + possibleRange.Item2) / 2));
                    }
                    else
                    {
                        flag = 0;
                        if (Distance(sp[curr], sheeps[i]) <= Distance(sp[0], sp[curr]) && Distance(sp[0], sheeps[i]) <= Distance(sp[0], sp[curr]))
                            count++;
                        break;
                    }
                }
                while (Math.Abs(possibleRange.Item1 - possibleRange.Item2) != 1);

                if (flag != 0)
                    if (Cross(sp[0], sp[possibleRange.Item1], sheeps[i]) >= 0 && Cross(sp[possibleRange.Item1], sp[possibleRange.Item2], sheeps[i]) >= 0 && Cross(sp[possibleRange.Item2], sp[0], sheeps[i]) >= 0)
                        count++;
            }
            return count;
        }
        public (double, double) OrtogonalProjection((double, double) point, (float A, float B, float C) eq)
        {
            if (eq.B == 0)
            {
                if (eq.A == 0)
                    return (0, 0);
                else
                    return (-eq.C / eq.A, point.Item2);
            }
            if (eq.A == 0)
            {
                if (eq.B == 0)
                    return (0, 0);
                else
                    return (point.Item1, -eq.C / eq.B);
            }
            return ((-1 * eq.A * eq.B * point.Item2 + Math.Pow(eq.B, 2) * point.Item1 - eq.A * eq.C) / (Math.Pow(eq.A, 2) + Math.Pow(eq.B, 2)),
                ((-1 * eq.A * (-1 * eq.A * eq.B * point.Item2 + Math.Pow(eq.B, 2) * point.Item1 - eq.A * eq.C)) / (eq.B * (Math.Pow(eq.A, 2) + Math.Pow(eq.B, 2)))) - eq.C / eq.B);
        }
        public int Cross((double, double) o, (double, double) a, (double, double) b)
        {
            double value = (a.Item1 - o.Item1) * (b.Item2 - o.Item2) - (a.Item2 - o.Item2) * (b.Item1 - o.Item1);
            return Math.Abs(value) < 1e-10 ? 0 : value < 0 ? -1 : 1;
        }
        public double Distance((double, double) point1, (double, double) point2)
        {
            return Math.Sqrt(Math.Pow(point1.Item1 - point2.Item1, 2) + Math.Pow(point1.Item2 - point2.Item2, 2));
        }
    }
}