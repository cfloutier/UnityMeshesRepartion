using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PlatoSolid
{
    public enum Type
    {
        Tetrahedron,
        Cube,
        Octahedron,
        Dodecahedron,
        Icosahedron
    }

    public Type type;


    [HideInInspector]
    public List<Vector3> points = new List<Vector3>();
    public class Edge
    {
        public Edge(int start, int end)
        {
            this.start = start; this.end = end;
        }

        public int start;
        public int end;
    }
    public class Face : List<int>
    {
        public Face(int[] pts)
        {
            AddRange(pts);
        }

        public List<int> Triangulate()
        {
            return null;
        }


        public Vector3 center;
        public Vector3 normal;

    }

    public List<Edge> edges = new List<Edge>();
    public List<Face> faces = new List<Face>();

    public void ComputeFaces()
    {
        foreach (var f in faces)
        {
            Vector3 center = Vector3.zero;
            foreach (var i in f)
            {
                var p = points[i];
                center += p;
            }



            f.center = center / f.Count;

            Vector3 a = points[f[1]] - points[f[0]];
            Vector3 b = points[f[2]] - points[f[1]];

            Vector3 norm = Vector3.Cross(a, b).normalized;

            f.normal = norm;
        }


    }

    public void Build()
    {
        switch (type)
        {
            case Type.Tetrahedron: Tetrahedron(); break;
            case Type.Cube: Cube(); break;
            case Type.Octahedron: Octahedron(); break;
            case Type.Dodecahedron: Dodecahedron(); break;
            case Type.Icosahedron: Icosahedron(); break;
            default:
                break;
        }
    }

    void Tetrahedron()
    {
        points = new List<Vector3>();

        points.Add(0.5f * new Vector3(Mathf.Sqrt(8f / 9f), -1 / 3f, 0));
        points.Add(0.5f * new Vector3(-Mathf.Sqrt(2f / 9f), -1 / 3f, Mathf.Sqrt(2f / 3f)));
        points.Add(0.5f * new Vector3(-Mathf.Sqrt(2f / 9f), -1 / 3f, -Mathf.Sqrt(2f / 3f)));
        points.Add(0.5f * new Vector3(0, 1f, 0));

        edges = new List<Edge>();
        edges.Add(new Edge(0, 1));
        edges.Add(new Edge(0, 2));
        edges.Add(new Edge(0, 3));
        edges.Add(new Edge(1, 2));
        edges.Add(new Edge(1, 3));
        edges.Add(new Edge(2, 3));

        faces = new List<Face>();

        faces.Add(new Face(new int[] { 0, 1, 2 }));
        faces.Add(new Face(new int[] { 1, 3, 2 }));
        faces.Add(new Face(new int[] { 0, 2, 3 }));
        faces.Add(new Face(new int[] { 0, 3, 1 }));

        ComputeFaces();
    }

    void Cube()
    {
        points = new List<Vector3>();

        points.Add(new Vector3(0.5f, 0.5f, 0.5f));
        points.Add(new Vector3(0.5f, 0.5f, -0.5f));
        points.Add(new Vector3(-0.5f, 0.5f, -0.5f));
        points.Add(new Vector3(-0.5f, 0.5f, 0.5f));

        points.Add(new Vector3(0.5f, -0.5f, 0.5f));
        points.Add(new Vector3(0.5f, -0.5f, -0.5f));
        points.Add(new Vector3(-0.5f, -0.5f, -0.5f));
        points.Add(new Vector3(-0.5f, -0.5f, 0.5f));

        edges = new List<Edge>();
        edges.Add(new Edge(0, 1));
        edges.Add(new Edge(1, 2));
        edges.Add(new Edge(2, 3));
        edges.Add(new Edge(3, 0));


        edges.Add(new Edge(4, 5));
        edges.Add(new Edge(5, 6));
        edges.Add(new Edge(6, 7));
        edges.Add(new Edge(7, 4));

        for (int i = 0; i < 4; i++)
            edges.Add(new Edge(i, i + 4));

        faces.Add(new Face(new int[] { 0, 1, 2, 3 }));
        faces.Add(new Face(new int[] { 7, 6, 5, 4 }));
        faces.Add(new Face(new int[] { 2, 1, 5, 6 }));
        faces.Add(new Face(new int[] { 3, 2, 6, 7 }));
        faces.Add(new Face(new int[] { 0, 3, 7, 4 }));
        faces.Add(new Face(new int[] { 1, 0, 4, 5 }));


        ComputeFaces();
    }

    void Octahedron()
    {
        float y = 1f / Mathf.Sqrt(2f);

        points = new List<Vector3>();

        points.Add(new Vector3(0, y, 0));
        points.Add(new Vector3(y, 0, 0));
        points.Add(new Vector3(0, 0, -y));
        points.Add(new Vector3(-y, 0, 0));

        points.Add(new Vector3(0, 0, y));
        points.Add(new Vector3(0, -y, 0));

        edges = new List<Edge>();
        edges.Add(new Edge(0, 1));
        edges.Add(new Edge(0, 2));
        edges.Add(new Edge(0, 3));
        edges.Add(new Edge(0, 4));

        edges.Add(new Edge(5, 1));
        edges.Add(new Edge(5, 2));
        edges.Add(new Edge(5, 3));
        edges.Add(new Edge(5, 4));

        edges.Add(new Edge(1, 2));
        edges.Add(new Edge(2, 3));
        edges.Add(new Edge(3, 4));
        edges.Add(new Edge(4, 1));

        // 8 faces
        faces.Add(new Face(new int[] { 0, 1, 2 }));
        faces.Add(new Face(new int[] { 0, 2, 3 }));
        faces.Add(new Face(new int[] { 0, 3, 4 }));
        faces.Add(new Face(new int[] { 0, 4, 1 }));

        faces.Add(new Face(new int[] { 5, 2, 1 }));
        faces.Add(new Face(new int[] { 5, 3, 2 }));
        faces.Add(new Face(new int[] { 5, 4, 3 }));
        faces.Add(new Face(new int[] { 5, 1, 4 }));

        ComputeFaces();
    }

    void Dodecahedron()
    {
        // Value t1 is actually never used.
        float s = 1;
        //double t1 = 2.0 * Math.PI / 5.0;
        float t2 = Mathf.PI / 10f;
        float t3 = 3f * Mathf.PI / 10f;
        float t4 = Mathf.PI / 5f;
        float d1 = s / 2f / Mathf.Sin(t4);
        float d2 = d1 * Mathf.Cos(t4);
        float d3 = d1 * Mathf.Cos(t2);
        float d4 = d1 * Mathf.Sin(t2);
        float Fx =
            (s * s - (2f * d3) * (2f * d3) -
                (d1 * d1 - d3 * d3 - d4 * d4)) /
                    (2f * (d4 - d1));
        float d5 = Mathf.Sqrt(0.5f *
            (s * s + (2f * d3) * (2f * d3) -
                (d1 - Fx) * (d1 - Fx) -
                    (d4 - Fx) * (d4 - Fx) - d3 * d3));
        float Fy = (Fx * Fx - d1 * d1 - d5 * d5) / (2f * d5);
        float Ay = d5 + Fy;

        Vector3 A = new Vector3(d1, Ay, 0);
        Vector3 B = new Vector3(d4, Ay, d3);
        Vector3 C = new Vector3(-d2, Ay, s / 2);
        Vector3 D = new Vector3(-d2, Ay, -s / 2);
        Vector3 E = new Vector3(d4, Ay, -d3);
        Vector3 F = new Vector3(Fx, Fy, 0);
        Vector3 G = new Vector3(Fx * Mathf.Sin(t2), Fy,
            Fx * Mathf.Cos(t2));
        Vector3 H = new Vector3(-Fx * Mathf.Sin(t3), Fy,
            Fx * Mathf.Cos(t3));
        Vector3 I = new Vector3(-Fx * Mathf.Sin(t3), Fy,
            -Fx * Mathf.Cos(t3));
        Vector3 J = new Vector3(Fx * Mathf.Sin(t2), Fy,
            -Fx * Mathf.Cos(t2));
        Vector3 K = new Vector3(Fx * Mathf.Sin(t3), -Fy,
            Fx * Mathf.Cos(t3));
        Vector3 L = new Vector3(-Fx * Mathf.Sin(t2), -Fy,
            Fx * Mathf.Cos(t2));
        Vector3 M = new Vector3(-Fx, -Fy, 0);
        Vector3 N = new Vector3(-Fx * Mathf.Sin(t2), -Fy,
            -Fx * Mathf.Cos(t2));
        Vector3 O = new Vector3(Fx * Mathf.Sin(t3), -Fy,
            -Fx * Mathf.Cos(t3));
        Vector3 P = new Vector3(d2, -Ay, s / 2);
        Vector3 Q = new Vector3(-d4, -Ay, d3);
        Vector3 R = new Vector3(-d1, -Ay, 0);
        Vector3 S = new Vector3(-d4, -Ay, -d3);
        Vector3 T = new Vector3(d2, -Ay, -s / 2);

        //20 points
        points.Add(A);
        points.Add(B);
        points.Add(C);
        points.Add(D);
        points.Add(E);
        points.Add(F);
        points.Add(G);
        points.Add(H);
        points.Add(I);
        points.Add(J);
        points.Add(K);
        points.Add(L);
        points.Add(M);
        points.Add(N);
        points.Add(O);
        points.Add(P);
        points.Add(Q);
        points.Add(R);
        points.Add(S);
        points.Add(T);

        //36 edges
        edges.Add(new Edge(0, 1));
        edges.Add(new Edge(1, 2));
        edges.Add(new Edge(2, 3));
        edges.Add(new Edge(3, 4));
        edges.Add(new Edge(4, 0));
        edges.Add(new Edge(1, 6));
        edges.Add(new Edge(6, 10));
        edges.Add(new Edge(10, 5));
        edges.Add(new Edge(5, 0));
        edges.Add(new Edge(2, 7));
        edges.Add(new Edge(7, 11));
        edges.Add(new Edge(11, 6));
        edges.Add(new Edge(3, 8));
        edges.Add(new Edge(8, 12));
        edges.Add(new Edge(12, 7));
        edges.Add(new Edge(4, 9));
        edges.Add(new Edge(9, 13));
        edges.Add(new Edge(13, 8));
        edges.Add(new Edge(4, 9));
        edges.Add(new Edge(9, 13));
        edges.Add(new Edge(13, 8));
        edges.Add(new Edge(5, 14));
        edges.Add(new Edge(14, 9));
        edges.Add(new Edge(14, 19));
        edges.Add(new Edge(19, 18));
        edges.Add(new Edge(18, 13));
        edges.Add(new Edge(14, 19));
        edges.Add(new Edge(19, 18));
        edges.Add(new Edge(18, 13));
        edges.Add(new Edge(18, 17));
        edges.Add(new Edge(17, 12));
        edges.Add(new Edge(17, 16));
        edges.Add(new Edge(16, 11));
        edges.Add(new Edge(16, 15));
        edges.Add(new Edge(15, 10));
        edges.Add(new Edge(15, 19));


        //12 faces

        faces.Add(new Face(new int[] { 4, 3, 2, 1, 0 }));

        faces.Add(new Face(new int[] { 0, 5, 14, 9, 4 }));
        faces.Add(new Face(new int[] { 4, 9, 13, 8, 3 }));
        faces.Add(new Face(new int[] { 3, 8, 12, 7, 2 }));
        faces.Add(new Face(new int[] { 2, 7, 11, 6, 1 }));
        faces.Add(new Face(new int[] { 1, 6, 10, 5, 0 }));

        faces.Add(new Face(new int[] { 5, 10, 15, 19, 14 }));
        faces.Add(new Face(new int[] { 9, 14, 19, 18, 13 }));
        faces.Add(new Face(new int[] { 13, 18, 17, 12, 8 }));
        faces.Add(new Face(new int[] { 12, 17, 16, 11, 7 }));
        faces.Add(new Face(new int[] { 11, 16, 15, 10, 6 }));

        faces.Add(new Face(new int[] { 15, 16, 17, 18, 19 }));


        ComputeFaces();

    }

    void Icosahedron()
    {
        // t1 and t3 are actually not used in calculations.
        float S = 1;
        //double t1 = 2.0 * Math.PI / 5;
        float t2 = Mathf.PI / 10f;
        float t4 = Mathf.PI / 5f;
        //double t3 = -3.0 * Math.PI / 10.0;
        float R = (S / 2f) / Mathf.Sin(t4);
        float H = Mathf.Cos(t4) * R;
        float Cx = R * Mathf.Sin(t2);
        float Cz = R * Mathf.Cos(t2);
        float H1 = Mathf.Sqrt(S * S - R * R);
        float H2 = Mathf.Sqrt((H + R) * (H + R) - H * H);
        float Y2 = (H2 - H1) / 2f;
        float Y1 = Y2 + H1;


        points.Add(new Vector3(0, Y1, 0));
        points.Add(new Vector3(R, Y2, 0));
        points.Add(new Vector3(Cx, Y2, Cz));
        points.Add(new Vector3(-H, Y2, S / 2));
        points.Add(new Vector3(-H, Y2, -S / 2));
        points.Add(new Vector3(Cx, Y2, -Cz));
        points.Add(new Vector3(-R, -Y2, 0));
        points.Add(new Vector3(-Cx, -Y2, -Cz));
        points.Add(new Vector3(H, -Y2, -S / 2));
        points.Add(new Vector3(H, -Y2, S / 2));
        points.Add(new Vector3(-Cx, -Y2, Cz));
        points.Add(new Vector3(0, -Y1, 0));

        edges = new List<Edge>();

        edges.Add(new Edge(0, 1));
        edges.Add(new Edge(0, 2));
        edges.Add(new Edge(0, 3));
        edges.Add(new Edge(0, 4));
        edges.Add(new Edge(0, 5));

        edges.Add(new Edge(1, 2));
        edges.Add(new Edge(2, 3));
        edges.Add(new Edge(3, 4));
        edges.Add(new Edge(4, 5));
        edges.Add(new Edge(5, 1));

        edges.Add(new Edge(1, 9));
        edges.Add(new Edge(9, 2));
        edges.Add(new Edge(2, 10));
        edges.Add(new Edge(10, 3));
        edges.Add(new Edge(3, 6));
        edges.Add(new Edge(6, 4));
        edges.Add(new Edge(4, 7));
        edges.Add(new Edge(7, 5));
        edges.Add(new Edge(5, 8));
        edges.Add(new Edge(8, 1));

        edges.Add(new Edge(6, 7));
        edges.Add(new Edge(7, 8));
        edges.Add(new Edge(8, 9));
        edges.Add(new Edge(9, 10));
        edges.Add(new Edge(10, 6));

        edges.Add(new Edge(11, 6));
        edges.Add(new Edge(11, 7));
        edges.Add(new Edge(11, 8));
        edges.Add(new Edge(11, 9));
        edges.Add(new Edge(11, 10));


        faces.Add(new Face(new int[] { 0, 2, 1 }));
        faces.Add(new Face(new int[] { 0, 3, 2 }));
        faces.Add(new Face(new int[] { 0, 4, 3 }));
        faces.Add(new Face(new int[] { 0, 5, 4 }));
        faces.Add(new Face(new int[] { 0, 1, 5 }));

        faces.Add(new Face(new int[] { 1, 2, 9 }));
        faces.Add(new Face(new int[] { 2, 3, 10 }));
        faces.Add(new Face(new int[] { 3, 4, 6 }));
        faces.Add(new Face(new int[] { 4, 5, 7 }));
        faces.Add(new Face(new int[] { 5, 1, 8 }));

        faces.Add(new Face(new int[] { 6, 4, 7 }));
        faces.Add(new Face(new int[] { 7, 5, 8 }));
        faces.Add(new Face(new int[] { 8, 1, 9 }));
        faces.Add(new Face(new int[] { 9, 2, 10 }));
        faces.Add(new Face(new int[] { 10, 3, 6 }));

        faces.Add(new Face(new int[] { 11, 6, 7 }));
        faces.Add(new Face(new int[] { 11, 7, 8 }));
        faces.Add(new Face(new int[] { 11, 8, 9 }));
        faces.Add(new Face(new int[] { 11, 9, 10 }));
        faces.Add(new Face(new int[] { 11, 10, 6 }));

        ComputeFaces();
    }
}
