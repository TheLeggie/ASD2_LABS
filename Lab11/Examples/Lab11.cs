using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ASD
{
    public class Lab11 : System.MarshalByRefObject
    {
        // iloczyn wektorowy
        private int Cross((double, double) o, (double, double) a, (double, double) b)
        {
            double value = (a.Item1 - o.Item1) * (b.Item2 - o.Item2) - (a.Item2 - o.Item2) * (b.Item1 - o.Item1);
            return Math.Abs(value) < 1e-10 ? 0 : value < 0 ? -1 : 1;
        }

        // Etap 1
        // po prostu otoczka wypukła

        private double DistSq((double, double) p1, (double, double) p2)
        {
            return (p1.Item1 - p2.Item1) * (p1.Item1 - p2.Item1) + (p1.Item2 - p2.Item2) * (p1.Item2 - p2.Item2);
        }

        public (double, double)[] ConvexHull((double, double)[] points)
        {
            if (points.Length < 3) return points;
            var p = points.Distinct().ToArray();
            int n = p.Length;
            if (n < 3) return p;
            (double, double)[] sorted = new (double Item1, double Item2)[n];
            double minY = int.MaxValue;
            int startIdx = -1;
            for (int i = 0; i < n; i++)
            {
                if (p[i].Item2 < minY || (p[i].Item2 == minY && p[i].Item1 < p[startIdx].Item1))
                {
                    minY = p[i].Item2;
                    startIdx = i;
                }
            }

            for (int i = 0; i < n; i++)
            {
                sorted[i] = p[i];
            }

            var start = sorted[0];
            sorted[0] = sorted[startIdx];
            sorted[startIdx] = start;

            Array.Sort(sorted, 1, n - 1, Comparer<(double, double)>.Create((a, b) =>
            {
                int cross = Cross(sorted[0], a, b);
                if (cross == 0) // Jeśli punkty są na tej samej prostej
                {
                    return DistSq(sorted[0], a).CompareTo(DistSq(sorted[0], b));
                }

                return cross == 1 ? -1 : 1;
            }));

            (double, double)[] eliminateLinear = new (double, double)[sorted.Length];
            eliminateLinear[0] = sorted[0];
            int count = 1;
            for (int i = 1; i < n; i++)
            {
                while (i < n - 1 && Cross(sorted[0], sorted[i], sorted[i + 1]) == 0)
                {
                    i++;
                }

                eliminateLinear[count] = sorted[i];
                count++;
            }
            
            if (count < 3) 
            {
                // Zwracamy tylko tyle punktów, ile faktycznie ma sens
                var res = new (double, double)[count];
                Array.Copy(eliminateLinear, res, count);
                return res;
            }

            var s = new Stack<(double, double)>();
            if (eliminateLinear.Length < 3) return eliminateLinear;
            s.Push(eliminateLinear[0]);
            s.Push(eliminateLinear[1]);
            s.Push(eliminateLinear[2]);
            for (int i = 3; i < count; i++)
            {
                while (s.Count > 1)
                {
                    var top = s.Pop();
                    var top2 = s.Peek();
                    if (Cross(top2, top, eliminateLinear[i]) <= 0)
                    {
                    }
                    else
                    {
                        s.Push(top);
                        break;
                    }
                }

                s.Push(eliminateLinear[i]);
            }

            var result = s.ToArray();
            Array.Reverse(result);
            return result;
        }

        // Etap 2
        // oblicza otoczkę dwóch wielokątów wypukłych
        private bool IsSmaller((double, double) p1, (double, double) p2)
        {
            if (Math.Abs(p1.Item1 - p2.Item1) > 1e-10)
                return p1.Item1 < p2.Item1;
            return p1.Item2 < p2.Item2;
        }
        public (double, double)[] ConvexHullOfTwo((double, double)[] poly1, (double, double)[] poly2)
        {
            void SplitPoly((double, double)[] poly, out List<(double, double)> lower, out List<(double, double)> upper)
            {
                int n = poly.Length;
                int minIdx = 0;
                int maxIdx = 0;

                for (int i = 1; i < n; i++)
                {
                    if(IsSmaller(poly[i], poly[minIdx])) minIdx = i;
                    if (IsSmaller(poly[maxIdx], poly[i])) maxIdx = i;
                }
                lower = new List<(double, double)>();
                int curr = minIdx;
                while (true)
                {
                    lower.Add(poly[curr]);
                    if (curr == maxIdx) break;
                    curr = (curr + 1) % n;
                }
                upper = new List<(double, double)>();
                curr = maxIdx;
                while (true)
                {
                    upper.Add(poly[curr]);
                    if (curr == minIdx) break;
                    curr = (curr + 1) % n;
                }
                upper.Reverse();
            }
            
            
            SplitPoly(poly1, out var lower1, out var upper1);
            SplitPoly(poly2, out var lower2, out var upper2);
            
            List<(double, double)> MergeLists(List<(double, double)> l1, List<(double, double)> l2)
            {
                var merged = new List<(double, double)>(l1.Count + l2.Count);
                int i = 0, j = 0;
                while (i < l1.Count && j < l2.Count)
                {
                    if (IsSmaller(l1[i], l2[j])) merged.Add(l1[i++]);
                    else merged.Add(l2[j++]);
                }
                while (i < l1.Count) merged.Add(l1[i++]);
                while (j < l2.Count) merged.Add(l2[j++]);
                return merged;
            }

            var mergedLower = MergeLists(lower1, lower2);
            var mergedUpper = MergeLists(upper1, upper2);
            
            List<(double, double)> BuildHalfHull(List<(double, double)> points, bool isLower)
            {
                var hull = new List<(double, double)>();
                foreach (var p in points)
                {
                    while (hull.Count >= 2)
                    {
                        var top = hull[hull.Count - 1];
                        var top2 = hull[hull.Count - 2];
                        int cross = Cross(top2, top, p);

                        if (isLower)
                        {
                            if (cross <= 0) hull.RemoveAt(hull.Count - 1);
                            else break;
                        }
                        else
                        {
                            if (cross >= 0) hull.RemoveAt(hull.Count - 1);
                            else break;
                        }
                    }
                    hull.Add(p);
                }
                return hull;
            }

            var finalLower = BuildHalfHull(mergedLower, true);
            var finalUpper = BuildHalfHull(mergedUpper, false);
            
            var result = new List<(double, double)>();
    
            // Dodajemy całą dolną otoczkę (zgodnie z ruchem CCW)
            result.AddRange(finalLower);
    
            // Dodajemy górną otoczkę od tyłu (by zamknąć cykl CCW).
            // Pomijamy ostatni element (Count - 1) i pierwszy (0), ponieważ pokrywają się 
            // one z początkiem i końcem otoczki dolnej.
            for (int i = finalUpper.Count - 2; i > 0; i--)
            {
                result.Add(finalUpper[i]);
            }

            return result.ToArray();
        }
    }
}