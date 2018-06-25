using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Spreadsheets;
using Newtonsoft.Json;
using Data = Google.Apis.Sheets.v4.Data;


namespace googlesheetstest
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        //Get And Set Spread sheet data
        static string[] Scopes = { SheetsService.Scope.Drive };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        static List<object> listData = new List<object>();
        static List<Google.Apis.Drive.v2.Data.File> Files;
       

        static void Main(string[] args)
        {
           
        }
        static UserCredential GetCredential()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("C:/Users/chartsuff/Downloads/client_id.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            return credential;
        }
        static List<Google.Apis.Drive.v2.Data.File> RetrieveAllFiles()
        {
            DriveService service = DriveServiceCreate();
            List<Google.Apis.Drive.v2.Data.File> result = new List<Google.Apis.Drive.v2.Data.File>();
            FilesResource.ListRequest request = service.Files.List();
            do
            {
                try
                {
                    FileList files = request.Execute();

                    result.AddRange(files.Items);
                    request.PageToken = files.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
            return result;
        }   
        static DriveService DriveServiceCreate()
        {
            DriveService service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });
            return service;
        }
        static void DeleteFile(string fileId)
        {
            DriveService service = DriveServiceCreate();
            try
            {
                service.Files.Delete(fileId).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        } 
        static void CreateNewSheet(string SheetName)
        {
            SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            // TODO: Assign values to desired properties of `requestBody`:
            Data.Spreadsheet requestBody = new Data.Spreadsheet();
            requestBody.Properties = new SpreadsheetProperties();
            requestBody.Properties.Title = SheetName;


            SpreadsheetsResource.CreateRequest request = sheetsService.Spreadsheets.Create(requestBody);


            Data.Spreadsheet response = request.Execute();
            string ok = response.SpreadsheetId;
            //1xt6CqlJgpxW2W8rNZvpT9nVRm--xzyo7dVSNY4qQE5w

            // TODO: Change code below to process the `response` object:
            //Console.WriteLine(JsonConvert.SerializeObject(response));
        }
        static void GetSpreadData(string ID ,string SheetName, string Column, int Min, int Max)
        {
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = ID;
            String range = SheetName + "!" + Column + Convert.ToString(Min) + ":" + Column + Convert.ToString(Max);
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    foreach (var col in row)
                    {
                        // Print columns A and E, which correspond to indices 0 and 4.
                        Console.Write(col + ",");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            Console.Read();
        }
        static void WriteData(List<object> cellData, string SheetName, string Column, int StartingCell)
        {
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

            //var oblist = new List<object>() {};
            var oblist = cellData;
            valueRange.Values = new List<IList<object>> { oblist };

            // Define request parameters.
            String spreadsheetId = "160fIGNBuzud5JJ4Cd6cjLiUrVF3looxxw-gd1G3dc_s";
            String range = SheetName + "!" + Column + Convert.ToString(StartingCell) + ":" + Column + Convert.ToString(StartingCell + cellData.Count);
            SpreadsheetsResource.ValuesResource.UpdateRequest request =
                    service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result2 = request.Execute();

        }
    }
}
