﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

public class ResultFormatArea : UtilComponent {

    public RenderTexture RenderTextureRef;
    [Header("Text Propaty")]
    [SerializeField] private Text txtDate; 
    [SerializeField] private Text txtUserId; 
    [SerializeField] private Text txtUserName; 
    [SerializeField] private Text txtUserAge; 
    [SerializeField] private Text txtUserHeight; 
    [SerializeField] private Text txtPoint;
    [SerializeField] private Text txtRank; 
    [SerializeField] private List<Text> txtLabels; 

    [Header("Graph Propaty")]
    [SerializeField] private WMG_Radar_Graph graph1;
    [SerializeField] private WMG_Axis_Graph graph2;
    [SerializeField] private WMG_Axis_Graph graph3;
    [SerializeField] private Transform graph4P1; // 今回
    [SerializeField] private Transform graph4P2; // 前回
    [SerializeField] private WMG_Axis_Graph graph5;

    [Header("Series Propaty")]
    [SerializeField] private WMG_Series graph2_1;
    [SerializeField] private WMG_Series graph2_2;
    [SerializeField] private WMG_Series graph2_3;
    [SerializeField] private WMG_Series graph3_1;
    [SerializeField] private WMG_Series graph5_1;

    public void Init () {
        StartCoroutine(ConnectAPI("http://dev.rikuty.net/api/GetFormatData.php", GetUserData));
    }

    // Use this for initialization
    //void Start () {
    //    StartCoroutine(ConnectAPI("http://dev.rikuty.net/api/GetFormatData.php", GetUserData));

    //}

    private void GetUserData(string val) {
        //Debug.Log(val);
        if(val.Length == 1){
            Debug.Log("アクティブなユーザーが設定されていません。");
        } else {
            UserData userData = JsonConvert.DeserializeObject<UserData>(val);
			Cache.user.UserData = userData;

            // first data
            MeasureData firstData = userData.measure[1];

            SetLabel(this.txtDate, DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
            SetLabel(this.txtUserId, userData.user_id);

            SetLabel(this.txtUserName, userData.user_name);
            SetLabel(this.txtUserAge, userData.age+"歳");
            SetLabel(this.txtUserHeight, userData.height+"cm");
            
            SetLabel(this.txtPoint, firstData.point);
            SetLabel(this.txtRank, userData.rank);

            SettingGraph();

            ApiSetPrint();
        }
    }

    private void SettingGraph(){

		// first data
		UserData userData = Cache.user.UserData;
        MeasureData firstData = userData.measure[1];

        graph1.SetGraph1ValueList(firstData.AppraisalValues());

        List<string> graphLabelVaues = firstData.GraphLabelVaues();
        for(int i = 0; i < txtLabels.Count; i++){
            txtLabels[i].text = graphLabelVaues[i];
        }

        for(int i=userData.measure.Count; i>=1; i--){
            float x = (float)(i);
            graph2_3.pointValues.Add(new Vector2(x, userData.measure[i].post_rest_pain));
            graph2_2.pointValues.Add(new Vector2(x, userData.measure[i].post_move_pain));
            graph2_1.pointValues.Add(new Vector2(x, userData.measure[i].post_move_fear));
            graph3_1.pointValues.Add(new Vector2(x, userData.measure[i].average_max_rom));
            graph5_1.pointValues.Add(new Vector2(x, userData.measure[i].point));
            
            DateTime measureDate = DateTime.ParseExact(userData.measure[i].measure_date, "yyyy-MM-dd HH:mm:ss", null);
            graph2.xAxis.axisLabels[userData.measure.Count - i] = measureDate.ToString("MM/dd");
            graph3.xAxis.axisLabels[userData.measure.Count - i] = measureDate.ToString("MM/dd");
            graph5.xAxis.axisLabels[userData.measure.Count - i] = measureDate.ToString("MM/dd");
        }
        graph2.xAxis.AxisNumTicks = userData.measure.Count;
        graph3.xAxis.AxisNumTicks = userData.measure.Count;
        graph5.xAxis.AxisNumTicks = userData.measure.Count;

        graph4P1.localPosition = new Vector3(userData.measure[1].rom_value * 5.3f, userData.measure[1].point_value * 3.85f, 0.0f);
        graph4P2.localPosition = new Vector3(userData.measure[2].rom_value * 5.3f, userData.measure[2].point_value * 3.85f, 0.0f);

        // Update Graph
        graph1.GraphChanged();
        graph2.GraphChanged();
        graph3.GraphChanged();
        graph5.GraphChanged();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) { SavePng(); }
    }

    void SavePng()
    {
        //Debug.Log("SavePng");
        Texture2D tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGB24, false);
        RenderTexture.active = RenderTextureRef;
        tex.ReadPixels(new Rect(0, 0, RenderTextureRef.width, RenderTextureRef.height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        //Write to a file in the project folder
        //Debug.Log(Application.dataPath);
        File.WriteAllBytes(Application.dataPath + "/Resources/ResultSheet.png", bytes);
    }

    IEnumerator ResultFileCheck()
    {
        string targetFile = Application.dataPath + "/Resources/ResultSheet.png";
        while(true){
            if(System.IO.File.Exists (targetFile)){
                break;
            } else {
                //Debug.Log("file not found");
                yield return null; 
            } 
        }

        // サーバへPOSTするデータを設定 
        string url = "http://dev.rikuty.net/api/SetResultData.php";

        Dictionary<string, string> dic = new Dictionary<string, string>();
        
        dic.Add("user_id", "1");
        dic.Add("max_rom_exercise_1", "12");
        dic.Add("max_rom_exercise_2", "44");
        dic.Add("max_rom_exercise_3", "44");
        dic.Add("max_rom_exercise_4", "33");
        dic.Add("max_rom_exercise_5", "44");
        dic.Add("max_rom_exercise_6", "34");
        dic.Add("max_rom_exercise_7", "44");
        dic.Add("max_rom_exercise_8", "24");
        dic.Add("average_max_rom", "40");
        dic.Add("average_time_1", "15");
        dic.Add("average_time_2", "3");
        dic.Add("average_time_3", "10");
        dic.Add("average_time_4", "5");
        dic.Add("average_time_5", "10");
        dic.Add("average_time_6", "17");
        dic.Add("average_time_7", "3");
        dic.Add("average_time_8", "10");
        dic.Add("appraisal_value_1", "15");
        dic.Add("appraisal_value_2", "20");
        dic.Add("appraisal_value_3", "36");
        dic.Add("appraisal_value_4", "5");
        dic.Add("appraisal_value_5", "7");
        dic.Add("appraisal_value_6", "8");
        dic.Add("appraisal_value_7", "11");
        dic.Add("appraisal_value_8", "14");
        dic.Add("post_rest_pain", "5");
        dic.Add("post_move_pain", "5");
        dic.Add("post_move_fear", "5");
        dic.Add("point", "6156");
        dic.Add("rom_value", "33");
        dic.Add("point_value", "33");
		
        string filePath = Application.dataPath + "/Resources/";
        string fileName = "ResultSheet.png";
        
        yield return StartCoroutine(HttpPost(url, dic, filePath, fileName));
    }


    private void ApiSetResultData()
    {
        // サーバへPOSTするデータを設定 
        string url = "http://dev.rikuty.net/api/SetResultData.php";

        Dictionary<string, string> dic = new Dictionary<string, string>();

        dic.Add("user_id", Cache.user.UserData.user_id);
        dic.Add("max_rom_exercise_1", Cache.user.MeasureData.max_rom_exercise_1.ToString());
        dic.Add("max_rom_exercise_2", Cache.user.MeasureData.max_rom_exercise_2.ToString());
        dic.Add("max_rom_exercise_3", Cache.user.MeasureData.max_rom_exercise_3.ToString());
        dic.Add("max_rom_exercise_4", Cache.user.MeasureData.max_rom_exercise_4.ToString());
        dic.Add("max_rom_exercise_5", Cache.user.MeasureData.max_rom_exercise_5.ToString());
        dic.Add("max_rom_exercise_6", Cache.user.MeasureData.max_rom_exercise_6.ToString());
        dic.Add("max_rom_exercise_7", Cache.user.MeasureData.max_rom_exercise_7.ToString());
        dic.Add("max_rom_exercise_8", Cache.user.MeasureData.max_rom_exercise_8.ToString());
        dic.Add("average_max_rom", Cache.user.MeasureData.average_max_rom.ToString());
        dic.Add("average_time_1", Cache.user.MeasureData.average_time_1.ToString());
        dic.Add("average_time_2", Cache.user.MeasureData.average_time_2.ToString());
        dic.Add("average_time_3", Cache.user.MeasureData.average_time_3.ToString());
        dic.Add("average_time_4", Cache.user.MeasureData.average_time_4.ToString());
        dic.Add("average_time_5", Cache.user.MeasureData.average_time_5.ToString());
        dic.Add("average_time_6", Cache.user.MeasureData.average_time_6.ToString());
        dic.Add("average_time_7", Cache.user.MeasureData.average_time_7.ToString());
        dic.Add("average_time_8", Cache.user.MeasureData.average_time_8.ToString());
        dic.Add("appraisal_value_1", Cache.user.MeasureData.appraisal_value_1.ToString());
        dic.Add("appraisal_value_2", Cache.user.MeasureData.appraisal_value_2.ToString());
        dic.Add("appraisal_value_3", Cache.user.MeasureData.appraisal_value_3.ToString());
        dic.Add("appraisal_value_4", Cache.user.MeasureData.appraisal_value_4.ToString());
        dic.Add("appraisal_value_5", Cache.user.MeasureData.appraisal_value_5.ToString());
        dic.Add("appraisal_value_6", Cache.user.MeasureData.appraisal_value_6.ToString());
        dic.Add("appraisal_value_7", Cache.user.MeasureData.appraisal_value_7.ToString());
        dic.Add("appraisal_value_8", Cache.user.MeasureData.appraisal_value_8.ToString());
        dic.Add("post_rest_pain", Cache.user.MeasureData.post_rest_pain.ToString());
        dic.Add("post_move_pain", Cache.user.MeasureData.post_move_pain.ToString());
        dic.Add("post_move_fear", Cache.user.MeasureData.post_move_fear.ToString());
        dic.Add("point", Cache.user.MeasureData.point.ToString());
        dic.Add("rom_value", Cache.user.MeasureData.rom_value.ToString());
        dic.Add("point_value", Cache.user.MeasureData.point_value.ToString());

        StartCoroutine(HttpPost(url, dic));
    }


    private void ApiSetPrint()
    {
        // サーバへPOSTするデータを設定 
        string url = "https://dev.rikuty.net/SetPrint.php?facility_id=1&user_id=1";


        StartCoroutine(ConnectAPI(url, CallbackSetPrint));
    }


    private void CallbackSetPrint(String str)
    {
        Debug.Log("SuccessPrint");
    }
}