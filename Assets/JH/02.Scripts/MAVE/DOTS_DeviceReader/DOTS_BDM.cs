using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using InTheHand.Devices.Bluetooth;
using InTheHand.Devices.Bluetooth.Rfcomm;
using InTheHand.Devices.Enumeration;
using Unity.Burst;
using Unity.Entities;

namespace DOTS_BLE
{
    /// <summary>
    /// 장비찾는 메서드 컨텐츠 개발단계에서 Start 영역에 있는거 빼기
    /// </summary>
    [BurstCompile]
    public class DOTS_BDM : MonoBehaviour
    {
        #region Public Implementation

        public DOTS_DeviceReader deviceReader;
        public DOTS_BufferManager bufferManager;

        #endregion Public Implementation

        #region private Variables

        private DeviceInformation MAVE_Device;
        private Stream _Stream;

        #endregion private Variables


        #region Unity Methods

        // Start is called before the first frame update
        void Start()
        {
            //var picker = new DevicePicker();

            //var deviceInfo = picker.PickSingleDevice();

            //Debug.Log(deviceInfo.Id);
            //Debug.Log(deviceInfo.Name);

            //장비검색 메서드. 추후 이벤트로 분리
            FindDeviceStream();

            if (_Stream != null)
            {
                Debug.Log("stream successfully found");
            }
            else
            {
                Debug.LogError("Cant find the device stream.");
                Destroy(gameObject);
            }
            
        }

        [BurstCompile]
        // Update is called once per frame
        void Update()
        {
            if(_Stream != null)
            {
                DeviceRead(_Stream);
            }
        }

        #endregion Unity Methods

        #region Methods

        #region Public : FindDeviceStream

        /// <summary>
        /// Pub 1 : 장비검색, 스트림 따기
        /// </summary>
        [BurstCompile]
        public void FindDeviceStream()
        {
            //장비검색(FindAll)
            var deviceList = FindAll(RfcommServiceId.SerialPort);

            for (int i = 0; i < deviceList.Length; i++)
            {
                //Debug.Log(deviceList[i].Name);
                if(deviceList[i].Name == "MAVE_0011")
                {
                    MAVE_Device = deviceList[i];
                    deviceReader.CatchData(MAVE_Device.Name, 100);  //DeviceReader로 장비명 갱신
                    break;
                }
                else if (deviceList[i].Name == "Meddiction_0022")
                {
                    MAVE_Device = deviceList[i];
                    deviceReader.CatchData(MAVE_Device.Name, 100);  //DeviceReader로 장비명 갱신
                    break;
                }

            }

            //장비 못찾음
            if(MAVE_Device == null) { throw new InvalidDataException("Fail to retrive information - is the device turned on?"); }

            //장비 받으면 OpenBluetoothStream
            _Stream = OpenBluetoothStream(MAVE_Device, RfcommServiceId.SerialPort.Uuid);

            //스트림 상태 받기
            deviceReader.CatchData(_Stream.CanRead, 101);

            //스트림 못받음
            if (_Stream == null) { throw new InvalidDataException("Failed to open stream = required service does not exist"); }
        }

        /// <summary>
        /// Pri 1-1 : 서비스에 관계된 장비 Id 다 갖고옴
        /// </summary>
        [BurstCompile]
        private DeviceInformation[] FindAll(RfcommServiceId serviceId)
        {
            return DeviceInformation.FindAll(RfcommDeviceService.GetDeviceSelector(serviceId)).ToArray();
        }

        /// <summary>
        /// Pri 1-2 : Bluetooth device picker를 열고 시리얼 포트의 서비스에서 스트림을 여는 것을 시도
        /// </summary>
        private static Stream OpenBluetoothStream(DeviceInformation deviceInformation, RfcommServiceId serviceId)
        {
            //서비스 받아오기
            var device = BluetoothDevice.FromDeviceInformation(deviceInformation);
            var result = device.GetRfcommServices(BluetoothCacheMode.Cached);
            var services = result.Services;

            //요청한 서비스를 찾고 접속합니다.
            for (int i = 0; i < services.Count; ++i)
            {
                var current = services[i];
                if (current.ServiceId == serviceId)
                {
                    return current.OpenStream();
                }
            }

            return null;
        }

        #endregion FindDeviceStream

        #region Update : DeviceRead

        /// <summary>
        /// Pri Update : 장비의 스트림에서 10바이트의 데이터를 읽어옵니다.
        /// </summary>
        [BurstCompile]
        private void DeviceRead(Stream stream)
        {
            if (_Stream == null) { throw new InvalidDataException("Cannot read to null stream"); }

            //장비에서 변수값만큼 데이터를 읽어옵니다.
            var buffer = new byte[10];
            int read = _Stream.Read(buffer, 0, 10);
            if (read != 0)
            {
                //Debug.Log("12");
                //동기화가 된 버퍼일 경우 버퍼읽기로 넘어감
                if (buffer[0] == 165 && buffer[9] == 90)
                {
                    //Debug.Log("13");
                    bufferManager.BufferRead(buffer);
                }
                //동기화가 안됐을 경우 동기화를 위해 버퍼 1byte씩 넘김
                //조건넣어서 바로 맞추려는 것보다 1byte씩 동기화용 버퍼읽기가 효과적임
                else
                {
                    //Debug.Log("14");
                    print(_Stream.Read(buffer, 0, 1).ToString());
                }
            }

        }

        #endregion DeviceRead

        #region Public : shootData

        /// <summary>
        /// DevReader : 장비명, 스트림 보냄
        /// </summary>
        /// <param name="assignPath"></param>
        public void shootData(int assignPath)
        {
            switch(assignPath)
            {
                case 100:       //장비명 받기(deviceReader에서 호출용)
                    deviceReader.CatchData(MAVE_Device.Name, 100);  //DeviceReader로 장비명 갱신
                    break;

                case 101:       //스트림 상태 받기(deviceReader 호출용)
                    deviceReader.CatchData(_Stream.CanRead, 101);   //DeviceReader로 스트림 상태 갱신
                    break;
            }
        }

        #endregion shootData

        #endregion Methods
    }

}
