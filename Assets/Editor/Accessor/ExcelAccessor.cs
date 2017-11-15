using System;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using UnityEngine;
using System.Text;
using GameFW.OrganizeData.Entity;
using GameFW.ID;

public class ExcelAccessor
{
    public static string[] SheetNames = { "Sheet1", "Sheet2", "Sheet3", };

    public static void SaveBuildingInfos(string inputFileDir, Dictionary<GameObject, EntitySaveOption> buildingSaveOptions)
    {
        Debug.Log("save called");
        string[] strs = inputFileDir.Split('.');
        string outputDir = new StringBuilder(strs[0]).Append("_instances.").Append(strs[1]).ToString();

        FileInfo file = new FileInfo(outputDir);
        ExcelPackage package = new ExcelPackage(file);

        ExcelWorksheet excelWorksheet = null;
        HashSet<int> buildingPositions = new HashSet<int>();

        int row = 1;
        if (package.Workbook.Worksheets.Count <= 0)
        {
            package.Workbook.Worksheets.Add("Sheet1");
            Debug.Log("Has no sheet");
        }
        else
        {
            excelWorksheet = package.Workbook.Worksheets[1];
            row = excelWorksheet.Dimension.End.Row + 1;
            for (int m = excelWorksheet.Dimension.Start.Row, n = excelWorksheet.Dimension.End.Row; m <= n; m++)
            {
                float posX = excelWorksheet.GetValue<float>(m, 2);
                float posY = excelWorksheet.GetValue<float>(m, 3);
                float posZ = excelWorksheet.GetValue<float>(m, 4);
                int key = GetInstanceHashCode(posX, posY, posZ);
                if (!buildingPositions.Contains(key))
                {
                    buildingPositions.Add(key);
                }
            }
        }

        excelWorksheet = package.Workbook.Worksheets[1];
        //绝对位置 建筑类型 父节点、兄弟位置、localPosition、localEulerAngles、localScale  绝对方向、绝对缩放
        foreach (KeyValuePair<GameObject, EntitySaveOption> p in buildingSaveOptions)
        {
            if (p.Value.ifLoadRuntime)
            {
                GameObject go = p.Key;
                Vector3 position = go.transform.position;
                Vector3 localPos = go.transform.localPosition;
                Vector3 localEulerAngles = go.transform.localEulerAngles;
                Vector3 localScale = go.transform.localScale;
                Vector3 eulerAngles = go.transform.eulerAngles;
                Vector3 lossyScale = go.transform.lossyScale;

                int categoryId = p.Value.categoryIndex + 1;
                int specieId = p.Value.specieIndex + 1;
                int k = GetInstanceHashCode(position.x, position.y, position.z);
                if (buildingPositions.Contains(k))
                    continue;

                if (p.Value.ifSaveHierachy && p.Value.ifSaveTrans)
                {
                    excelWorksheet.Cells[row, 1].Value = categoryId;
                    excelWorksheet.Cells[row, 2].Value = position.x;
                    excelWorksheet.Cells[row, 3].Value = position.y;
                    excelWorksheet.Cells[row, 4].Value = position.z;
                    excelWorksheet.Cells[row, 5].Value = specieId;
                    excelWorksheet.Cells[row, 6].Value = p.Value.name;
                    if (go.transform.parent == null)
                        excelWorksheet.Cells[row, 7].Value = -1;
                    else
                        excelWorksheet.Cells[row, 7].Value = IDCaculater.TransformIdInSceneHierachy(go.transform.parent);

                    excelWorksheet.Cells[row, 8].Value = go.transform.GetSiblingIndex();
                    excelWorksheet.Cells[row, 9].Value = localPos.x;
                    excelWorksheet.Cells[row, 10].Value = localPos.y;
                    excelWorksheet.Cells[row, 11].Value = localPos.z;
                    excelWorksheet.Cells[row, 12].Value = localEulerAngles.x;
                    excelWorksheet.Cells[row, 13].Value = localEulerAngles.y;
                    excelWorksheet.Cells[row, 14].Value = localEulerAngles.z;
                    excelWorksheet.Cells[row, 15].Value = localScale.x;
                    excelWorksheet.Cells[row, 16].Value = localScale.y;
                    excelWorksheet.Cells[row, 17].Value = localScale.z;
                    excelWorksheet.Cells[row, 18].Value = eulerAngles.x;
                    excelWorksheet.Cells[row, 19].Value = eulerAngles.y;
                    excelWorksheet.Cells[row, 20].Value = eulerAngles.z;
                    excelWorksheet.Cells[row, 21].Value = lossyScale.x;
                    excelWorksheet.Cells[row, 22].Value = lossyScale.y;
                    excelWorksheet.Cells[row, 23].Value = lossyScale.z;
                    excelWorksheet.Cells[row, 24].Value = p.Value.teamIndex + 1;
                    excelWorksheet.Cells[row, 25].Value = p.Value.assetName;
                }
                else if (p.Value.ifSaveTrans)
                {
                    excelWorksheet.Cells[row, 1].Value = categoryId;
                    excelWorksheet.Cells[row, 2].Value = position.x;
                    excelWorksheet.Cells[row, 3].Value = position.y;
                    excelWorksheet.Cells[row, 4].Value = position.z;
                    excelWorksheet.Cells[row, 5].Value = specieId;
                    excelWorksheet.Cells[row, 6].Value = p.Value.name;
                    excelWorksheet.Cells[row, 18].Value = eulerAngles.x;
                    excelWorksheet.Cells[row, 19].Value = eulerAngles.y;
                    excelWorksheet.Cells[row, 20].Value = eulerAngles.z;
                    excelWorksheet.Cells[row, 21].Value = lossyScale.x;
                    excelWorksheet.Cells[row, 22].Value = lossyScale.y;
                    excelWorksheet.Cells[row, 23].Value = lossyScale.z;
                    excelWorksheet.Cells[row, 24].Value = p.Value.teamIndex + 1;
                    excelWorksheet.Cells[row, 25].Value = p.Value.assetName;
                }
                else if (p.Value.ifSaveHierachy)
                {
                    excelWorksheet.Cells[row, 1].Value = categoryId;
                    excelWorksheet.Cells[row, 2].Value = position.x;
                    excelWorksheet.Cells[row, 3].Value = position.y;
                    excelWorksheet.Cells[row, 4].Value = position.z;
                    excelWorksheet.Cells[row, 5].Value = specieId;
                    excelWorksheet.Cells[row, 6].Value = p.Value.name;
                    if (go.transform.parent == null)
                        excelWorksheet.Cells[row, 7].Value = -1;
                    else
                        excelWorksheet.Cells[row, 7].Value = IDCaculater.TransformIdInSceneHierachy(go.transform.parent);

                    excelWorksheet.Cells[row, 8].Value = go.transform.GetSiblingIndex();
                    excelWorksheet.Cells[row, 9].Value = localPos.x;
                    excelWorksheet.Cells[row, 10].Value = localPos.y;
                    excelWorksheet.Cells[row, 11].Value = localPos.z;
                    excelWorksheet.Cells[row, 12].Value = localEulerAngles.x;
                    excelWorksheet.Cells[row, 13].Value = localEulerAngles.y;
                    excelWorksheet.Cells[row, 14].Value = localEulerAngles.z;
                    excelWorksheet.Cells[row, 15].Value = localScale.x;
                    excelWorksheet.Cells[row, 16].Value = localScale.y;
                    excelWorksheet.Cells[row, 17].Value = localScale.z;
                    excelWorksheet.Cells[row, 24].Value = p.Value.teamIndex + 1;
                    excelWorksheet.Cells[row, 25].Value = p.Value.assetName;
                }
                row++;
                Debug.Log(row);
            }
        }
        package.Save();
        package.Dispose();
    }

    public static void ReadCatgoryNames(string path, ref Dictionary<int, string> categoryNames, ref Dictionary<int, Dictionary<int, string>> specieNames)
    {
        FileStream fs = File.Open(path, FileMode.Open);
        ExcelPackage package = new ExcelPackage(fs);
        try
        {
            ExcelWorksheet sht = package.Workbook.Worksheets[1];

            for (int m = sht.Dimension.Start.Row + 1, n = sht.Dimension.End.Row; m <= n; m++)
            {
                if (string.IsNullOrEmpty(sht.GetValue<string>(m, 1)))
                    continue;

                int categoryId = sht.GetValue<int>(m, 1);
                string categoryName = sht.GetValue<string>(m, 2);
                int specieId = sht.GetValue<int>(m,3);
                string specieName = sht.GetValue<string>(m,4);
                if (!categoryNames.ContainsKey(categoryId))
                {
                    categoryNames.Add(categoryId, categoryName);
                }
                if (!specieNames.ContainsKey(categoryId)) {
                    specieNames.Add(categoryId, new Dictionary<int, string>());
                }
                if (!specieNames[categoryId].ContainsKey(specieId)) {
                    specieNames[categoryId].Add(specieId, specieName);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            package.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }

    public static int GetInstanceHashCode(float x, float y, float z)
    {
        return Mathf.CeilToInt(x).GetHashCode() + Mathf.CeilToInt(y).GetHashCode() + Mathf.CeilToInt(z).GetHashCode();
    }
}
