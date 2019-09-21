using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Burst;

public class DeviceReader : MonoBehaviour
{
    public TestCube cube;
    //데이터를 가져올 클래스의 위치
    //public BluetoothDeviceManager bluetoothDeviceManager;
    //public BufferManager bufferManager;
    //public FFTProcess fftProcess;

    //텍스트 갖고있기
    public Text rawData;
    public Text deviceData;
    public Text slideData;
    public Text bufferData;
    public Text indexData;

    //FFTProcess Data
    public Text EEG1Data;
    public Text EEG2Data;
    public Text PPGData;

    public Text EEG1PercentData;
    public Text EEG2percentData;

    public Text timer;
    int times;

    public double testData;

    // Start is called before the first frame update
    void Start()
    {
        times = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 문자열 읽어오기
    /// </summary>
    /// <param name="data"></param>
    /// <param name="channel"></param>
    [BurstCompile]
    public void ReadText(string data, int channel)
    {
        switch(channel)
        {
            case 0:     //device data
                textOut(deviceData, data);
                break;

            case 1:     //raw data
                textOut(rawData, data);
                break;

            case 2:     //slide data
                textOut(slideData, data);
                break;

            case 3:     //buffer data
                textOut(bufferData, data);
                break;

            case 4:     //Index data
                textOut(indexData, data);
                break;

            case 10:    //EEG1 data
                textOut(EEG1Data, data);
                break;

            case 11:
                textOut(EEG1PercentData, data);
                break;

            case 20:    //EEG2 data
                textOut(EEG2Data, data);
                break;

            case 21:
                textOut(EEG2percentData, data);
                break;

            case 30:    //PPG data
                textOut(PPGData, data);
                break;

            case 99:    //timer
                textOut(timer);
                break;
        }
    }

    /// <summary>
    /// 읽어온 문자열 텍스트 출력
    /// </summary>
    /// <param name="name"></param>
    /// <param name="data"></param>
    [BurstCompile]
    private void textOut(Text name, string data)
    {
        name.text = data;
    }

    [BurstCompile]
    private void textOut(Text name)
    {
        name.text = ++times + "";
    }

    [BurstCompile]
    public void CatchData(double data)
    {
        cube.Change(data);
    }
}
