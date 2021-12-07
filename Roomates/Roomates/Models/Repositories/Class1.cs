using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Linq;


namespace Roommates.Repositories
{
    class ChoresRepository : BaseRepository
    {
        public ChoresRepository(string connectionString) : base(connectionString) { }
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Chore chore = null;

                        // If we only expect a single row back from the database, we don't need a while loop.
                        if (reader.Read())
                        {
                            chore = new Chore
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };
                        }
                        return chore;
                    }

                }
            }
        }
        public List<Chore> GetAll()
        {

            using (SqlConnection conn = Connection)
            {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // A list to hold the rooms we retrieve from the database.
                        List<Chore> chores = new List<Chore>();

                        // Read() will return true if there's more data to read
                        while (reader.Read())
                        {
                            // The "ordinal" is the numeric position of the column in the query results.
                            //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                            int idColumnPosition = reader.GetOrdinal("Id");

                            // We user the reader's GetXXX methods to get the value for a particular ordinal.
                            int idValue = reader.GetInt32(idColumnPosition);

                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);



                            // Now let's create a new Chore object using the data from the database.
                            Chore chore = new Chore
                            {
                                Id = idValue,
                                Name = nameValue,
                            };

                            // ...and add that room object to our list.
                            chores.Add(chore);
                        }
                        return chores;
                    }


                }
            }
        }

        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }
        }

        public List<Chore> GetUnassignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM Chore
                            Left Join RoommateChore on RoommateChore.ChoreId =      Chore.Id
                            where RoommateChore.RoommateId IS NULL";
                    List<Chore> unassignedChores = new List<Chore>();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Chore noAssignment = new Chore
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };
                            unassignedChores.Add(noAssignment);
                        }
                        return unassignedChores;
                    }
                }
            }
        }
        
        public void ReasignChore(int choreId, int roommateId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE RoomateChore
                                    SET RoommateId = @RoommateId,
                                    Where ChoreId = @ChoreId";
                                   
                    cmd.Parameters.AddWithValue("@RoommateId", roommateId);
                    cmd.Parameters.AddWithValue("@ChoreId", choreId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AsignChore(int choreId, int roommateId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT RoomateChore
                                    SET RoommateId = @RoommateId,
                                    Where ChoreId = @ChoreId";

                    cmd.Parameters.AddWithValue("@RoommateId", roommateId);
                    cmd.Parameters.AddWithValue("@ChoreId", choreId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
   
}
