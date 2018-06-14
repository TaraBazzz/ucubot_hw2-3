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
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var sel_all = (new MySqlConnection(_configuration.GetConnectionString("BotDatabase"))).Query<LessonSignalDto>(
                "select lesson_signal.Id as Id, lesson_signal.Timestamp as Timestamp, lesson_signal.SignalType as Type, student.UserId as UserId from lesson_signal inner join student on lesson_signal.student_id=student.Id");

            return sel_all;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var sel1 = (new MySqlConnection(_configuration.GetConnectionString("BotDatabase"))).Query<LessonSignalDto>(
                "select lesson_signal.Id as Id, lesson_signal.Timestamp as Timestamp, lesson_signal.SignalType as Type, student.UserId as UserId from lesson_signal inner join student on lesson_signal.student_id=student.Id where lesson_signal.Id=@Id", new{ Id = id});
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
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            int existance;
            // TODO: add query to get a signal by the given id
            var connection = new MySqlConnection(connectionString);
            
            var student = connection.Query<int>("SELECT Id FROM student WHERE UserId=@UserId", new {UserId = userId});
            if (student.Count() > 0)
            {
                existance = student.First();
            }
            else
            {
                return StatusCode(409);
            }

            await connection.ExecuteAsync("INSERT INTO lesson_signal (SignalType, student_id, Timestamp) values (@SignalType, @Id, @Timestamp)",
                new
                {
                    SignalType = signalType, Id = existance, Timestamp = DateTime.Now});
            return Accepted();
            
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get a signal by the given id
            var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM lesson_signal WHERE Id = @ID", new {ID = id});
            return Accepted();
        }
    }
}
