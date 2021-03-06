﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class carcontrol : MonoBehaviour
{
    static List<string> mWriteTxt = new List<string>();
    private string outPath;
    public Text timetext;
    private float timer = 0f;
    private int time = 0;
    private int pixelcount = 25;//(car.y-startpostion.y)/0.2
    private float carmoveforward = 0;
    private float randNormal = 0f;
    private float mean = 0f;
    private float stdDev = 0f;
    private float stdDev1 = 0f;
    private float stdDev2 = 0f;
    private float stdDev3 = 0f;
    private float stdDev4 = 0f;
    private float stdDev5 = 0f;
    private float carspeedY = 0f;
    private double x = 0f;
    private double v = 0f;
    private double tt = 1/60f;  //不加f这个值就是0

    public void Awake()
    {
        //定时器
        InvokeRepeating("LaunchProjectile", 0, 0.05F);  //0秒后，每0.1f调用一次

    }
    void LaunchProjectile()
    {
        //产生高斯扰动

        System.Random rand = new System.Random(); //reuse this if you are generating many
        double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
        double u2 = rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        randNormal = (float)
                     (mean + stdDev * randStdNormal); //random normal(mean,stdDev^2)
    }
    void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        string filename = "";
        CSVHelper.Instance().ReadCSVFile("configuration", (table) => {

            // 可以遍历整张表
            foreach (CSVLine line in table)
            {
                foreach (KeyValuePair<string, string> item in line)
                {
                    Debug.Log(string.Format("item key = {0} item value = {1}", item.Key, item.Value));
                }
            }
            //可以拿到表中任意一项数据
            filename = table["1"]["name"];
            //Debug.Log(filename);
            outPath = Application.dataPath + filename + ".csv";
            stdDev1 = float.Parse(table["2"]["name"]);
            stdDev2 = float.Parse(table["3"]["name"]);
            stdDev3 = float.Parse(table["4"]["name"]);
            stdDev4 = float.Parse(table["5"]["name"]);
            stdDev5 = float.Parse(table["6"]["name"]);
            carspeedY= float.Parse(table["7"]["name"]);
        });


        //每次启动客户端删除之前保存的Log  
        if (File.Exists(outPath))
        {
            File.Delete(outPath);
        }
    }

    void Update()
    {
        float JoystickX = Input.GetAxis("Horizontal");
        x = v * tt + 0.5 * 1000*(JoystickX + randNormal) * tt * tt + x;
        v = (JoystickX + randNormal) * tt + v;
        Debug.Log(x);
        if (x > 8)
        {
            x = 8;
        }
        else if (x < -8)
        {
            x = -8;
        }

        Vector3 move = new Vector3((float)x, carmoveforward, 0);
        transform.position = move;
        
        if (move.x <= (Bezier.pixel[pixelcount].x + 3.5) && move.x >= (Bezier.pixel[pixelcount].x + 0.4))
        {
            carmoveforward += carspeedY;
            pixelcount += 4;
        }
        else if (move.x <= (Bezier.pixel[pixelcount].x - 0.4) && move.x >= (Bezier.pixel[pixelcount].x - 3.5))
        {
            carmoveforward += carspeedY;
            pixelcount += 4;

        }
        else if (move.x <= (Bezier.pixel[pixelcount].x + 0.4) && move.x >= (Bezier.pixel[pixelcount].x - 0.4))
        {
            carmoveforward += carspeedY / 2;
            pixelcount += 2;
        }
        else
        {
            carmoveforward += carspeedY / 4;
            pixelcount++;
        }

        //if (transform.position.y >= 0 && transform.position.y <= 190)
        //{
        //    stdDev = stdDev1;
        //}
        //else if (transform.position.y >= 190 && transform.position.y <= 380)
        //{
        //    stdDev = stdDev2;
        //}
        //else  if(transform.position.y >= 380 && transform.position.y <= 570)
        //{
        //    stdDev = stdDev3;
        //}
        //else if (transform.position.y >= 570 && transform.position.y <= 760)
        //{
        //    stdDev = stdDev4;
        //}
        //else if (transform.position.y >= 760 && transform.position.y <= 950)
        //{
        //    stdDev = stdDev5;
        //}
        if (carmoveforward >= 950)
        {
            Application.Quit();
        }

        timer = Time.time;
        //Debug.Log(h);
        time = (int)timer;
        timetext.text = ("Time:" + time);

        string[] temp = { JoystickX.ToString(), ",", transform.position.x.ToString(), ",", Bezier.pixel[pixelcount-1].x.ToString(), "\r\n" };
        foreach (string t in temp)
        {
            using (StreamWriter writer = new StreamWriter(outPath, true, Encoding.UTF8))
            {
                writer.Write(t);
            }
            mWriteTxt.Remove(t);
        }
        
    }
}
