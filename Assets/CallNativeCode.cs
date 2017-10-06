using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class CallNativeCode : MonoBehaviour
{

#if !UNITY_EDITOR && UNITY_IOS
	[DllImport("__Internal")]
#else
	[DllImport("libKcp")]
#endif
	private static extern bool _InitKcp(string ip, short port,int mtu, int wndSize , int dataShards, int parityShards,bool useStreamMode);

#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")]
#else
	[DllImport("libKcp")]
#endif
	private unsafe static extern void _SendData(ref byte buff, int size);

#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")]
#else
	[DllImport("libKcp")]
#endif
	private unsafe static extern int _ReceiveData(ref byte buff, int size);

#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")]
#else
	[DllImport("libKcp")]
#endif
	private static extern void _Update();

#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")]
#else
	[DllImport("libKcp")]
#endif
	private static extern void _Destroy(); 
	




    byte[] buffer = new byte[1024];

    public UnityEngine.UI.Text debugLabel;

    int index;
    int rCount;

    private void Awake()
    {
        debugLabel.text = "Awake\n";
        if(_InitKcp("127.0.0.1", 5050, 1400, 128, 0, 0, false))
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
            _Update();
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
