using Newtonsoft.Json;
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

            Backand.Sdk backandSdk = new Backand.Sdk();

            backandSdk.SignIn("backandtodoapp", "dotnet@backand.com", "qwerty1!");
            // this is the total row of the entire table in the database, not just what that is returned in this page. 
            // use this for paging
            int? totalRows = null;
            // get all todo rows
            var todoList = backandSdk.GelList<Todo>("todo", out totalRows);
            Debug.Write(JsonConvert.SerializeObject(todoList));


            // post a todo
            Todo todo = new Todo() { description = "Something to do", completed = false };
            bool deep = false;
            string id = null;
            backandSdk.Post<Todo>("todo", todo, out id, deep);
            Debug.Write(id);

            // put a todo
            todo.completed = true;
            todo.description = "do something else";
            backandSdk.Put<Todo>("todo", id, todo);

            // get a todo
            todo = backandSdk.GetOne<Todo>("todo", id);
            Debug.Write(JsonConvert.SerializeObject(todo));

            // delete todo
            backandSdk.Delete("todo", id);
            
        }
    }

    public class Todo
    {
        public string description { get; set; }
        public bool completed { get; set; }
    }
}
