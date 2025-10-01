using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Button b_Uzb;
    public Button b_Rus;
    
    public Transform ParentPpoints;
    public List<Transform> Points = new List<Transform>();
    public GameObject DirPrefab;

    public Image TransportPanel;
    public Transform TransportObject;
    public List<Sprite> FonTransport = new List<Sprite>();
    public List<Sprite> Transport = new List<Sprite>();

    public GameObject StayTransportPanel;
    
    
    private OSCClass _oscClass;
    private int CurrentLang;
    private List<Vector3> points3 = new List<Vector3>();
    private List<Vector3> points4 = new List<Vector3>();
    private List<Vector3> points5 = new List<Vector3>();
    
    void Start()
    {
        b_Uzb.onClick.AddListener(OnLangUzb);
        b_Rus.onClick.AddListener(OnLangRus);
        _oscClass = GetComponent<OSCClass>();
        List<MeshRenderer> mesh = ParentPpoints.GetComponentsInChildren<MeshRenderer>().ToList();
        foreach (var meshRenderer in mesh)
        {
            Points.Add(meshRenderer.transform);
        }
        Debug.Log(Points.Count);
        _oscClass.Init();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            List<Vector3> vector3s = new List<Vector3>();
            foreach (var point in Points)
            {
                vector3s.Add(point.position);
            }
            if (Points.Count == 3) GetDirection3(vector3s);
            if (Points.Count == 4) GetDirection4(vector3s);
            if (Points.Count == 5) GetDirection5(vector3s);
        }

        
    }

    private void FixedUpdate()
    {
        if (Input.touchCount == 3)
        {
            List<Vector3> vector3s = new List<Vector3>();
            for (int i = 0; i < 3; i++)
            {
                vector3s.Add(Input.GetTouch(i).position);
            }

            Vector3 middleNew = GetMiddlePoint(vector3s);
            Vector3 middleOld = GetMiddlePoint(points3);

            if ((middleNew - middleOld).magnitude > 1f)
            {
                points3.Clear();
                points3 = new List<Vector3>(vector3s);
                GetDirection3(points3);
            }
        }
        
        if (Input.touchCount == 4)
        {
            List<Vector3> vector3s = new List<Vector3>();
            for (int i = 0; i < 4; i++)
            {
                vector3s.Add(Input.GetTouch(i).position);
            }

            Vector3 middleNew = GetMiddlePoint(vector3s);
            Vector3 middleOld = GetMiddlePoint(points4);

            if ((middleNew - middleOld).magnitude > 1f)
            {
                points4.Clear();
                points4 = new List<Vector3>(vector3s);
                GetDirection4(points4);
            }
        }
        
        if (Input.touchCount == 5)
        {
            List<Vector3> vector3s = new List<Vector3>();
            for (int i = 0; i < 5; i++)
            {
                vector3s.Add(Input.GetTouch(i).position);
            }

            Vector3 middleNew = GetMiddlePoint(vector3s);
            Vector3 middleOld = GetMiddlePoint(points5);

            if ((middleNew - middleOld).magnitude > 1f)
            {
                points5.Clear();
                points5 = new List<Vector3>(vector3s);
                GetDirection5(points5);
            }
        }
    }

    private void GetDirection3(List<Vector3> points)
    {
        Vector3 middle = GetMiddlePoint(points);
        
        float langht1 = (points[0] - points[1]).magnitude;
        float langht2 = (points[1] - points[2]).magnitude;
        float langht3 = (points[2] - points[0]).magnitude;

        if (langht1 < langht2 && langht1 < langht3)
        {
            Vector2 dir = points[2] - middle;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Instantiate(DirPrefab, middle, Quaternion.AngleAxis(angle, Vector3.forward));
        }
        if (langht2 < langht1 && langht2 < langht3)
        {
            Vector2 dir = points[0] - middle;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Instantiate(DirPrefab, middle, Quaternion.AngleAxis(angle, Vector3.forward));
        }
        if (langht3 < langht1 && langht3 < langht2)
        {
            Vector2 dir = points[1] - middle;
            CreateTransport(dir, middle, FonTransport[0], Transport[0]);
        }
    }

    private void GetDirection4(List<Vector3> points)
    {
        Vector2 dir = Vector2.zero;
        Vector3 middle = GetMiddlePoint(points);
        float distance = 999999f;
        Vector3 vector3 = Vector3.zero;
        foreach (var point in points)
        {
            float dist = (point - middle).magnitude;
            if (dist < distance)
            {
                distance = dist;
                vector3 = point;
            }
        }

        dir = middle - vector3;
        CreateTransport(dir, middle, FonTransport[1], Transport[1]);
    }
    
    private void GetDirection5(List<Vector3> points)
    {
        Vector2 dir = Vector2.zero;
        Vector3 middle = GetMiddlePoint(points);
        float distance = 999999f;
        Vector3 vector3 = Vector3.zero;
        foreach (var point in points)
        {
            float dist = (point - middle).magnitude;
            if (dist < distance)
            {
                distance = dist;
                vector3 = point;
            }
        }

        dir = middle - vector3;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        CreateTransport(dir, middle, FonTransport[2], Transport[2]);
    }

    private void CreateTransport(Vector2 direction, Vector3 position, Sprite fon, Sprite transport)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        TransportObject.gameObject.SetActive(true);
        TransportObject.position = position;
        TransportObject.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        TransportPanel.sprite = fon;
        TransportObject.GetComponent<Image>().sprite = transport;
    }

    private Vector3 GetMiddlePoint(List<Vector3> points)
    {
        Vector3 middle = Vector2.zero;
        foreach (var vector3 in points)
        {
            middle += vector3;
        }
        middle /= points.Count;
        return middle;
    }

    private void OnLangUzb()
    {
        CurrentLang = 0;
        StayTransportPanel.SetActive(true);
    }

    private void OnLangRus()
    {
        CurrentLang = 1;
        StayTransportPanel.SetActive(true);
    }
}
