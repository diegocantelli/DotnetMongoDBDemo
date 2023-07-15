using MongoDB.Bson.Serialization.Attributes;

namespace DriversAppApi.Models
{
    public class DriverWithTeam : Driver
    {

        public IEnumerable<Team> Team { get; set; }
    }
}