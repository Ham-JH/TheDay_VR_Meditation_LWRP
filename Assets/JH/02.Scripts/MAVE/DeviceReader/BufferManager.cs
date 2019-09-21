using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Burst;

[Serializable]
public class Slide
{
    #region Raw data Area

    /// <summary>
    /// RawData input struct
    /// 장비의 정보가 1개의 버퍼를 기준으로 아래의 변수에 각각 할당된다.
    /// </summary>
    public struct inBuffer
    {
        public byte counter;
        public int ch1;
        public int ch2;
        public int ch3;
        public int ch4;
    };

    /// <summary>
    /// 1개의 Slider 객체가 256개의 카운터를 가진다.
    /// 256개의 카운터는 10byte의 버퍼를 입력받는다.
    /// 10byte의 버퍼는 의도에 맞게 inBuffer의 각 변수에 할당된다.
    /// </summary>
    public inBuffer[] counter;

    #endregion Raw data Area

    #region Slide Constructor (Slide instance has counter)
    public Slide()
    {
        //인덱스는 카운터를 기준으로 들어갈 버퍼를 의미한다.
        counter = new inBuffer[256];
    }
    #endregion Slide Constructor
}

public class BufferManager : MonoBehaviour
{
    #region Public implementation

    public DeviceReader deviceReader;       //데이터 출력
    public FFT_Op fft;                      //FFT 처리하는 클래스
    public Files fileWriter;                //파일 쓰는 클래스

    #endregion Public implementation

    #region Buffer Control

    //세 채널의 queue 선언. 뇌파에 2채널, 맥파에 1채널 할당한다.
    private Queue<double> ch1_queue;
    private Queue<double> ch2_queue;
    private Queue<double> ch3_queue;

    public int slideCounter;        //슬라이드 개수
    private Slide[] slide;          //슬라이드 배열

    //Peak Data 찾는 배열
    //private double

    private byte checkCounter;      //카운터가 변하는 것을 받는 변수. 0번 카운터를 받으면 SlidePosition의 값이 바뀐다.
    [HideInInspector]
    public int slidePosition;      //현재 쓰이는 슬라이드의 위치를 확인한다.
    private int slideSwitchCount;   //슬라이드를 바꾼 회수를 입력한다.

    #endregion Buffer Control

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        //==================================================================

        //세 채널의 큐 생성
        ch1_queue = new Queue<double>();
        ch2_queue = new Queue<double>();
        ch3_queue = new Queue<double>();

        //==================================================================
        //슬라이드 구성

        //환경설정에 사용해야될 변수 1 : 슬라이드 개수
        slideCounter = 16;
        
        // slideCounter만큼 Slide 객체 배열 생성
        slide = new Slide[slideCounter];

        for(int i=0; i<slide.Length; i++)
        {
            //슬라이드 인스턴스 생성
            slide[i] = new Slide();
        }

        //==================================================================

        checkCounter = 0;       //받아온 버퍼의 카운터 값을 입력받는 변수
        slidePosition = 0;      //현재 슬라이드 위치를 가진 변수
        slideSwitchCount = 0;   //슬라이드가 변경된 회수를 가진 변수

        //==================================================================

        //print("Slide count : " + slide.Length);
        //print("counter count : " + slide[0].counter.Length);
        //print("buffer count : " + slide[0].counter[0]);
    }

    #endregion Unity Methods

    #region BufferRead
    /// <summary>
    /// 버퍼 분류 메서드 실행(비동기화된 블루투스 값은 시작 싱크값이 0번 인덱스 값에 맞춰서 들어오지 않는다.)
    /// </summary>
    [BurstCompile]
    public void BufferRead(byte[] _buffer)
    {
        /*
         * 넘어온 버퍼는 동기화가 된 배열이므로 동기화에 관련된 연산을 할 필요가 없다.
         */

        //==================================================================

        //버퍼에서 카운터 값을 받아온다.
        checkCounter = _buffer[1];

        //카운터가 0으로 돌아오게 되었을 경우 슬라이드를 변경해준다.
        if(checkCounter == 0)
        {
            slidePosition = (slidePosition + 1) % slideCounter;     //사용할 슬라이드의 인덱스를 바꾼다.
            slideSwitchCount++;                                     //슬라이드를 바꾼 회수를 증가시킨다.

            deviceReader.ReadText("slideCount : " + slideCounter
                               +"\nslidePosition : " + slidePosition
                               , 2);

            deviceReader.ReadText("1", 99);

            Debug.Log("카운터 0 : 슬라이드 " + slidePosition +"으로 변경 " + slideSwitchCount + "회째 변경");
        }
        else if(checkCounter == 1)      //counter 1일 경우 : ch1, 2데이터 연산
        {
            //slide 개수보다 slide 변환회수가 클때 연산
            if(slideSwitchCount > slideCounter)
            {
                //이 시점에서 queue의 크기는 256 * slideCounter 만큼의 크기를 가진 상태
                fft.FFT(ch1_queue.ToArray(), true, 1);      //queue 내용 배열로 변환, 실수 true, 1채널
                fft.FFT(ch2_queue.ToArray(), true, 2);      //queue 내용 배열로 변환, 실수 true, 2채널

                fileWriter.textPrint(ch1_queue.ToArray(), "ch1RawData.txt");
                fileWriter.textPrint(ch2_queue.ToArray(), "ch2RawData.txt");
            }
        }
        else if(checkCounter == 2)      //counter 2일 경우 : ch3데이터 연산
        {
            //slide 개수보다 slide 변환회수가 클때 연산
            if (slideSwitchCount > slideCounter)
            {
                fft.FFT(ch3_queue.ToArray(), true, 3);      //queue 내용 배열로 변환, 실수 true, 3채널

                //fileWriter.textPrint(ch3_queue.ToArray(), "ch3RawData.txt");
            }
        }

        //==================================================================
        //RawData 가공단계
        //현재 사용 슬라이드 내 현재 카운터 내 변수 입력

        //counter   버퍼 카운터
        //ch1, 2    뇌파
        //ch3       맥파
        //ch4       배터리
        slide[slidePosition].counter[_buffer[1]].counter = _buffer[1];
        slide[slidePosition].counter[_buffer[1]].ch1 = (((int)(_buffer[2] & 0xFF) << 8) | (_buffer[3] & 0xFF));
        slide[slidePosition].counter[_buffer[1]].ch2 = (((int)(_buffer[4] & 0xFF) << 8) | (_buffer[5] & 0xFF));
        slide[slidePosition].counter[_buffer[1]].ch3 = (((int)(_buffer[6] & 0xFF) << 8) | (_buffer[7] & 0xFF)) * 20;
        slide[slidePosition].counter[_buffer[1]].ch4 = ((int)(_buffer[8] & 0xFF));

        //OpenBCI 신호 조정 단계(뇌파 ch1, ch2)
        slide[slidePosition].counter[_buffer[1]].ch1 -= 2048;
        slide[slidePosition].counter[_buffer[1]].ch2 -= 2048;

        slide[slidePosition].counter[_buffer[1]].ch1 *= 20;
        slide[slidePosition].counter[_buffer[1]].ch2 *= 20;

        //=============================================================
        //저장용 Queue 갱신단계

        //최소수집 Slide 개수 * Counter보다 많은 데이터를 얻기 시작한 경우
        if (ch1_queue.Count >= 256 * slideCounter)
        {
            ch1_queue.Dequeue();
            ch2_queue.Dequeue();
            ch3_queue.Dequeue();

            ch1_queue.Enqueue(slide[slidePosition].counter[_buffer[1]].ch1);
            ch2_queue.Enqueue(slide[slidePosition].counter[_buffer[1]].ch2);
            ch3_queue.Enqueue(slide[slidePosition].counter[_buffer[1]].ch3);
        }
        //최소수집 Slide 개수 * Counter보다 적은 데이터를 얻기 시작한 경우
        else if (ch1_queue.Count < 256 * slideCounter)
        {
            ch1_queue.Enqueue(slide[slidePosition].counter[_buffer[1]].ch1);
            ch2_queue.Enqueue(slide[slidePosition].counter[_buffer[1]].ch2);
            ch3_queue.Enqueue(slide[slidePosition].counter[_buffer[1]].ch3);
        }

        //==================================================================

        //화면에 Raw Data 출력
        deviceReader.ReadText("counter : " + slide[slidePosition].counter[_buffer[1]].counter
                       + "\nch1 : " + slide[slidePosition].counter[_buffer[1]].ch1
                       + "\nch2 : " + slide[slidePosition].counter[_buffer[1]].ch2
                       + "\nch3 : " + slide[slidePosition].counter[_buffer[1]].ch3
                       + "\nch4 : " + slide[slidePosition].counter[_buffer[1]].ch4
                       , 1);

        deviceReader.ReadText("bufferLength(ch1) : " + ch1_queue.Count, 3);
    }
    #endregion BufferRead
}
