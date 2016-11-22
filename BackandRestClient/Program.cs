using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackandRestClient
{
    class Program
    {
        static void Main(string[] args)
        {

            BackandClient client = new BackandClient("backandtodoapp", "dotnet@backand.com", "qwerty1!");

            var response = client.GelAll("todo");
            
            Debug.Write(response.Content);
        }
    }
}
