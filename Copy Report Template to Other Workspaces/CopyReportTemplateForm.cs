using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Stimulsoft.Server.Connect;
using Stimulsoft.Server.Objects;

namespace CopyReportTemplateToOtherWorkspaces
{
    public partial class CopyReportTemplateForm : Form
    {
        public CopyReportTemplateForm()
        {
            InitializeComponent();
        }

        #region Fields
        private static readonly string ServerAddress = "localhost:17764";
        private static readonly string ServerRestApiUrl = $"http://{ServerAddress}/1";

        private static readonly string Username = "user@name.com";
        private static readonly string Password = "password";

        private static readonly string SourceReportTemplateName = "Report Template to Copy";

        private static readonly string ReportTemplateCopierRoleName = "ReportTemplateCopier_2ade28214454412ba3bd4042dc963d56";

        private static readonly string TemporaryUserPassword = "tmpPassword";
        #endregion

        #region Methods
        private string GetTemporaryUserName()
        {
            return $"{Guid.NewGuid().ToString("N")}@tmp.tmp";
        }

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

        private static string UploadReportTemplate(string uploaderSessionKey, string reportTemplateName, string reportTemplateDescription, byte[] reportTemplateBytes)
        {
            // Server REST API requires uploads to be no more than 90kb.
            var maxChunkSizeBytes = 90 * 1024;

            var chunks = SplitByteArrayIntoChunks(reportTemplateBytes, maxChunkSizeBytes);

            // First request creates item.
            var createReportTemplateUrl = $"{ServerRestApiUrl}/files";

            var createReportTemplateRequest = WebRequest.Create(createReportTemplateUrl);
            createReportTemplateRequest.Method = "POST";
            createReportTemplateRequest.Headers.Add("x-sti-SessionKey", uploaderSessionKey);
            AddBody(createReportTemplateRequest, new JObject
            {
                new JProperty("Ident", "ReportTemplateItem"),
                new JProperty("Name", reportTemplateName),
                new JProperty("Description", reportTemplateDescription),
                new JProperty("Resource", Convert.ToBase64String(chunks[0].ToArray())),
            });

            var createReportTemplateResponse = GetResponseJson(createReportTemplateRequest);

            // Keys are needed to upload remaining bytes.
            var fileKey = createReportTemplateResponse.Value<JArray>("ResultCommands")[0].Value<JArray>("ResultItems")[0].Value<string>("Key");
            var versionKey = createReportTemplateResponse.Value<JArray>("ResultCommands")[1].Value<string>("ResultVersionKey");

            var appendReportTemplateUrl = $"{ServerRestApiUrl}/files/{fileKey}";

            // Now remaining chunks are added. 
            for (int chunkIndex = 1; chunkIndex < chunks.Count; ++chunkIndex)
            {
                var appendReportTemplateRequest = WebRequest.Create(appendReportTemplateUrl);
                appendReportTemplateRequest.Method = "PUT";
                appendReportTemplateRequest.Headers.Add("x-sti-SessionKey", uploaderSessionKey);
                // Previous chunk upload's version key is required here.
                appendReportTemplateRequest.Headers.Add("x-sti-VersionKey", versionKey);
                AddBody(appendReportTemplateRequest, new JObject
                {
                    new JProperty("Resource", Convert.ToBase64String(chunks[chunkIndex].ToArray())),
                });

                var appendReportTemplateResponse = GetResponseJson(appendReportTemplateRequest);

                // Store this chunk upload's version key for the next one.
                versionKey = appendReportTemplateResponse.Value<JArray>("ResultCommands")[0].Value<string>("ResultVersionKey");
            }

            return fileKey;
        }

        private static List<ArraySegment<byte>> SplitByteArrayIntoChunks(byte[] byteArray, int maxChunkSize)
        {
            var chunks = new List<ArraySegment<byte>>();

            var remainingSize = byteArray.Length;

            var chunkOffset = 0;

            do
            {
                var newChunkSize = Math.Min(remainingSize, maxChunkSize);

                chunks.Add(new ArraySegment<byte>(byteArray, chunkOffset, newChunkSize));

                chunkOffset += newChunkSize;
                remainingSize -= newChunkSize;
            }
            while (remainingSize > 0);

            return chunks;
        }
        #endregion

        #region Handlers
        private void CopyReportTemplateThroughNetApi(object sender, EventArgs e)
        {
            // Create connection to Stimulsoft Server.
            var supervisorConnection = new StiServerConnection(ServerAddress);

            // Log in to the Server.
            supervisorConnection.Accounts.Users.Login(Username, Password);

            // Check that logged user is Supervisor,
            // for only Supervisor can access all workspaces.
            if (!supervisorConnection.Accounts.Roles.Current.IsSupervisor)
            {
                MessageBox.Show(
                    $"User \"{Username}\" is not a Supervisor and cannot access other workspaces.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // Create source Report Template, just to ensure that we have one.
            var newReportTemplate = supervisorConnection.Items.Root.NewReportTemplate(SourceReportTemplateName, StiItemContentType.Report);
            newReportTemplate.Save();

            // Get source Report Template.
            // We fetch the new one, but it may be some already existing Template.
            var sourceReportTemplateItem = (StiReportTemplateItem)supervisorConnection.Items.GetByKey(newReportTemplate.Key);
            var sourceReportTemplateBytes = sourceReportTemplateItem.DownloadToArray();

            // Get workspace which stores source Template.
            var sourceWorkspace = supervisorConnection.Accounts.Workspaces.Current;

            // Get all workspaces into which we are going to copy the Template.
            var targetWorkspaces = supervisorConnection.Accounts.Workspaces
                .FetchAll()
                .Where(workspace => workspace.Key != sourceWorkspace.Key);

            foreach (var targetWorkspace in targetWorkspaces)
            {
                // Only user from workspace can save items into that workspace. So:

                // First, we create a role in the target workspace that allows creating and uploading Report Templates ...
                var reportTemplateCopierRole = supervisorConnection.Accounts.Roles.New(ReportTemplateCopierRoleName, new StiCommandPermissions
                {
                    ItemReportTemplates = StiPermissions.Create | StiPermissions.Modify,
                });
                reportTemplateCopierRole.WorkspaceKey = targetWorkspace.Key;
                reportTemplateCopierRole.Save();

                // ... then we create temporary user in the target workspace with that role ...
                var temporaryUserName = GetTemporaryUserName();

                var temporaryUser = supervisorConnection.Accounts.Users.NewUser(temporaryUserName, TemporaryUserPassword);
                temporaryUser.WorkspaceKey = targetWorkspace.Key;
                temporaryUser.RoleKey = reportTemplateCopierRole.Key;
                temporaryUser.Save();

                // ... connect and log in as that user ...
                var temporaryUserConnection = new StiServerConnection(ServerAddress);

                temporaryUserConnection.Login(temporaryUserName, TemporaryUserPassword);

                // ... create and save Template copy item ...
                var reportTemplateCopy = temporaryUserConnection.Items.Root.NewReportTemplate($"Copy of \"{SourceReportTemplateName}\"", StiItemContentType.Report);
                reportTemplateCopy.Save();
                // ... upload Report Template contents ...
                reportTemplateCopy.UploadFromArray(sourceReportTemplateBytes);

                // ... and finally log out and delete the user ...
                temporaryUserConnection.Logout();

                supervisorConnection.Accounts.Users.DeleteByKey(temporaryUser.Key);

                // ... and the role.
                supervisorConnection.Accounts.Roles.DeleteByKey(reportTemplateCopierRole.Key);
            }

            // Log out.
            supervisorConnection.Accounts.Users.Logout();
        }

        private void CopyReportTemplateThroughRestApi(object sender, EventArgs e)
        {
            // Log in to Server and acquire session key.
            var logInUrl = $"{ServerRestApiUrl}/login";

            var logInRequest = WebRequest.Create(logInUrl);
            logInRequest.Method = "GET";
            logInRequest.Headers.Add("x-sti-UserName", Username);
            logInRequest.Headers.Add("x-sti-Password", Password);

            var logInResponse = GetResponseJson(logInRequest);

            var supervisorSessionKey = logInResponse.Value<string>("ResultSessionKey");

            // Check that logged user is Supervisor,
            // for only Supervisor can access all workspaces.
            var isLoggedInAsSupervisor = logInResponse.Value<JObject>("ResultRole").Value<bool>("IsSupervisor");

            if (!isLoggedInAsSupervisor)
            {
                MessageBox.Show(
                     $"User \"{Username}\" is not a Supervisor and cannot access other workspaces.",
                     "Error",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Error);

                return;
            }

            // Create source Report Template, just to ensure that we have one.
            var sourceReportItemKey = UploadReportTemplate(
                supervisorSessionKey,
                SourceReportTemplateName,
                string.Empty,
                Encoding.UTF8.GetBytes("Invalid report template content"));

            // Get source Report Template.
            // We fetch the new one, but it may be some already existing Template.
            var sourceReportTemplateFileUrl = $"{ServerRestApiUrl}/files/{sourceReportItemKey}";

            var sourceReportTemplateFileRequest = WebRequest.Create(sourceReportTemplateFileUrl);
            sourceReportTemplateFileRequest.Method = "GET";
            sourceReportTemplateFileRequest.Headers.Add("x-sti-SessionKey", supervisorSessionKey);

            var sourceReportTemplateFileResponse = GetResponseJson(sourceReportTemplateFileRequest);

            var sourceReportTemplateBytesBase64 = sourceReportTemplateFileResponse.Value<JArray>("ResultCommands")[1].Value<string>("ResultResource");
            var sourceReportTemplateBytes = Convert.FromBase64String(sourceReportTemplateBytesBase64);

            // Get workspace which stores source Template.
            var sourceWorkspaceKey = sourceReportTemplateFileResponse.Value<JArray>("ResultCommands")[0].Value<JObject>("ResultItem").Value<string>("WorkspaceKey");

            // Get all workspaces into which we are going to copy the Template.
            var workspacesUrl = $"{ServerRestApiUrl}/workspaces";

            var workspacesRequest = WebRequest.Create(workspacesUrl);
            workspacesRequest.Method = "GET";
            workspacesRequest.Headers.Add("x-sti-SessionKey", supervisorSessionKey);

            var workspacesResponse = GetResponseJson(workspacesRequest);

            var targetWorkspaceKeys = workspacesResponse
                .Value<JArray>("ResultWorkspaces")
                .Cast<JObject>()
                .Select(workspaceEntry => workspaceEntry.Value<string>("Key"))
                .Where(workspaceKey => workspaceKey != sourceWorkspaceKey)
                .ToArray();

            foreach (var targetWorkspaceKey in targetWorkspaceKeys)
            {
                var temporaryUserName = GetTemporaryUserName();

                // Only user from workspace can save items into that workspace. So:

                // First, we create a role in the target workspace that allows creating and uploading Report Templates ...
                var reportCopierRoleCreationUrl = $"{ServerRestApiUrl}/roles";

                var reportCopierRoleCreationRequest = WebRequest.Create(reportCopierRoleCreationUrl);
                reportCopierRoleCreationRequest.Method = "POST";
                reportCopierRoleCreationRequest.Headers.Add("x-sti-SessionKey", supervisorSessionKey);
                AddBody(reportCopierRoleCreationRequest, new JObject
                {
                    new JProperty("Name", ReportTemplateCopierRoleName),
                    new JProperty("WorkspaceKey", targetWorkspaceKey),
                    new JProperty("Permissions", new JObject
                    {
                        new JProperty("ItemReportTemplates", "Create, Modify"),
                    }),
                });

                var reportCopierRoleCreationResponse = GetResponseJson(reportCopierRoleCreationRequest);

                var reportCopierRoleKey = reportCopierRoleCreationResponse.Value<JObject>("ResultRole").Value<string>("Key");

                // ... then we create temporary user in the target workspace with that role ...
                var temporaryUserCreationUrl = $"{ServerRestApiUrl}/users";

                var temporaryUserCreationRequest = WebRequest.Create(temporaryUserCreationUrl);
                temporaryUserCreationRequest.Method = "POST";
                temporaryUserCreationRequest.Headers.Add("x-sti-SessionKey", supervisorSessionKey);
                AddBody(temporaryUserCreationRequest, new JObject
                {
                    new JProperty("UserName", temporaryUserName),
                    new JProperty("Password", TemporaryUserPassword),
                    new JProperty("WorkspaceKey", targetWorkspaceKey),
                    new JProperty("RoleKey", reportCopierRoleKey),
                });

                var temporaryUserCreationResponse = GetResponseJson(temporaryUserCreationRequest);

                // ... connect and log in as that user ...
                var temporaryUserLogInUrl = $"{ServerRestApiUrl}/login";

                var temporaryUserLogInRequest = WebRequest.Create(temporaryUserLogInUrl);
                temporaryUserLogInRequest.Method = "GET";
                temporaryUserLogInRequest.Headers.Add("x-sti-UserName", temporaryUserName);
                temporaryUserLogInRequest.Headers.Add("x-sti-Password", TemporaryUserPassword);

                var temporaryUserLogInResponse = GetResponseJson(temporaryUserLogInRequest);

                var temporaryUserSessionKey = temporaryUserLogInResponse.Value<string>("ResultSessionKey");

                // ... create and upload Template ...
                UploadReportTemplate(
                    temporaryUserSessionKey,
                    $"Copy of \"{SourceReportTemplateName}\"",
                    string.Empty,
                    sourceReportTemplateBytes);

                // ... and finally log out and delete the user ...
                var temporaryUserLogOutUrl = $"{ServerRestApiUrl}/logout";

                var temporaryUserLogOutRequest = WebRequest.Create(temporaryUserLogOutUrl);
                temporaryUserLogOutRequest.Method = "DELETE";
                temporaryUserLogOutRequest.Headers.Add("x-sti-SessionKey", temporaryUserSessionKey);

                temporaryUserLogOutRequest.GetResponse();

                var temporaryUserDeletionUrl = $"{ServerRestApiUrl}/users/{temporaryUserName}";

                var temporaryUserDeletionRequest = WebRequest.Create(temporaryUserDeletionUrl);
                temporaryUserDeletionRequest.Method = "DELETE";
                temporaryUserDeletionRequest.Headers.Add("x-sti-SessionKey", supervisorSessionKey);

                var temporaryUserDeletionResponse = GetResponseJson(temporaryUserDeletionRequest);

                // ... and the role.
                var reportCopierRoleDeletionUrl = $"{ServerRestApiUrl}/roles/{reportCopierRoleKey}";

                var reportCopierRoleDeletionRequest = WebRequest.Create(reportCopierRoleDeletionUrl);
                reportCopierRoleDeletionRequest.Method = "DELETE";
                reportCopierRoleDeletionRequest.Headers.Add("x-sti-SessionKey", supervisorSessionKey);

                var reportCopierRoleDeletionResponse = GetResponseJson(reportCopierRoleDeletionRequest);
            }

            // Log out.
            var logOutUrl = $"{ServerRestApiUrl}/logout";

            var logOutRequest = WebRequest.Create(logOutUrl);
            logOutRequest.Method = "DELETE";
            logOutRequest.Headers.Add("x-sti-SessionKey", supervisorSessionKey);

            logOutRequest.GetResponse();
        }
        #endregion
    }
}
