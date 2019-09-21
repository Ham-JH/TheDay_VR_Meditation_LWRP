using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FFTWSharp;
using System.Runtime.InteropServices;
using Unity.Burst;

public class FFT_Op : MonoBehaviour
{
    #region public implementation

    public FFTProcess fftProcess;       //FFT처리결과 자료처리

    #endregion public implementation

    // ex) FFT 사용법
    
    // Start is called before the first frame update
    //void Start()
    //{
    //    //====================================================

    //    // Test 1: 실수 입력
    //    // 배정도 수들로 변수를 생성한다.
    //    // Define an array of double-precision numbers
    //    double[] x = { 11.8, 11.4, 9.6, 11.8 };

    //    // Compute the FFT
    //    var dft = FFT(x, true); //true = real input
    //    //Format and display the results of the FFT
    //    Debug.Log("Test 1 : Real input");
    //    Debug.Log(" ");
    //    Debug.Log("FFT =");
    //    DisplayComplex(dft);
    //    Debug.Log(" ");

    //    // Compute the IFFT
    //    var idft = IFFT(dft);
    //    // format and display th results of the IFFT
    //    Debug.Log("IFFT =");
    //    DisplayReal(idft);
    //    Debug.Log(" ");

    //    //====================================================

    //    //====================================================

    //    // Test 2 : Complex input
    //    x = new double[] { 1, -2, 3, 4, 5, -6, 7, 8 };  //that is, 1 -2i, . . . , 7 + 8i

    //    dft = FFT(x, false);        //false = complex input
    //    Debug.Log("Test 2 : Complex input");
    //    Debug.Log(" ");
    //    Debug.Log("FFT =");
    //    DisplayComplex(dft);
    //    Debug.Log(" ");

    //    idft = IFFT(dft);
    //    // format and display th results of the IFFT
    //    Debug.Log("IFFT =");
    //    DisplayComplex(idft);
    //    // Prevent the console window from closing immediately
    //    Console.ReadKey();

    //    //====================================================
    //}

    // Update is called once per frame

    #region FFT, IFFT

    /// <summary>
    /// 고속 푸리에 변환을 실수나 복소수의 1차 배열로 계산한다.
    /// </summary>
    /// <param name="data">입력값.</param>
    /// <param name="real">실수나 복소수 입력 상태.</param>
    /// <returns>FFT 값을 변환한다.</returns>
    [BurstCompile]
    public void FFT(double[] data, bool real, int channel)
    {
        if (real)   data = ToComplex(data);     // 입력값이 실수일 경우 복소수로 바꿔준다.
        int n = data.Length;                    // 배열의 길이를 받아온다.

        /* 할당되지 않은 메모리 블록에 입출력 데이터를 할당한다.
         * (입력과 출력은 이 경우에 같은 길이를 가지고, 그래서 한 메모리 블록만 있으면 된다. */
        IntPtr ptr = fftw.malloc(n * sizeof(double)); // or: n * sizeof(double) 로 할당해주면 된다.
        
        // 할당되지 않은 메모리 블록에 입력 데이터를 전달한다.
        Marshal.Copy(data, 0, ptr, n);
        
        // FFT를 계획하고 실행한다. (n/2는 복소수가 인덱스 2배수 위치에 저장되어 있기 때문에 쓴 소리다.
        IntPtr plan = fftw.dft_1d(n / 2, ptr, ptr, fftw_direction.Forward, fftw_flags.Estimate);
        fftw.execute(plan);
        
        // 출력 변수를 저장하기 위한 배열을 생성한다.
        var fft = new double[n];
        
        // 관리되지 않은 출력값을 할당되지 않은 변수에 전달한다.
        Marshal.Copy(ptr, fft, 0, n);
        
        // 사용 후의 변수들을 정리한다.
        fftw.destroy_plan(plan);
        fftw.free(ptr);
        fftw.cleanup();

        // FFT 출력값을 처리하는 메서드를 호출한다.
        fftProcess.ProcessFFT(fft, channel);
    }

    /// <summary>
    /// 역 푸리에 변환을 복소수의 1차 배열로 계산한다.
    /// </summary>
    /// <param name="data">입력값.</param>
    /// <returns>정규화된 IFFT값을 반환한다.</returns>
    [BurstCompile]
    static double[] IFFT(double[] data)
    {
        // 배열의 길이를 구한다.
        int n = data.Length;

        /* 할당되지 않은 메모리 블록에 입출력 데이터를 할당한다.
         * (입력과 출력은 이 경우에 같은 길이를 가지고, 그래서 한 메모리 블록만 있으면 된다. */
        IntPtr ptr = fftw.malloc(n * sizeof(double)); // or: n * 8 로 할당해주면 된다.

        // 할당되지 않은 메모리 블록에 입력 데이터를 전달한다.
        Marshal.Copy(data, 0, ptr, n);

        // IFFT(역고속 푸리에 변환)을 계획하고 실행한다. (n/2는 복소수가 인덱스 2배수 위치에 저장되어 있기 때문에 쓴 소리다.
        IntPtr plan = fftw.dft_1d(n / 2, ptr, ptr, fftw_direction.Backward, fftw_flags.Estimate);
        fftw.execute(plan);
        
        //출력 변수를 저장하기 위한 배열을 생성한다.
        var ifft = new double[n];

        // 관리되지 않은 출력값을 할당되지 않은 변수에 전달한다.
        Marshal.Copy(ptr, ifft, 0, n);
        
        // Do some cleaning
        fftw.destroy_plan(plan);
        fftw.free(ptr);
        fftw.cleanup();
        
        //출력값의 크기를 출력한다.
        //Scale the output values
        for(int i=0, nh = n/2; i<n; i++)
        {
            ifft[i] /= nh; 
        }
        
        // IFFT 출력값을 변환한다.
        return ifft;
    }

    #endregion FFT, IFFT

    #region inside static Methods

    /// <summary>
    /// 표시된 복소수들을 FFTW 변환에 맞추기 위해 배열에 0을 섞는다.
    /// </summary>
    /// <param name="real">실수가 들어간 배열.</param>
    /// <returns>복소수 배열을 반환한다.</returns>
    [BurstCompile]
    static double[] ToComplex(double[] real)
    {
        int n = real.Length;
        var comp = new double[n * 2];
        for(int i=0; i<n; i++)
        {
            comp[2 * i] = real[i];
        }
        return comp;
    }

    /// <summary>
    /// 복소수 값들을 a +/- bi 형식으로 표시한다.
    /// </summary>
    /// <param name="x">복소수 배열 값이다.</param>
    //[BurstCompile]
    static void DisplayComplex(double[] x)
    {
        if (x.Length % 2 != 0)
        {
            throw new Exception("The number of elements must be even.");
        }
        for (int i = 0, n = x.Length; i < n; i += 2)
        {
            if (x[i + 1] < 0)
            {
                Debug.Log(string.Format("{0} - {1}i", x[i], Math.Abs(x[i + 1])));
            }
            else
            {
                Debug.Log(string.Format("{0} + {1}i", x[i], x[i + 1]));
            }
        }
    }

    /// <summary>
    /// 복소수의 실수 영역을 표시한다.
    /// </summary>
    /// <param name="x">복소수 배열 값이다.</param>
    //[BurstCompile]
    static void DisplayReal(double[] x)
    {
        if (x.Length % 2 != 0)
        {
            throw new Exception("The number of elements must be even.");
        }
        for (int i = 0, n = x.Length; i < n; i += 2)
        {
            Debug.Log(x[i]);
        }
    }

    #endregion inside static Methods
}
