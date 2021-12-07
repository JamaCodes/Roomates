using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Roommates.Models;


namespace Roommates.Repositories
{
        public class RoommateRepository : BaseRepository
        {
            public RoommateRepository(string connectionString) : base(connectionString) { }
            public Roommate GetById(int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT m.FirstName, m.RentPortion, m.RoomId, r.Name
                        FROM Roommate m 
                        LEFT JOIN Room r
                        ON r.id = m.RoomId 
                        WHERE m.Id = @id";
                        cmd.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Roommate roommate = null;

                            // If we only expect a single row back from the database, we don't need a while loop.
                            if (reader.Read())
                            {
                                roommate = new Roommate
                                {
                                    Id = id,
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                    Room = new Room
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                                        Name = reader.GetString(reader.GetOrdinal("Name"))
                                    }

                                };
                            }
                            return roommate;
                        }

                    }
                }
            }

        }
    }
