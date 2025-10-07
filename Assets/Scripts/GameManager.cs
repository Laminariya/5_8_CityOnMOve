using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{

    public GameObject LangPanel;
    public Button b_StartPanel;
    public GameObject StayTransportPanel;
    public VideoClip Plane;
    public VideoClip Car;
    public VideoClip Trane;
    public VideoPlayer VideoPlayer;

    public Button b_Uzb;
    public Button b_Rus;
    
    public Transform ParentPpoints;
    public List<Transform> Points = new List<Transform>();

    public Transform TransportParent;

    public TMP_Text Log;
    public TMP_Text LogVector;
    
    
    private OSCClass _oscClass;
    private int CurrentLang;
    private List<Vector3> points3 = new List<Vector3>();
    private List<Vector3> points4 = new List<Vector3>();
    private List<Vector3> points5 = new List<Vector3>();

    private float _timer;

    void Start()
    {
        b_Uzb.onClick.AddListener(OnLangUzb);
        b_Rus.onClick.AddListener(OnLangRus);
        b_StartPanel.onClick.AddListener(OnStartClick);
        _oscClass = GetComponent<OSCClass>();
        List<MeshRenderer> mesh = ParentPpoints.GetComponentsInChildren<MeshRenderer>().ToList();
        foreach (var meshRenderer in mesh)
        {
            Points.Add(meshRenderer.transform);
        }
        Debug.Log(Points.Count);
        _oscClass.Init();
        points3.Add(Vector3.zero);
        points3.Add(Vector3.zero);
        points3.Add(Vector3.zero);
        points5.Add(Vector3.zero);
        points5.Add(Vector3.zero);
        points5.Add(Vector3.zero);
        points5.Add(Vector3.zero);
        points5.Add(Vector3.zero);
        points4.Add(Vector3.zero);
        points4.Add(Vector3.zero);
        points4.Add(Vector3.zero);
        points4.Add(Vector3.zero);
        OffAllPlane();
    }

    private void OnStartClick()
    {
        LangPanel.SetActive(true);
    }

    private void OffAllPlane()
    {
        LangPanel.SetActive(false);
        StayTransportPanel.SetActive(false);
        TransportParent.gameObject.SetActive(false);
        VideoPlayer.Stop();
        if (CurrentLang == 1)
        {
            _oscClass.MySendMessage("slide_Standby");
        }

        if (CurrentLang==2)
        {
            _oscClass.MySendMessage("slide_Standby");
        }
        
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

        if (Input.anyKey)
        {
            _timer = Time.time;
        }

        if (LangPanel.activeSelf && Time.time - _timer > 30f)
        {
            OffAllPlane();
        }

        Log.text = Input.touchCount.ToString();
    }

    private void FixedUpdate()
    {
        if(!StayTransportPanel.activeSelf) return;
        if (Input.touchCount == 3)
        {
            List<Vector3> vector3s = new List<Vector3>();
            for (int i = 0; i < 3; i++)
            {
                vector3s.Add(Input.GetTouch(i).position);
            }

            Vector3 middleNew = GetMiddlePoint(vector3s);
            Vector3 middleOld = GetMiddlePoint(points3);

            LogVector.text = middleNew.ToString();
            LogVector.text += (middleNew - middleOld).magnitude;
            
            if ((middleNew - middleOld).magnitude > 0.1f)
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
        
        if (Input.touchCount < 3)
        {
            TransportParent.gameObject.SetActive(false);
            VideoPlayer.Stop();
        }
    }

    private void GetDirection3(List<Vector3> points)
    {
        Vector3 middle = GetMiddlePoint(points);
        
        float langht1 = (points[0] - points[1]).magnitude;
        float langht2 = (points[1] - points[2]).magnitude;
        float langht3 = (points[2] - points[0]).magnitude;

        Vector2 dir = Vector2.zero;
        if (langht1 < langht2 && langht1 < langht3)
        {
            dir = points[2] - middle;
        }
        if (langht2 < langht1 && langht2 < langht3)
        {
            dir = points[0] - middle;
        }
        if (langht3 < langht1 && langht3 < langht2)
        {
            dir = points[1] - middle;
        }
        
        if (CurrentLang == 1)
        {
            CreateTransport(dir, middle, Plane, "slide_Air_uzb");
        }

        if (CurrentLang==2)
        {
            CreateTransport(dir, middle, Plane, "slide_Air_ru");
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
        if (CurrentLang == 1)
        {
            CreateTransport(dir, middle, Car, "slide_Car_uzb");
        }

        if (CurrentLang==2)
        {
            CreateTransport(dir, middle, Car, "slide_Car_ru");
        }
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
        if (CurrentLang == 1)
        {
            CreateTransport(dir, middle, Trane, "slide_Train_uzb");
        }

        if (CurrentLang==2)
        {
            CreateTransport(dir, middle, Trane, "slide_Train_ru");
        }

        
    }

    private void CreateTransport(Vector2 direction, Vector3 position, VideoClip clip, string mess)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        TransportParent.gameObject.SetActive(true);
        TransportParent.position = position;
        TransportParent.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        VideoPlayer.clip = clip;
        if(!VideoPlayer.isPlaying) VideoPlayer.Play();
        _oscClass.MySendMessage(mess);
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
