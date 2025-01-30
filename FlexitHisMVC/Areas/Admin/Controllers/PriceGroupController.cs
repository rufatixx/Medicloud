using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using Medicloud.BLL.Service;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

[Authorize]
[Area("Admin")]
public class PriceGroupController : Controller
{
    private readonly string _connectionString;
    private readonly IWebHostEnvironment _hostingEnvironment;
    public IConfiguration Configuration;
    private ServiceGroupsRepo sgRepo;
    private ServiceTypeRepo stRepo;
    private ServicesRepo sRepo;
    private OOrganizationService organizationService;
    private DepartmentsRepo departmentsRepo;
    //Communications communications;
    public PriceGroupController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        Configuration = configuration;
        _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
        _hostingEnvironment = hostingEnvironment;
        sgRepo = new ServiceGroupsRepo(_connectionString);
        sRepo = new ServicesRepo(_connectionString);
        organizationService = new OOrganizationService(_connectionString);
        departmentsRepo = new DepartmentsRepo(_connectionString);
        stRepo = new ServiceTypeRepo(_connectionString);
        //communications = new Communications(Configuration, _hostingEnvironment);
    }

    // Get all PriceGroups
    public IActionResult Index()
    {
        var orgID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));
        List<PriceGroup> priceGroups = new List<PriceGroup>();
        using (MySqlConnection con = new MySqlConnection(_connectionString))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM price_group where organizationID = @orgID", con))
            {
                cmd.Parameters.AddWithValue("@orgID", orgID);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PriceGroup priceGroup = new PriceGroup
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Name = reader["Name"].ToString(),
                        ParentID = Convert.ToInt32(reader["ParentID"])
                    };
                    priceGroups.Add(priceGroup);
                }
            }
        }
        return View(priceGroups);


    }
    // Get all PriceGroups
    public IActionResult getCompaniesByPriceGroupID(int priceGroupID)
    {
      
            List<dynamic> companies = new List<dynamic>();
            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM price_group_company_rel where priceGroupID=@priceGroupID", con))
                {
                    cmd.Parameters.AddWithValue("@priceGroupID", priceGroupID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dynamic company = new
                        {
                            ID = Convert.ToInt32(reader["id"]),
                            PriceGroupID = reader["PriceGroupID"].ToString(),
                            CompanyID = Convert.ToInt32(reader["CompanyID"])
                        };
                        companies.Add(company);
                    }
                }
            }
            return Ok(companies);
     

    }

    [HttpPost]
    public IActionResult updateSelectedCompanies(int priceGroupID, int selectedCompanyID, bool delete)
    {
     
            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();
                if (delete)
                {
                    // Delete the specified company record for the given price group ID
                    using (MySqlCommand deleteCmd = new MySqlCommand("DELETE FROM price_group_company_rel WHERE priceGroupID = @priceGroupID AND CompanyID = @companyID", con))
                    {
                        deleteCmd.Parameters.AddWithValue("@priceGroupID", priceGroupID);
                        deleteCmd.Parameters.AddWithValue("@companyID", selectedCompanyID);
                        deleteCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Insert the specified company for the given price group ID
                    using (MySqlCommand insertCmd = new MySqlCommand("INSERT INTO price_group_company_rel (priceGroupID, CompanyID) VALUES (@priceGroupID, @companyID)", con))
                    {
                        insertCmd.Parameters.AddWithValue("@priceGroupID", priceGroupID);
                        insertCmd.Parameters.AddWithValue("@companyID", selectedCompanyID);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            // Return a success response if everything is executed successfully
            return Ok("Selected companies have been updated successfully.");
        

     

    }

    // Действие для создания новой группы цен (GET)
    public IActionResult Create()
    {
        var orgID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));
        using (MySqlConnection con = new MySqlConnection(_connectionString))
        {
            con.Open();
            string query = "SELECT * FROM price_group WHERE ParentID = 0 and organizationID=@orgID";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@orgID", orgID);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                var parentGroups = new List<SelectListItem>();
                while (reader.Read())
                {
                    var parentGroup = new SelectListItem
                    {
                        Value = reader["ID"].ToString(),
                        Text = reader["Name"].ToString()
                    };
                    parentGroups.Add(parentGroup);
                }
                ViewBag.ParentGroups = new SelectList(parentGroups, "Value", "Text");
            }
        }

        return View();

      


    }

    [HttpPost]
    public IActionResult Create(PriceGroup priceGroup)
    {
        var orgID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));
        using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();
                string query = "INSERT INTO price_group (Name, ParentID,organizationID) VALUES (@Name, @ParentID,@orgID)";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", priceGroup.Name);
                cmd.Parameters.AddWithValue("@ParentID", priceGroup.ParentID);
                cmd.Parameters.AddWithValue("@orgID", orgID);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
       



    }


    // Действие для редактирования группы цен (GET)
    public IActionResult Edit(int id)
    {
      
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM price_group WHERE ID = @ID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var priceGroup = new PriceGroup
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = reader["Name"].ToString(),
                            ParentID = Convert.ToInt32(reader["ParentID"])
                        };
                        return View(priceGroup);
                    }
                }
            }
            return NotFound();
        


    }

    // Действие для редактирования группы цен (POST)
    [HttpPost]
    public IActionResult Edit(PriceGroup priceGroup)
    {
         if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE price_group SET Name = @Name, ParentID = @ParentID WHERE ID = @ID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", priceGroup.Name);
                    command.Parameters.AddWithValue("@ParentID", priceGroup.ParentID);
                    command.Parameters.AddWithValue("@ID", priceGroup.ID);
                    command.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }

            return View(priceGroup);
     





    }

    // Действие для удаления группы цен (GET)
    public IActionResult Delete(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM price_group WHERE ID = @ID";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var priceGroup = new PriceGroup
                    {
                        ID = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        ParentID = Convert.ToInt32(reader["ParentID"])
                    };
                    return View(priceGroup);
                }
            }
        }
        return NotFound();
    }

    // Действие для удаления группы цен (POST)
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
      
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM PriceGroups WHERE ID = @ID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
     
    }

    public IActionResult AddService(int id)
    {


            PriceGroup priceGroup = GetPriceGroup(id);
            List<dynamic> services = GetServices(id, Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")));

            if (priceGroup != null)
            {
                ViewBag.PriceGroup = priceGroup;
                ViewBag.Services = services;
                return View();
            }

            return NotFound();
     


    }


    [HttpPost]
    public IActionResult AddService(int id, int serviceID, decimal price)
    {
   
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Service_PriceGroup (ServiceID, PriceGroupID, Price) VALUES (@ServiceID, @PriceGroupID, @Price)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ServiceID", serviceID);
                command.Parameters.AddWithValue("@PriceGroupID", id);
                command.Parameters.AddWithValue("@Price", price);
                command.ExecuteNonQuery();
            }

            // Retrieve the price group and services again
            var priceGroup = GetPriceGroup(id);
            var services = GetServices(id, Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")));

            // Pass the updated data to the view
            ViewBag.PriceGroup = priceGroup;
            ViewBag.Services = services;

            // Return the view
            return View("AddService");
   



    }
    [HttpPost]
    public IActionResult AddServicesToPriceGroup(int percent, int priceGroupID, bool isDiscount)
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    INSERT INTO service_pricegroup (serviceID, pricegroupID, price)
                    SELECT s.id, @priceGroupID, s.price * @newPriceRatio
                    FROM services s
                    WHERE NOT EXISTS (
                        SELECT 1 FROM service_pricegroup 
                        WHERE serviceID = s.id 
                        AND pricegroupID = @priceGroupID
                    );
                ";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@priceGroupID", priceGroupID);
                    double newPriceRatio = isDiscount ? (100 - percent) / 100.0 : (100 + percent) / 100.0;
                    command.Parameters.AddWithValue("@newPriceRatio", newPriceRatio);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return BadRequest("A record with the same service ID and price group ID already exists.");
                    }
                }

                return Ok("Services added to price group successfully.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost]
    public IActionResult getActiveCompanies(int organizationID)
    {
     
            CompanyRepo select = new CompanyRepo(_connectionString);
            var list = select.GetActiveCompanies(organizationID);
            ResponseDTO<Company> response = new ResponseDTO<Company>();
            response.data = new List<Company>();
            response.data = list;
            return Ok(response);



    }
    private PriceGroup GetPriceGroup(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM price_group WHERE ID = @ID";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new PriceGroup
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Name = reader["Name"].ToString(),
                        ParentID = Convert.ToInt32(reader["ParentID"])
                    };
                }
            }
        }
        return null;
    }

    private List<dynamic> GetServices(int id, long organizationID)
    {
        List<dynamic> services = new List<dynamic>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = @"SELECT s.*, sp.price as new_price, sp.ID AS ServicePriceID 
FROM Services s 
Left JOIN Service_PriceGroup sp ON s.ID = sp.ServiceID
AND sp.PriceGroupID = @PriceGroupID 
WHERE s.organizationID = @organizationID;";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PriceGroupID", id);
            command.Parameters.AddWithValue("@organizationID", organizationID);

            using (MySqlDataReader serviceReader = command.ExecuteReader())
            {
                while (serviceReader.Read())
                {
                    dynamic service = new System.Dynamic.ExpandoObject();
                    service.ID = Convert.ToInt32(serviceReader["ID"]);
                    service.name = serviceReader["Name"].ToString();
                    service.price = Convert.ToDouble(serviceReader["price"]);
                    service.newPrice = serviceReader.IsDBNull(serviceReader.GetOrdinal("new_price")) ? 0 : Convert.ToDouble(serviceReader["new_price"]);
                    service.servicePriceID = serviceReader.IsDBNull(serviceReader.GetOrdinal("ServicePriceID")) ? 0 : Convert.ToInt32(serviceReader["ServicePriceID"]);

                    services.Add(service);
                }
            }
        }

        return services;
    }


}

public class PriceGroup
{
    public int ID { get; set; }

    [Required(ErrorMessage = "Qrupun adını qeyd etməyiniz vacibdir")]
    [DisplayName("Qrupun adı")]
    public string Name { get; set; }

    [DisplayName("Üst qrup")]
    public int ParentID { get; set; }
    
}