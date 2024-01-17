using MySql.Data.MySqlClient;
using Newtonsoft.Json;

public class SQL
{

    private static MySqlConnection connection;

    static SQL()
    {
        // Static constructor to initialize the MySqlConnection
        string connectionString = "server=127.0.0.1;port=3306;database=Currency;user=root;password=kagemand;";
        connection = new MySqlConnection(connectionString);
    }
    public static void Connect()
    {        
        
        try
        {
            connection.Open();
            Console.WriteLine("Connected to MySQL Server.");
           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }
    
    public static string Insert(string currencyFrom, string currencyTo, double amount)
    {
        //SQL commando
        string insertQuery = "INSERT INTO transaction (transaction_date, currencyFrom,currencyTo, amount) VALUES (NOW(),@currencyFrom,@currencyTo, @Amount)";
        

        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
        {
            // Replace parameter values with actual values
            command.Parameters.AddWithValue("@currencyFrom", currencyFrom);
            command.Parameters.AddWithValue("@currencyTo", currencyTo);
            command.Parameters.AddWithValue("@Amount", amount);

            int rowsAffected = command.ExecuteNonQuery();
            string message = $"{rowsAffected} row(s) inserted.";
            Console.WriteLine(message);
            return message;
        }
    }

    public static List<Transaction> Select()
{
    // SQL command
    string selectQuery = "SELECT transaction_id, transaction_date, currencyFrom, currencyTo, amount FROM transaction;";

    List<Transaction> transactions = new List<Transaction>();

    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
    {
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            // Check if the reader has rows
            if (reader.HasRows)
            {
                // Loop through the rows and process data
                while (reader.Read())
                {
                    Transaction transaction = new Transaction
                    {
                        TransactionId = reader.GetInt32(0),
                        TransactionDate = reader.GetDateTime(1),
                        CurrencyFrom = reader.GetString(2),
                        CurrencyTo = reader.GetString(3),
                        Amount = reader.GetDouble(4)
                    };

                    transactions.Add(transaction);
                }

                return transactions;
            }
            else
            {
                // Return an empty list if no rows found
                return new List<Transaction>();
            }
        }
    }
}



}

class Program
{
    static void Main()
    {
        SQL.Connect();
        CreateHostBuilder().Build().Run();
    }

    public static IHostBuilder CreateHostBuilder() => //Sets up thecall with http calls (API)
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseUrls("http://localhost:5000");
            });
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", builder => //adds who can access
            {
                builder.WithOrigins("http://127.0.0.1:5500") 
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });


    }

    public void Configure(IApplicationBuilder app)
    {
        // Sets up all the routing with PUT and GET and whatwill happen
        app.UseRouting();

        app.UseCors("AllowLocalhost"); 

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello, World!");
            });

            endpoints.MapGet("/api/select", async context =>
            {
                List<Transaction> transactions = SQL.Select();
                string jsonResult = JsonConvert.SerializeObject(transactions);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(jsonResult);
            });

            endpoints.MapPut("/api/insert", async context =>
            {
                try
                {
                    // Model binding automatically reads and parses JSON from the request body
                    var requestData = await context.Request.ReadFromJsonAsync<Transaction>();

                    if (requestData != null)
                    {
                        // Access properties from the model
                        double amount = requestData.Amount;
                        string currencyFrom = requestData.CurrencyFrom;
                        string currencyTo = requestData.CurrencyTo;

                        // Execute method with sql commands for insert
                        await context.Response.WriteAsync(SQL.Insert(currencyFrom,currencyTo,amount));
                    }
                    else
                    {
                        await context.Response.WriteAsync("Invalid JSON data.");
                    }
                }
                catch (Exception ex)
                {
                    await context.Response.WriteAsync($"Error: {ex.Message}");
                }
            });
            

        });
    }
}
public class Transaction
{    public int TransactionId { get; set; }

    public DateTime TransactionDate { get; set; }

    public double Amount { get; set; }
    public string CurrencyFrom { get; set; }
    public string CurrencyTo { get; set; }

    // Constructor to initialize non-nullable properties / default value
    public Transaction()
    {
        CurrencyFrom = string.Empty; 
        CurrencyTo = string.Empty; 
    }
}