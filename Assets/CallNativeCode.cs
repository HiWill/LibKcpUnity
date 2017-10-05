using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class CallNativeCode : MonoBehaviour
{ 

#if !UNITY_EDITOR && UNITY_IOS
	[DllImport("__Internal")]
	private static extern bool _InitKcp();

    [DllImport("__Internal")]
    private unsafe static extern void _SendData(ref byte buff,int size);

    [DllImport("__Internal")]
    private unsafe static extern int _ReceiveData(ref byte buff, int size);

    [DllImport("__Internal")]
    private static extern int _Destroy();

#else
	[DllImport("libKcp")]
	private static extern bool _InitKcp(string ip, short port);

	[DllImport("libKcp")]
	private unsafe static extern void _SendData(ref byte buff, int size);

	[DllImport("libKcp")]
	private unsafe static extern int _ReceiveData(ref byte buff, int size);

	[DllImport("libKcp")]
	private static extern int _Destroy();
#endif



    byte[] buffer = new byte[1024];

    public UnityEngine.UI.Text debugLabel;

    int index;
    int rCount;

    private void Awake()
    {
        debugLabel.text = "Awake\n";
        if(_InitKcp("35.187.211.201",5050))
        {
            StartCoroutine(SendTest());
            StartCoroutine(ReceiveTest());
        }    
    }

    IEnumerator ReceiveTest()
    {
        while(rCount<10)
        {
			Receive();
			yield return null; 
        } 
    }

    IEnumerator SendTest()
    {
        while(index<10)
        {
            Send();
            index++;
            yield return null;
        }
    }

    unsafe void Receive()
    {
        int count=_ReceiveData(ref buffer[0],buffer.Length);
        if(count>0)
        {
            string result= System.Text.Encoding.Default.GetString(buffer,0,count);
            debugLabel.text += result + "\n";
            rCount++;
        }
    }

    unsafe void Send()
    {
        string str = "hello"+index;
        byte[] ar = System.Text.Encoding.UTF8.GetBytes(str);
        _SendData(ref ar[0],ar.Length);
    } 
}
