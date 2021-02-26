using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DrawController : MonoBehaviour
{

    public float LineWidth = 3;
    public Camera mainCamera;
    public GameObject LinePrefab;
    public GameObject LegsCubePrefab;
    public Transform LegsSpawnerPivot;
    public Transform LegsSpawnerPivot2;
    public Material LegsMaterial;

    List<Vector3> LinePoints;
    float rayDepth = 1;
    LineRenderer _lineRenderer;

    bool isDrawingLine;
    int linePointsIndex;
    float drawCooldown;
    List<GameObject> CreatedCubes;

    GameObject RightLeg;
    GameObject LeftLeg;

    // Start is called before the first frame update
    void Start()
    {
        LinePoints = new List<Vector3>();
        CreatedCubes = new List<GameObject>();
        linePointsIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        //Cooldown que permite desenhar somente a cada 0.5 seg

        drawCooldown -= Time.deltaTime;
        
        //No primeiro toque na tela adicionamos um ponto de linha no local do clique e outro ponto no mesmo local

        if (Input.GetMouseButtonDown(0))
        {


            MovingForce.instance.StopMoving();
            isDrawingLine = true;


            GameObject g = Instantiate(LinePrefab, Vector3.zero, LinePrefab.transform.rotation) as GameObject;
            _lineRenderer = g.GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = LineWidth;
            _lineRenderer.endWidth = LineWidth;
            LinePoints.Add(GetMousePositionFromCamera());
            LinePoints.Add(GetMousePositionFromCamera());
            _lineRenderer.SetPosition(linePointsIndex, LinePoints[linePointsIndex]);
            linePointsIndex++;
            _lineRenderer.SetPosition(linePointsIndex, LinePoints[linePointsIndex]);
            linePointsIndex++;
            drawCooldown = 0.002f;


        } 
        
        //Se manteve o botão pressionado
        
        else if (Input.GetMouseButton(0) && isDrawingLine && drawCooldown < 0)                      
        {
            AddLinePoint();
            drawCooldown = 0.002f;

        } 
        
        //Quando solta o botão do mouse
        if (Input.GetMouseButtonUp(0) && isDrawingLine)
        {
            ResetLegs();
            AddLinePoint();
            drawCooldown = 0.002f;
            GenerateCubes();
            CombineCubesIntoOneMesh();
            ResetLines();

        }

    }
    void GenerateCubes()
    {

        Vector3 LastPoint;
        LastPoint = LegsSpawnerPivot.transform.position;


        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {

            if (i + 1 < _lineRenderer.positionCount)
            {
                Vector3 spawnPos = _lineRenderer.GetPosition(i + 1) - _lineRenderer.GetPosition(i);

                GameObject g = Instantiate(LegsCubePrefab, LastPoint + spawnPos,
                                Quaternion.identity);

                LastPoint = g.transform.position;

                CreatedCubes.Add(g);

            }

        }

    }

    void CombineCubesIntoOneMesh()
    {

        GameObject obj = new GameObject();
        obj.AddComponent<MeshFilter>();

        List<MeshFilter> meshFilters = new List<MeshFilter>();

        foreach (GameObject g in CreatedCubes)
        {
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, LegsSpawnerPivot.transform.position.z);
            meshFilters.Add(g.GetComponent<MeshFilter>());
            Debug.Log("Added mesh filter");
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        int i = 0;
        while (i < meshFilters.Count)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            meshFilters[i].gameObject.SetActive(false);

            i++;
        }


        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
        obj.transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        obj.transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        obj.transform.GetComponent<MeshFilter>().mesh.Optimize();
        obj.AddComponent<MeshRenderer>();
        obj.GetComponent<MeshRenderer>().material = LegsMaterial;
        obj.AddComponent<MeshCollider>();
        obj.GetComponent<MeshCollider>().convex = true;

        GameObject obj2 = Instantiate(obj);
        obj2.transform.position = new Vector3(obj2.transform.position.x, obj2.transform.position.y, obj2.transform.position.z + 1.25f);

        obj.transform.SetParent(LegsSpawnerPivot);

        obj2.transform.SetParent(LegsSpawnerPivot2);


        //Seta as pernas e salva as referências
        RightLeg = obj;
        LeftLeg = obj2;


        //Começa o movimento depois de desenhar as pernas
        StartMoving();

    }


    void AddLinePoint()
    {

        _lineRenderer.positionCount++;
        LinePoints.Add(GetMousePositionFromCamera());
        _lineRenderer.SetPosition(linePointsIndex, LinePoints[linePointsIndex]);
        linePointsIndex++;


    }

    void ResetLines()
    {
        isDrawingLine = false;
        LinePoints = new List<Vector3>();
        linePointsIndex = 0;

    }

    void ResetLegs()
    {

        Destroy(LeftLeg);
        Destroy(RightLeg);
        Destroy(_lineRenderer.gameObject);
    }

    Vector3 GetMousePositionFromCamera()
    {

        Ray ray;
        Vector3 MousePos;

        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        MousePos = ray.origin + (ray.direction * rayDepth);
        return MousePos;


    }

    void StartMoving()
    {
        MovingForce.instance.gameObject.GetComponent<Rigidbody>().useGravity = true;
        MovingForce.instance.SetMovingVelocity();



    }
}
