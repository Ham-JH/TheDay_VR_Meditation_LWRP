using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueTest : MonoBehaviour
{
    private Queue<double> queue;

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<double>();

        for(int i=0; i< 100; i++)
        {
            queue.Enqueue(i);
        }

        print(queue.Count);
        print(queue.Dequeue());
        print(queue.Count);

        for (int i = 0 ; i < 10 ; i++)
        {
            print(queue.Dequeue());
        }

        print(queue.Count);

        double[] x = new double[queue.Count];

        x = queue.ToArray();
        //queue.CopyTo(x, queue.Count-1);

        print("index : " + x.Length);

        for(int i=0; i<x.Length; i++)
        {
            //print("x");
            print("x : " + x[i]);
        }

        //Enqueue : 데이터구조의 뒤에 데이터 입력
        //Dequeue : 데이터구조의 앞의 데이터 뺌
        //Count : 데이터구조의 전체 요소 개수 구하기
        //ToArray : 데이터 구조 복사
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
