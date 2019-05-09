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
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
        {
            _config = config;
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
                    cmd.CommandText = @"SELECT c.Id AS CohortId, c.Designation, s.Id AS StudentId, s.FirstName AS StudentFirstName, 
                            s.LastName AS StudentLastName, s.SlackName AS StudentSlack, i.Id AS InstructorId, 
                            i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName, i.SlackName AS InstructorSlack 
                            FROM Cohort c
                            JOIN Student s ON c.Id = s.CohortId
                            JOIN Instructor i ON c.Id = i.CohortId";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();

                    Dictionary<int, Cohort> cohortHash = new Dictionary<int, Cohort>();
                    Dictionary<int, Student> studentHash = new Dictionary<int, Student>();
                    Dictionary<int, Instructor> instructorHash = new Dictionary<int, Instructor>();

                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("CohortId"));
                        int studentId = reader.GetInt32(reader.GetOrdinal("StudentId"));
                        int instructorId = reader.GetInt32(reader.GetOrdinal("InstructorId"));

                        if (!cohortHash.ContainsKey(cohortId))
                        {
                            cohortHash[cohortId] = new Cohort
                            {
                                Id = cohortId,
                                Name = reader.GetString(reader.GetOrdinal("Designation"))
                            };
                        }

                        if (!studentHash.ContainsKey(studentId))
                        {
                            Student student = new Student
                            {
                                Id = studentId,
                                _firstname = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                _lastname = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                _handle = reader.GetString(reader.GetOrdinal("StudentSlack")),
                                _cohortId = cohortId
                            };
                            cohortHash[cohortId].Students.Add(student);
                            studentHash[studentId] = student;
                        }

                        if (!instructorHash.ContainsKey(instructorId))
                        {
                            Instructor instructor = new Instructor
                            {
                                Id = instructorId,
                                _firstname = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                _lastname = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                _handle = reader.GetString(reader.GetOrdinal("InstructorSlack")),
                                _cohortId = cohortId
                            };
                            cohortHash[cohortId].Instructors.Add(instructor);
                            instructorHash[instructorId] = instructor;
                        }
                    }
                    cohorts = cohortHash.Values.ToList();
                    reader.Close();

                    return Ok(cohorts);
                }
            }
        }
        [HttpGet("{id}", Name = "GetCohort")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS CohortId, c.Designation, s.Id AS StudentId, s.FirstName AS StudentFirstName, 
                            s.LastName AS StudentLastName, s.SlackName AS StudentSlack, i.Id AS InstructorId, 
                            i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName, i.SlackName AS InstructorSlack 
                            FROM Cohort c
                            JOIN Student s ON c.Id = s.CohortId
                            JOIN Instructor i ON c.Id = i.CohortId WHERE c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = new Cohort();

                    Dictionary<int, Student> studentHash = new Dictionary<int, Student>();
                    Dictionary<int, Instructor> instructorHash = new Dictionary<int, Instructor>();

                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("CohortId"));
                        int studentId = reader.GetInt32(reader.GetOrdinal("StudentId"));
                        int instructorId = reader.GetInt32(reader.GetOrdinal("InstructorId"));

                        cohort.Id = cohortId;
                        cohort.Name = reader.GetString(reader.GetOrdinal("Designation"));

                        if (!studentHash.ContainsKey(studentId))
                        {
                            studentHash[studentId] = new Student
                            {
                                Id = studentId,
                                _firstname = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                _lastname = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                _handle = reader.GetString(reader.GetOrdinal("StudentSlack")),
                                _cohortId = cohortId
                            };
                        }

                        if (!instructorHash.ContainsKey(instructorId))
                        {
                            instructorHash[instructorId] = new Instructor
                            {
                                Id = instructorId,
                                _firstname = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                _lastname = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                _handle = reader.GetString(reader.GetOrdinal("InstructorSlack")),
                                _cohortId = cohortId
                            };
                        }
                    }
                    cohort.Students = studentHash.Values.ToList();
                    cohort.Instructors = instructorHash.Values.ToList();
                    reader.Close();

                    return Ok(cohort);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cohort Cohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cohort (Designation)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Designation)";
                    cmd.Parameters.Add(new SqlParameter("@Designation", Cohort.Name));

                    int newId = (int)cmd.ExecuteScalar();
                    Cohort.Id = newId;
                    return CreatedAtRoute("GetCohort", new { id = newId }, Cohort);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Cohort Cohort)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Cohort SET Designation = @Designation WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Designation", Cohort.Name));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CohortExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Cohort WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CohortExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CohortExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackName, CohortId
                                        FROM Cohort WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
