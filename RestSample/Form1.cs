using Stimulsoft.Base.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace RestSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var url = "";
            string postData = "";

            // Items
            //var filefolderKey = "10b5e54e99554805b95ad4d43b64f21d";

            // Login to the Server
            #region Login
            url = "http://localhost:40010/1/login";

            var request = WebRequest.Create(url);
            request.Headers.Add("x-sti-UserName", "1@1.com");
            request.Headers.Add("x-sti-Password", "111111");

            var stream = request.GetResponse().GetResponseStream();
            var sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            var json = JObject.Parse(Out);
            // Get session key
            var sessionKey = (string)json["ResultSessionKey"];
            #endregion

            #region Create Snapshot
            url = "http://localhost:40010/1/reportsnapshots";
            var requestCreateSnapshot = WebRequest.Create(url);
            requestCreateSnapshot.Method = "POST";
            requestCreateSnapshot.ContentType = "application/x-www-form-urlencoded";
            requestCreateSnapshot.Headers.Add("x-sti-SessionKey", sessionKey);

            postData = "{'Ident': 'ReportSnapshotItem', 'Name': 'ReportSnapshot01', 'Description': ''}";
            Request(requestCreateSnapshot, postData);
            // Check Result
            var s = GetResponseResult(requestCreateSnapshot);
            var json = JObject.Parse(s);
            var items = ((JArray)json["ResultItems"]);
            #endregion

            // Run Report to Snapshot
            var reportItem = "30bca27f62594b27b46d6f000b50f717";
            var reportSnapshotItem = items[0]["Key"];

            url = "http://localhost:40010/1/reporttemplates/" + reportItem + "/run";
            var requestReportRun = WebRequest.Create(url);
            requestReportRun.Method = "PUT";
            requestReportRun.ContentType = "application/x-www-form-urlencoded";

            requestReportRun.Headers.Add("x-sti-SessionKey", sessionKey);
            requestReportRun.Headers.Add("x-sti-DestinationItemKey", reportSnapshotItem);

            postData = "{'ResultType': 'Pdf'}";
            Request(requestReportRun, postData);
            // Check Result
            var resultRun = GetResponseResult(requestReportRun);

            // Export Snapshot
            var reportSnapshotKey = "d01381b33e664738bdb6ee0cf410d869";
            var fileStorageKey = "df4b46e06f6d4df7b919cde7a904f86c";

            url = "http://localhost:40010/1/reportsnapshots/" + reportSnapshotKey + "/export";
            var requestRun = WebRequest.Create(url);
            requestRun.Method = "PUT";
            requestRun.ContentType = "application/x-www-form-urlencoded";

            requestRun.Headers.Add("x-sti-SessionKey", sessionKey);
            requestRun.Headers.Add("x-sti-DestinationItemKey", fileStorageKey);

            postData = "{ 'FileItemName':'ExportReport.pdf', 'ExportSet':{ 'Ident':'Pdf', 'PageRange':{ },'EmbeddedFonts':false,'DitheringType':'None','PdfACompliance':true} }";
            
            Request(requestRun, postData);
            s = GetResponseResult(requestRun);
            
        }

        private string GetResponseResult(WebRequest request)
        {
            var resp = request.GetResponse();
            var respStream = resp.GetResponseStream();
            if (respStream != null)
            {
                using (var stream1 = new StreamReader(respStream))
                {
                    var s = stream1.ReadToEnd();
                    return s;
                }
            }
            return null;
        }

        private void Request(WebRequest request, string postData)
        {
            var bytesCreateSnapshot = Encoding.GetEncoding(1251).GetBytes(postData);
            request.ContentLength = bytesCreateSnapshot.Length;
            using (Stream ws = request.GetRequestStream())
            {
                ws.Write(bytesCreateSnapshot, 0, bytesCreateSnapshot.Length);
                ws.Flush();
            }
        }

    }
}
