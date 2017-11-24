using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisioner
{

    public bool colliding;
    public Transform tip;
    public Transform target;
    private int maxIteratinos = 15;

    List<Vector3> simplex;
    Vector3 direction;

    public Collisioner (Transform shape1, Transform shape2) {
        tip = shape1;
        target = shape2;
        Start();
    }
    // Use this for initialization
    void Start()
    {
        colliding = false;

    }

    // Update is called once per frame
    public void Update()
    {
        colliding = CheckCollision(tip, target);
    }

    //Checks collision
    bool CheckCollision(Transform shape1, Transform shape2)
    {
        Vector3 p1 = Support(shape1, shape2, new Vector3(0.1f, -1f, 1f));
        simplex = new List<Vector3>(4);
        simplex.Insert(0, p1);
        direction = -p1;
        int i = 0;
        while (true)
        {
            i++;
            Vector3 newPoint = Support(shape1, shape2, direction);
            if (Vector3.Dot(newPoint, direction) < 0f)
            { //no intersection
                return false;
            }

            simplex.Insert(0, newPoint);

            if (DoSimplex())
            {
                return true;
            }

            if (i > maxIteratinos)
            {
                return true;
            }
        }
    }

    //Calculates the Minkowski sum of the farthest points
    Vector3 Support(Transform s1, Transform s2, Vector3 d)
    {
        Vector3 p1 = GetFarPoint(s1, d);
        Vector3 p2 = GetFarPoint(s2, -d);
        Vector3 point = p1 - p2;

        return point;
    }

    bool DoSimplex()
    {
        if (simplex.Count == 2) //Line
        {
            Vector3 A = simplex[0];
            Vector3 B = simplex[1];
            Vector3 AB = B - A;
            Vector3 AO = -A;

            Vector3 perp = TripleCrossProduct(AB, AO, AB);
            direction = perp;
            return false;

        }
        else if (simplex.Count == 3) //Triangle 
        {
            Vector3 A = simplex[0];
            Vector3 B = simplex[1];
            Vector3 C = simplex[2];
            Vector3 AB = B - A;
            Vector3 AC = C - A;
            Vector3 AO = -A;
            Vector3 ABC = Vector3.Cross(AB, AC);

            // If AC side of triangle
            Vector3 ABCxAC = Vector3.Cross(ABC, AC);
            if (Vector3.Dot(ABCxAC, AO) > 0f)
            {
                if (Vector3.Dot(AC, AO) > 0f) // Origin is on AC side of trianlge 
                {
                    direction = TripleCrossProduct(AC, AO, AC);
                    simplex.Remove(B);
                    return false;
                }
            }

            // If AB side of triangle
            Vector3 ABxABC = Vector3.Cross(AB, ABC);
            if (Vector3.Dot(ABxABC, AO) > 0f)
            {
                direction = TripleCrossProduct(AB, AO, AB);
                simplex.Remove(C);
                return false;
            }
            else
            {

                if (Vector3.Dot(ABC, AO) > 0f)
                {
                    direction = ABC;
                    return false;
                }
                // Below triangle
                if (Vector3.Dot(-ABC, AO) > 0f)
                {
                    direction = -ABC;
                    return false;
                }
                return true; // Should never be reached (probably?)
            }
        }

        else if (simplex.Count == 4) //Pyramid
        {

            Vector3 A = simplex[0];
            Vector3 B = simplex[1];
            Vector3 C = simplex[2];
            Vector3 D = simplex[3];
            Vector3 AB = B - A;
            Vector3 AC = C - A;
            Vector3 AD = D - A;
            Vector3 AO = -A;
            Vector3 ABC = Vector3.Cross(AB, AC);
            Vector3 ADB = Vector3.Cross(AD, AB);
            Vector3 ACD = Vector3.Cross(AC, AD);


            // ABC 
            if (Vector3.Dot(ABC, AO) > 0)
            {
                
            }
            else if (Vector3.Dot(ACD, AO) > 0)
            {
                simplex.Clear();
                simplex.Add(A);
                simplex.Add(C);
                simplex.Add(D);
            }
            else if (Vector3.Dot(ADB, AO) > 0)
            {
                simplex.Clear();
                simplex.Add(A);
                simplex.Add(D);
                simplex.Add(B);
            }
            else
            {
                return true;
            }

            A = simplex[0];
            B = simplex[1];
            C = simplex[2];
            AB = B - A;
            AC = C - A;
            ABC = Vector3.Cross(AB, AC);
            Vector3 ABxABC = Vector3.Cross(AB, ABC);
            Vector3 ABCxAC = Vector3.Cross(ABC, AC);
            if (Vector3.Dot(ABxABC, AO) > 0)
            {
                simplex.Remove(C);
                direction = TripleCrossProduct(AB, AO, AB);
                return false;
            }
            else if (Vector3.Dot(ABCxAC, AO) > 0)
            {
                simplex.Remove(B);
                direction = TripleCrossProduct(AC, AO, AC);
                return false;
            }

            else
            {
                direction = ABC;
                return false;
            }

        }
        return false;
    }

    Vector3 TripleCrossProduct(Vector3 A, Vector3 B, Vector3 C)
    {
        return Vector3.Cross(Vector3.Cross(A, B), C);
    }

    //Gets farthest point in shape given a direction. Borrowed from
    //http://stackoverflow.com/questions/17386299/get-farthest-vertice-on-a-specified-direction-of-a-3d-model-unity
    Vector3 GetFarPoint(Transform obj, Vector3 direction)
    {

        Vector3[] vertices;
        Vector3 farthestPoint;
        float farDistance;

        vertices = obj.GetComponent<MeshFilter>().mesh.vertices;
        farthestPoint = vertices[0];
        farDistance = 0f;

        foreach (Vector3 vert in vertices)
        {
            float temp = Vector3.Dot(direction, vert);
            if (temp > farDistance)
            {
                farDistance = temp;
                farthestPoint = vert;
            }
        }
        farthestPoint = obj.TransformPoint(farthestPoint);
        return farthestPoint;
    }

}
