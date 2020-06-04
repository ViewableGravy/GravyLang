using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestProject1
{

    //test
    public static class MultiLineTestsData
    {

        public static Dictionary<string, Dictionary<string, string[]>> data = new Dictionary<string, Dictionary<string, string[]>> 
        {
            {
                "Single MultilLine string StartToEnd", new Dictionary<string, string[]>
                {
                    { "\"multi line", new string[] { "0", "\"multi line\"", "\n" } },
                    {"string\"", new string[] { "0", "\"string\"", "\n" } }
                } 
            },
            {
                "string followed by Multi-line string", new Dictionary<string, string[]> 
                {
                    { "\"string\" followed by \"a multi", new string[] { "0", "\"string\"", "followed", "by", "\"a multi\"", "\n" } },
                    {"line string\"", new string[] { "0", "\"line string\"", "\n" } } 
                }
            }
        };


    }
}
