﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tita
{
    public static class TestFunctions
    {
        public static void xmlparsetest()
        {
            DataFile file = new DataFile("subjects.xml");
            ClassInfoList lst = file.LoadClassInfo();
            Console.WriteLine(lst.Groups);
        }
    }
}
