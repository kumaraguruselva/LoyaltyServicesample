using System.Data.SqlClient;

namespace LoyaltyWorkerServicesample
{
    public class Worker : BackgroundService
    {
        //private readonly CRUDContext _context;
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                string fileLocation = _configuration.GetValue<string>("FilePath");
                string fileName = _configuration.GetValue<string>("FileName");
                CreateFile(fileLocation, fileName);
                await Task.Delay(2000);
                DeleteFile(fileLocation);


                await Task.Delay(1000, stoppingToken);
            }
        }

        private void CreateFile(string fileLocation, string fileName)
        {
            var file = fileLocation + "\\" + fileName;
            if (!File.Exists(file))
            {
                for (int i = 1; i <= 50; i++)
                {
                    var fName = fileLocation + "\\" + "Logfile_" + i + ".txt";
                    File.Create(fName).Dispose();
                    _logger.LogInformation("File created :{fName}", fName);
                }
                var log = "File created in " + System.Environment.MachineName + " at " + file;
                InsertDataIntoDatabase(log);
            }
        }
        private void InsertDataIntoDatabase(string log)
        {
            try
            {
                string connection = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
                var query = "INSERT INTO Loyaltyservcsample (LogInfo,CreatedDate,DeletedDate) VALUES(@LogInfo,@CreatedDate,@DeletedDate)";
                SqlConnection con = new SqlConnection(connection);
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("LogInfo", log);
                    cmd.Parameters.AddWithValue("CreatedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("DeletedDate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        private void DeleteFile(string fileLocation)
        {
            try
            {
                string[] files = Directory.GetFiles(fileLocation);
                foreach (var file in files)
                {
                    File.Delete(file);
                    _logger.LogInformation("Deleted the files {file} at {time}", file, DateTime.Now);
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }
    }

}

