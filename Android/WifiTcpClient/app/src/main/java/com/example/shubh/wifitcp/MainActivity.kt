package com.example.shubh.wifitcp

import android.content.Intent
import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import kotlinx.android.synthetic.main.activity_main.*
import java.net.Socket
import java.io.DataOutputStream
import java.lang.Exception
import java.lang.NullPointerException


class MainActivity : AppCompatActivity() {

    private lateinit var deviceName: String;
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
    }

    override fun onResume() {
        super.onResume()

        bt_connect.setOnClickListener{
            run{
                deviceName = et_device_name.text.toString()
                val tcpRunnable:TcpRunnable = TcpRunnable(deviceName)
                val newThread: Thread = Thread(tcpRunnable)
                newThread.start()
            }
        }

    }
}
