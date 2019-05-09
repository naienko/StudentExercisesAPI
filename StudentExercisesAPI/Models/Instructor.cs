using System.ComponentModel.DataAnnotations;

namespace StudentExercisesAPI.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        [Required]
        public string _firstname { get; set; }
        [Required]
        public string _lastname { get; set; }
        [Required]
        [StringLength(12, MinimumLength = 3)]
        public string _handle { get; set; }
        public string _specialty { get; set; }
        public int _cohortId { get; set; }
        public Cohort _cohort { get; set; }
    }
}
