using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.TextJuicer;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public class ChangeCTAText : MonoBehaviour
{

    public float Delay;
    public float Speed;
    public GameObject ParentObject;
    public List<string> Text = new List<string>(); 
    
    private TMP_TextJuicer _textJuicer;
    private float _timer;
    private Coroutine _coroutine;
    private int _currentTextIndex;
    public GameManager manager;
    
    void Start()
    {
        _textJuicer = GetComponent<TMP_TextJuicer>();
        _textJuicer.SetProgress(1f);
        _currentTextIndex = 0;
        _textJuicer.Text = Text[_currentTextIndex];
        _textJuicer.Update();
    }
    
    void Update()
    {
        if (!ParentObject.activeSelf && Time.time - _timer >= Delay)
        {
            if(_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(ChangeText());
            _timer = Time.time;
        }
    }

    IEnumerator ChangeText()
    {
        float progress = 1f;
        while (progress > 0.001f)
        {
            progress -= Time.deltaTime * Speed;
            if (progress < 0.001f) progress = 0f;
            _textJuicer.SetProgress(progress);
            _textJuicer.Update();
            yield return null;
        }

        _currentTextIndex++;
        if(_currentTextIndex >= Text.Count)
            _currentTextIndex = 0;
        manager.CurrentLang = _currentTextIndex;
        
        _textJuicer.Text = Text[_currentTextIndex];
        manager._oscClass.MySendMessage(manager.GetAddLang("slide_Standby"));
        
        progress = 0f;
        while (progress < 0.999f)
        {
            progress += Time.deltaTime * Speed;
            if (progress > 0.999f) progress = 1f;
            _textJuicer.SetProgress(progress);
            _textJuicer.Update();
            yield return null;
        }
    }
}
