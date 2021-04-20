using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LensMeshGenerator : MonoBehaviour
{
    // Start is called before the first frame update

    MeshFilter testmeshfilter;
    //[Range(0.0f, 4.0f)]
    //public float testfloat;
    //[Range(1.01f, 20.0f)]


    public float circradius = 3.0f;
    [Range(-1, 1)]
    public float radcalc = 0;


    // not to change ingame
    [Range(3, 50)]
    public int sectionPoints = 10;
    [Range(3, 50)]
    public int domeSegments = 10;



    void Start()
    {
        testmeshfilter = gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
    }



    float calcRadiusFromTopedge(float edgeinput, float lensradius)
    {

        // tan alpha = lensradius/ edgeinput
        float edgeinp_sav = edgeinput;

        // clamp forbidden values
        if (edgeinput > 0 && edgeinput < 0.01f) edgeinput = 0.01f;
        if (edgeinput < 0 && edgeinput > -0.01f) edgeinput = -0.01f;
        if (edgeinput > 0.999) edgeinput = 0.999f;
        if (edgeinput < -0.999) edgeinput = -0.999f;

        /*
        if(Mathf.Abs(edgeinput) < 0.01)
        {
            edgeinput = 0.01f;
        }
        if (Mathf.Abs(edgeinput) > 0.999)
        {
            if(edgeinput> 0)
            {
                edgeinput = 0.999f;
            }
            else
            {
                edgeinput = 0.999f;
            }
            
        }*/

        float alpha = Mathf.Atan(lensradius / edgeinput) * Mathf.Rad2Deg;
        //2ndtriangle = 90 - (180 - 90 - alpha)

        // segmentr = 90 - 2ndtriangle

        float radiusangle =  Mathf.Abs(alpha)*2 -90.0f;



        // radius = lensradius / sin ( segmentr)

        float radius = lensradius / Mathf.Sin((radiusangle + 90) *Mathf.Deg2Rad); //todo invert why no wurk 

        //Debug.Log(radiusangle);
        //Debug.Log(radius);
        //Debug.Log("---");

        if (edgeinp_sav > 0) radius = radius * -1;


        return radius;
    }



    float calcSectionAngle(float radius, float lensradius)
    {
        //CARE, radiants

        float size = 1.0f;
        //return Mathf.Rad2Deg *Mathf.Atan2(1.0f, radius); // hardcoded, lenssize could be bigger/smaller
        float angle=  Mathf.Rad2Deg* Mathf.Asin((lensradius/ radius));
        return angle;

    }


    //gets points of arc, starting at (0,0) and ending at (x, y(1.0))
    Vector3[] getSectionPoints(int numpoints, float sectionangle, float radius)
    {
        Vector3 radvec = new Vector3(radius, 0.0f, 0.0f);

        //radvec = Quaternion.Euler(0, sectionangle / 10.0f, 0)*radvec; // what axis? 

        Vector3[] points = new Vector3[numpoints];
        //iterate over all points
        for(int i = 0; i < numpoints; i++)
        {
            //i+1/numpoints
            points[i] = Quaternion.Euler(0, sectionangle * (i + 1) / numpoints, 0) * radvec;
            points[i].x = points[i].x - circradius;
        }


        // return list of points in array

        //afterwards rotate whole section and connect them together

        return points;
    }

    List<Vector3[]> getDomeVertices(Vector3[] slice, int numberofribs)
    {

        float rotAngle = 360 / numberofribs;

        // numberofribs - 1
        List<Vector3[]> dome = new List<Vector3[]>();
        dome.Add(slice);
        //create copies and rotate around the rib
        for (int i = 0; i < numberofribs -1; i++)
        {

            // new list, rotate
            Vector3[] rotated = (Vector3[])slice.Clone();

            //rotate
            for( int j = 0; j< rotated.Length; j++)
            {
                rotated[j]= Quaternion.Euler(rotAngle* (i+1), 0, 0) * rotated[j];
            }

            dome.Add(rotated);
        }

        

        return dome;
    }


    //makemesh function
    // also return vertex positions of outer ring
    (List<Vector3>, List<int>) makeFacesAndVerts(List<Vector3[]> dome)
    {
        Vector3 startVec = new Vector3(0.0f, 0.0f, 0.0f);

        //startVec.x = 3.0f;

        int dcount = dome[0].Length;

        List<Vector3> vertBuffer = new List<Vector3>();
        List<int> trisBuffer = new List<int>();
        vertBuffer.Add(startVec);
        //filled up list of verts
        for(int i = 0; i < dome.Count; i++)
        {
            vertBuffer.AddRange(dome[i]);
        }
        for(int i = 0; i < dome.Count; i++)
        {
            //make middle vertex
            trisBuffer.Add(0);
            trisBuffer.Add((i + 1)%dome.Count * dcount + 1);
            trisBuffer.Add(i * dcount + 1);
            
            for(int j = 0; j < dcount-1; j++)
            {
                //fill the rest of the triangles
                trisBuffer.Add((i + 1) % dome.Count * dcount + j + 1);
                trisBuffer.Add(i * dcount + 2 + j);
                trisBuffer.Add(i * dcount + 1 + j);


                trisBuffer.Add((i + 1) % dome.Count * dcount + j + 2);
                trisBuffer.Add(i * dcount + 2 + j);
                trisBuffer.Add((i + 1) % dome.Count * dcount + j + 1);



            }





        }




        return (vertBuffer, trisBuffer);
    } 




    // Update is called once per frame
    void Update()
    {
        Mesh mymesh = new Mesh();

        float radiuss = calcRadiusFromTopedge(radcalc, 1.0f);
        circradius = radiuss;

        float sectionangle = calcSectionAngle(circradius, 1.0f); //circradius
        var pts = getSectionPoints(sectionPoints, sectionangle, circradius); //circradiuss


        // first vertex is 0 0 0 
        var domeL = getDomeVertices(pts, domeSegments);
        (List<Vector3> verts, List<int> tris) = makeFacesAndVerts(domeL);

        

        //Debug.Log(radcalc + "radius" + radiuss);

        // Set vertices and triangles to the mesh
        //mymesh.vertices = vertices;
        //mymesh.triangles = triangles;
        mymesh.SetVertices(verts);
        mymesh.SetTriangles(tris, 0);


        testmeshfilter.mesh = mymesh;
        mymesh.RecalculateNormals();

    }
}
