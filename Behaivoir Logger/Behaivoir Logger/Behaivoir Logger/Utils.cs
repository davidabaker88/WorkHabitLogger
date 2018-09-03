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


namespace Behaivoir_Logger
{
    public static class Utils
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        //Get And Set Spread sheet data
        static string[] Scopes = { SheetsService.Scope.Drive };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        static List<object> listData = new List<object>();
        static List<Google.Apis.Drive.v2.Data.File> Files;


        public static UserCredential GetCredential()
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

        public static DriveService DriveServiceCreate()
        {
            DriveService service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });
            return service;
        }

        //Delets file with file id
        public static void DeleteFileById(string fileId)
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
        public static void DeleteFileByTitle(string FileTitle)
        {
            DeleteFileById(GetSheetIdByTitle(FileTitle));
        }
        /// <summary>
        /// creates a new Google Sheet and returns the ID
        /// </summary>
        /// <param name="SheetName"></param>
        /// <returns></returns>
        public static String CreateNewSheet(string SheetName)
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
            return response.SpreadsheetId;

        }
        /// <summary>
        /// create a new Spreadsheet with defined tabs specified by the Sheets parameter
        /// </summary>
        /// <param name="SheetTitle"></param>
        /// <param name="Sheets"></param>
        public static String CreateNewSheet(string SheetTitle, IList<Sheet> Sheets)
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
            return response.SpreadsheetId;

           
        }
        /// <summary>
        /// Gets a rectangle of data from the googleSheet indicated by spreadSheetID.
        /// startCell and Endcell are in the "A10" ,"C12" format
        /// sheetName is the sheet in the googleSheet.
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="SheetName"></param>
        /// <param name="startCell"></param>
        /// <param name="endCell"></param>
        public static IList<IList<Object>> GetSpreadData(String spreadsheetId, String sheetName, String startCell, String endCell)
        {
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String range = sheetName + "!" + startCell + ":" + endCell;
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            return response.Values;

        }

        public static int GetNextRowNum(String spreadsheetId, String sheetName, String startCell, String endCell)
        {
            IList<IList<Object>> values = GetSpreadData(spreadsheetId, sheetName, startCell, endCell);
            return values.Count+1;
        }
        /// <summary>
        /// this will get the index of the sheet's location to be used for GridRange usages,returns -1 if not found
        /// </summary>
        /// <param name="SheetName"></param>
        /// <returns></returns>
        public static int? GetSheetIdBySheetName(string spreadsheetId, string SheetName)
        {
            SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });
            SpreadsheetsResource.GetRequest request = sheetsService.Spreadsheets.Get(spreadsheetId);

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data.Spreadsheet response = request.Execute();
            
            for ( int i =0; i<response.Sheets.Count;i++){
                Sheet tempSheet = response.Sheets[i];
                if (tempSheet.Properties.Title == SheetName)
                {
                    return tempSheet.Properties.SheetId;
                }
            
            }
            return -1;
        }
        /// <summary>
        /// gets the spreadsheet id from the name of the file (not sure if this works in the case of multiple files with same name
        /// </summary>
        /// <param name="SheetTitle"></param>
        /// <returns></returns>
        public static string GetSheetIdByTitle(string SheetTitle)
        {
            List<FileList> fileLists = RetrieveAllFiles(false);
            Console.WriteLine("num files: " + fileLists.Count);
            for (int l = 0; l < fileLists.Count; l++)
                for (int f = 0; f < fileLists[l].Items.Count; f++)
                {
                    Console.WriteLine("file name: " + fileLists[l].Items[f].Title);
                    if (fileLists[l].Items[f].Title == SheetTitle)
                        
                        return fileLists[l].Items[f].Id;
                }
           
            return null;
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
        //Fills out basic sheet data
        public static Sheet MakeNewSheetObject(string _Title)
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
        /// <summary>
        /// adds tabs specified by "Sheets" to existing sheet //FIXME is this what this does
        /// </summary>
        /// <param name="SheetTitle"></param>
        /// <param name="Sheets"></param>
        /// <param name="CellRange"></param>
        public static void AddSheetsToSheetTitle(string SheetTitle, IList<Sheet> Sheets, string CellRange)
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

            for (int i = 0; i < valueRequests.Count; i++)
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
            for (int i = 0; i < responses.Count; i++)
            {
                SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                       sheetsService.Spreadsheets.Values.Update(responses[i], GetSheetIdByTitle(SheetTitle), requestBody.Sheets[i].Properties.Title + "!" + CellRange);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                updateRequest.Execute();
            }
        }
        /// <summary>
        /// adds blank tab specified by SheetName to spreadsheet
        /// </summary>
        /// <param name="SpreadSheetID"></param>
        /// <param name="SheetName"></param>
        public static void AddTabToSpreadSheet(string SpreadSheetID, string SheetName)
        {

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            // Add new Sheet
            var addSheetRequest = new AddSheetRequest();
            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = SheetName;
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request
            {
                AddSheet = addSheetRequest
            });

            var batchUpdateRequest =
                service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, SpreadSheetID);

            batchUpdateRequest.Execute();
        }
        /// <summary>
        /// locks the cells for a sheet, 
        /// </summary>
        /// <param name="SpreadSheetID"></param>
        /// <param name="SheetName"></param>
        public static void AddLockCells(string SpreadSheetID, string SheetName, int startCol,int endCol,int? startRow,int? endRow)
        {

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            // Add locked request parameters
            var addLockedCellRequest = new AddProtectedRangeRequest();
            addLockedCellRequest.ProtectedRange = new ProtectedRange();
            addLockedCellRequest.ProtectedRange.Range = new GridRange();
            addLockedCellRequest.ProtectedRange.Range.SheetId = GetSheetIdBySheetName(SpreadSheetID,SheetName);
            addLockedCellRequest.ProtectedRange.Range.EndColumnIndex = endCol;
            addLockedCellRequest.ProtectedRange.Range.EndRowIndex = endRow;
            addLockedCellRequest.ProtectedRange.Range.StartColumnIndex = startCol;
            addLockedCellRequest.ProtectedRange.Range.StartRowIndex = startRow;
            addLockedCellRequest.ProtectedRange.Editors = new Editors();
            addLockedCellRequest.ProtectedRange.Editors.Users = new List<string>();
            addLockedCellRequest.ProtectedRange.Editors.Users.Add("davidabaker88@gmail.com");
            addLockedCellRequest.ProtectedRange.Editors.DomainUsersCanEdit = false;

            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request
            {
                AddProtectedRange = addLockedCellRequest
            });

            var batchUpdateRequest =
                service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, SpreadSheetID);

           BatchUpdateSpreadsheetResponse result = batchUpdateRequest.Execute();
            int i = 1;
        }
        /// <summary>
        /// Writes Data in list to rectangle specified by startCell and endCell.  This will overwrite existing data
        /// </summary>
        /// <param name="spreadSheetID"></param>
        /// <param name="cellData"></param>
        /// <param name="SheetName"></param>
        /// <param name="startCell"></param>
        /// <param name="endCell"></param>
        public static void WriteData(String spreadSheetID, List<object> cellData, string SheetName, String startCell, String endCell)
        {
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });

            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "ROWS";//COLUMNS
            //var oblist = new List<object>() {};
            var oblist = cellData;
            valueRange.Values = new List<IList<object>> { oblist };

            // Define request parameters.
            String range = SheetName + "!" + startCell + ":" + endCell;
            SpreadsheetsResource.ValuesResource.UpdateRequest request =
                    service.Spreadsheets.Values.Update(valueRange, spreadSheetID, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            UpdateValuesResponse result2 = request.Execute();

        }
        /// <summary>
        /// Writes Data to the cell specified by writeCell. This will overwrite existing data
        /// </summary>
        /// <param name="spreadSheetID"></param>
        /// <param name="cellData"></param>
        /// <param name="SheetName"></param>
        /// <param name="startCell"></param>
        /// <param name="endCell"></param>
        public static void WriteCellData(String spreadSheetID, object cellData, string SheetName, String writeCell)
        {
            string endCell = writeCell;
            List<object> cellDataList = new List<object> { cellData };
            WriteData(spreadSheetID, cellDataList, SheetName, writeCell, endCell);

        }
        //create lock cells function

        //create unlock cells function

        //sharing sheets
        public static void ShareSheet(String spreadSheetID, string emailAddress)
        {
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCredential(),
                ApplicationName = ApplicationName,
            });
            InsertPermission(service, spreadSheetID, emailAddress, "user", "writer");
        }

        public static Permission InsertPermission(DriveService service, String fileId, String value,
     String type, String role)
        {
            Permission newPermission = new Permission();
            newPermission.Value = value;
            newPermission.Type = type;
            newPermission.Role = role;
            try
            {
                return service.Permissions.Insert(newPermission, fileId).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return null;
            }
        }
    }
  

    /*static void Main(string[] args)
    {

        IList<Sheet> sheets = new Sheet[1];
        sheets[0] = MakeNewSheetObject("Anotha sheet");

        CreateNewSheet("MySheet", sheets);
        //AddSheetsToSheetTitle("Grandolf", sheets, "A1:Z500");
        //DeleteFileByTitle("Kid Sheet");
        //List<object> data = new List<object>() { "where", "Will", "these", "be", "put?"};
    }*/

   



  



 

    

  

   

   

    

   

}
