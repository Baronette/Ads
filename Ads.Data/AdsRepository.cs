using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Ads.Data
{
    public class AdsRepository
    {
        private readonly string _connection;

        public AdsRepository(string connection)
        {
            _connection = connection;
        }

        public List<Ad> GetAds()
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Listings";
            connection.Open();
            var ads = new List<Ad>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Details"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                }) ;
            }
            return ads;
        }
        public List<Ad> GetAdsById(int id)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Listings WHERE UserId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var ads = new List<Ad>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Details"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;
        }
        public User GetUserByEmail(string email)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Email = @email";
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Password = (string)reader["Password"]
            };
        }
        public User VerifyLogin(User user)
        {
            User currentUser = GetUserByEmail(user.Email);
            if (currentUser == null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password);
            return isValid ? currentUser : null;

        }
        public void AddListing(Ad ad)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Listings (Details, PhoneNumber, UserId, Name)
                                    Values (@details, @number, @id, @name)";
            command.Parameters.AddWithValue("@details", ad.Description);
            command.Parameters.AddWithValue("@number", ad.PhoneNumber);
            command.Parameters.AddWithValue("@id", ad.UserId);
            command.Parameters.AddWithValue("@name", ad.Name);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void AddUser(User user)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Users Values (@name, @email, @password)";
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(user.Password));
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void DeleteAd(int id)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Listings WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
