using System.Collections.Generic;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{

    public class GiftWrappingAlgorithm : IHullAlgorithm
    {
        public string Name => "Gift Wrapping";

        public string[] PseudocodeLines => new string[]
        {
            "<color=red>Gift Wrapping:</color>",                          //0
            "p0 = leftmost point,  p1 = find second point",               //1
            "p2 = pivot(p0, p1)  //find third hull vertex",              //2
            "add triangle(p0, p1, p2)",                                   //3
            "queue = all 3 edges of first triangle",                      //4
            "<color=red>while</color> queue not empty:",                  //5
            "    edge = dequeue next edge",                               //6
            "    <color=red>if</color> edge already processed: skip",     //7
            "    best = pivot(edge.a, edge.b)",                           //8
            "    <color=red>if</color> pivot point found:",               //9
            "        add triangle(edge.a, edge.b, best)",                 //10
            "<color=red>pivot(a, b):</color>",                            //11
            "    best = first non-edge point",                            //12
            "    <color=red>for each</color> remaining point p:",         //13
            "        <color=red>if</color> p is more outward than best:", //14
            "            best = p",                                       //15
            "    <color=red>return</color> best",                         //16
            "",                                                           //17
            "<color=red>Hull complete</color>",                           //18
        };

        private struct UndirectedEdge
        {
            public int a, b;
            public UndirectedEdge(int a, int b) { this.a = Mathf.Min(a, b); this.b = Mathf.Max(a, b); }

            public override bool Equals(object obj)
            {
                return obj is UndirectedEdge other && a == other.a && b == other.b;
            }
        }

        public void Run(ConvexHull3D ctx)
        {
            ctx.ResetAllPointColors();
            List<Vector3> pts = ctx.Points;

            int p0 = 0;
            ctx.SetPointColor(p0, ctx.CurrentPointColor);

            for(int i = 1; i < pts.Count; i++)
            {
                if(pts[i].x < pts[p0].x)
                {
                    ctx.SetPointColor(p0, ctx.NormalPointColor);
                    p0 = i;
                    ctx.SetPointColor(p0, ctx.CurrentPointColor);
                }
                else
                {
                    ctx.SetPointColor(i, ctx.NormalPointColor);
                }
            }

            ctx.CaptureStep(0);


            int p1 = FindSecondPoint(pts, p0);

            ctx.SetPointColor(p0, ctx.HullPointColor);
            ctx.SetPointColor(p1, ctx.HullPointColor);
            ctx.AddHighlightLine(pts[p0], pts[p1], ctx.ActiveEdgeColor);

            ctx.CaptureStep(1);


            HashSet<int> hullPoints = new HashSet<int> { p0, p1 };

            ctx.ClearHighlightLines();
            ctx.AddHighlightLine(pts[p0], pts[p1], ctx.ActiveEdgeColor);

            int p2 = PivotOnEdgeAnimated(ctx, pts, hullPoints, p0, p1);
            ctx.SetPointColor(p2, ctx.HullPointColor);
            hullPoints.Add(p2);
            ctx.ClearHighlightLines();

            ctx.CaptureStep(2);


            var initialFace = new List<int> { p0, p1, p2 };
            ctx.AddHullFace(initialFace);
            ctx.DrawHullFace(initialFace);
            ctx.HighlightHullFaces(new List<List<int>> { initialFace }, ctx.NewFaceColor);

            ctx.CaptureStep(3);


            Queue<HullUtils.Edge> queue = new Queue<HullUtils.Edge>();
            queue.Enqueue(new HullUtils.Edge(p1, p0));
            queue.Enqueue(new HullUtils.Edge(p2, p1));
            queue.Enqueue(new HullUtils.Edge(p0, p2));

            ctx.ResetHullFaceColors();

            Dictionary<UndirectedEdge, int> edgeFaceCount = new Dictionary<UndirectedEdge, int>();
            RegisterFace(edgeFaceCount, p0, p1, p2);

            ctx.CaptureStep(4);


            while(queue.Count > 0)
            {
                ctx.ClearHighlightLines();
                ctx.ResetHullFaceColors();

                ctx.CaptureStep(5);


                HullUtils.Edge e = queue.Dequeue();

                ctx.AddHighlightLine(pts[e.a], pts[e.b], ctx.ActiveEdgeColor);
                ctx.UpdateSearchLine(pts[e.a], pts[e.b]);

                ctx.CaptureStep(6);


                if(IsEdgeResolved(edgeFaceCount, e.a, e.b))
                {
                    ctx.ClearHighlightLines();
                    ctx.CaptureStep(7);
                    continue;
                }


                ctx.CaptureStep(8);

                int pivotResult = PivotOnEdgeAnimated(ctx, pts, hullPoints, e.a, e.b);
                if(pivotResult == -1)
                {
                    ctx.ClearHighlightLines();
                    ctx.CaptureStep(9);
                    continue;
                }

                ctx.SetPointColor(pivotResult, ctx.CurrentPointColor);
                ctx.ClearHighlightLines();

                ctx.AddHighlightLine(pts[e.a], pts[e.b],         ctx.ActiveEdgeColor);
                ctx.AddHighlightLine(pts[e.a], pts[pivotResult], ctx.ActiveEdgeColor);
                ctx.AddHighlightLine(pts[e.b], pts[pivotResult], ctx.ActiveEdgeColor);

                ctx.UpdateSearchLine(pts[e.a], pts[pivotResult]);

                ctx.CaptureStep(9);

                ctx.ClearHighlightLines();
                var face = new List<int> { e.a, e.b, pivotResult };

                ctx.AddHullFace(face);
                ctx.DrawHullFace(face);
                ctx.SetPointColor(pivotResult, ctx.HullPointColor);
                hullPoints.Add(pivotResult);
                
                ctx.HighlightHullFaces(new List<List<int>> { face }, ctx.NewFaceColor);

                ctx.CaptureStep(10);

                RegisterFace(edgeFaceCount, e.a, e.b, pivotResult);

                HullUtils.Edge e1 = new HullUtils.Edge(pivotResult, e.b);
                HullUtils.Edge e2 = new HullUtils.Edge(e.a, pivotResult);

                if(!IsEdgeResolved(edgeFaceCount, e1.a, e1.b))
                {
                    queue.Enqueue(e1);
                }
                if(!IsEdgeResolved(edgeFaceCount, e2.a, e2.b))
                {
                    queue.Enqueue(e2);
                }
            }

            ctx.ClearSearchLine();
            ctx.ClearHighlightLines();
            ctx.ResetHullFaceColors();

            ctx.CaptureStep(18);
        }

        void RegisterFace(Dictionary<UndirectedEdge, int> edgeFaceCount, int a, int b, int c)
        {
            foreach(var (x, y) in new[] { (a, b), (b, c), (c, a) })
            {
                var key = new UndirectedEdge(x, y);
                edgeFaceCount.TryGetValue(key, out int count);
                edgeFaceCount[key] = count + 1;
            }
        }

        bool IsEdgeResolved(Dictionary<UndirectedEdge, int> edgeFaceCount, int a, int b)
        {
            return edgeFaceCount.TryGetValue(new UndirectedEdge(a, b), out int count) && count >= 2;
        }

        int FindSecondPoint(List<Vector3> pts, int p0)
        {
            int p1 = -1;
            for(int i = 0; i < pts.Count; i++)
            {
                if(i == p0) continue;
                if(p1 == -1) { p1 = i; continue; }

                Vector3 edge = pts[i] - pts[p0];
                Vector3 normal = Vector3.Cross(edge, Vector3.up).normalized;
                if(normal.magnitude < HullUtils.Epsilon)
                {
                    normal = Vector3.Cross(edge, Vector3.right).normalized;
                }

                bool allOnSameSide = true;
                float sideSign = 0;
                for(int j = 0; j < pts.Count; j++)
                {
                    if(j == p0 || j == i) continue;

                    float side = Vector3.Dot(pts[j] - pts[p0], normal);
                    if(Mathf.Abs(side) > HullUtils.Epsilon)
                    {
                        if(sideSign == 0) sideSign = Mathf.Sign(side);
                        else if(Mathf.Sign(side) != sideSign) { allOnSameSide = false; break; }
                    }
                }

                if(allOnSameSide) 
                {
                    p1 = i;
                }
            }
            return p1;
        }

        int PivotOnEdgeAnimated(ConvexHull3D ctx, List<Vector3> pts, HashSet<int> hullPoints, int a, int b)
        {
            int pivot = -1;

            ctx.CaptureStep(11);

            for(int i = 0; i < pts.Count; i++)
            {
                if(i == a || i == b) continue;

                if(pivot == -1)
                {
                    pivot = i;
                    if(!hullPoints.Contains(pivot)) 
                    {
                        ctx.SetPointColor(pivot, ctx.CurrentPointColor);
                    }
                    ctx.UpdateSearchLine(pts[a], pts[pivot]);

                    ctx.CaptureStep(12);
                    continue;
                }

                Vector3 edge = pts[b] - pts[a];
                Vector3 toPivot = pts[pivot] - pts[a];
                Vector3 toPoint = pts[i] - pts[a];
                Vector3 normalPivot = Vector3.Cross(edge, toPivot).normalized;

                float side = Vector3.Dot(toPoint, normalPivot);

                bool isNewBest = side > HullUtils.Epsilon;

                if(!isNewBest)
                {
                    if(!hullPoints.Contains(i)) ctx.SetPointColor(i, ctx.NormalPointColor);

                    if(Mathf.Abs(side) < HullUtils.Epsilon)
                    {
                        float anglePivot = Vector3.Angle(toPivot, edge);
                        float anglePoint = Vector3.Angle(toPoint, edge);

                        isNewBest = anglePoint < anglePivot;
                    }
                }

                if(isNewBest)
                {
                    if(!hullPoints.Contains(pivot)) 
                    {
                        ctx.SetPointColor(pivot, ctx.NormalPointColor);
                    }
                    pivot = i;
                    
                    if(!hullPoints.Contains(pivot)) 
                    {
                        ctx.SetPointColor(pivot, ctx.CurrentPointColor);
                    }

                    ctx.UpdateSearchLine(pts[a], pts[pivot]);

                    ctx.CaptureStep(15);
                }
            }
            ctx.CaptureStep(16);

            return pivot;
        }
    }
}