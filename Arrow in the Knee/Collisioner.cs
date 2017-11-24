using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisioner
{

    public bool colliding;
    public Transform tip;
    public Transform target;

    List<Vector3> simplex;
    Vector3 direction;

    public Collisioner() {
        colliding = false;
        tip = GameObject.Find("Tip").transform;
        target = GameObject.Find("Target").transform;
    }
 
    // Update is called once per frame
    void Update()
    {
        if (!colliding)
        {
            colliding = CheckCollision();
        }
    }

    //Checks collision
    bool CheckCollision()
    {
        Vector3 p1 = Support(tip, target, new Vector3(0.1f, -1f, 1f));
        simplex = new List<Vector3>(4);
        simplex.Insert(0, p1);
        direction = -p1;
        //Debug.Log(p1);

        while (true)
        {
            Vector3 newPoint = Support(tip, target, direction);
            if (Vector3.Dot(newPoint, direction) < 0f)
            { //no intersection
                return false;
            }

            simplex.Insert(0, newPoint);
            Debug.Log(simplex);

            if (!DoSimplex())
            {
                return false;
            }
            return true;
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

            if (Vector3.Dot(AB, AO) > 0f) // No intersection
            {
                Vector3 perp = TripleCrossProduct(AB, AO, AB);
                direction = perp;
            }

            direction = AO;
            simplex.RemoveAt(1);
            return true;
        }
        if (simplex.Count == 3) //Triangle 
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
                    return true;
                }

                else // Origin is further away than A 
                {
                    return false;
                }
            }

            // If AB side of triangle
            Vector3 ABxABC = Vector3.Cross(AB, ABC);
            if (Vector3.Dot(ABxABC, AO) > 0f)
            {

                if (Vector3.Dot(AB, AO) > 0f) // Origin is on AB side of trianlge 
                {
                    direction = TripleCrossProduct(AB, AO, AB);
                    simplex.Remove(B);
                    return true;
                }

                else // Origin is further away than A 
                {
                    return false;
                }
            }

            // If within (Above/below) triangle
            else
            {
                // Above triangle
                if (Vector3.Dot(ABC, AO) > 0f)
                {
                    direction = ABC;
                    return true;
                }
                // Below triangle
                else if (Vector3.Dot(-ABC, AO) > 0f)
                {
                    direction = -ABC;
                    return true;
                }

                Debug.Log("is reached");
                return false; // Should never be reached (probably?)
            }
        }

        if (simplex.Count == 4) //Pyramid
        {
            return true;
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
