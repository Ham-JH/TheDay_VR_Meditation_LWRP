using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Burst;

namespace DOTS_BLE
{
    public class DOTS_DeviceReader : MonoBehaviour
    {
        public DOTS_Files files;
        //데이터를 가져올 클래스의 위치
        //public BluetoothDeviceManager bluetoothDeviceManager;
        //public BufferManager bufferManager;
        //public FFTProcess fftProcess;

        //텍스트 갖고있기
        //public Text rawData;
        public Text deviceData;
        //public Text slideData;
        //public Text bufferData;
        //public Text indexData;

        //FFTProcess Data
        public Text EEG1Data;
        public Text EEG2Data;
        public Text EEG1PercentData;
        public Text EEG2percentData;
        public Text PPGData;

        [HideInInspector] public Text timer;
        [HideInInspector] public double times;

        //=================================================
        // DOTS_BDM 갱신 변수
        [HideInInspector] public string deviceName;           //장비명
        [HideInInspector] public bool streamReadStatus;       //스트림 상태

        //DOTS_BufferManager 갱신 변수(public 변수로 직접 읽는다.)
        [HideInInspector] public DOTS_BufferManager bufferManager;


        //DOTS_FFTProcess 갱신 변수
        [HideInInspector] public double[] EEG1_PSum;          //EEG1_PSum     double 5
        [HideInInspector] public double[] EEG2_PSum;          //EEG2_PSum     double 5
        [HideInInspector] public double[] EEG1_PAverage;      //EEG1_PAverage double 5
        [HideInInspector] public double[] EEG2_PAverage;      //EEG2_PAverage double 5
        [HideInInspector] public double BPM;                  //BPM           double 1

        // Start is called before the first frame update
        void Start()
        {
            times = 0;

            EEG1_PSum = new double[5];
            EEG2_PSum = new double[5];
            EEG1_PAverage = new double[5];
            EEG2_PAverage = new double[5];
            BPM = 0;
        }

        // Update is called once per frame
        void Update()
        {
            SetText(99);
            SetText(100);
            SetText(311);
            SetText(312);
            SetText(321);
            SetText(322);
            SetText(331);
        }

        public void SetText(int channel)
        {
            string txt;

            switch(channel)
            {
                case 99:        //타이머
                    times += Time.deltaTime;
                    ReadText(times.ToString(), 99);
                    break;

                case 100:       //장비 상태
                    txt = "DeviceName : " + deviceName
                        + "\nStreamreadStatus : " + streamReadStatus.ToString();
                    break;

                case 311:       //EEG1_PSum
                    txt = "EEG1PSum1 : " + EEG1_PSum[0]
                        + "\nEEG1PSum2 : " + EEG1_PSum[1]
                        + "\nEEG1PSum3 : " + EEG1_PSum[2]
                        + "\nEEG1PSum4 : " + EEG1_PSum[3]
                        + "\nEEG1PSum5 : " + EEG1_PSum[4];
                    ReadText(txt, 311);
                    break;

                case 312:       //EEG1_PAverage
                    txt = "EEG1PAverage1 : " + EEG1_PAverage[0]
                        + "\nEEG1PAverage2 : " + EEG1_PAverage[1]
                        + "\nEEG1PAverage3 : " + EEG1_PAverage[2]
                        + "\nEEG1PAverage4 : " + EEG1_PAverage[3]
                        + "\nEEG1PAverage5 : " + EEG1_PAverage[4];
                    ReadText(txt, 312);
                    break;

                case 321:       //EEG2_PSum
                    txt = "EEG2PSum1 : " + EEG2_PSum[0]
                        + "\nEEG2PSum2 : " + EEG2_PSum[1]
                        + "\nEEG2PSum3 : " + EEG2_PSum[2]
                        + "\nEEG2PSum4 : " + EEG2_PSum[3]
                        + "\nEEG2PSum5 : " + EEG2_PSum[4];
                    ReadText(txt, 321);
                    break;

                case 322:       //EEG2_PAverage
                    txt = "EEG2PAverage1 : " + EEG2_PAverage[0]
                        + "\nEEG2PAverage2 : " + EEG2_PAverage[1]
                        + "\nEEG2PAverage3 : " + EEG2_PAverage[2]
                        + "\nEEG2PAverage4 : " + EEG2_PAverage[3]
                        + "\nEEG2PAverage5 : " + EEG2_PAverage[4];
                    ReadText(txt, 322);
                    break;

                case 331:
                    txt = "BPM : " + BPM;
                    ReadText(txt, 331);
                    break;
            }
        }

        /// <summary>
        /// 문자열 읽어오기
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        [BurstCompile]
        public void ReadText(string data, int channel)
        {
            switch (channel)
            {
                case 99:    //timer
                    textOut(timer);
                    break;

                case 100:     //device data
                    textOut(deviceData, data);
                    break;

                //case 1:     //raw data
                //    textOut(rawData, data);
                //    break;

                //case 2:     //slide data
                //    textOut(slideData, data);
                //    break;

                //case 3:     //buffer data
                //    textOut(bufferData, data);
                //    break;

                //case 4:     //Index data
                //    textOut(indexData, data);
                //    break;

                case 311:    //EEG1 data
                    textOut(EEG1Data, data);
                    break;

                case 312:
                    textOut(EEG1PercentData, data);
                    break;

                case 321:    //EEG2 data
                    textOut(EEG2Data, data);
                    break;

                case 322:
                    textOut(EEG2percentData, data);
                    break;

                case 331:    //PPG data
                    textOut(PPGData, data);
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

        /// <summary>
        /// 문자열형 데이터 갖고오기(데이터 발생 코드에서 호출)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assignPath"></param>
        [BurstCompile]
        public void CatchData(string value, int assignPath)
        {
            switch(assignPath)
            {
                case 100:   //장비명
                    deviceName = value;
                    //Debug.Log(deviceName);
                    break;
            }
        }

        /// <summary>
        /// 논리형 데이터 갖고오기(데이터 발생 코드에서 호출)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assignPath"></param>
        [BurstCompile]
        public void CatchData(bool value, int assignPath)
        {
            switch(assignPath)
            {
                case 101:   //장비 스트림 상태 할당
                    streamReadStatus = value;
                    //Debug.Log(streamReadStatus);
                    break;
            }
        }

        /// <summary>
        /// double형 데이터 갖고오기
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assignPath"></param>
        [BurstCompile]
        public void CatchData(double value, int assignPath)
        {
            switch(assignPath)
            {
                case 331:   //BPM 갱신
                    BPM = value;
                    files.textAppend(value, "BPM.txt");
                    break;
            }
        }


        /// <summary>
        /// double 배열형 데이터 갖고오기
        /// </summary>
        /// <param name="values"></param>
        /// <param name="assignPath"></param>
        [BurstCompile]
        public void CatchData(double[] values, int assignPath)
        {
            switch(assignPath)
            {
                case 311:   //EEG1_PSum 갱신
                    EEG1_PSum[0] = values[0];
                    EEG1_PSum[1] = values[1];
                    EEG1_PSum[2] = values[2];
                    EEG1_PSum[3] = values[3];
                    EEG1_PSum[4] = values[4];
                    files.textAppend(values, "EEG1PSum.txt");
                    break;

                case 312:   //EEG1_PAverage 갱신
                    EEG1_PAverage[0] = values[0];
                    EEG1_PAverage[1] = values[1];
                    EEG1_PAverage[2] = values[2];
                    EEG1_PAverage[3] = values[3];
                    EEG1_PAverage[4] = values[4];
                    files.textAppend(values, "EEG1PAverage.txt");
                    break;

                case 321:   //EEG2_PSum 갱신
                    EEG2_PSum[0] = values[0];
                    EEG2_PSum[1] = values[1];
                    EEG2_PSum[2] = values[2];
                    EEG2_PSum[3] = values[3];
                    EEG2_PSum[4] = values[4];
                    files.textAppend(values, "EEG2PSum.txt");
                    break;

                case 322:   //EEG2_PAverage 갱신
                    EEG2_PAverage[0] = values[0];
                    EEG2_PAverage[1] = values[1];
                    EEG2_PAverage[2] = values[2];
                    EEG2_PAverage[3] = values[3];
                    EEG2_PAverage[4] = values[4];
                    files.textAppend(values, "EEG2PAverage.txt");
                    break;
            }
        }

        /// <summary>
        /// bufferManager 코드에서 실시간 RawData를 가지고 오는 영역.
        /// 성능 저하를 고려해 public 변수 상태로 데이터를 받아온다.
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="ch1"></param>
        /// <param name="ch2"></param>
        /// <param name="ch3"></param>
        /// <param name="battery"></param>
        [BurstCompile]
        public void CatchRawData(int counter, int ch1, int ch2, int ch3, int battery)
        {
            
        }
    }

}
