using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;
using Dapper;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class StudentEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public StudentEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<Student> ShowSignals()
        {
            var sel_all = (new MySqlConnection(_configuration.GetConnectionString("BotDatabase"))).Query<Student>(
                "select student.Id as Id, student.FirstName as FirstName, student.LastName as Lastname, student.UserId as UserId FROM student");

            return sel_all;
        }
        
        [HttpGet("{id}")]
        public Student ShowSignal(long id)
        {
            var sel1 = (new MySqlConnection(_configuration.GetConnectionString("BotDatabase"))).Query<Student>(
                "select student.Id as Id, student.FirstName as FirstName, student.LastName as Lastname, student.UserId as UserId FROM student WHERE student.Id=@Id", new{ Id = id});
            if (sel1.Count() > 0)
            {
                return sel1.First();
            }
            else
            {
                return null;
            }
            
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(Student student)
        {
            
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            
            // TODO: add query to get a signal by the given id
            var connection = new MySqlConnection(connectionString);
            
            var std = connection.Query<int>("SELECT Id FROM student WHERE UserId=@UserId", new {UserId = student.UserId});
            if (std.Count() > 0)
            {
                return BadRequest();
            }
           

            await connection.ExecuteAsync("INSERT INTO student (FirstName, LastName, UserId) values (@FirstName, @LastName, @UserId)",
                student);
            return Accepted();
            
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateSignal(Student student)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync("UPDATE student SET FirstName=@FirstName, LastName=@LastName, UserId=@UserId WHERE Id=@Id",
                    student);

            }
            catch (Exception e)
            {
                return BadRequest();
            }
           
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            var connection = new MySqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync("DELETE FROM student WHERE Id = @ID", new {ID = id});

            }
            catch (Exception e)
            {
                return BadRequest();
            }
           
            return Accepted();
        }
    }
}
