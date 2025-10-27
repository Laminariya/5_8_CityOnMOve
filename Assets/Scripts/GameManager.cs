using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    
    public Button b_StartPanel;
    public GameObject StayTransportPanel;
    //public VideoClip Plane;
    //public VideoClip Car;
    //public VideoClip Trane;
    //public VideoPlayer VideoPlayer;
    public Sprite Plane;
    public Sprite Car;
    public Sprite Trane;
    public Image TransportRoadImage;
    public Image CloudsImage1;
    public Image CloudsImage2;

    public Button b_Uzb;
    public Button b_Rus;
    
    public Transform ParentPpoints;
    public List<Transform> Points = new List<Transform>();

    public Transform TransportParent;

    public TMP_Text Log;
    public TMP_Text LogVector;

    public TMP_Text StartPanelText;
    public string RusText;
    public string UzbText;

    public TMP_Text StayMaket;
    public string RusStayMaket;
    public string UzbStayMaket;
    
    
    public OSCClass _oscClass;
    private CTAClass _ctaClass;
    public ChangeCTAText ChangeCTAText;
    public int CurrentLang;
    private List<Vector3> points3 = new List<Vector3>();
    private List<Vector3> points4 = new List<Vector3>();
    private List<Vector3> points5 = new List<Vector3>();

    private float _timer;
    private int _countFrame;
    private int _currentTouch;
    private int _NumberVideo;
    private bool _isPlayVideo;
    private int DeltaFrame = 5;

    private bool _isHide;

    void Start()
    {
        CurrentLang = 0;
        _isHide = false;
        b_Uzb.onClick.AddListener(OnLangUzb);
        b_Rus.onClick.AddListener(OnLangRus);
        b_StartPanel.onClick.AddListener(OnStartClick);
        _oscClass = GetComponent<OSCClass>();
        _oscClass.Init();
        _ctaClass = FindObjectOfType<CTAClass>(true);
        _ctaClass.Init(this);
        ChangeCTAText = FindObjectOfType<ChangeCTAText>(true);
        List<MeshRenderer> mesh = ParentPpoints.GetComponentsInChildren<MeshRenderer>().ToList();
        foreach (var meshRenderer in mesh)
        {
            Points.Add(meshRenderer.transform);
        }
        Debug.Log(Points.Count);
        
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
        ChangeText();
        InitTransportPanel();
        OffAllPlane();
    }

    private void OnStartClick()
    {
        
    }

    private void InitTransportPanel()
    {
        _countFrame = 0;
        _NumberVideo = 0;
        _isPlayVideo = false; //slide_HideVideo
        _oscClass.MySendMessage("slide_HideVideo");
    }

    public void OffAllPlane()
    {
        StayTransportPanel.SetActive(false);
        TransportParent.gameObject.SetActive(false);
        //VideoPlayer.Stop();
        _oscClass.MySendMessage("slide_Standby");
    }


    void Update()
    {
        if (Input.anyKey)
        {
            _timer = Time.time;
        }

        if (StayTransportPanel.activeSelf && !_ctaClass.gameObject.activeSelf && Time.time - _timer > 15f)
        {
            _ctaClass.Show();
        }

        if (!StayTransportPanel.activeSelf) return;


        if (Input.touchCount == 3)
        {
            _isHide = false;
            if (_currentTouch == Input.touchCount)
            {
                _countFrame++;
            }
            else
            {
                _currentTouch = Input.touchCount;
                _countFrame = 0;
            }

            if (_countFrame < _oscClass.jsonData.delayFrame) return;

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

        else if (Input.touchCount == 4)
        {
            _isHide = false;
            if (_currentTouch == Input.touchCount)
            {
                _countFrame++;
            }
            else
            {
                _currentTouch = Input.touchCount;
                _countFrame = 0;
            }

            if (_countFrame < _oscClass.jsonData.delayFrame) return;

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

        else if (Input.touchCount == 5)
        {
            _isHide = false;
            if (_currentTouch == Input.touchCount)
            {
                _countFrame++;
            }
            else
            {
                _currentTouch = Input.touchCount;
                _countFrame = 0;
            }

            if (_countFrame < _oscClass.jsonData.delayFrame) return;

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

        else if (Input.touchCount == 0)
        {
            if (_currentTouch == Input.touchCount)
            {
                _countFrame++;
            }
            else
            {
                _currentTouch = Input.touchCount;
                _countFrame = 0;
            }

            if (_countFrame < _oscClass.jsonData.delayFrame) return;
            TransportParent.gameObject.SetActive(false);
            if (!_isHide)
            {
                InitTransportPanel();
                _isHide = true;
            }
        }
        
        Log.text = Input.touchCount.ToString();
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
        
        if (CurrentLang == 0)
        {
            CreateTransport(dir, middle, Plane, "slide_Air_uzb", 3);
        }

        if (CurrentLang==1)
        {
            CreateTransport(dir, middle, Plane, "slide_Air_ru",3);
        }

        _NumberVideo = 3;
        _isPlayVideo = true;
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
        if (CurrentLang == 0)
        {
            CreateTransport(dir, middle, Car, "slide_Car_uzb", 4);
        }

        if (CurrentLang==1)
        {
            CreateTransport(dir, middle, Car, "slide_Car_ru", 4);
        }
        
        _NumberVideo = 4;
        _isPlayVideo = true;
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
        if (CurrentLang == 0)
        {
            CreateTransport(dir, middle, Trane, "slide_Train_uzb", 5);
        }

        if (CurrentLang==1)
        {
            CreateTransport(dir, middle, Trane, "slide_Train_ru", 5);
        }
        
        _NumberVideo = 5;
        _isPlayVideo = true;
    }

    private void CreateTransport(Vector2 direction, Vector3 position, Sprite sprite, string mess, int numberVideo)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        TransportParent.gameObject.SetActive(true);
        TransportParent.position = position;
        TransportParent.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        TransportRoadImage.sprite = sprite;
        CloudsImage1.enabled = sprite==Plane;
        CloudsImage2.enabled = sprite==Plane;
        if(_NumberVideo==numberVideo) return;
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
        _isHide = false;
        ChangeText();
    }

    private void OnLangRus()
    {
        _isHide = false;
        CurrentLang = 1;
        StayTransportPanel.SetActive(true);
        ChangeText();
    }

    private void ChangeText()
    {
        if (CurrentLang == 0)
        {
            StartPanelText.text = UzbText;
            StayMaket.text = UzbStayMaket;
        }

        if (CurrentLang == 1)
        {
            StartPanelText.text = RusText;
            StayMaket.text = RusStayMaket;
        }
    }
    
    public string GetAddLang(string id)
    {
        string _id = "";
        if(CurrentLang==0) _id = id + "_uzb";
        if(CurrentLang==1) _id = id + "_ru";
        return _id;
    }
}
