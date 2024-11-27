using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

[Route("api/[controller]/[action]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public CategoryController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    // Kategori Getirme 
    [HttpGet(Name = "GetCategories")]
    public JsonResult GetCategories()
    {
        string query = "SELECT * FROM dbo.Category";
        DataTable table = new DataTable();
        string sqlDatasource = _configuration.GetConnectionString("BlogDB");
        using (SqlConnection myCon = new SqlConnection(sqlDatasource))
        {
            myCon.Open();
            using (SqlCommand myCommand = new SqlCommand(query, myCon))
            {
                SqlDataReader myReader = myCommand.ExecuteReader();
                table.Load(myReader);
                myReader.Close();
            }
        }
        return new JsonResult(table);
    }
}
