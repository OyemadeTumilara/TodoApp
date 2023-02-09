using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Diagnostics;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
          
            var todoList = GetAllTodos();
            return View(todoList);
        }

        internal TodoViewModel GetAllTodos() 
        {
            List<TodoModel> list = new List<TodoModel>();   
            using (SqliteConnection con = new SqliteConnection("Data Source= db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "Select * from todo";

                    using(var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                list.Add(new TodoModel
                                {
                                    Id = reader.GetInt32(0),
                                    TodoName = reader.GetString(1),
                                    TimeCreated = reader.GetDateTime(2),
                                });
                            }
                        }
                        else
                        {
                            return new TodoViewModel
                            {
                                TodoList = list
                            };
                        }
                    }
                }
            }
            return new TodoViewModel
            { 
                TodoList = list 
            };
        }

        public RedirectResult Insert(TodoModel todo)
        {
            var TimeCreated = DateTime.Now;
            using (SqliteConnection con = new SqliteConnection("Data Source= db.sqlite"))
            {
               using(var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "Insert into todo TodoName, TimeCreated Values('{todo.TodoName}', '{TimeCreated}')";

                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
               }
            }
            return Redirect("https://localhost:5001");
        }
        
        public JsonResult Delete(int id) 
        {
            using (SqliteConnection con = new SqliteConnection("Data Source= db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "Delete from todo Where Id = '{id}'";
                    tableCmd.ExecuteNonQuery();
                }
            }
            return Json(new { });
        }

        public JsonResult populateForm(int id)
        {
            var todo = GetById(id); 
            return Json(todo);
        }

        internal TodoModel GetById(int id)
        {
            TodoModel todo = new();
            using (SqliteConnection con = new SqliteConnection("Data Source= db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "Select * from todo WHERE Id = '{id}'";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            todo.Id = reader.GetInt32(0);
                            todo.TodoName = reader.GetString(1);    
                            todo.TimeCreated = reader.GetDateTime(2);
                                   
                            
                        }
                        else
                        {
                            return todo;
                        }
                    }
                }
            }
            return todo;
        }

        public RedirectResult Update(TodoModel todo)
        {
            using (SqliteConnection con = new SqliteConnection("Data Source= db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "UPDATE todo SET todoname = '{todo.TodoName}' WHERE Id= '{todo.Id}'";

                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return Redirect("https://localhost:5001");
        }


    }
}