using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InTheHand.Devices.Bluetooth;
using InTheHand.Devices.Bluetooth.Rfcomm;
using InTheHand.Devices.Enumeration;
using Unity.Burst;

public class BluetoothDeviceManager : MonoBehaviour
{
    #region Public Variables

    public DeviceReader deviceReader;       //데이터 쓰는 클래스
    public BufferManager bufferManager;     //버퍼 쓰는 클래스

    #endregion Public Variables

    #region Private Variables

    private DeviceInformation MAVE_device;
    //데이터 스트림을 받는 변수
    private Stream _Stream;

    #endregion Private Variables

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        var DeviceList = FindAll(RfcommServiceId.SerialPort);

        for(int i=0; i<DeviceList.Length; i++)
        {
            if (DeviceList[i].Name == "Meddiction_0022")
            {
                MAVE_device = DeviceList[i];
                break;
            }
        }
        

        //장비 정보 갖고오기
        //var deviceInformation = PickDevice();
        //var deviceInformation = FindAll(RfcommServiceId.SerialPort)[1];
        if (MAVE_device == null) { throw new InvalidDataException("Fail to retrive information - is the device turned on?"); }

        deviceReader.ReadText("device Id       : \n" + MAVE_device.Id
                           + "\ndevice Name     : \n" + MAVE_device.Name
                           + "\ndevice IsPaired : " + MAVE_device.Pairing.IsPaired
                           + "\ndevice CanPair  : " + MAVE_device.Pairing.CanPair
                           , 0);
        //print("device Id       : " + MAVE_device.Id);
        //print("device Name     : " + MAVE_device.Name);
        //print("device IsPaired : " + MAVE_device.Pairing.IsPaired);
        //print("device CanPair  : " + MAVE_device.Pairing.CanPair);

        //장비의 스트림 갖고오기
        _Stream = OpenBluetoothStream(MAVE_device, RfcommServiceId.SerialPort.Uuid);
        if(_Stream == null) { throw new InvalidDataException("Failed to open stream = required service does not exist"); }

    }

    // Update is called once per frame
    void Update()
    {
        if (_Stream != null)
        {
            DeviceRead(_Stream);
        }
    }

    #endregion Unity Methods

    #region Private Methods

    /// <summary>
    /// 장비 정보 갖고오기
    /// </summary>
    private static DeviceInformation PickDevice()
    {
        //BluetoothAdapter a;
        //BluetoothClassOfDevice b;
        

        var picker = new DevicePicker();
        
        
        var deviceInfo = picker.PickSingleDevice(); //picker.PickSingleDevice();

        //b = a.ClassOfDevice;
        return deviceInfo;
    }

    /// <summary>
    /// Bluetooth device picker를 열고 시리얼 포트의 서비스에서 스트림을 여는 것을 시도합니다.
    /// </summary>
    private static Stream OpenBluetoothStream(DeviceInformation deviceInformation, RfcommServiceId serviceId)
    {
        //서비스 받아오기
        var device = BluetoothDevice.FromDeviceInformation(deviceInformation);
        var result = device.GetRfcommServices(BluetoothCacheMode.Cached);
        var services = result.Services;

        //print(serviceId);   

        //요청한 서비스를 찾고 접속합니다.
        for (int i=0; i<services.Count; ++i)
        {
            print(services[0].ServiceId.Uuid);
            print(services[1].ServiceId.Uuid);
            print(services.Count);

            var current = services[i];
            if(current.ServiceId == serviceId)
            {
                return current.OpenStream();
            }
        }

        return null;
    }

    /// <summary>
    /// finds all devices that support specified service id
    /// </summary>
    private DeviceInformation[] FindAll(RfcommServiceId serviceId)
    {
        return DeviceInformation.FindAll(RfcommDeviceService.GetDeviceSelector(serviceId)).ToArray();
    }
    
    /// <summary>
    /// 장비의 스트림에서 10바이트의 데이터를 읽어옵니다.
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
            //동기화가 된 버퍼일 경우 버퍼읽기로 넘어감
            if(buffer[0] == 165 && buffer[9] == 90)
            {
                bufferManager.BufferRead(buffer);
            }
            //동기화가 안됐을 경우 동기화를 위해 버퍼 1byte씩 넘김
            //조건넣어서 바로 맞추려는 것보다 1byte씩 동기화용 버퍼읽기가 효과적임
            else
            {
                print(_Stream.Read(buffer, 0, 1).ToString());
            }
        }
    }
    
    #endregion Private Methods
}
