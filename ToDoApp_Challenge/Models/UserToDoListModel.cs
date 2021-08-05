using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoApp_Challenge.Models
{
    public class UserToDoListModel
    {
        public List<TodoModel> ToDoList { get; set; }

        public UserToDoListModel()
        {
            ToDoList = new List<TodoModel>();
        }

    }
}
