using BlogApp.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BlogApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private IConfiguration _configuration;
        public PostController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Postları alma metodu
        [HttpGet(Name = "GetPosts")]
        public JsonResult GetPosts()
        {
            string query = @"
        SELECT 
            p.PostId, 
            p.Title, 
            p.Content, 
            p.UserId, 
            u.Username, 
            p.CategoryId, 
            c.Name AS CategoryName
        FROM dbo.Post p
        INNER JOIN dbo.[User] u ON p.UserId = u.UserId
        INNER JOIN dbo.Category c ON p.CategoryId = c.CategoryId";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("BlogDB");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        //Post Silme
        [HttpDelete("deletePost/{postId}")]
        public JsonResult DeletePost(int postId)
        {
            string query = "DELETE FROM dbo.Post WHERE PostId = @PostId";
            string sqlDatasource = _configuration.GetConnectionString("BlogDB");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@PostId", postId);
                    myCommand.ExecuteNonQuery();
                    myCon.Close();
                }
            }

            return new JsonResult("Post başarıyla silindi.");
        }


        //ID ye göre Post alma
        [HttpGet("{postId}", Name = "GetPost")]
        public JsonResult GetPost(int postId)
        {
            string query = @"
        SELECT 
            p.PostId, 
            p.Title, 
            p.Content, 
            p.UserId, 
            u.Username, 
            p.CategoryId, 
            c.Name AS CategoryName
        FROM dbo.Post p
        INNER JOIN dbo.[User] u ON p.UserId = u.UserId
        INNER JOIN dbo.Category c ON p.CategoryId = c.CategoryId
        WHERE p.PostId = @PostId"; 

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("BlogDB");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    
                    myCommand.Parameters.AddWithValue("@PostId", postId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        // Kullanıcıya ait postları alma metodu
        [HttpGet(Name = "GetUserPosts")]
        public JsonResult GetUserPosts(string userId)
        {
            string query = @"
        SELECT 
            p.PostId, 
            p.Title, 
            p.Content, 
            p.UserId, 
            u.Username, 
            p.CategoryId, 
            c.Name AS CategoryName
        FROM dbo.Post p
        INNER JOIN dbo.[User] u ON p.UserId = u.UserId
        INNER JOIN dbo.Category c ON p.CategoryId = c.CategoryId
        WHERE p.UserId = @UserId";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("BlogDB");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }


        //Post ekleme metodu
        [HttpPost(Name = "AddPost")]
        public JsonResult AddPost(PostDto dto)
        {

            string query = @"INSERT INTO dbo.Post (Title, Content, UserId, CategoryId) 
                     VALUES (@Title, @Content, @UserId, @CategoryId)";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("BlogDB");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {

                    myCommand.Parameters.AddWithValue("@Title", dto.Title);
                    myCommand.Parameters.AddWithValue("@Content", dto.Content);
                    myCommand.Parameters.AddWithValue("@UserId", dto.UserId);

                    if (dto.CategoryId.HasValue)
                    {
                        myCommand.Parameters.AddWithValue("@CategoryId", dto.CategoryId.Value);
                    }
                    else
                    {
                        myCommand.Parameters.AddWithValue("@CategoryId", DBNull.Value);
                    }

                    myCommand.ExecuteNonQuery();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }


        // Filtrelme metodu
        [HttpGet(Name = "CategoryFilter")]
        public JsonResult CategoryFilter(int categoryId)
        {
            string query = "SELECT PostId, Title, Content, UserId, CategoryId FROM dbo.Post WHERE CategoryId = @CategoryId";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("BlogDB");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

       
    }
}
