using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class CallNativeCode : MonoBehaviour
{
    #region native_bind

#if !UNITY_EDITOR && UNITY_IOS
	[DllImport("__Internal")]
#else
    [DllImport("libKcp")]
#endif
    private static extern bool _InitKcp(string ip, short port, int mtu, int wndSize, int dataShards, int parityShards, bool useStreamMode);

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

	#endregion //end native_bind



	#region wrapper

	/// <summary>
    /// The udpsocket: c# version of the kcp , work for windows, 
    /// added this because native plugin only work on ios/osx/android
    /// do not support fec
	/// </summary>
	KcpProject.v2.UdpSocket udpsocket;//

    public bool InitKcp(string ip, short port)
    {
#if UNITY_EDITOR && !UNITY_EDITOR_OSX
        udpsocket = new KcpProject.v2.UdpSocket();
        udpsocket.Connect(ip, (ushort)port);
        return true;
#else
        return _InitKcp(ip, port, 1400, 128, 0, 0, false);//enable fec by: _InitKcp(ip, port, 1400, 128, 2, 2, false);
#endif
	}

    public void SendData(byte[] ar)
    {
#if UNITY_EDITOR && !UNITY_EDITOR_OSX
        udpsocket.Send(ar);
#else
		_SendData(ref ar[0],ar.Length);
#endif
	}

	public int ReceiveData(byte[] ar)
	{
#if UNITY_EDITOR && !UNITY_EDITOR_OSX
        return	udpsocket.ReceiveData(ar);
#else
		return _ReceiveData(ref ar[0],ar.Length);
#endif
	}

    public void UpdateKcp()
    {
#if UNITY_EDITOR && !UNITY_EDITOR_OSX
        udpsocket.Update();
#else
		_Update();
#endif
    }
	#endregion



    byte[] receiveBuffer = new byte[2048];

    public UnityEngine.UI.Text debugLabel;

    
    int receiveCount; 
    float elapseTime;

    private void Awake()
    {
        debugLabel.text = "Awake\n";
        if (InitKcp("127.0.0.1", 5050))
        {
            SendTest();
            ReceiveTest(); 
		}    
    } 

    void ReceiveTest()
    { 
        while(receiveCount<1)
		{
			Receive();
			UpdateKcp();
		}
		elapseTime = Time.realtimeSinceStartup - elapseTime;
		print(elapseTime);
    }

    void SendTest()
    {
		Send();
		elapseTime = Time.realtimeSinceStartup;
	}

    void Receive()
    {
        int count=0;
        do
        {
			count=ReceiveData(receiveBuffer);
			if (count > 0)
			{
				string result = System.Text.Encoding.Default.GetString(receiveBuffer, 0, count);
				debugLabel.text += result + "\n";
				receiveCount++;
			}
        } while (count>0); 
    }

    void Send()
    {
        string str = "hello";
        byte[] ar = System.Text.Encoding.UTF8.GetBytes(str);
        SendData(ar);
    } 
}
