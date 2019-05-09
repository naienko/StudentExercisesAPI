using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentExercisesAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 2)]
        public string _firstname { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 2)]
        public string _lastname { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 2)]
        public string _handle { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 2)]
        public int _cohortId { get; set; }
        public Cohort _cohort;

        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
