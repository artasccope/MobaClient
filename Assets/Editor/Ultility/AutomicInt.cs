using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


public class AutomicInt
{
    private int value;
    private Mutex tex = new Mutex();

    public AutomicInt()
    {
        value = 0;
    }

    public AutomicInt(int value)
    {
        this.value = value;
    }

    public int GetIncrease()
    {
        lock (this)
        {
            tex.WaitOne();
            this.value++;
            tex.ReleaseMutex();
            return value;
        }
    }

    public int GetReduce()
    {
        lock (this)
        {
            tex.WaitOne();
            value--;
            tex.ReleaseMutex();
            return value;
        }
    }

    public int Get()
    {
        return value;
    }
}

