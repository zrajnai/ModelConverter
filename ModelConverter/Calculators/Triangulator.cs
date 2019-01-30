using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.Calculators
{
    internal static class Triangulator
    {
        public static int[][] Triangulate(IModel model, Face f)
        {
            var vertices = new List<int>(Enumerable.Range(0, f.VertexIndices.Length));
            var results = new List<int[]>();

            return Triangulate(model, f, vertices, results).ToArray();
        }

        private static List<int[]> Triangulate(IModel model, Face f, List<int> vertices, List<int[]> results, int startIndex = 0)
        {
            var i0 = new CircularIterator(vertices.Count, startIndex).Current;
            var i1 = new CircularIterator(vertices.Count, startIndex + 1).Current;
            var i2 = new CircularIterator(vertices.Count, startIndex + 2).Current;

            if (vertices.Count == 3)
            {
                results.Add(vertices.ToArray());
                return results;
            }
            
            if (!CanClip(model, f, vertices, i0, i1, i2))
            {
                return Triangulate(model, f, vertices, results, i1);
            }

            results.Add(new[] { vertices[i0], vertices[i1], vertices[i2] });
            Debug.Assert(vertices.Remove(vertices[i1]));

            Triangulate(model, f, vertices, results, i1);
            return results;
        }

        private static bool CanClip(IModel model, Face f, IList<int> vertices, int i0, int i1, int i2)
        {
            if (IsConcave(model, f, vertices[i0], vertices[i1], vertices[i2]))
                return false;

            var ip = new CircularIterator(vertices.Count, i2 + 1);
            do
            {
                if (!IsOutSideTriangle(model, f, vertices[i0], vertices[i1], vertices[i2], ip.Current))
                    return false;
                ip.MoveNext();
            } while (ip.Current != i0);
            return true;
        }

        private static bool IsOutSideTriangle(IModel model, Face f, int i0, int i1, int i2, int ip)
        {
            return IsOutSide(model, f, ip, i0, i1) ||
                   IsOutSide(model, f, ip, i2, i0) ||
                   IsOutSide(model, f, ip, i1, i2);
        }

        private static bool IsConcave(IModel model, Face f, int i0, int i1, int i2)
        {
            var v0 = (Vector)model.Vertices[f.VertexIndices[i0]];
            var v1 = (Vector)model.Vertices[f.VertexIndices[i1]];
            var v2 = (Vector)model.Vertices[f.VertexIndices[i2]];

            return (v2 - v1) % (v0 - v1) * f.Normal > 0;
        }

        private static bool IsOutSide(IModel model, Face f, int ip, int i0, int i1)
        {
            var v0 = (Vector)model.Vertices[f.VertexIndices[i0]];
            var v1 = (Vector)model.Vertices[f.VertexIndices[i1]];
            var vp = (Vector)model.Vertices[f.VertexIndices[ip]];

            var v10 = v0 - v1;
            var v1p = vp - v1;

            return (v1 % v10) * v1p < 0;
        }
    }

    internal class CircularIterator : IEnumerator<int>
    {
        public CircularIterator(int count, int startIndex = 0)
        {
            Current = startIndex % count;
            Count = count;
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            Current = (Current + 1 + Count) % Count;
            return true;
        }

        public void Reset()
        {
            Current = 0;
        }

        public int Current { get; private set; }

        public int Count { get; }

        object IEnumerator.Current => Current;
    }
}