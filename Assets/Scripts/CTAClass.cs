using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CTAClass : MonoBehaviour
{

    public Image Circle;
    public TMP_Text TimeText;
    public List<string> Langs = new List<string>();
    public TMP_Text Discription;
    
    private Button button;
    private float _time = 0;
    public int Timeout = 20;
    private GameManager _manager;
    
    public void Init(GameManager manager)
    {
        _manager = manager;
        button = GetComponent<Button>();
        button.onClick.AddListener(Hide);
        Hide();
    }

    public void Show()
    {
        _time = Timeout;
        gameObject.SetActive(true);
        Discription.text = Langs[_manager.CurrentLang];
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            _time -= Time.deltaTime;
            TimeText.text = _time.ToString("00");
            Circle.fillAmount = _time / Timeout;
            if (_time <= 0)
            {
                _manager.OffAllPlane();
                Hide();
                _manager.ChangeCTAText.Show();
            }
        }
    }
}
