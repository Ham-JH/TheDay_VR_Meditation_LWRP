using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
//using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;

/// <summary>
/// RawData 출력여부 고려
/// </summary>
namespace DOTS_BLE
{
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

    //[BurstCompile]
    public class DOTS_BufferManager : MonoBehaviour
    {
        #region Public implementation

        public DOTS_DeviceReader deviceReader;       //데이터 출력
        public DOTS_FFT_Op fft;                      //FFT 처리하는 클래스
        //public Files fileWriter;                //파일 쓰는 클래스

        #endregion Public implementation

        #region Buffer Control

        //세 채널의 queue 선언. 뇌파에 2채널, 맥파에 1채널 할당한다.
        public Queue<double> ch1_queue;
        public Queue<double> ch2_queue;
        public Queue<double> ch3_queue;

        [HideInInspector]
        public int slideCounter;        //슬라이드 개수
        private Slide[] slide;          //슬라이드 배열

        private byte checkCounter;      //카운터가 변하는 것을 받는 변수. 0번 카운터를 받으면 SlidePosition의 값이 바뀐다.
        [HideInInspector]
        public int slidePosition;      //현재 쓰이는 슬라이드의 위치를 확인한다.
        private int slideSwitchCount;   //슬라이드를 바꾼 회수를 입력한다.

        #endregion Buffer Control

        #region Unity Methods : Initializing instance

        //슬라이드 초기값 할당영역
        private void Awake()
        {
            slideCounter = 16;
        }

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
            //slideCounter = 16;

            // slideCounter만큼 Slide 객체 배열 생성
            slide = new Slide[slideCounter];

            for (int i = 0; i < slide.Length; i++)
            {
                slide[i] = new Slide();     //슬라이드 인스턴스 생성
            }

            fft.Set_FFTEnv(slideCounter);

            //==================================================================

            checkCounter = 0;       //받아온 버퍼의 카운터 값을 입력받는 변수
            slidePosition = 0;      //현재 슬라이드 위치를 가진 변수
            slideSwitchCount = 0;   //슬라이드가 변경된 회수를 가진 변수

            //==================================================================
        }

        #endregion Unity Methods

        #region BufferRead

        [BurstCompile]
        public void BufferRead(byte[] _buffer)
        {

            NativeArray<int> resultBuffer = new NativeArray<int>(5, Allocator.Persistent);
            NativeArray<byte> BufferArray = new NativeArray<byte>(10, Allocator.Persistent);

            BufferArray[1] = _buffer[1];
            BufferArray[2] = _buffer[2];
            BufferArray[3] = _buffer[3];
            BufferArray[4] = _buffer[4];
            BufferArray[5] = _buffer[5];
            BufferArray[6] = _buffer[6];
            BufferArray[7] = _buffer[7];
            BufferArray[8] = _buffer[8];

            checkCounter = _buffer[1];

            //버퍼 카운터에 따라 연산변경
            switch (checkCounter)
            {
                case 0:
                    slidePosition = (slidePosition + 1) % slideCounter;     //사용할 슬라이드의 인덱스를 바꾼다.
                    slideSwitchCount++;                                     //슬라이드를 바꾼 회수를 증가시킨다.

                    //deviceReader.ReadText("slideCount : " + slideCounter
                    //                   + "\nslidePosition : " + slidePosition
                    //                   , 2);

                    //deviceReader.ReadText("1", 99);

                    Debug.Log("카운터 0 : 슬라이드 " + slidePosition + "으로 변경 " + slideSwitchCount + "회째 변경");
                    break;

                case 1:
                    //slide 개수보다 slide 변환회수가 클때 연산
                    if (slideSwitchCount > slideCounter)
                    {
                        //이 시점에서 queue의 크기는 256 * slideCounter 만큼의 크기를 가진 상태
                        fft.FFT(ch1_queue.ToArray(), true, 1);      //queue 내용 배열로 변환, 실수 true, 1채널

                        //fileWriter.textPrint(ch1_queue.ToArray(), "ch1RawData.txt");
                    }
                    break;

                case 2:
                    //slide 개수보다 slide 변환회수가 클때 연산
                    if (slideSwitchCount > slideCounter)
                    {
                        fft.FFT(ch2_queue.ToArray(), true, 2);      //queue 내용 배열로 변환, 실수 true, 2채널

                        //fileWriter.textPrint(ch2_queue.ToArray(), "ch2RawData.txt");
                    }
                    break;

                case 3:
                    //slide 개수보다 slide 변환회수가 클때 연산
                    if (slideSwitchCount > slideCounter)
                    {
                        fft.FFT(ch3_queue.ToArray(), true, 3);      //queue 내용 배열로 변환, 실수 true, 3채널

                        //fileWriter.textPrint(ch3_queue.ToArray(), "ch3RawData.txt");
                    }
                    break;
            }

            //버퍼 할당 Job 생성
            ChannelOperater channelOperator = new ChannelOperater
            {
                //결과 버퍼 할당
                operateBuffer = resultBuffer,
                //버퍼 연결
                buffer = BufferArray
            };


            JobHandle jobHandle = channelOperator.Schedule(); //Job List에 적재
            jobHandle.Complete();

            BufferArray.Dispose();

            slide[slidePosition].counter[_buffer[1]].counter = _buffer[1];
            slide[slidePosition].counter[_buffer[1]].ch1 = channelOperator.operateBuffer[1];
            slide[slidePosition].counter[_buffer[1]].ch2 = channelOperator.operateBuffer[2];
            slide[slidePosition].counter[_buffer[1]].ch3 = channelOperator.operateBuffer[3];
            slide[slidePosition].counter[_buffer[1]].ch4 = _buffer[8];

            resultBuffer.Dispose();

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


        }

        #endregion BufferRead
    }

    #region Struct : ChannelOperator Job

    /// <summary>
    /// 버퍼를 원하는 데이터로 변환하는 Single Job
    /// </summary>
    [BurstCompile]
    public struct ChannelOperater : IJob
    {
        public NativeArray<int> operateBuffer;
        public NativeArray<byte> buffer;

        public void Execute()
        {
            operateBuffer[0] = buffer[1];
            operateBuffer[1] = (((int)(buffer[2] & 0xFF) << 8) | (buffer[3] & 0xFF));
            operateBuffer[2] = (((int)(buffer[4] & 0xFF) << 8) | (buffer[5] & 0xFF));
            operateBuffer[3] = (((int)(buffer[6] & 0xFF) << 8) | (buffer[7] & 0xFF)) * 20;
            operateBuffer[4] = buffer[8];

            operateBuffer[1] -= 2048;
            operateBuffer[2] -= 2048;

            operateBuffer[1] *= 20;
            operateBuffer[2] *= 20;
        }
    }

    #endregion ChannelOperator

}
