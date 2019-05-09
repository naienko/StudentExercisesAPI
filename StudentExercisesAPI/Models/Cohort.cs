using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesAPI.Models
{
    public class Cohort
    {
		public int Id { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 2)]
        public string Name { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();
        public List<Instructor> Instructors { get; set; } = new List<Instructor>();

    }
}
