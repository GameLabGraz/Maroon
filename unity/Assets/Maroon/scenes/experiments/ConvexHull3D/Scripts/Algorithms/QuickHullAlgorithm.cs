using System.Collections.Generic;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class QuickHullAlgorithm : IHullAlgorithm
    {
        public string Name => "Quick Hull";

        public string[] PseudocodeLines => new string[]
        {
            "<color=red>QuickHull:</color>",                            //0
            "hull = tetrahedron(4 non-coplanar points)",                //1
            "assign each point to its nearest face",                    //2
            "<color=red>while</color> any face has assigned points:",   //3
            "    F = pick a face with assigned points",                 //4
            "    eye = furthest assigned point from F",                 //5
            "    visible = faces that eye can see",                     //6
            "    horizon = boundary edges of visible faces",            //7
            "    remove all visible faces",                             //8
            "    <color=red>for each</color> edge in horizon:",         //9
            "        add new triangle(edge, eye)",                      //10
            "    reassign points from removed faces",                   //11
            "",                                                         //12
            "<color=red>Hull complete</color>",                         //13
        };

        public void Run(ConvexHull3D ctx)
        {
            ctx.ResetAllPointColors();

            List<Vector3> pts = ctx.Points;
            List<HullUtils.Face> faces = new List<HullUtils.Face>();


            int[] initial = HullUtils.FindInitialTetrahedron(pts);
            if(initial == null) return;

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

                if(AssignPointToFace(i, faces, pts))
                {
                    ctx.SetPointColor(i, ctx.AssignedPointColor);
                }
            }
            ctx.CaptureStep(2);


            while(TryGetFaceWithPoints(faces, out HullUtils.Face currentFace))
            {
                ctx.ClearHighlightLines();
                ctx.ResetHullFaceColors();
                ctx.CaptureStep(3);


                ctx.HighlightHullFaces(new List<List<int>> { currentFace.Verts }, ctx.VisibleFaceColor);
                foreach(int pi in currentFace.AssignedPoints)
                {
                    ctx.SetPointColor(pi, ctx.AssignedPointColor);
                }

                ctx.CaptureStep(4);


                int eyeIdx = -1;
                float maxDist = -1;

                foreach(int pi in currentFace.AssignedPoints)
                {
                    float d = currentFace.Distance(pts[pi], pts);
                    if(d > maxDist)
                    {
                        maxDist = d;
                        eyeIdx = pi;
                    }
                }
                ctx.SetPointColor(eyeIdx, ctx.CurrentPointColor);


                Vector3 faceCenter = currentFace.Center(pts);
                ctx.UpdateSearchLine(faceCenter, pts[eyeIdx]);

                ctx.CaptureStep(5);


                ctx.ResetHullFaceColors();
                var visible = HullUtils.FindVisibleFaces(faces, pts[eyeIdx], pts);
                var visibleVerts = new List<List<int>>();

                foreach(var vf in visible)
                {
                    visibleVerts.Add(vf.Verts);
                }
                ctx.HighlightHullFaces(visibleVerts, ctx.VisibleFaceColor);

                ctx.CaptureStep(6);


                var horizon = HullUtils.FindHorizonEdges(faces, visible);
                foreach(var e in horizon)
                {
                    ctx.AddHighlightLine(pts[e.a], pts[e.b], ctx.HorizonEdgeColor);
                }
                ctx.CaptureStep(7);


                var remainingPoints = new List<int>();
                foreach(var vf in visible)
                {
                    foreach(int pi in vf.AssignedPoints)
                    {
                        if(pi != eyeIdx) 
                        {
                            remainingPoints.Add(pi);
                        }
                    }
                }



                foreach(var vf in visible)
                {
                    ctx.RemoveHullFace(vf.Verts);
                    faces.Remove(vf);
                }
                ctx.ResetHullFaceColors();
                ctx.CaptureStep(8);


                ctx.CaptureStep(9);

                var newFaces = HullUtils.CreateFacesFromHorizon(horizon, eyeIdx, faces, pts);

                foreach(var nf in newFaces)
                {
                    faces.Add(nf);
                    ctx.AddHullFace(nf.Verts);
                    ctx.DrawHullFace(nf.Verts);
                }

                ctx.SetPointColor(eyeIdx, ctx.HullPointColor);
                hullPoints.Add(eyeIdx);

                ctx.ClearHighlightLines();
                ctx.ClearSearchLine();

                ctx.CaptureStep(10);


                foreach(int pi in remainingPoints)
                {
                    if(AssignPointToFace(pi, newFaces, pts))
                    {
                        ctx.SetPointColor(pi, ctx.AssignedPointColor);
                    }
                    else
                    {
                        ctx.SetPointColor(pi, ctx.NormalPointColor);
                    }
                }
                ctx.CaptureStep(11);
            }


            ctx.ClearSearchLine();
            ctx.ClearHighlightLines();
            ctx.ResetHullFaceColors();

            for(int i = 0; i < pts.Count; i++)
            {
                if(!hullPoints.Contains(i))
                {
                    ctx.SetPointColor(i, ctx.NormalPointColor);
                }
            }

            ctx.CaptureStep(13);
        }

        bool TryGetFaceWithPoints(List<HullUtils.Face> faces, out HullUtils.Face result)
        {
            foreach(var face in faces)
            {
                if(face.AssignedPoints.Count > 0) 
                { 
                    result = face;
                    return true;
                }
            }
            result = null;
            return false;
        }

        bool AssignPointToFace(int pointIdx, List<HullUtils.Face> faces, List<Vector3> pts)
        {
            HullUtils.Face bestFace = null;
            float bestDist = HullUtils.Epsilon;

            foreach(var f in faces)
            {
                float d = f.Distance(pts[pointIdx], pts);
                if(d > bestDist) 
                { 
                    bestDist = d;
                    bestFace = f; 
                }
            }

            if(bestFace != null)
            {
                bestFace.AssignedPoints.Add(pointIdx);
                return true;
            }
            return false;
        }
    }
}