
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

class SweepLine
    {

    /// <summary>
    /// Struktura pomocnicza opisująca zdarzenie
    /// </summary>
    /// <remarks>
    /// Można jej użyć, przerobić, albo w ogóle nie używać i zrobić po swojemu
    /// </remarks>
    struct SweepEvent
        {
        /// <summary>
        /// Współrzędna zdarzenia
        /// </summary>
        public double Coord;

        /// <summary>
        /// Czy zdarzenie oznacza początek odcinka/prostokąta
        /// </summary>
        public bool IsStartingPoint;

        /// <summary>
        /// Indeks odcinka/prodtokąta w odpowiedniej tablicy
        /// </summary>
        public int Idx;

        public SweepEvent(double c, bool sp, int i=-1 ) { Coord=c; IsStartingPoint=sp; Idx=i; }
        }

    /// <summary>
    /// Funkcja obliczająca długość teoriomnogościowej sumy pionowych odcinków
    /// </summary>
    /// <returns>Długość teoriomnogościowej sumy pionowych odcinków</returns>
    /// <param name="segments">Tablica z odcinkami, których teoriomnogościowej sumy długość należy policzyć</param>
    /// Każdy odcinek opisany jest przez dwa punkty: początkowy i końcowy
    /// </param>
    public double VerticalSegmentsUnionLength(Geometry.Segment[] segments)
    {
        int n = segments.GetLength(0);
        var events = new List<SweepEvent>(n*2);
        for (int i = 0; i < n; i++)
        {
            double y1 = segments[i].ps.y;
            double y2 = segments[i].pe.y;

            double startY = Math.Min(y1, y2);
            double endY = Math.Max(y1, y2);

            events.Add(new SweepEvent(startY, true, i));
            events.Add(new SweepEvent(endY, false, i));
        }
        events = events.OrderBy(y => y.Coord).ToList();
        double totalLength = 0;
        int activeSegments = 0;
        double currentSeriesStart = 0;

        foreach (var ev in events)
        {
            if (activeSegments == 0)
            {
                currentSeriesStart = ev.Coord;
            }

            if (ev.IsStartingPoint)
            {
                activeSegments++;
            }
            else
            {
                activeSegments--;
            }

            if (activeSegments == 0)
            {
                totalLength +=(ev.Coord - currentSeriesStart);
                
            }
            
        }

        return totalLength;
    }

    /// <summary>
    /// Funkcja obliczająca pole teoriomnogościowej sumy prostokątów
    /// </summary>
    /// <returns>Pole teoriomnogościowej sumy prostokątów</returns>
    /// <param name="rectangles">Tablica z prostokątami, których teoriomnogościowej sumy pole należy policzyć</param>
    /// Każdy prostokąt opisany jest przez cztery wartości: minimalna współrzędna X, minimalna współrzędna Y, 
    /// maksymalna współrzędna X, maksymalna współrzędna Y.
    /// </param>
    public double RectanglesUnionArea(Geometry.Rectangle[] rectangles)
        {
            // Struktura pamiętająca indeksy prostokątów, które aktualnie przecina nasza linia
var activeRectIndices = new HashSet<int>();

double totalArea = 0;
double currentD = 0; // Ostatnio wyliczona suma pionowych odcinków
double lastX = events.Count > 0 ? events[0].Coord : 0; // Pozycja poprzedniego zdarzenia

// Główna pętla przeskakująca po krawędziach (zamiatanie)
for (int i = 0; i < events.Count; i++)
{
    var ev = events[i];

    // 1. OBLICZANIE POLA OD POPRZEDNIEGO ZDARZENIA
    double deltaX = ev.Coord - lastX;
    if (deltaX > 0)
    {
        // Pole "plastra" to przesunięcie X * pionowy przekrój D
        totalArea += deltaX * currentD;
        lastX = ev.Coord; // Aktualizujemy pozycję skanera
    }

    // 2. AKTUALIZACJA "LISTY OBECNOŚCI" PROSTOKĄTÓW
    if (ev.IsStartingPoint)
    {
        activeRectIndices.Add(ev.Idx); // Wchodzimy w prostokąt -> dodaj
    }
    else
    {
        activeRectIndices.Remove(ev.Idx); // Wychodzimy z prostokąta -> usuń
    }

    // 3. WYLICZENIE NOWEGO "D" (Wysokości pionowej)
    // Wykonujemy to TYLKO na końcu grupy zdarzeń o tym samym X
    if (i == events.Count - 1 || events[i + 1].Coord > ev.Coord)
    {
        // Zamieniamy aktywne prostokąty na listę pionowych odcinków Y
        var segmentsForEtap1 = new Geometry.Segment[activeRectIndices.Count];
        int k = 0;
        
        foreach (int idx in activeRectIndices)
        {
            // Bierzemy dół (minY) i górę (maxY) aktywnego prostokąta 
            // i tworzymy z niego odcinek dla Etapu 1
            segmentsForEtap1[k] = new Geometry.Segment 
            { 
                ps = new Geometry.Point { y = rectangles[idx].minY, x = ev.Coord },
                pe = new Geometry.Point { y = rectangles[idx].maxY, x = ev.Coord }
            };
            k++;
        }

        // Korzystamy z Etapu 1, by usunąć nakładające się części 
        // i uzyskać czystą łączną wysokość "D" na obecnej linii
        currentD = VerticalSegmentsUnionLength(segmentsForEtap1);
    }
}
        }

    }

}
