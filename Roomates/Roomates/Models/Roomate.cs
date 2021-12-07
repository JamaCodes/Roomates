using System;

namespace Roommates.Models
{
    // C# representation of the Roommate table
    public class Roommate
    {
        public Roommate(string firstName, string lastName, int rentPortion, DateTime movedInDate, Room room)
        {
            FirstName = firstName;
            LastName = lastName;
            RentPortion = rentPortion;
            MovedInDate = movedInDate;
            Room = room;
        }
        public Roommate() { }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RentPortion { get; set; }
        public DateTime MovedInDate { get; set; }
        public Room Room { get; set; }
    }
}

