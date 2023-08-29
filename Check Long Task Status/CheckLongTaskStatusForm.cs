using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace CheckLongTaskStatus
{
    public partial class CheckLongTaskStatusForm : Form
    {
        public CheckLongTaskStatusForm()
        {
            InitializeComponent();
        }

        #region Fields
        private static readonly string ServerAddress = "localhost:17764";
        private static readonly string ServerRestApiUrl = $"http://{ServerAddress}/1";

        private static readonly string Username = "user@name.com";
        private static readonly string Password = "password";

        //Put here Key of existing Report template item.
        private static readonly string ReportTemplateKey = "";
        #endregion

        #region Methods
        private static void AddBody(WebRequest request, JObject data)
        {
            using (var requestStream = request.GetRequestStream())
            using (var requestWriter = new StreamWriter(requestStream))
            {
                requestWriter.Write(data.ToString());
            }
        }

        private static JObject GetResponseJson(WebRequest request)
        {
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var responseReader = new StreamReader(responseStream))
            {
                var responseString = responseReader.ReadToEnd();

                return JObject.Parse(responseString);
            }
        }
        #endregion

        #region Handlers
        private void CheckStatusThroughRestApi(object sender, EventArgs e)
        {
            //
            // Log in to Server and acquire session key.
            //
            var logInUrl = $"{ServerRestApiUrl}/login";

            var logInRequest = WebRequest.Create(logInUrl);
            logInRequest.Method = "GET";
            logInRequest.Headers.Add("x-sti-UserName", Username);
            logInRequest.Headers.Add("x-sti-Password", Password);

            var logInResponse = GetResponseJson(logInRequest);

            var sessionKey = logInResponse.Value<string>("ResultSessionKey");

            //
            // Start long task, report exporting is used here as an example.
            // It is assumed that there is a Report Template with known key on the Server.
            //

            // First, create destination PDF file item.
            var filesUrl = $"{ServerRestApiUrl}/files";

            var createDestinationPdfRequest = WebRequest.Create(filesUrl);

            createDestinationPdfRequest.Method = "POST";
            createDestinationPdfRequest.Headers.Add("x-sti-SessionKey", sessionKey);
            AddBody(createDestinationPdfRequest, new JObject
            {
                new JProperty("Ident", "FileItem"),
                new JProperty("Name", "ReportExportedToPdf"),
                new JProperty("Description", "This is a sample export to PDF"),
                new JProperty("FileType", "Pdf"),
            });

            var createDestinationPdfResponse = GetResponseJson(createDestinationPdfRequest);

            // Response structure is described here:
            // https://www.stimulsoft.com/en/documentation/online/server-api/index.html?stimulsoft_server_rest_api_export_pdf.htm
            var pdfFileItemKey = createDestinationPdfResponse
                .Value<JArray>("ResultCommands")
                .Where(resultCommand => resultCommand.Value<string>("Ident") == "ItemSave").Single()
                .Value<JArray>("ResultItems").Single()
                .Value<string>("Key");

            // Now, start export of existing Report template
            // into created PDF file item.
            var exportReportUrl = $"{ServerRestApiUrl}/reporttemplates/{ReportTemplateKey}/run";

            var exportReportRequest = WebRequest.Create(exportReportUrl);
            exportReportRequest.Method = "PUT";
            exportReportRequest.Headers.Add("x-sti-SessionKey", sessionKey);
            exportReportRequest.Headers.Add("x-sti-DestinationItemKey", pdfFileItemKey);
            AddBody(exportReportRequest, new JObject
            {
                new JProperty("FileItemName", "ExportToPDF"),
                new JProperty("ExportSet", new JObject
                {
                    new JProperty("Ident", "Pdf"),
                    new JProperty("PageRange", new JObject { }),
                    new JProperty("EmbeddedFonts", "false"),
                    new JProperty("DitheringType", "None"),
                    new JProperty("PdfACompliance", "false"),
                }),
            });

            var exportReportResponse = GetResponseJson(exportReportRequest);

            // We need to get Task key from the response.
            // Again, the response structure is described here:
            // https://www.stimulsoft.com/en/documentation/online/server-api/index.html?stimulsoft_server_rest_api_export_pdf.htm
            var taskKey = exportReportResponse.Value<string>("ResultTaskKey");

            //
            // Request the task's status.
            //
            var taskStatusUrl = $"{ServerRestApiUrl}/task/{taskKey}";

            var taskStatusRequest = WebRequest.Create(taskStatusUrl);
            taskStatusRequest.Method = "GET";
            taskStatusRequest.Headers.Add("x-sti-SessionKey", sessionKey);

            var taskStatusResponse = GetResponseJson(taskStatusRequest);

            var taskStatus = taskStatusResponse.Value<JObject>("ResultStatus");

            //
            // Show the status.
            //
            var statusMessage =
$"Task status:{Environment.NewLine}" +
$"  Created: {taskStatus.Value<DateTime>("Created")}{Environment.NewLine}" +
$"  Started: {taskStatus.Value<DateTime>("Started")}{Environment.NewLine}" +
$"{Environment.NewLine}" +
$"  Status: {taskStatus.Value<string>("Status")}{Environment.NewLine}" +
$"    Waiting: {taskStatus.Value<bool>("IsWaiting")}{Environment.NewLine}" +
$"    Running: {taskStatus.Value<bool>("IsRunning")}{Environment.NewLine}" +
$"    Processed: {taskStatus.Value<bool>("IsProcessed")}{Environment.NewLine}" +
$"    Finished: {taskStatus.Value<bool>("IsFinished")}{Environment.NewLine}" +
$"    Stopped: {taskStatus.Value<bool>("IsStopped")}{Environment.NewLine}" +
$"    Error: {taskStatus.Value<bool>("IsError")}{Environment.NewLine}" +
$"";

            MessageBox.Show(
                statusMessage,
                "Exporting to PDF status",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            //
            // Log out.
            //
            var logOutUrl = $"{ServerRestApiUrl}/logout";

            var logOutRequest = WebRequest.Create(logOutUrl);
            logOutRequest.Method = "DELETE";
            logOutRequest.Headers.Add("x-sti-SessionKey", sessionKey);

            logOutRequest.GetResponse();
        }
        #endregion
    }
}
