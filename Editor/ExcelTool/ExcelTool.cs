using Excel;
using Codice.Client.BaseCommands.WkStatus.Printers;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using System.Text;

public class ExcelTool
{
    //要读取的Excel文件的存放位置
    public static string EXCEL_PATH = Application.dataPath + "/Resources/Excel/";
    //生成的数据结构类脚本存放位置
    public static string DATA_CLASS_PATH = Application.dataPath + "/Scripts/ExcelData/DataClass/";
    //容器类存放位置
    public static string DATA_CONTAINER_PATH = Application.dataPath + "/Scripts/ExcelData/DataContainer/";

    private static int BEGIN_INDEX = 4;


    /// <summary>
    /// 读取EXCEL_PATH下的所有excel表格的数据
    /// </summary>
    [MenuItem("Excel Tool/Create Excel Data")]
    private static void CreateExcelData()
    {
        //获取该目录下所有文件的信息
        DirectoryInfo dirInfo = Directory.CreateDirectory(EXCEL_PATH);
        FileInfo[] files = dirInfo.GetFiles();

        DataTableCollection tableCollection;

        for (int i = 0; i < files.Length; i++)
        {
            //只处理xlsx文件
            //不知道为啥读取不了xls
            if (files[i].Extension != ".xlsx" && files[i].Extension != ".xls")
                continue;

            //读取Excel
            using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = reader.AsDataSet().Tables;
                fs.Close();
            }

            //Show一下当前excel的所有table
            for(int j = 0; j < tableCollection.Count; j++)
            {
                CreateExcelDataClass(tableCollection[j]);
                CreateExcelDataContainer(tableCollection[j]);
                CreateExcelDataBinary(tableCollection[j]);
            }
        }
    }

    /// <summary>
    /// 根据DataTable在DATA_CLASS_PATH下生成class
    /// </summary>
    /// <param name="table"></param>
    private static void CreateExcelDataClass(DataTable table)
    {
        if(!Directory.Exists(DATA_CLASS_PATH))
        {
            Directory.CreateDirectory(DATA_CLASS_PATH);
        }

        DataRow nameRow = table.Rows[0];
        DataRow typeRow = table.Rows[1];

        //编辑脚本的内容
        string src = "public class" + " " + table.TableName + "\n{\n";

        for(int i = 0; i < table.Columns.Count; i++)
        {
            src += "    public " + typeRow[i].ToString() + " " + nameRow[i].ToString() + ";\n";
        }

        src += "}";

        //把String写入脚本
        File.WriteAllText(DATA_CLASS_PATH + table.TableName + ".cs", src);
    }

    /// <summary>
    /// 根据DataTable在DATA_CONTAINER_PATH下生成存储数据的Dic
    /// </summary>
    /// <param name="table"></param>
    private static void CreateExcelDataContainer(DataTable table)
    {
        int keyIndex = GetKeyIndex(table);
        DataRow typeRow = table.Rows[1];

        if(!Directory.Exists(DATA_CONTAINER_PATH))
        {
            Directory.CreateDirectory(DATA_CONTAINER_PATH);
        }

        string src = "using System.Collections.Generic;\n";
        src += "public class" + " " + table.TableName + "Container" + "\n{\n";

        src += "    public Dictionary<" + typeRow[keyIndex].ToString() + ", " + table.TableName + ">";
        src += " dataDic = new " + "Dictionary<" + typeRow[keyIndex].ToString() + ", " + table.TableName + ">();\n";

        src += "}";

        //把String写入脚本
        File.WriteAllText(DATA_CONTAINER_PATH + table.TableName + "Container" + ".cs", src);
    }

    /// <summary>
    /// 根据table生成2进制文件
    /// </summary>
    /// <param name="table"></param>
    private static void CreateExcelDataBinary(DataTable table)
    {
        if(!Directory.Exists(ProjectBase.BinaryMgr.DATA_BINARY_PATH))
        {
            Directory.CreateDirectory(ProjectBase.BinaryMgr.DATA_BINARY_PATH);
        }

        using (FileStream fs = new FileStream(ProjectBase.BinaryMgr.DATA_BINARY_PATH + table.TableName + ".zhou", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //存储要读取的数据一共有多少行
            fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);

            //主键名字,并转编码
            string keyname = table.Rows[0][GetKeyIndex(table)].ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(keyname);

            //存储主键名字的长度和内容
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            fs.Write(bytes, 0, bytes.Length);

            //开始写入数据
            DataRow row;
            DataRow typeRow = table.Rows[1];
            for(int i = BEGIN_INDEX; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                for(int j = 0; j < table.Columns.Count; j++)
                {
                    switch(typeRow[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                            break;
                        case "string":
                            //string转编码，并写入其长度和内容
                            bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                    }
                }
            }
            fs.Close();
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取excel表中主键key的列
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static int GetKeyIndex(DataTable table)
    {
        DataRow row = table.Rows[2];

        for(int i = 0; i < table.Columns.Count; i++)
        {
            if (row[i].ToString() == "key")
            {
                return i;
            }
        }
        //若没有key，把第0列当成key
        return 0;
    }
}
