using System;
using UnityEngine;

public class MatrixMover : MonoBehaviour
{
    [Header("Material & Mesh")]
    public Material mat;

    [Header("Cube Dimensions")]
    public float width = 1f;
    public float height = 1f;
    public float depth = 1f;

    [Header("Movement (A/D/W/S)")]
    public float moveSpeed = 3f;

    [Header("Spin (Spacebar)")]
    public float maxSpinSpeed = 720f;  
    public float spinDamping = 1.5f;    
    [Header("Orbit (E key, hold)")]
    public float orbitRadius = 2f;
    public float orbitSpeed = 120f;   
    private Mesh cubeMesh;

  
    private Vector3 basePosition = Vector3.zero;

  
    private float spinAngle = 0f;  
    private float spinSpeed = 0f;  


    private bool orbiting = false;
    private Vector3 orbitPivot;
    private float orbitAngle = 0f;

    private void Start()
    {
        cubeMesh = CreateCubeMesh();
    }

    void Update()
    {
        if (cubeMesh == null) return;

        HandleMovement();
        HandleSpin();
        HandleOrbit();

        Vector3 finalPosition = orbiting
            ? orbitPivot + OrbitOffset(orbitAngle)
            : basePosition;

        Quaternion rotation = Quaternion.Euler(0f, spinAngle, 0f);

        Matrix4x4 m = Matrix4x4.TRS(finalPosition, rotation, Vector3.one);
        Graphics.DrawMesh(cubeMesh, m, mat, 0);
    }


    void HandleMovement()
    {

        if (orbiting) return;

        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) input += Vector3.left;
        if (Input.GetKey(KeyCode.D)) input += Vector3.right;
        if (Input.GetKey(KeyCode.W)) input += Vector3.up;
        if (Input.GetKey(KeyCode.S)) input += Vector3.down;

        basePosition += input * moveSpeed * Time.deltaTime;
    }


    void HandleSpin()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spinSpeed = maxSpinSpeed;
        }

        spinAngle += spinSpeed * Time.deltaTime;

        spinSpeed = Mathf.MoveTowards(spinSpeed, 0f, maxSpinSpeed * spinDamping * Time.deltaTime);
    }


    void HandleOrbit()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            orbiting = true;
            orbitPivot = basePosition;  
            orbitAngle = 0f;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            orbiting = false;

            basePosition = orbitPivot + OrbitOffset(orbitAngle);
        }

        if (orbiting)
        {
            orbitAngle += orbitSpeed * Time.deltaTime;
        }
    }

    Vector3 OrbitOffset(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * orbitRadius;
    }

    Mesh CreateCubeMesh()
    {
        var mesh = new Mesh();

        Vector3[] vertices = new Vector3[8]
        {
           
            new Vector3(0, 0, 0), 
            new Vector3(width, 0, 0), 
            new Vector3(width, 0, depth),
            new Vector3(0, 0, depth), 

        
            new Vector3(0, height, 0),       
            new Vector3(width, height, 0),    
            new Vector3(width, height, depth),
            new Vector3(0, height, depth)  
        };


        int[] triangles = new int[36]
        {

            0, 4, 1,
            1, 4, 5,


            2, 6, 3,
            3, 6, 7,


            0, 3, 4,
            4, 3, 7,


            1, 5, 2,
            2, 5, 6,

            0, 1, 3,
            3, 1, 2,


            4, 7, 5,
            5, 7, 6
        };

        Vector2[] uvs = new Vector2[8];
        for (int i = 0; i < 8; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / width, vertices[i].z / depth);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
