
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class DataParser
{
    static readonly int DATA_START_IDX = 3;
    static readonly int DATA_HEADER_IDX = 1;

    static public List<T> Parse<T>(string filename) where T : new()
    {
        List<T> data = new List<T>();

        //csv파일가져오기
        TextAsset csv = Resources.Load<TextAsset>($"Data/{filename}");
        if (csv == null)
        {
            Debug.LogWarning($"csv 파일이 없습니다. Data/{filename}");
        }

        Debug.Log(csv.text);
        /*예시 Gacha
        인덱스,그룹 ID,타워ID,확률
        Index,GroupID,TowerID,Probability
        int,int,int,float
        1004,1,1234,0.1
        1005,1,1234,0.1
        1006,1,1234,0.1
        1007,1,1234,0.1
        1008,1,1234,0.1
        1009,1,1234,0.1
        1010,1,1234,0.1
        1011,1,1234,0.1
        1012,1,1234,0.1
        1013,1,1234,0.1
        1014,1,1234,0.1
         */
        string[] rows = csv.text.Trim().Split(new string[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        if (rows.Length < DATA_START_IDX)
        {
            Debug.LogWarning($"데이터가 없습니다.");
            return data;
        }

        string[] headers = rows[DATA_HEADER_IDX].Split(',');

        //데이터 타입 행부터
        for (int i = DATA_START_IDX; i < rows.Length; i++)
        {
            string[] values = rows[i].Split(',');

            //제목의 개수와 데이터의 개수가 다르면 오류이므로 패스!
            if (values.Length != headers.Length) continue;

            T item = new T();
            //T클래스에 선언된 변수들을 배열 형태로 반환
            //BindingFlags.Public : 접근제어가 public인것만 찾음
            //BindingFlags.Instance : static 변수가 아닌, 객체를 생성해야 메모리가 올라가는 일반 인스턴스 변수만 찾음.
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

            for (int j = 0; j < headers.Length; j++)
            {
                string header = headers[j].Trim();
                string value = values[j].Trim();

                Debug.Log($"{j} : {header},{value}");

                //T클래스의 변수명과 csv파일의 헤더명이 같은지
                //StringComparison.OrdinalIgnoreCase : 대소문자 구분하지않고 비교하는 옵션
                FieldInfo field = Array.Find(fields, f => f.Name.Equals(header, StringComparison.OrdinalIgnoreCase));

                Debug.Log(field);

                if (field != null)
                {
                    //csv에서 가져온 데이터를 field데이터 타입으로 변환
                    object convertedValue = Convert.ChangeType(value, field.FieldType);
                    //실제 T클래스의 변수에 convertedValue할당
                    field.SetValue(item, convertedValue);
                }
            }
            data.Add(item);
        }
        Debug.Log($"{filename}파싱완료 : {data.Count}개 항목");
        return data;
    }
}
