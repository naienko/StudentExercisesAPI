using System.Collections.Generic;

namespace StudentExercisesAPI.Models
{
    public class Cohort
    {
		public int Id { get; set; }
        public string Name { get; set; }

        public List<Student> Students { get; private set; } = new List<Student>();
        public List<Instructor> Instructors { get; private set; } = new List<Instructor>();

    }
}
