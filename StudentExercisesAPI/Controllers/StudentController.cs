using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using StudentExercisesAPI.Models;
using Microsoft.AspNetCore.Http;

namespace StudentExercisesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT s.Id, s.FirstName, s.LastName, s.SlackName, s.CohortId, c.Designation FROM Student s JOIN Cohort c ON s.CohortId = c.Id;";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("s.Id")),
                            _firstname = reader.GetString(reader.GetOrdinal("s.FirstName")),
                            _lastname = reader.GetString(reader.GetOrdinal("s.LastName")),
                            _handle = reader.GetString(reader.GetOrdinal("s.SlackName")),
                            _cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("s.CohortId")),
                                Name = reader.GetString(reader.GetOrdinal("c.Designation"))
                            }
                        };

                        students.Add(student);
                    }
                    reader.Close();

                    return Ok(students);
                }
            }
        }
    }
}
