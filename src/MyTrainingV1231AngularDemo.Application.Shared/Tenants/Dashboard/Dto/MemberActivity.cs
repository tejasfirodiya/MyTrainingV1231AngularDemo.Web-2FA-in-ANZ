namespace MyTrainingV1231AngularDemo.Tenants.Dashboard.Dto
{
    public class MemberActivity
    {
        public string Name { get; set; }
        public string Earnings { get; set; }
        public int Cases { get; set; }
        public int Closed { get; set; }
        public string Rate { get; set; }
        
        public string ProfilePictureName { get; set; }

        public MemberActivity(string name, string earnings, int cases, int closed, string rate, string profilePictureName)
        {
            Name = name;
            Earnings = earnings;
            Cases = cases;
            Closed = closed;
            Rate = rate;
            ProfilePictureName = profilePictureName;
        }
    }
}