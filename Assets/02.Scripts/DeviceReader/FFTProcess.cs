using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

public class FFTProcess : MonoBehaviour
{
    #region public implementation

    public DeviceReader deviceReader;       //데이터 출력
    public BufferManager bufferManager;
    public Files file;

    #endregion public implementation

    #region private variables

    private int SlideCount;
    private int SlidePosition;

    int minEEG_HzCheck;                 //ch1, ch2  EEG 검출 최소주파수
    int maxEEG_HzCheck;                 //ch1, ch2  EEG 검출 최대주파수

    int minTheta_HzCheck;               //최소 Theta Hz 확인
    int minAlpha_HzCheck;               //최소 Alpha Hz 확인
    int minBeta_HzCheck;                //최소 Beta Hz 확인
    int minGamma_HzCheck;               //최소 Gamma Hz 확인

    int minPPG_HzCheck;                 //ch3       PPG 검출 최소주파수
    int maxPPG_HzCheck;                 //ch3       PPG 검출 최대주파수

    private double[] EEG1_result;       //ch1       EEG1값 받는 배열
    private double[] EEG2_result;       //ch2       EEG2값 받는 배열
    private double[] PPG_result;        //ch3       PPG값 받는 배열

    private double[] EEG1_PSum;         //ch1       EEG1 5분할 뇌파 받는 배열
    private double[] EEG2_PSum;         //ch2       EEG2 5분할 뇌파 받는 배열
    private double[] EEG1_PAverage;     //ch1       EEG1 5분할 뇌파 점유율 받는 배열
    private double[] EEG2_PAverage;     //ch2       EEG2 5분할 뇌파 점유율 받는 배열

    private double EEG1PowerSum;         //EEG 5분할 뇌파 power 총합 받는 변수
    private double EEG2PowerSum;         //EEG 5분할 뇌파 power 총합 받는 변수

    //private double[] ch1PMax;
    //private double[] ch2PMax;
    private double[] ch3PMax;           //ch3       PPG Hz기준 최대 power 검출
    private double[] ch3PIndex;         //ch3       PPG Hz기준 최대 power index 갱신


    #endregion private variables

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        SlideCount = bufferManager.slideCounter;
        SlidePosition = bufferManager.slidePosition;

        //EEG 검출 영역 최대 최소 주파수값 검출
        minEEG_HzCheck = (int)(1.0 / (1.0 / bufferManager.slideCounter));
        maxEEG_HzCheck = (int)(50.0 / (1.0 / bufferManager.slideCounter));

        minTheta_HzCheck = (int)(4.0 / (1.0 / bufferManager.slideCounter));
        minAlpha_HzCheck = (int)(8.0 / (1.0 / bufferManager.slideCounter));
        minBeta_HzCheck = (int)(13.0 / (1.0 / bufferManager.slideCounter));
        minGamma_HzCheck = (int)(30.0 / (1.0 / bufferManager.slideCounter));

        //PPG 검출 영역 최대 최소 주파수값 검출
        minPPG_HzCheck = (int)(0.8 / (1.0 / bufferManager.slideCounter));
        maxPPG_HzCheck = (int)(2.5 / (1.0 / bufferManager.slideCounter));

        deviceReader.ReadText("minEEG index   (1Hz): " + minEEG_HzCheck
                           +"\nminTheta index (4Hz): " + minTheta_HzCheck
                           +"\nminAlpha index (8Hz): " + minAlpha_HzCheck
                           +"\nminBeta index (13Hz): " + minBeta_HzCheck
                           +"\nminGamma index(30Hz): " + minGamma_HzCheck
                           +"\nmaxEEG index  (50Hz): " + maxEEG_HzCheck
                           +"\nminPPG index (0.8Hz): " + minPPG_HzCheck
                           +"\nmaxPPG index (2.5Hz): " + maxPPG_HzCheck
                           , 4);

        //1Hz의 파형은 일정량의 df(주파수 간격)으로 표시할 수 있는데
        //df = 1 / dt가 성립한다. dt는 (1 / 초당 검출 데이터 개수) * (데이터 개수 * 모은 데이터의 시간(n초))인데
        //초당 검출 데이터 개수가 분모 안의 값에서 분자 / 분모에 같이 분포하므로 둘을 지우면
        //모은 데이터의 시간만 남게 된다. 즉, df = 1 / (모은 데이터의 시간)으로 정리된다.

        //ch1, ch2 EEG 50Hz까지 딴다. [총 50Hz * 모은 데이터 시간 + 1(0Hz값도 포함)]
        EEG1_result = new double[maxEEG_HzCheck - minEEG_HzCheck + 1];
        EEG2_result = new double[maxEEG_HzCheck - minEEG_HzCheck + 1];

        //ch3 PPG 10Hz까지 딴다. [총 10Hz * 모은 데이터 시간] (0Hz값 포함 x)
        PPG_result = new double[maxPPG_HzCheck - minPPG_HzCheck];

        //EEG 기법 5가지 뇌파를 받는 배열을 생성한다.
        EEG1_PSum = new double[5];
        EEG2_PSum = new double[5];

        //EEG 기법 5가지 뇌파의 점유율을 받는 배열을 생성한다.
        EEG1_PAverage = new double[5];
        EEG2_PAverage = new double[5];

        //FFT 최대값을 갱신하는 배열 작성 (EEG는 최대값을 검출하지 않는다.
        //ch1PMax = new double[SlideCount];
        //ch2PMax = new double[SlideCount];
        ch3PMax = new double[SlideCount];

        for(int i=0; i<ch3PMax.Length; i++)
        {
            ch3PMax[i] = 0;
        }

        //FFT 최대값 가진 *위치*검색
        ch3PIndex = new double[SlideCount];


    }

    #endregion Unity Methods

    #region public Methods (ProcessFFT)
    [BurstCompile]
    public void ProcessFFT(double[] fft, int channel)
    {
        switch(channel)
        {
            case 1:
                PowerSpectrum(fft, 1);      //ch1 FFT 연산
                //할당 과정도 성능을 가져간다. 간소화한다.
                //ch1FFT_result = fft;
                //PowerSpectrum(ch1FFT_result, 1);
                break;

            case 2:
                PowerSpectrum(fft, 2);      //ch2 FFT 연산
                break;

            case 3:
                PowerSpectrum(fft, 3);      //ch3 FFT 연산
                break;
        }
    }

    #endregion public Methods

    #region private Methods

    //fft결과 파워 스펙트럼
    [BurstCompile]
    private void PowerSpectrum(double[] fft, int channel)
    {
        SlidePosition = bufferManager.slidePosition;
        ch3PIndex[SlidePosition] = 0;
        EEG1PowerSum = 0;        //power 총합변수 초기화
        EEG2PowerSum = 0;        //power 총합변수 초기화

        //채널별로 확인
        switch (channel)
        {
            //=============================================================================================
            case 1:     //ch1
                EEG1_PSum[0] = 0;
                EEG1_PSum[1] = 0;
                EEG1_PSum[2] = 0;
                EEG1_PSum[3] = 0;
                EEG1_PSum[4] = 0;


                for (int i = minEEG_HzCheck; i < maxEEG_HzCheck; i++)
                {
                    EEG1_result[i - minEEG_HzCheck] = (fft[i * 2] * fft[i * 2] + fft[i * 2 + 1] * fft[i * 2 + 1])
                        / ((256 * SlideCount) * (256 * SlideCount) * 2);

                    //뇌파 5단계 분류
                    if (i < minTheta_HzCheck)
                    {
                        EEG1_PSum[0] += EEG1_result[i - minEEG_HzCheck];
                    }
                    else if (i < minAlpha_HzCheck)
                    {
                        EEG1_PSum[1] += EEG1_result[i - minEEG_HzCheck];
                    }
                    else if (i < minBeta_HzCheck)
                    {
                        EEG1_PSum[2] += EEG1_result[i - minEEG_HzCheck];
                    }
                    else if (i < minGamma_HzCheck)
                    {
                        EEG1_PSum[3] += EEG1_result[i - minEEG_HzCheck];
                    }
                    else if (i < maxEEG_HzCheck)
                    {
                        EEG1_PSum[4] += EEG1_result[i - minEEG_HzCheck];
                    }

                }
                
                print("EEG1");
                EEG1PowerSum = EEG1_PSum[0] + EEG1_PSum[1] + EEG1_PSum[2]
                              + EEG1_PSum[3] + EEG1_PSum[4];

                EEG1_PAverage[0] = EEG1_PSum[0] / EEG1PowerSum * 100;
                EEG1_PAverage[1] = EEG1_PSum[1] / EEG1PowerSum * 100;
                EEG1_PAverage[2] = EEG1_PSum[2] / EEG1PowerSum * 100;
                EEG1_PAverage[3] = EEG1_PSum[3] / EEG1PowerSum * 100;
                EEG1_PAverage[4] = EEG1_PSum[4] / EEG1PowerSum * 100;

                deviceReader.ReadText("Theta : " + EEG1_PSum[0]
                                   + "\nDelta : " + EEG1_PSum[1]
                                   + "\nAlpha : " + EEG1_PSum[2]
                                   + "\nBeta  : " + EEG1_PSum[3]
                                   + "\nGamma : " + EEG1_PSum[4]
                                   , 10);

                deviceReader.CatchData(EEG1_PAverage[0]);

                deviceReader.ReadText("Theta : " + EEG1_PAverage[0].ToString("F2")
                                   + "\nDelta : " + EEG1_PAverage[1].ToString("F2")
                                   + "\nAlpha : " + EEG1_PAverage[2].ToString("F2")
                                   + "\nBeta  : " + EEG1_PAverage[3].ToString("F2")
                                   + "\nGamma : " + EEG1_PAverage[4].ToString("F2")
                                   , 11);

                //file.textPrint(EEG1_result, "EEG1.txt");
                file.textAppend(EEG1_PAverage, "EEG1.txt");
                //file.textAppend(EEG1_PAverage[0], "EEG1_delta.txt");
                //file.textAppend(EEG1_PAverage[1], "EEG1_theta.txt");
                //file.textAppend(EEG1_PAverage[2], "EEG1_alpha.txt");
                //file.textAppend(EEG1_PAverage[3], "EEG1_beta.txt");
                //file.textAppend(EEG1_PAverage[4], "EEG1_gamma.txt");
                break;

            //=============================================================================================
            case 2:     //ch2
                EEG2_PSum[0] = 0;
                EEG2_PSum[1] = 0;
                EEG2_PSum[2] = 0;
                EEG2_PSum[3] = 0;
                EEG2_PSum[4] = 0;

                for (int i = minEEG_HzCheck; i < maxEEG_HzCheck; i++)
                {
                    EEG2_result[i - minEEG_HzCheck] = (fft[i * 2] * fft[i * 2] + fft[i * 2 + 1] * fft[i * 2 + 1])
                        / ((256 * SlideCount) * (256 * SlideCount) * 2);

                    //뇌파 5단계 분류
                    if (i < minTheta_HzCheck)
                    {
                        EEG2_PSum[0] += EEG2_result[i - minEEG_HzCheck];
                    }
                    else if (i < minAlpha_HzCheck)
                    {
                        EEG2_PSum[1] += EEG2_result[i - minEEG_HzCheck];
                    }
                    else if (i < minBeta_HzCheck)
                    {
                        EEG2_PSum[2] += EEG2_result[i - minEEG_HzCheck];
                    }
                    else if (i < minGamma_HzCheck)
                    {
                        EEG2_PSum[3] += EEG2_result[i - minEEG_HzCheck];
                    }
                    else if (i < maxEEG_HzCheck)
                    {
                        EEG2_PSum[4] += EEG2_result[i - minEEG_HzCheck];
                    }
                }
                print("EEG2");
                EEG2PowerSum = EEG2_PSum[0] + EEG2_PSum[1] + EEG2_PSum[2]
                              + EEG2_PSum[3] + EEG2_PSum[4];

                EEG2_PAverage[0] = EEG2_PSum[0] / EEG2PowerSum * 100;
                EEG2_PAverage[1] = EEG2_PSum[1] / EEG2PowerSum * 100;
                EEG2_PAverage[2] = EEG2_PSum[2] / EEG2PowerSum * 100;
                EEG2_PAverage[3] = EEG2_PSum[3] / EEG2PowerSum * 100;
                EEG2_PAverage[4] = EEG2_PSum[4] / EEG2PowerSum * 100;

                deviceReader.ReadText("Theta : " + EEG2_PSum[0]
                                   + "\nDelta : " + EEG2_PSum[1]
                                   + "\nAlpha : " + EEG2_PSum[2]
                                   + "\nBeta  : " + EEG2_PSum[3]
                                   + "\nGamma : " + EEG2_PSum[4]
                                   , 20);

                deviceReader.ReadText("Theta : " + EEG2_PAverage[0].ToString("F2")
                                   + "\nDelta : " + EEG2_PAverage[1].ToString("F2")
                                   + "\nAlpha : " + EEG2_PAverage[2].ToString("F2")
                                   + "\nBeta  : " + EEG2_PAverage[3].ToString("F2")
                                   + "\nGamma : " + EEG2_PAverage[4].ToString("F2")
                                   , 21);

                //file.textPrint(EEG2_result, "EEG2.txt");
                file.textAppend(EEG2_PAverage, "EEG2.txt");
                //file.textAppend(EEG2_PAverage[0], "EEG2_delta.txt");
                //file.textAppend(EEG2_PAverage[1], "EEG2_theta.txt");
                //file.textAppend(EEG2_PAverage[2], "EEG2_alpha.txt");
                //file.textAppend(EEG2_PAverage[3], "EEG2_beta.txt");
                //file.textAppend(EEG2_PAverage[4], "EEG2_gamma.txt");
                break;

            //=============================================================================================
            case 3:     //ch3 PPG값 검출

                //검출하고자 하는 Hz의 최소에서 최대 사이에서 반복 돌림
                for (int i = minPPG_HzCheck; i < maxPPG_HzCheck; i++)
                {
                    //FFT를 돌린 최소 최대 사이에서 데이터 검출
                    //i minPPG_HzCheck에서 시작하는 상태라 그만큼 빼주고 실행해야 정상동작함
                    PPG_result[i - minPPG_HzCheck] = (fft[i * 2] * fft[i * 2] + fft[i * 2 + 1] * fft[i * 2 + 1])
                        / ((256 * SlideCount) * (256 * SlideCount) * 2);

                    //할당한 데이터와 현재 가진 데이터의 power를 비교한다.
                    if (PPG_result[i - minPPG_HzCheck] > ch3PMax[SlidePosition])
                    {
                        ch3PMax[SlidePosition] = (int)PPG_result[i - minPPG_HzCheck];

                        //최대값의 위치를 갱신 (인덱스 위치값)
                        ch3PIndex[SlidePosition] = (double)i;
                    }

                }

                //*초기화* index power값 초기화
                ch3PMax[SlidePosition] = 0;

                //BPM 세팅
                ch3PIndex[SlidePosition] = (double)(ch3PIndex[SlidePosition] * (1.0 / bufferManager.slideCounter)) * 60;

                //print(ch3PIndex[SlidePosition]);

                deviceReader.ReadText("BPM : " + ch3PIndex[SlidePosition], 30);

                //PPG FFT 검출배열과 Peak timing 계산
                file.textPrint(PPG_result, "PPGFFT.txt");
                file.textAppend(ch3PIndex[SlidePosition], "BPM.txt");

                break;
            //=============================================================================================
        }
    }

    private void ResetValues()
    {
        //변수값 초기화 하는 메서드
    }

    #endregion private Methods
}
