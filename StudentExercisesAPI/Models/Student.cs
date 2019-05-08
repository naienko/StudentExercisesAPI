using System.Collections.Generic;

namespace StudentExercisesAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string _firstname { get; set; }
        public string _lastname { get; set; }
        public string _handle { get; set; }
        public int _cohortId { get; set; }
        public Cohort _cohort;

        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
