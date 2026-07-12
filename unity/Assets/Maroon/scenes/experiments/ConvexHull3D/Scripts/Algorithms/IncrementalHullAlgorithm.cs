using System.Collections.Generic;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class IncrementalHullAlgorithm : IHullAlgorithm
    {
        public string Name => "Incremental";

        public string[] PseudocodeLines => new string[]
        {
            "<color=red>Incremental Hull:</color>",                  //0
            "hull = tetrahedron(4 non-coplanar points)",             //1
            "<color=red>for each</color> remaining point p:",        //2
            "    visible = faces that p can see",                    //3
            "    <color=red>if</color> no visible faces: skip p",    //4
            "    remove all visible faces",                          //5
            "    horizon = boundary edges of the hole",              //6
            "    <color=red>for each</color> edge in horizon:",      //7
            "        add new triangle(edge, p)",                     //8
        };

        public void Run(ConvexHull3D ctx)
        {
            ctx.ResetAllPointColors();

            List<Vector3>        pts   = ctx.Points;
            List<HullUtils.Face> faces = new List<HullUtils.Face>();


            int[] initial = HullUtils.FindInitialTetrahedron(pts);

            int i0 = initial[0];
            int i1 = initial[1];
            int i2 = initial[2];
            int i3 = initial[3];

            ctx.SetPointColor(i0, ctx.HullPointColor);
            ctx.SetPointColor(i1, ctx.HullPointColor);
            ctx.SetPointColor(i2, ctx.HullPointColor);
            ctx.SetPointColor(i3, ctx.HullPointColor);

            ctx.CaptureStep(0);


            faces = HullUtils.BuildTetrahedron(i0, i1, i2, i3, pts);
            foreach(var face in faces)
            {
                ctx.AddHullFace(face.Verts);
                ctx.DrawHullFace(face.Verts);
            }
            ctx.CaptureStep(1);


            HashSet<int> hullPoints = new HashSet<int> { i0, i1, i2, i3 };

            for(int i = 0; i < pts.Count; i++)
            {
                if(hullPoints.Contains(i)) continue;

                ctx.ClearHighlightLines();
                ctx.ResetHullFaceColors();
                ctx.ClearSearchLine();
                ctx.SetPointColor(i, ctx.CurrentPointColor);

                ctx.CaptureStep(2);


                var visible = HullUtils.FindVisibleFaces(faces, pts[i], pts);
                ctx.CaptureStep(3);


                if(visible.Count == 0)
                {
                    ctx.SetPointColor(i, ctx.NormalPointColor);
                    ctx.CaptureStep(4);
                    continue;
                }


                var visibleVerts = new List<List<int>>();
                foreach(var vf in visible)
                {
                    visibleVerts.Add(vf.Verts);
                }
                ctx.HighlightHullFaces(visibleVerts, ctx.VisibleFaceColor);

                Vector3 fc = visible[0].Center(pts);
                ctx.UpdateSearchLine(pts[i], fc);

                ctx.CaptureStep(5);


                var horizon = HullUtils.FindHorizonEdges(faces, visible);
                foreach(var e in horizon)
                {
                    ctx.AddHighlightLine(pts[e.a], pts[e.b], ctx.HorizonEdgeColor);
                }
                ctx.CaptureStep(6);


                foreach(var vf in visible)
                {
                    ctx.RemoveHullFace(vf.Verts);
                    faces.Remove(vf);
                }
                ctx.ResetHullFaceColors();



                ctx.CaptureStep(7);

                var newFaces = HullUtils.CreateFacesFromHorizon(horizon, i, faces, pts);
                var newFaceVerts = new List<List<int>>();

                foreach(var nf in newFaces)
                {
                    faces.Add(nf);
                    ctx.AddHullFace(nf.Verts);
                    ctx.DrawHullFace(nf.Verts);
                    newFaceVerts.Add(nf.Verts);
                }

                ctx.HighlightHullFaces(newFaceVerts, ctx.NewFaceColor);
                ctx.SetPointColor(i, ctx.HullPointColor);
                hullPoints.Add(i);

                ctx.ClearHighlightLines();
                ctx.ClearSearchLine();

                ctx.CaptureStep(8);

                ctx.ResetHullFaceColors();
            }


            ctx.ClearSearchLine();
            ctx.ClearHighlightLines();
            ctx.ResetHullFaceColors();

            ctx.CaptureStep(9);
        }
    }
}