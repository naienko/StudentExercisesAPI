namespace StudentExercisesAPI.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string _firstname { get; set; }
        public string _lastname { get; set; }
        public string _handle { get; set; }
        public string _specialty { get; set; }
        public int _cohortId { get; set; }
        public Cohort _cohort { get; set; }
    }
}
