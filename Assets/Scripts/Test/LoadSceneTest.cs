using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneTest : MonoBehaviour
{

    private void Start()
    {
        TestGetCopose();
    }

    private void OnMouseDown()
    {

    }

    private string GetMergedString(string origin)
    {
        if (string.IsNullOrEmpty(origin) || origin.Length <= 1)
            return origin;

        List<char> newStr = new List<char>(origin.Length);
        newStr.Add(origin[0]);
        for (int i = 1; i < origin.Length; i++)
        {
            if (origin[i] == newStr[newStr.Count - 1])
            {
                continue;
            }
            else
            {
                newStr.Add(origin[i]);
            }
        }

        return new string(newStr.ToArray());
    }


    private int GetFactorial(int n)
    {
        int prevResult = 1;
        int sum = prevResult;
        for (int i = 2; i < n + 1; i++)
        {
            prevResult = prevResult * i;
            sum += prevResult;
        }

        return sum;
    }

    private long[] result = null;

    public long Fib(long n)
    {
        result = new long[n];
        for (long i = 0; i < n; i++)
        {
            result[i] = -1;
        }

        return GetFib(n);
    }

    private long GetFib(long n)
    {
        if (n < 3)
            return 1;
        if (result[n - 1] != -1)
            return result[n - 1];
        else
        {
            result[n - 1] = GetFib(n - 1) + GetFib(n - 2);
            return result[n - 1];
        }
    }

    public void TestGetCopose() {
        List<int> items = new List<int>(9);
        for (int i = 1; i <= 9; i++) {
            items.Add(i);
        }

        List<List<int>> result = GetCompose(items, 4);

        for (int i = 0; i < result.Count; i++) {
            string str = "";
            for (int j = 0; j < result[i].Count; j++) {
                str += result[i][j] + " ";
            }
            Debug.Log(str);
        }

        Debug.Log( "compose count:"+ GetComposeCount(9,4));
    }

    public List<List<int>> GetCompose(List<int> items, int composeCount) {
        List<List<int>> result = new List<List<int>>();
        if (items.Count <= composeCount) {
            result.Add(items);
            return result;
        }

        CalculateCompose(ref result, items, new List<int>(), 0, composeCount);

        return result;
    }


    private void CalculateCompose(ref List<List<int>> result, List<int> items, List<int> compose,int startPos, int composeCount) {
        if (compose.Count == composeCount) {
            result.Add(compose);
            return;
        }

        if (startPos >= items.Count || items.Count - startPos + compose.Count < composeCount)
            return;

        CalculateCompose(ref result, items, compose, startPos + 1, composeCount);

        List<int> newCompose = new List<int>(compose);
        newCompose.Add(items[startPos]);
        CalculateCompose(ref result, items, newCompose, startPos+1, composeCount);
    }


    public int GetComposeCount(int m, int n) {
        if (m <= 0 || n <= 0)
            return -1;

        return GetFac(m) / (GetFac(n) * GetFac(m - n));
    }

    public int GetFac(int n) {
        if (n <= 0)
            return -1;

        int res = 1;
        for (int i = 2; i < n + 1; i++) {
            res = res * i;
        }
        return res;
    }
}
