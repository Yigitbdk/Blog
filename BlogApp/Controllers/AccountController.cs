using BlogApp.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace BlogApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private IConfiguration _configuration;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /*
         * 
        [HttpGet(Name ="GetUser")]
        public JsonResult GetUser()
        {

            string query = "select * from dbo.[User]";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("BlogDB");
            SqlDataReader myReader;
            using(SqlConnection myCon=new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using(SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader=myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        */
     
        //Register

        [HttpPost(Name = "AddUser")]
        public JsonResult AddUser(AddUserRequestDto dto)
        {

            string query = @"INSERT INTO dbo.[Users] (Username, Email, Password, ProfilePicture, CreateDate) 
                            VALUES (@Username, @Email, @Password, @ProfilePicture, @CreateDate)";
            DataTable table = new DataTable();

            string sqlDatasource = _configuration.GetConnectionString("BlogDB");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Username", dto.Username);
                    myCommand.Parameters.AddWithValue("@Email", dto.Email);
                    myCommand.Parameters.AddWithValue("@Password", dto.Password);
                    myCommand.Parameters.AddWithValue("@ProfilePicture", string.IsNullOrEmpty(dto.ProfilePicture) ? (object)DBNull.Value : dto.ProfilePicture);
                    myCommand.Parameters.AddWithValue("@CreateDate", DateTime.Now);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }

        //Login

        [HttpPost(Name = "LoginUser")]
        public JsonResult LoginUser(LoginRequestDto dto)
        {
            string query = @"SELECT UserId, Username FROM dbo.[Users] 
                     WHERE Email = @Email AND Password = @Password";
            DataTable table = new DataTable();

            string sqlDatasource = _configuration.GetConnectionString("BlogDB");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Email", dto.Email);
                    myCommand.Parameters.AddWithValue("@Password", dto.Password);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
           
            if (table.Rows.Count > 0) 
            {
                DataRow row = table.Rows[0];

                LoginResponseDto userInfo = new LoginResponseDto();
                userInfo.UserId = Convert.ToInt32(row[0].ToString());
                userInfo.Username = row[1].ToString();

                return new JsonResult(userInfo);
            }
            else
            {
                return new JsonResult("Invalid Email or Password");
            }
        }

        //Edit

        [HttpPost(Name = "UpdateUserProfile")]
        public JsonResult UpdateUserProfile(UpdateProfileRequestDto dto)
        {
            string query = @"UPDATE dbo.[Users] 
                     SET Username = @Username, Bio = @Bio, ProfilePicture = @ProfilePicture
                     WHERE UserId = @UserId";

            string sqlDatasource = _configuration.GetConnectionString("BlogDB");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", dto.UserId);
                    myCommand.Parameters.AddWithValue("@Username", dto.Username);
                    myCommand.Parameters.AddWithValue("@Bio", dto.Bio ?? (object)DBNull.Value);
                    myCommand.Parameters.AddWithValue("@ProfilePicture", string.IsNullOrEmpty(dto.ProfilePicture) ? (object)DBNull.Value : dto.ProfilePicture);

                    int rowsAffected = myCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return new JsonResult("Profile updated successfully.");
                    }
                    else
                    {
                        return new JsonResult("User not found or no changes made.");
                    }
                }
            }
        }

        //Profile alma

        [HttpGet(Name = "GetUserProfile")]
        public JsonResult GetUserProfile(string userId)
        {
            string query = @"SELECT Username, Bio, ProfilePicture 
                     FROM dbo.[Users] 
                     WHERE UserId = @UserId";

            string sqlDatasource = _configuration.GetConnectionString("BlogDB");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var userProfile = new
                            {
                                Username = reader["Username"].ToString(),
                                Bio = reader["Bio"] != DBNull.Value ? reader["Bio"].ToString() : null,
                                ProfilePicture = reader["ProfilePicture"] != DBNull.Value ? reader["ProfilePicture"].ToString() : null
                            };

                            return new JsonResult(userProfile);
                        }
                        else
                        {
                            return new JsonResult("User not found.") { StatusCode = 404 };
                        }
                    }
                }
            }
        }


    }
}
