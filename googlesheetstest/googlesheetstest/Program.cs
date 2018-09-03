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
            
            IList<Sheet> sheets = new Sheet[1];
            sheets[0] = MakeNewSheetObject("Anotha sheet");

            CreateNewSheet("MySheet", sheets);
            //AddSheetsToSheetTitle("Grandolf", sheets, "A1:Z500");
            //DeleteFileByTitle("Kid Sheet");
            //List<object> data = new List<object>() { "where", "Will", "these", "be", "put?"};
        }

        static UserCredential GetCredential()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
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

        //Gets all files that exist on account
        static List<FileList> RetrieveAllFiles(bool WriteFiles)
        {
            DriveService service = DriveServiceCreate();
            List<Google.Apis.Drive.v2.Data.File> result = new List<Google.Apis.Drive.v2.Data.File>();
            FilesResource.ListRequest request = service.Files.List();
            List<FileList> _FileLists = new List<FileList>();
            do
            {
                try
                {
                    FileList files = request.Execute();

                    result.AddRange(files.Items);
                    request.PageToken = files.NextPageToken;
                    _FileLists.Add(files);
                    
                    if (WriteFiles)
                        for (int i = 0; i < files.Items.Count; i++)
                            Console.WriteLine(files.Items[i].Title);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
            return _FileLists;
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

        //Delets file with file id
        static void DeleteFileById(string fileId)
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

        //Delete file with file Title/name
        static void DeleteFileByTitle(string FileTitle)
        {
            DeleteFileById(GetSheetIdByTitle(FileTitle));
        }

        //Creates a new sheet with sheetname
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

        //Gets sheet data from cell range
        static void GetSpreadData(string SheetTitle ,string SheetName, string Column, int Min, int Max)
        {
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = GetSheetIdByTitle(SheetTitle);
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

        //Creates a new Sheet with tabs
        static void CreateNewSheet(string SheetTitle, IList<Sheet> Sheets)
        {
            SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            // TODO: Assign values to desired properties of `requestBody`:
            Data.Spreadsheet requestBody = new Data.Spreadsheet();
            requestBody.Properties = new SpreadsheetProperties();
            requestBody.Properties.Title = SheetTitle;

            requestBody.Sheets = Sheets;

            SpreadsheetsResource.CreateRequest request = sheetsService.Spreadsheets.Create(requestBody);

            Data.Spreadsheet response = request.Execute();


            // TODO: Change code below to process the `response` object:
            //Console.WriteLine(JsonConvert.SerializeObject(response));
        }

        //Fills out basic sheet data
        static Sheet MakeNewSheetObject(string _Title)
        {
            Sheet sheet = new Sheet()
            {
                Properties = new SheetProperties()
                {
                    Title = _Title,
                }
            };
            return sheet;
        }

        static void AddSheetsToSheetTitle(string SheetTitle, IList<Sheet> Sheets, string CellRange)
        {
            SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });
            string SheetId = GetSheetIdByTitle(SheetTitle);
            //Copys the current sheet data and adds to it
            SpreadsheetsResource.GetRequest getRequest = sheetsService.Spreadsheets.Get(SheetId);
            Spreadsheet requestBody = getRequest.Execute();
            requestBody.SpreadsheetId = null;

            //Update values in requestBody cells and sheets

            List<SpreadsheetsResource.ValuesResource.GetRequest> valueRequests = new List<SpreadsheetsResource.ValuesResource.GetRequest>();
            for (int i = 0; i < requestBody.Sheets.Count; i++)
                valueRequests.Add(sheetsService.Spreadsheets.Values.Get(SheetId, requestBody.Sheets[i].Properties.Title + "!" + CellRange));

            //execute all requests
            List<ValueRange> responses = new List<ValueRange>();

            for (int i =0; i < valueRequests.Count; i++)
                responses.Add(valueRequests[i].Execute());

            //Add sheets to requestBody
            for (int i = 0; i < Sheets.Count; i++)
                requestBody.Sheets.Add(Sheets[i]);

            //Overwrite current sheet, delete first sheet
            SpreadsheetsResource.CreateRequest createRequest = sheetsService.Spreadsheets.Create(requestBody);
            //delete sheet
            DeleteFileByTitle(SheetTitle);
            createRequest.Execute();

            //Update sheets and cells
            for (int i = 0; i < responses.Count; i++) {
                SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                       sheetsService.Spreadsheets.Values.Update(responses[i], GetSheetIdByTitle(SheetTitle), requestBody.Sheets[i].Properties.Title + "!" + CellRange);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                updateRequest.Execute();
            }
        }

        //Overwrites cells
        static void WriteData(string SheetTitle, string SheetName, string Column, int StartingCell, List<object> cellData)
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
            String spreadsheetId = GetSheetIdByTitle(SheetTitle);
            String range = SheetName + "!" + Column + Convert.ToString(StartingCell) + ":" + Column + Convert.ToString(StartingCell + cellData.Count);
            SpreadsheetsResource.ValuesResource.UpdateRequest request =
                    service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result2 = request.Execute();

        }

        //Gets sheet id from sheet name
        static string GetSheetIdByTitle(string SheetTitle)
        {
            List<FileList> fileLists = RetrieveAllFiles(false);
            for (int l = 0; l < fileLists.Count; l++)
                for (int f = 0; f < fileLists[l].Items.Count; f++)
                    if (fileLists[l].Items[f].Title == SheetTitle)
                        return fileLists[l].Items[f].Id;
            return null;
        }
        
    }
}
