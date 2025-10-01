using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using CoreOSC;
using CoreOSC.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class OSCClass : MonoBehaviour
{

    private string _nameConfig = "//config.txt";

    public string _ip;
    public int _port;
    public JsonData jsonData;
    
    private UdpClient udpClient = new UdpClient();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MySendMessage("idle");
        }
    }

    public void Init()
    {
        // try
        // {
        //     string text = File.ReadAllText(Directory.GetCurrentDirectory()+_nameConfig);
        //     jsonData = new JsonData();
        //     jsonData = JsonUtility.FromJson<JsonData>(text);
        //     _ip = jsonData.ip;
        //     _port = jsonData.port;
        // }
        // catch (FileNotFoundException)
        // {
        //     Debug.Log("Файл не найден!");
        // }
        // catch (Exception ex)
        // {
        //     Debug.Log($"Ошибка: {ex.Message}");
        // }

        //CreateJson();
        StartCoroutine(LoadConfig());
        
        udpClient.Connect(_ip, _port);
    }
    
    IEnumerator LoadConfig()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "config.txt");
        string jsonString;
    
        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(configPath))
            {
                yield return www.SendWebRequest();
                jsonString = www.downloadHandler.text;
            }
        }
        else
        {
            jsonString = File.ReadAllText(configPath);
        }
    
        jsonData = JsonUtility.FromJson<JsonData>(jsonString);
        Debug.Log("Game version: " + jsonData.ip);
    }

    public async Task MySendMessage(string id)
    {
        Debug.Log(id + " "+ GetAddress(id));
        if (udpClient.Client.Connected) //Проверяем есть ли подключение
        {
            var message = new OscMessage(new Address(GetAddress(id)), new object[] { 1 });
            await udpClient.SendMessageAsync(message);
        }
    }

    private string GetAddress(string key)
    {
        foreach (var command in jsonData.commands)
        {
            if(command.key == key)
                return command.address;
        }

        return "";
    }

    private void CreateJson()
    {
        JsonData script = new JsonData();
        script.ip = "127.0.0.1";
        script.port = 5555;
        script.delay = 20;
        script.commands.Add(new CommandsClass("slide_Air_ru", "/composition/layers/2/clips/1/connect"));
        script.commands.Add(new CommandsClass("slide_Car_ru", "/composition/layers/2/clips/2/connect"));
        script.commands.Add(new CommandsClass("slide_Train_ru", "/composition/layers/2/clips/3/connect"));
        script.commands.Add(new CommandsClass("slide_Air_uzb", "/composition/layers/1/clips/1/connect"));
        script.commands.Add(new CommandsClass("slide_Car_uzb", "/composition/layers/1/clips/2/connect"));
        script.commands.Add(new CommandsClass("slide_Train_uzb", "/composition/layers/1/clips/3/connect"));
        script.commands.Add(new CommandsClass("slide_Standby", "/composition/layers/1/clips/4/connect"));

        string json = JsonUtility.ToJson(script);
        
        using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + _nameConfig))
        {
            writer.Write(json);
        }
    }

}

[Serializable]
public class JsonData
{
    public string ip;
    public int port;
    public float delay;
    public List<CommandsClass> commands = new List<CommandsClass>();
}

[Serializable]
public class CommandsClass
{
    public string key;
    public string address;

    public CommandsClass(string key, string address)
    {
        this.key = key;
        this.address = address;
    }
}
