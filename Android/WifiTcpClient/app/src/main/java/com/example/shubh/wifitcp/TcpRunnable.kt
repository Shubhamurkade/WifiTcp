package com.example.shubh.wifitcp

import android.os.AsyncTask
import java.io.DataOutputStream
import java.lang.Exception
import java.net.InetAddress
import java.net.NetworkInterface
import java.net.Socket
import java.net.NetworkInterface.getNetworkInterfaces



class TcpRunnable(deviceName: String) : Runnable{

    val hostName: String = deviceName;
    override fun run() {

        val en = NetworkInterface.getNetworkInterfaces()
        while (en.hasMoreElements()) {
            val ni = en.nextElement()

            val ipAddresses = ni.inetAddresses

            while(ipAddresses.hasMoreElements())
            {
                val ipAd = ipAddresses.nextElement()
                val hostNam = ipAd.canonicalHostName

                try {
                    val address = InetAddress.getByName(hostName)
                    if(hostNam == address.canonicalHostName)
                    {
                        while(true)
                        {
                            val clientSocket: Socket = Socket(ipAd, 11000)
                            val dout = DataOutputStream(clientSocket.getOutputStream())
                            dout.writeUTF("Hello Server/0")
                            dout.flush()
                            dout.close()
                            clientSocket.close()

                        }
                    }
                }
                catch (e: Exception)
                {
                    e.printStackTrace()
                }

            }
        }
    }
}
