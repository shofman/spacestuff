using UnityEngine;
using System;
using System.Collections.Generic;

public class MeshCreation : MonoBehaviour {
   
    private int partNo = 30;
    private int groundRes = 25;
 
    private Mesh mesh;
    private List<Vector3> vertices;
    private int[] triangles;   
    private MeshRenderer meshRenderer;

    // Create Mesh, you need
    // mesh.vertices
    // mesh.triangles
    // 
    // mesh.vertices = [new Vector3(), new Vector3, new Vector3]
    // mesh.triangles = [0,1,2,0,1,3,0,2,3,1,2,3]
   
    void Start () 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        makeBasicPlane();
        makeBasicCube();
    }

    void Update()
    {
        int speed = 80;
        gameObject.transform.Rotate(Vector3.right * Time.deltaTime * speed);
        gameObject.transform.Rotate(Vector3.up * Time.deltaTime * speed);
        gameObject.transform.Rotate(Vector3.forward * Time.deltaTime * speed);
    }

    void makeBasicCube()
    {
        vertices = new List<Vector3>();
        triangles = new int[36];
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.Clear();
        mesh.name = "CubeMesh";

        Vector3 first = new Vector3(0,0,0);
        addCube(first);

        Vector3 second = new Vector3(1,0,0);
        addCube(second);

        Vector3 third = new Vector3(1,0,1);
        addCube(third);

        Vector3 fourth = new Vector3(0,0,1);
        addCube(fourth);

        addCube(new Vector3(0,1,0));
        addCube(new Vector3(1,1,0));
        addCube(new Vector3(0,1,1));
        addCube(new Vector3(1,1,1));

        mesh.vertices = vertices.ToArray();

        // Bottom plane
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        // Left plane
        triangles[6] = 0;
        triangles[7] = 5;
        triangles[8] = 1;

        triangles[9] = 0;
        triangles[10] = 4;
        triangles[11] = 5;

        // Right plane
        triangles[12] = 3;
        triangles[13] = 2;
        triangles[14] = 6;

        triangles[15] = 2;
        triangles[16] = 7;
        triangles[17] = 6;

        // Front plane
        triangles[18] = 1;
        triangles[19] = 5;
        triangles[20] = 2;

        triangles[21] = 2;
        triangles[22] = 5;
        triangles[23] = 7;

        // Rear plane
        triangles[24] = 0;
        triangles[25] = 3;
        triangles[26] = 4;

        triangles[27] = 3;
        triangles[28] = 6;
        triangles[29] = 4;

        // Top plane
        triangles[30] = 5;
        triangles[31] = 4;
        triangles[32] = 6;

        triangles[33] = 5;
        triangles[34] = 6;
        triangles[35] = 7;


        mesh.triangles = triangles;
    }

    void makeBasicPlane()
    {
        vertices = new List<Vector3>();
        triangles = new int[6];
       
       
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.Clear();
        mesh.name = "PlaneMesh";
 
        Vector3 first = new Vector3(0,0,0);
        addCube(first);

        Vector3 second = new Vector3(1,0,0);
        addCube(second);

        Vector3 third = new Vector3(1,0,1);
        addCube(third);

        Vector3 fourth = new Vector3(0,0,1);
        addCube(fourth);
       
        mesh.vertices = vertices.ToArray();

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.triangles = triangles;
    }

    void addCube(Vector3 pos)
    {
        vertices.Add(pos);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = pos;
        cube.transform.localScale = new Vector3(.1f, .1f, .1f);
    }
}